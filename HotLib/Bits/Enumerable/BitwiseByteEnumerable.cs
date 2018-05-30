using System;
using System.Collections;
using System.Collections.Generic;

namespace HotLib.Bits.Enumerable
{
    /// <summary>
    /// A base class for wrapping enumerables of bytes and enumerating the bits from the bytes a
    /// set number of bits at a time, justified to the right end of a given container type object.
    /// </summary>
    /// <typeparam name="TBitContainer">The type of container to enumerate.
    ///     Determines how many bits can be enumerated at once.</typeparam>
    public abstract class BitwiseByteEnumerable<TBitContainer> : IEnumerable<TBitContainer>
    {
        /// <summary>
        /// The enumerator for bitwise byte enumerable types.
        /// </summary>
        public class Enumerator : IEnumerator<TBitContainer>
        {
            /// <summary>
            /// The number of bits in a byte.
            /// </summary>
            protected const int ByteBits = sizeof(byte) * 8;

            /// <summary>
            /// Gets the enumerable being enumerated.
            /// </summary>
            protected BitwiseByteEnumerable<TBitContainer> Enumerable { get; }

            /// <summary>
            /// Gets the enumerator for byte values.
            /// </summary>
            protected IEnumerator<byte> BytesEnumerator { get; }

            /// <summary>
            /// The current enumerated value. Backs the <see cref="TBitContainer"/> property.
            /// </summary>
            protected TBitContainer _current;
            /// <summary>
            /// Gets the current enumerated value.
            /// </summary>
            /// <exception cref="ObjectDisposedException">The enumerator has been disposed.</exception>
            public TBitContainer Current
            {
                get
                {
                    if (IsDisposed)
                        throw new ObjectDisposedException(null);

                    return _current;
                }
            }
            /// <summary>
            /// Gets the current enumerated value.
            /// </summary>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Gets/Sets the bit offset in the current byte.
            /// </summary>
            protected int CurrentByteOffset { get; set; }

            /// <summary>
            /// Gets whether the enumerator has been disposed.
            /// </summary>
            public bool IsDisposed { get; protected set; } = false;

            /// <summary>
            /// Instantiates a new <see cref="Enumerator"/>.
            /// </summary>
            /// <param name="enumerable">The enumerable being enumerated.</param>
            /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
            public Enumerator(BitwiseByteEnumerable<TBitContainer> enumerable)
            {
                Enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));

                BytesEnumerator = enumerable.Bytes.GetEnumerator();
                BytesEnumerator.MoveNext(); // This needs to be ready to go with good data for MoveNext()
            }

            /// <summary>
            /// Finalizes the enumerator by disposing it.
            /// </summary>
            ~Enumerator() => Dispose(false);

            /// <summary>
            /// Advances ahead a number of bits. Bits advanced past are discarded.
            /// </summary>
            /// <param name="bitCount">The number of bits to advance.</param>
            public virtual void Advance(int bitCount) => CurrentByteOffset += bitCount;

            /// <summary>
            /// Moves the enumerator to the next value.
            /// </summary>
            /// <returns>True if there is another value being enumerated, false if not.</returns>
            /// <exception cref="InvalidOperationException">Unexpected end of byte data.</exception>
            /// <exception cref="ObjectDisposedException">The enumerator has been disposed.</exception>
            public bool MoveNext()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);

                Enumerable.ClearContainer(ref _current);

                var bitsNeeded = Enumerable.BitCount;
                var started = false;
                while (bitsNeeded > 0)
                {
                    if (!EnsureBitsLeft())
                    {
                        if (started)
                            throw new InvalidOperationException($"Unexpected end of byte data (missing " +
                                                                $"final {bitsNeeded} bit(s) for current value)!");

                        return false;
                    }

                    started = true;

                    var bitsAvailable = ByteBits - CurrentByteOffset;
                    var bitsToMove = System.Math.Min(bitsNeeded, bitsAvailable);
                    MaskAndCopyBits(ref _current, BytesEnumerator.Current, bitsToMove);

                    Advance(bitsToMove);
                    bitsNeeded -= bitsToMove;
                }

                return true;
            }

            /// <summary>
            /// Ensures that there are bits left to be read from the byte enumerator, moving to the next
            /// byte if necessary/possible. If we move to another byte, <see cref="CurrentByteOffset"/>
            /// is changed accordingly.
            /// </summary>
            /// <returns>True if there are bits left, false if not.</returns>
            protected virtual bool EnsureBitsLeft()
            {
                while (CurrentByteOffset >= 8)
                {
                    if (BytesEnumerator.MoveNext())
                    {
                        CurrentByteOffset -= ByteBits;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Masks bits from the source based on the given count and <see cref="CurrentByteOffset"/> and adds them to the container.
            /// </summary>
            /// <param name="container">The container to move bits into.</param>
            /// <param name="source">The source byte containing the bits to copy.</param>
            /// <param name="bitCount">The number of bits to copy.</param>
            protected virtual void MaskAndCopyBits(ref TBitContainer container, byte source, int bitCount)
            {
                var mask = BitHelpers.GetLeftMaskByte(bitCount) >> CurrentByteOffset;

                var bits = (byte)((source & mask) >> (ByteBits - bitCount - CurrentByteOffset));

                Enumerable.AddToContainer(ref container, bits, bitCount);
            }

            /// <summary>
            /// Resets the state of the enumerator.
            /// </summary>
            /// <exception cref="ObjectDisposedException">The enumerator has been disposed.</exception>
            public virtual void Reset()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);

                BytesEnumerator.Reset();
                Enumerable.ClearContainer(ref _current);
                CurrentByteOffset = 0;
            }

            /// <summary>
            /// Disposes the enumerator.
            /// </summary>
            public void Dispose()
            {
                if (!IsDisposed)
                {
                    Dispose(true);
                    IsDisposed = true;
                }
            }

            /// <summary>
            /// Disposes the enumerator.
            /// </summary>
            /// <param name="disposing">Indicates whether this was called during disposal
            ///     (<see langword="true"/>) or finalization (<see langword="false"/>).</param>
            protected void Dispose(bool disposing)
            {
                if (disposing)
                    BytesEnumerator?.Dispose();
            }
        }

        /// <summary>
        /// Gets the enumerable of bytes being read from.
        /// </summary>
        protected IEnumerable<byte> Bytes { get; }

        /// <summary>
        /// Gets the number of bits enumerated.
        /// </summary>
        public int BitCount { get; }

        /// <summary>
        /// Instantiates a new <see cref="BitwiseByteEnumerable"/>.
        /// </summary>
        /// <param name="bytes">The enumerable of bytes to read from.</param>
        /// <param name="bitCount">The number of bits to enumerate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public BitwiseByteEnumerable(IEnumerable<byte> bytes, int bitCount)
        {
            Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            BitCount = bitCount;
        }

        /// <summary>
        /// Gets the bitwise enumerator for the enumerable.
        /// </summary>
        /// <returns>The created enumerator.</returns>
        public Enumerator GetEnumerator() => new Enumerator(this);
        /// <summary>
        /// Gets the bitwise enumerator for the enumerable.
        /// </summary>
        /// <returns>The created enumerator.</returns>
        IEnumerator<TBitContainer> IEnumerable<TBitContainer>.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Gets the bitwise enumerator for the enumerable.
        /// </summary>
        /// <returns>The created enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Clears the given bit container.
        /// </summary>
        /// <param name="container">The container to clear.</param>
        protected abstract void ClearContainer(ref TBitContainer container);

        /// <summary>
        /// Adds bits from the given byte to the container.
        /// </summary>
        /// <param name="container">The container to add to.</param>
        /// <param name="bits">A byte containing the bits to add. The bits should
        ///     be justified to the right end of the byte.</param>
        /// <param name="count">The number of bits being added.</param>
        protected abstract void AddToContainer(ref TBitContainer container, byte bits, int count);
    }
}
