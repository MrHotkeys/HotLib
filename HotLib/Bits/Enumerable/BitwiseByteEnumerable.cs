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
        where TBitContainer : struct
    {
        /// <summary>
        /// The enumerator for bitwise byte enumerable types.
        /// </summary>
        public class Enumerator : IEnumerator<TBitContainer>
        {
            /// <summary>
            /// The number of bits in a byte.
            /// </summary>
            protected const int BitsInByte = sizeof(byte) * 8;

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
            /// <exception cref="ObjectDisposedException">The enumerator has been disposed.</exception>
            public virtual void Advance(int bitCount)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);

                CurrentByteOffset += bitCount;
            }

            /// <summary>
            /// Advances ahead by how ever many bits until the enumerator moves to a new byte.
            /// If already at the beginning of a new byte, does nothing.
            /// </summary>
            /// <exception cref="ObjectDisposedException">The enumerator has been disposed.</exception>
            public virtual void AdvanceUntilByteAligned()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);

                var hangingBitCount = CurrentByteOffset % 8;
                if (hangingBitCount > 0)
                    CurrentByteOffset += BitsInByte - hangingBitCount;
            }

            /// <summary>
            /// Tries to takes the given number of bits and return them right-justified in a container.
            /// The enumerator is moved to the following bit. Returns true on success, false if no more
            /// data exists. Throws an <see cref="InvalidOperationException"/> if some data exists that
            /// can be taken but not enough to meet <paramref name="bitCount"/>.
            /// </summary>
            /// <param name="bitCount">The number of bits to take.</param>
            /// <param name="container">Will be set to the container of bits.</param>
            /// <returns>True if successful, false if not.</returns>
            /// <exception cref="ArgumentException"><paramref name="bitCount"/> is negative or too large.</exception>
            /// <exception cref="InvalidOperationException">Some data was taken but not enough remains to finish.</exception>
            /// <exception cref="ObjectDisposedException">The enumerator has been disposed.</exception>
            public virtual bool TryTake(int bitCount, out TBitContainer container)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);
                if (bitCount < 0)
                    throw new ArgumentException("Must be >= 0!", nameof(bitCount));
                if (bitCount > Enumerable.BitContainerCapacity)
                    throw new ArgumentException($"{typeof(TBitContainer)} only has a capacity " +
                                                $"of {Enumerable.BitContainerCapacity} bits!", nameof(bitCount));

                container = default;

                var bitsNeeded = bitCount;
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

                    var bitsAvailable = BitsInByte - CurrentByteOffset;
                    var bitsToMove = System.Math.Min(bitsNeeded, bitsAvailable);
                    MaskAndCopyBits(ref container, BytesEnumerator.Current, bitsToMove);

                    Advance(bitsToMove);
                    bitsNeeded -= bitsToMove;
                    started = true;
                }

                return true;
            }

            /// <summary>
            /// Takes the given number of bits and returns them right-justified in a container.
            /// The enumerator is moved to the following bit.
            /// </summary>
            /// <param name="bitCount">The number of bits to take.</param>
            /// <returns>The container of bits.</returns>
            /// <exception cref="ArgumentException"><paramref name="bitCount"/> is negative or too large.</exception>
            /// <exception cref="InvalidOperationException">No more data remains to be taken.
            ///     -or-Some data was taken but not enough remains to finish.</exception>
            /// <exception cref="ObjectDisposedException">The enumerator has been disposed.</exception>
            public virtual TBitContainer Take(int bitCount)
            {
                if (TryTake(bitCount, out var container))
                    return container;
                else
                    throw new InvalidOperationException("No data remains!");
            }

            /// <summary>
            /// Moves the enumerator to the next value.
            /// </summary>
            /// <returns>True if there is another value being enumerated, false if not.</returns>
            /// <exception cref="InvalidOperationException">Unexpected end of byte data.</exception>
            /// <exception cref="ObjectDisposedException">The enumerator has been disposed.</exception>
            public bool MoveNext()
            {
                if (TryTake(Enumerable.BitCount, out var container))
                {
                    _current = container;
                    return true;
                }
                else
                {
                    _current = default;
                    return false;
                }
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
                        CurrentByteOffset -= BitsInByte;
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

                var bits = (byte)((source & mask) >> (BitsInByte - bitCount - CurrentByteOffset));

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
                _current = default;
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
        /// Gets the capcity of the bit container type.
        /// </summary>
        protected abstract int BitContainerCapacity { get; }

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
        /// Adds bits from the given byte to the container.
        /// </summary>
        /// <param name="container">The container to add to.</param>
        /// <param name="bits">A byte containing the bits to add. The bits should
        ///     be justified to the right end of the byte.</param>
        /// <param name="count">The number of bits being added.</param>
        protected abstract void AddToContainer(ref TBitContainer container, byte bits, int count);
    }
}
