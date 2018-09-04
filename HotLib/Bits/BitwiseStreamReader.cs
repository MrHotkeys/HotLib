using System;
using System.Collections.Generic;
using System.IO;

namespace HotLib.Bits
{
    public unsafe class BitwiseStreamReader<TContainer>
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

        public BitwiseStreamReader(Stream stream, int bufferSize)
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
            HotMath.DivRem8((ulong)newPosition, out var byteOffset, out var bitOffset);
            
            Stream.Seek((int)byteOffset, origin);
            BufferBytes(Buffer.Length);

            BufferIndex = (int)byteOffset;
            BitOffset = bitOffset;

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

        public virtual TContainer Read(int bitCount)
        {
            if (bitCount < 0)
                throw new ArgumentException("Must be >= 0!", nameof(bitCount));
            if (bitCount > MaxBits)
                throw new ArgumentException($"{typeof(TContainer)} can only accommodate {MaxBits} bits!", nameof(bitCount));

            var result = default(TContainer);

            var bitsLeft = bitCount;
            while (bitsLeft > 0)
            {
                // Check if we have bytes left in the buffer to work with
                if (BufferIndex >= BufferedBytes)
                {
                    // Read new bytes into the buffer and make sure
                    // that more bytes are available to buffer
                    if (!BufferBytes(Buffer.Length))
                    {
                        if (bitsLeft > 1)
                            throw new InvalidOperationException($"Attempted to read {bitsLeft} bits past the end of the stream!");
                        else
                            throw new InvalidOperationException($"Attempted to read 1 bit past the end of the stream!");
                    }
                }

                var bitsAvailable = BitsInByte - BitOffset;
                var bitsToMove = Math.Min(bitsLeft, bitsAvailable);

                var bufferMask = BitHelpers.GetRightMaskByte(bitsToMove) << BitOffset;

                var bits = (byte)((Buffer[BufferIndex] & bufferMask) << (bitCount - bitsLeft));

                result = GenericBitwiseOperationsHelper<TContainer>.OrWithOffset(result, bits, (uint)(bitsLeft - bitsToMove));

                Advance(bitsToMove);

                bitsLeft -= bitsToMove;
            }

            return result;
        }

        protected virtual void Advance(int bitCount)
        {
            if (bitCount < 0)
                throw new ArgumentException("Must be >= 0!", nameof(bitCount));

            BitOffset += bitCount;
            if (BitOffset >= BitsInByte)
            {
                BitOffset = 0;
                BufferIndex++;
            }
        }

        protected virtual bool BufferBytes(int count)
        {
            BufferedBytes = Stream.Read(Buffer, 0, count);
            BufferIndex = 0;
            BitOffset = 0;

            return BufferedBytes > 0;
        }

        public void Flush() => Stream.Flush();
    }
}
