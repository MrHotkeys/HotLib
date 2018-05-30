using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace HotLib.IO
{
    /// <summary>
    /// Wraps a <see cref="Stream"/> so that it can be enumerated byte by byte.
    /// </summary>
    public class EnumerableStreamWrapper : IEnumerable<byte>, IDisposable
    {
        /// <summary>
        /// Gets the base stream being enumerated.
        /// </summary>
        protected Stream BaseStream { get; }

        /// <summary>
        /// Gets if the wrapper has been disposed of or not.
        /// </summary>
        public bool IsDisposed { get; protected set; } = false;

        /// <summary>
        /// Gets/Sets the size of the byte buffer to use when reading the base stream.
        /// </summary>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        /// Gets/Sets whether the base stream should be left open or not when <see cref="Dispose"/> is called.
        /// </summary>
        public bool LeaveOpen { get; set; } = false;

        /// <summary>
        /// Instantiates a new <see cref="EnumerableStreamWrapper"/>.
        /// </summary>
        /// <param name="baseStream">The base stream to enumerate.</param>
        public EnumerableStreamWrapper(Stream baseStream)
        {
            BaseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            if (!baseStream.CanRead)
                throw new ArgumentException();
        }

        /// <summary>
        /// Finalizes the <see cref="EnumerableStreamWrapper"/> by disposing it.
        /// </summary>
        ~EnumerableStreamWrapper() => Dispose(false);

        /// <summary>
        /// Gets an enumerator for the bytes in the base stream.
        /// </summary>
        /// <returns>An enumerator for all bytes in the base stream.</returns>
        public virtual IEnumerator<byte> GetEnumerator()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            var buffer = new byte[BufferSize];

            int bytesRead;
            do
            {
                bytesRead = BaseStream.Read(buffer, 0, buffer.Length);

                for (var i = 0; i < bytesRead; i++)
                    yield return buffer[i];
            }
            while (bytesRead == buffer.Length);            
        }

        /// <summary>
        /// Gets an enumerator for the bytes in the base stream.
        /// </summary>
        /// <returns>An enumerator for all bytes in the base stream.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Disposes of the wrapper, closing the base stream if requested.
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
        /// Disposes of the wrapper, closing the base stream if requested.
        /// </summary>
        /// <param name="disposing">Whether this was called during disposal (<see langword="true"/>)
        ///     or finalization (<see langword="false"/>).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!LeaveOpen)
                BaseStream?.Dispose();
        }
    }
}
