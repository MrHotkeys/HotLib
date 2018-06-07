using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HotLib.IO
{
    /// <summary>
    /// A stream which acts as a concatenation of zero or more smaller streams.
    /// </summary>
    public class CompositeStream : Stream, IDisposable
    {
        /// <summary>
        /// Gets the array of base streams that make up the composite, in the order they should be traversed.
        /// </summary>
        protected List<Stream> BaseStreams { get; } = new List<Stream>();

        /// <summary>
        /// Gets/Sets the index of the base stream that the current position lies on in the composite.
        /// </summary>
        protected int CurrentStreamIndex { get; set; }

        /// <summary>
        /// Gets whether the stream can be read from.
        /// </summary>
        public override bool CanRead { get; }

        /// <summary>
        /// Gets whether the base stream can be seeked in. As this is a requirement
        /// of all base streams, this should always return true.
        /// </summary>
        public override bool CanSeek => true;

        /// <summary>
        /// Gets whether the stream can be written to.
        /// </summary>
        public override bool CanWrite { get; }

        /// <summary>
        /// The length of the stream. Backs the <see cref="Length"/> property.
        /// </summary>
        protected long _length;
        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        public override long Length => _length;

        /// <summary>
        /// Backs the <see cref="Position"/> property.
        /// </summary>
        protected long _position;
        /// <summary>
        /// Gets/Sets the position within the stream.
        /// </summary>
        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Gets/Sets whether to leave the base streams open or not when the
        /// composite stream is disposed. Defaults to <see langword="false"/>.
        /// </summary>
        public bool LeaveOpen { get; set; } = false;

        /// <summary>
        /// Gets whether the stream has been disposed.
        /// </summary>
        public bool IsDisposed { get; protected set; } = false;

        /// <summary>
        /// Instantiates a new <see cref="CompositeStream"/>.
        /// </summary>
        /// <param name="canRead">Whether or not this stream will support reading.
        ///     If true, all base streams must support reading.</param>
        /// <param name="canWrite">Whether or not this stream will support writing.
        ///     If true, all base streams must support writing.</param>
        /// <param name="baseStreams">The array of base streams to make up the composite stream, in the order
        ///     they should be traversed. All base streams must support seeking and have a fixed length.</param>
        /// <exception cref="ArgumentException"><paramref name="canRead"/> is true but a stream in
        ///     <paramref name="baseStreams"/> does not support reading.-or-<paramref name="canWrite"/>
        ///     is true but a stream in <paramref name="baseStreams"/> does not support writing.-or-A
        ///     stream in <paramref name="baseStreams"/> does not support seeking.-or-<paramref name="baseStreams"/>
        ///     contains null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="baseStreams"/> is null.</exception>
        public CompositeStream(bool canRead, bool canWrite, params Stream[] baseStreams)
            : this(canRead, canWrite, baseStreams as IEnumerable<Stream>)
        { }

        /// <summary>
        /// Instantiates a new <see cref="CompositeStream"/>.
        /// </summary>
        /// <param name="canRead">Whether or not this stream will support reading.
        ///     If true, all base streams must support reading.</param>
        /// <param name="canWrite">Whether or not this stream will support writing.
        ///     If true, all base streams must support writing.</param>
        /// <param name="baseStreams">The enumerable of base streams to make up the composite stream, in the order
        ///     they should be traversed. All base streams must support seeking and have a fixed length.</param>
        /// <exception cref="ArgumentException"><paramref name="canRead"/> is true but a stream in
        ///     <paramref name="baseStreams"/> does not support reading.-or-<paramref name="canWrite"/>
        ///     is true but a stream in <paramref name="baseStreams"/> does not support writing.-or-A
        ///     stream in <paramref name="baseStreams"/> does not support seeking.-or-<paramref name="baseStreams"/>
        ///     contains null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="baseStreams"/> is null.</exception>
        public CompositeStream(bool canRead, bool canWrite, IEnumerable<Stream> baseStreams)
        {
            if (baseStreams is null)
                throw new ArgumentNullException(nameof(baseStreams));

            CanRead = canRead;
            CanWrite = canWrite;

            foreach (var baseStream in baseStreams)
            {
                if (baseStream is null)
                    throw new ArgumentException("Cannot contain null!", nameof(baseStreams));

                AddBaseStream(baseStream);
            }

            Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Adds a base stream to the composite stream.
        /// </summary>
        /// <param name="baseStream"></param>
        /// <exception cref="ArgumentException"><see cref="CanRead"/> is true but <paramref name="baseStream"/> does not support reading.
        ///     -or-<see cref="CanWrite"/> is true but <paramref name="baseStream"/> does not support writing.
        ///     -or-<paramref name="baseStream"/> does not support seeking.</exception>
        protected virtual void AddBaseStream(Stream baseStream)
        {
            // All sub streams must always be able to be seeked no matter what
            if (!baseStream.CanSeek)
                throw new ArgumentException("All streams must be able to be seeked!", nameof(baseStream));

            if (CanRead && !baseStream.CanRead)
                throw new ArgumentException("All streams must be able to be read from!", nameof(baseStream));
            if (CanWrite && !baseStream.CanWrite)
                throw new ArgumentException("All streams must be able to be written to!", nameof(baseStream));

            BaseStreams.Add(baseStream);
            _length += baseStream.Length;
        }

        /// <summary>
        /// Flushes all base streams.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override void Flush()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            foreach (var stream in BaseStreams)
                stream.Flush();
        }

        /// <summary>
        /// Reads a number of bytes from the stream into a buffer.
        /// Advances the stream's position by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read bytes into.</param>
        /// <param name="offset">The offset in the buffer at which to start inserting read bytes.</param>
        /// <param name="count">The number of bytes to try to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is negative or too large for the buffer.
        ///     -or-<paramref name="count"/> is negative or too large for the buffer.</exception>
        /// <exception cref="NotSupportedException">The base stream does not support reading.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (!CanRead)
                throw new NotSupportedException("This stream is not set to support reading!");
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentOutOfRangeException("Must be within bounds of the buffer!", nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException("Must be >= 0!", nameof(count));
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("Buffer offset + byte count must be within the bounds of the buffer!", nameof(count));

            var totalBytesRead = 0;

            while (count > 0 && Position < Length)
            {
                var bytesRead = BaseStreams[CurrentStreamIndex].Read(buffer, offset, count);

                count -= bytesRead;
                offset += bytesRead;
                totalBytesRead += bytesRead;
                Position += bytesRead;
            }

            return totalBytesRead;
        }

        /// <summary>
        /// Sets the position within the stream, relative to an origin point.
        /// </summary>
        /// <param name="offset">The offset from the origin.</param>
        /// <param name="origin">The origin point to offset from.</param>
        /// <returns>The new position in the stream.</returns>
        /// <exception cref="ArgumentException"><paramref name="origin"/> has an invalid value.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The new position is outside the bounds of the stream.</exception>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            var newPosition = _position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;
                case SeekOrigin.Current:
                    newPosition += offset;
                    break;
                case SeekOrigin.End:
                    newPosition = Length + offset;
                    break;
                default:
                    throw new ArgumentException($"Invalid {typeof(SeekOrigin)} value {origin}!", nameof(origin));
            }

            if (newPosition < 0 || newPosition > Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            else
                _position = newPosition;

            // Move to the correct stream and move that stream to the right position
            if (Position == Length)
            {
                // Make sure there actually is at least one base stream
                if (BaseStreams.Count > 0)
                {
                    // Special handling for if at the end since it'll try to move outside the bounds of Streams
                    CurrentStreamIndex = BaseStreams.Count - 1;
                    BaseStreams[CurrentStreamIndex].Seek(0, SeekOrigin.End);
                }
            }
            else
            {
                var localizedPosition = Position;
                for (var i = 0; 0 < BaseStreams.Count; i++)
                {
                    var currentStream = BaseStreams[i];

                    if (localizedPosition >= currentStream.Length)
                    {
                        localizedPosition -= currentStream.Length;
                    }
                    else
                    {
                        CurrentStreamIndex = i;
                        BaseStreams[CurrentStreamIndex].Seek(localizedPosition, SeekOrigin.Begin);
                        break;
                    }
                }
            }            

            return Position;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <summary>
        /// Writes bytes from a buffer into the stream. Advances the stream's position by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer of bytes to write.</param>
        /// <param name="offset">The offset in the buffer from which to start writing.</param>
        /// <param name="count">The total number of bytes to write.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is negative or too large for the buffer.
        ///     -or-<paramref name="count"/> is negative or too large for the buffer.
        ///     -or-There is not enough space remaining in the stream to write the requested number of bytes.</exception>
        /// <exception cref="NotSupportedException">The base stream does not support writing.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (!CanWrite)
                throw new NotSupportedException("This stream is not set to support writing!");
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentOutOfRangeException("Must be within bounds of the buffer!", nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException("Must be >= 0!", nameof(count));
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("Buffer offset + byte count must be within the bounds of the buffer!", nameof(count));
            if (Position + count > Length)
                throw new ArgumentOutOfRangeException("Not enough room left in stream!", nameof(count));
            
            while (count > 0)
            {
                var currentStream = BaseStreams[CurrentStreamIndex];
                var bytesLeftInCurrentStream = currentStream.Length - currentStream.Position;

                var bytesToWrite = (int)System.Math.Min(count, bytesLeftInCurrentStream);

                currentStream.Write(buffer, offset, bytesToWrite);

                count -= bytesToWrite;
                offset += bytesToWrite;

                Position += bytesToWrite;
            }
        }

        /// <summary>
        /// Disposes of the stream, closing the base streams if requested.
        /// </summary>
        /// <param name="disposing">Whether this was called during disposal (<see langword="true"/>)
        ///     or finalization (<see langword="false"/>)</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !LeaveOpen)
            {
                if (!(BaseStreams is null))
                {
                    foreach (var stream in BaseStreams)
                        stream?.Dispose();
                }
            }

            base.Dispose(disposing);

            IsDisposed = true;
        }
    }
}
