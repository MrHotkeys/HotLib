using System;
using System.IO;

namespace HotLib.IO
{
    /// <summary>
    /// Represents a segment of a <see cref="Stream"/> with a fixed length.
    /// </summary>
    public class StreamSlice : Stream, IDisposable
    {
        /// <summary>
        /// Gets the base stream the slice belongs to.
        /// </summary>
        protected Stream BaseStream { get; }

        /// <summary>
        /// Gets the start of the slice in the base stream.
        /// </summary>
        public long Start { get; }

        /// <summary>
        /// Gets the exclusive end of the slice in the base stream.
        /// </summary>
        public long End => Start + Length;

        /// <summary>
        /// Gets the length of the slice.
        /// </summary>
        public override long Length { get; }

        /// <summary>
        /// Gets/Sets whether to leave the base stream open or not when the
        /// slice is disposed. Defaults to <see langword="false"/>.
        /// </summary>
        public bool LeaveOpen { get; set; } = false;

        /// <summary>
        /// Gets whether the base stream can be read from.
        /// </summary>
        public override bool CanRead => BaseStream.CanRead;

        /// <summary>
        /// Gets whether the base stream can be seeked in. As this is a requirement
        /// of <see cref="StreamSlice"/>, this should always return true.
        /// </summary>
        public override bool CanSeek => true;

        /// <summary>
        /// Gets whether the base stream can be written to.
        /// </summary>
        public override bool CanWrite => BaseStream.CanWrite;

        /// <summary>
        /// Backs the <see cref="Position"/> property.
        /// </summary>
        protected long _position;
        /// <summary>
        /// Gets/Sets the position within the slice.
        /// </summary>
        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Gets whether the slice has been disposed.
        /// </summary>
        public bool IsDisposed { get; protected set; } = false;

        /// <summary>
        /// Instantiates a new <see cref="StreamSlice"/>.
        /// </summary>
        /// <param name="baseStream">The base <see cref="Stream"/> to slice. Must support seeking.</param>
        /// <param name="start">The start position of the slice in the base stream.</param>
        /// <param name="length">The length of the slice.</param>
        /// <exception cref="ArgumentException"><paramref name="start"/> or <paramref name="length"/> is negative.
        ///     -or-<paramref name="baseStream"/> does not support seeking.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="baseStream"/> is null.</exception>
        public StreamSlice(Stream baseStream, long start, long length)
        {
            BaseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            if (!BaseStream.CanSeek)
                throw new ArgumentException("Stream must support seeking!");

            if (start < 0)
                throw new ArgumentException("Must be >= 0!", nameof(start));
            Start = start;

            if (length < 0)
                throw new ArgumentException("Must be >= 0!", nameof(length));
            Length = length;
        }

        /// <summary>
        /// Flushes the base stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override void Flush() => BaseStream.Flush();

        /// <summary>
        /// Reads a number of bytes from the slice into a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read bytes into.</param>
        /// <param name="offset">The offset in the buffer at which to start inserting read bytes.</param>
        /// <param name="count">The number of bytes to try to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        /// <exception cref="ArgumentException"><paramref name="offset"/> is negative or too large for the buffer.
        ///     -or-<paramref name="count"/> is negative or too large for the buffer.</exception>
        /// <exception cref="NotSupportedException">The base stream does not support reading.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentException("Must be within bounds of the buffer!", nameof(offset));
            if (count < 0)
                throw new ArgumentException("Must be >= 0!", nameof(count));
            if (offset + count > buffer.Length)
                throw new ArgumentException("Buffer offset + byte count must be within the bounds of the buffer!", nameof(count));

            BaseStream.Seek(Start + Position, SeekOrigin.Begin);

            var bytesLeft = End - BaseStream.Position;
            count = (int)System.Math.Min(count, bytesLeft);

            var bytesRead = BaseStream.Read(buffer, offset, count);
            Position += bytesRead;

            return bytesRead;
        }

        /// <summary>
        /// Sets the position within the slice, relative to an origin point.
        /// </summary>
        /// <param name="offset">The offset from the origin.</param>
        /// <param name="origin">The origin point to offset from.</param>
        /// <returns>The new position in the slice.</returns>
        /// <exception cref="ArgumentException"><paramref name="origin"/> has an invalid value.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The new position is outside the bounds of the stream.</exception>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            var startPosition = Position;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;
                case SeekOrigin.Current:
                    _position += offset;
                    break;
                case SeekOrigin.End:
                    _position = Length + offset;
                    break;
                default:
                    throw new ArgumentException($"Invalid {typeof(SeekOrigin)} value {origin}!", nameof(origin));
            }

            if (Position < 0 || Position > Length)
            {
                _position = startPosition;
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            return Position;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <summary>
        /// Writes bytes from a buffer into the slice. Advances the slice's position by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer of bytes to write.</param>
        /// <param name="offset">The offset in the buffer from which to start writing.</param>
        /// <param name="count">The total number of bytes to write.</param>
        /// <exception cref="ArgumentException"><paramref name="offset"/> is negative or too large for the buffer.
        ///     -or-<paramref name="count"/> is negative or too large for the buffer.
        ///     -or-There is not enough space remaining in the slice to write the requested number of bytes.</exception>
        /// <exception cref="NotSupportedException">The base stream does not support writing.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentException("Must be within bounds of the buffer!", nameof(offset));
            if (count < 0)
                throw new ArgumentException("Must be >= 0!", nameof(count));
            if (offset + count > buffer.Length)
                throw new ArgumentException("Buffer offset + byte count must be within the bounds of the buffer!", nameof(count));

            BaseStream.Seek(Start + Position, SeekOrigin.Begin);

            var bytesLeft = End - BaseStream.Position;
            if (count > bytesLeft)
                throw new ArgumentException($"Not enough space to write {count} bytes (only {bytesLeft} left)!", nameof(count));

            BaseStream.Write(buffer, offset, count);
            Position += count;
        }

        /// <summary>
        /// Disposes of the slice, closing the base stream if requested.
        /// </summary>
        /// <param name="disposing">Whether this was called during disposal (<see langword="true"/>)
        ///     or finalization (<see langword="false"/>)</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !LeaveOpen)
                BaseStream?.Dispose();

            base.Dispose(disposing);

            IsDisposed = true;
        }
    }
}
