using System;
using System.Collections.Generic;
using System.IO;

namespace HotLib.Bits
{
    public unsafe class BitwiseStreamWriter<TContainer>
        where TContainer : unmanaged
    {
        /// <summary>
        /// The number of bits in a byte.
        /// </summary>
        protected const int BitsInByte = 8;

        protected static int MaxBits = sizeof(TContainer) * BitsInByte;

        protected Stream Stream { get; }

        protected byte[] Buffer { get; }

        protected int BufferedBytes { get; set; } = 0;

        protected int BufferIndex { get; set; } = 0;

        protected int BitOffset { get; set; } = 0;

        public long Position
        {
            get => BufferIndex * BitsInByte + BitOffset;
            set => Seek(value, SeekOrigin.Begin);
        }

        public BitwiseStreamWriter(Stream stream, int bufferSize)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (bufferSize <= 0)
                throw new ArgumentException("Must be >= 1!", nameof(bufferSize));
            Buffer = new byte[bufferSize];
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            if (!Stream.CanSeek)
                throw new InvalidOperationException("Base stream does not support seeking!");

            var newPosition = GetNewPosition();
            HotMath.DivRem(newPosition, BitsInByte, out var byteOffset, out var bitOffset);

            Stream.Seek(byteOffset, origin);

            BufferIndex = (int)byteOffset;
            BitOffset = (int)bitOffset;

            return newPosition;

            long GetNewPosition()
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        return offset;
                    case SeekOrigin.Current:
                        return Position + offset;
                    case SeekOrigin.End:
                        return Stream.Length + offset;
                    default:
                        throw new ArgumentException($"Invalid {typeof(SeekOrigin)} value {origin}!", nameof(origin));
                }
            }
        }

        public virtual void Write(TContainer value, int bitCount)
        {
            if (bitCount < 0)
                throw new ArgumentException("Must be >= 0!", nameof(bitCount));
            if (bitCount > MaxBits)
                throw new ArgumentException($"{typeof(TContainer)} can only accommodate {MaxBits} bits!", nameof(bitCount));

            var bitsLeft = bitCount;
            var valueOffset = 0;
            while (bitsLeft > 0)
            {
                var bitsAvailable = BitsInByte - BitOffset;
                var bitsToMove = Math.Min(bitsLeft, bitsAvailable);

                var valueMask = BitHelpers.GetRightMaskByte(bitsToMove);

                var bits = GenericBitwiseOperationsHelper<TContainer>.MaskFrom(value, valueMask, (uint)valueOffset);
                valueOffset += bitsToMove;

                var currentValue = Buffer[BufferIndex];
                currentValue &= (byte)~valueMask;
                currentValue |= (byte)(bits << BitOffset);
                Buffer[BufferIndex] = currentValue;

                Advance(bitsToMove);

                bitsLeft -= bitsToMove;
            }
        }

        public virtual void Advance(int bitCount)
        {
            if (bitCount < 0)
                throw new ArgumentException("Must be >= 0!", nameof(bitCount));

            BitOffset += bitCount;
            if (BitOffset >= BitsInByte)
            {
                BitOffset = 0;
                BufferIndex++;

                if (BufferIndex >= Buffer.Length)
                    Flush();
            }
        }

        public void Flush()
        {
            Stream.Write(Buffer, 0, BufferIndex + 1);
            Stream.Flush();
            BufferIndex = 0;
        }
    }
}
