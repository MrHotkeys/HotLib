using System;
using System.Linq;
using System.IO;

namespace HotLib
{
    /// <summary>
    /// A write-only stream that works as a collection of streams, mirroring
    /// the output to each member stream. Not thread safe on instances.
    /// </summary>
    public sealed class MultiDestinationStream : Stream, IDisposable
    {
        /// <summary>
        /// The array of destination streams that will be written to.
        /// </summary>
        private Stream[] Destinations { get; }

        /// <summary>
        /// The internal buffer the collection uses.
        /// </summary>
        private byte[] Buffer { get; }

        /// <summary>
        /// Gets if the collection of streams can be read from (false).
        /// </summary>
        public override bool CanRead { get { return false; } }

        /// <summary>
        /// Gets if the collection of streams can be seeked in (false).
        /// </summary>
        public override bool CanSeek { get { return false; } }

        /// <summary>
        /// Gets if the collection of streams can be written to (true).
        /// </summary>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// Not supported for the collection as multiple streams may have multiple lengths.
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Getter is called.</exception>
        public override long Length
        {
            get => throw new NotSupportedException($"{nameof(Length)} getter not supported " +
                                                   $"for {nameof(MultiDestinationStream)}!");
        }

        /// <summary>
        /// Not supported for the collection as multiple streams may have multiple positions.
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Getter or setter are called.</exception>
        public override long Position
        {
            get => throw new NotSupportedException($"{nameof(Position)} getter not supported " +
                                                   $"for {nameof(MultiDestinationStream)}!");
            set => throw new NotSupportedException($"{nameof(Position)} setter not supported " +
                                                   $"for {nameof(MultiDestinationStream)}!");
        }

        /// <summary>
        /// Gets the number of streams in the collection.
        /// </summary>
        public int Count { get { return Destinations.Length; } }

        /// <summary>
        /// Gets/Sets whether the destination streams are flushed automatically
        /// after calls to <see cref="Write(byte[], int, int)"/>.
        /// </summary>
        public bool AutoFlush { get; set; } = false;

        /// <summary>
        /// Initializes a new <see cref="MultiDestinationStream"/>.
        /// </summary>
        /// <param name="destinations">The destination streams.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destinations"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="destinations"/> is empty or contains null.</exception>
        public MultiDestinationStream(params Stream[] destinations)
            : this(1024, destinations)
        { }

        /// <summary>
        /// Initializes a new <see cref="MultiDestinationStream"/>.
        /// </summary>
        /// <param name="bufferSize">The size of the buffer.</param>
        /// <param name="destinations">The destination streams.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destinations"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="destinations"/> is empty or contains null.
        ///     -or- <paramref name="bufferSize"/> is non-positive.</exception>
        public MultiDestinationStream(int bufferSize, params Stream[] destinations)
        {
            if (bufferSize < 1)
                throw new ArgumentException("Must be a positive number!", nameof(bufferSize));
            if (destinations == null)
                throw new ArgumentNullException(nameof(destinations));
            if (destinations.Length == 0)
                throw new ArgumentException("Need at least one destination stream!", nameof(destinations));
            if (destinations.Contains(null))
                throw new ArgumentException("Stream array cannot contain null!");

            Buffer = new byte[bufferSize];

            Destinations = new Stream[destinations.Length];
            destinations.CopyTo(Destinations, 0);
        }

        /// <summary>
        /// Finalizes the <see cref="MultiDestinationStream"/> by calling <see cref="Dispose(bool)"/>.
        /// </summary>
        ~MultiDestinationStream()
        {
            Dispose(false);
        }

        /// <summary>
        /// Writes bytes from the buffer to each member stream.
        /// </summary>
        /// <param name="buffer">The buffer of bytes to write to the streams.</param>
        /// <param name="offset">The position in the byte buffer to start at.</param>
        /// <param name="count">The number of bytes to write from the buffer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="offset"/> is negative or
        ///     larger than the length of <paramref name="buffer"/>, or <paramref name="count"/>
        ///     is negative or greater than what's left after <paramref name="offset"/> is applied.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentException("Offset out of range!");
            if (count < 0 || buffer.Length - offset < count)
                throw new ArgumentException("Count out of range!");

            foreach (var destination in Destinations)
                destination.Write(buffer, offset, count);

            if (AutoFlush)
                Flush();
        }

        /// <summary>
        /// Flushes every member stream.
        /// </summary>
        public override void Flush()
        {
            foreach (var destination in Destinations)
                destination.Flush();
        }

        /// <summary>
        /// Not supported, as the member streams may be at multiple positions.
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Method is called.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException($"{nameof(Seek)} not supported for {nameof(MultiDestinationStream)}!");
        }

        /// <summary>
        /// Not supported, member streams may be written to but not otherwise transformed
        /// by the collection. Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Method is called.</exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException($"{nameof(SetLength)} not supported for {nameof(MultiDestinationStream)}!");
        }

        /// <summary>
        /// Not supported, stream is write-only. Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Method is called.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException($"{nameof(Read)} not supported for {nameof(MultiDestinationStream)}!");
        }

        /// <summary>
        /// Disposes of the collection, disposing of each member stream.
        /// </summary>
        /// <param name="disposing">Whether this is being called during disposal or finalization.</param>
        protected override void Dispose(bool disposing)
        {
            // Dispose of each member stream
            if (Destinations != null)
            {
                foreach (var destination in Destinations)
                    destination?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
