using System;
using System.Collections.Concurrent;
using System.IO;

namespace HotLib.IO
{
    /// <summary>
    /// A stream which wraps an array of <see cref="BlockingCollection{T}"/> of <see langword="byte"/>
    /// and blocks the thread when reading and there is no data available. Only supports reading.
    /// </summary>
    public class BlockingCollectionReadStream : Stream
    {
        /// <summary>
        /// Gets the collection of buffers to stream.
        /// </summary>
        protected BlockingCollection<byte[]> Buffers { get; }

        /// <summary>
        /// Gets/Sets the current buffer being streamed from.
        /// </summary>
        protected byte[] CurrentBuffer { get; set; } = Array.Empty<byte>();
        /// <summary>
        /// Gets/Sets the current index in the current buffer being streamed from.
        /// </summary>
        protected int CurrentBufferIndex { get; set; } = 0;

        /// <summary>
        /// Gets if the stream can be read from (true).
        /// </summary>
        public override bool CanRead => true;
        /// <summary>
        /// Gets if the stream can be seeked in (false).
        /// </summary>
        public override bool CanSeek => false;
        /// <summary>
        /// Gets if the stream can be written to (false).
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override long Length => throw new NotSupportedException();

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Gets whether the stream has been disposed.
        /// </summary>
        public bool IsDisposed { get; protected set; } = false;

        /// <summary>
        /// Gets/Sets whether to dispose the <see cref="BlockingCollection{T}"/> of buffers when disposed.
        /// </summary>
        public bool DisposeBuffers { get; set; } = true;

        /// <summary>
        /// Instantiates a new <see cref="BlockingCollectionReadStream"/>/
        /// </summary>
        /// <param name="buffers">The collection of buffers to stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffers"/> is null.</exception>
        public BlockingCollectionReadStream(BlockingCollection<byte[]> buffers)
        {
            Buffers = buffers ?? throw new ArgumentNullException(nameof(buffers));
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override void Flush() => throw new NotSupportedException();

        /// <summary>
        /// Reads a number of bytes from the stream into a buffer.
        /// Advances the stream's position by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read bytes into.</param>
        /// <param name="offset">The offset in the buffer at which to start inserting read bytes.</param>
        /// <param name="count">The number of bytes to try to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        /// <exception cref="ArgumentException"><paramref name="offset"/> is negative or too large for the buffer.
        ///     -or-<paramref name="count"/> is negative or too large for the buffer.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentOutOfRangeException("Must be within bounds of the buffer!", nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException("Must be >= 0!", nameof(count));
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("Buffer offset + byte count must be within the bounds of the buffer!", nameof(count));

            var bytesTaken = 0;
            var bufferIndex = 0;
            while (bytesTaken < count)
            {
                var bytesAvailable = CurrentBuffer.Length - CurrentBufferIndex;
                if (bytesAvailable == 0)
                {
                    if (!MoveToNextBuffer())
                        return bytesTaken;
                    else
                        bytesAvailable = CurrentBuffer.Length;
                }

                var bytesToTake = System.Math.Min(count - bytesTaken, bytesAvailable);
                while (bytesToTake > 0)
                {
                    buffer[offset + bufferIndex] = CurrentBuffer[CurrentBufferIndex];

                    bufferIndex++;
                    CurrentBufferIndex++;
                    bytesTaken++;
                    bytesToTake--;
                }
            }

            return bytesTaken;
        }

        /// <summary>
        /// Advances to the next buffer from <see cref="Buffers"/>.
        /// </summary>
        /// <returns><see langword="true"/> if there was a new buffer and we've
        ///     successfuly moved to it, <see langword="false"/> if not.</returns>
        protected bool MoveToNextBuffer()
        {
            if (Buffers.TryTake(out var newBuffer, -1))
            {
                CurrentBuffer = newBuffer;
                CurrentBufferIndex = 0;
                return true;
            }
            else
            {
                CurrentBuffer = Array.Empty<byte>();
                CurrentBufferIndex = 0;
                return false;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <summary>
        /// Disposes of the stream.
        /// </summary>
        /// <param name="disposing">Whether this was called during disposal (<see langword="true"/>)
        ///     or finalization (<see langword="false"/>)</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && DisposeBuffers)
                Buffers.Dispose();

            base.Dispose(disposing);

            IsDisposed = true;
        }
    }
}
