using System;
using System.Collections.Generic;
using System.IO;

using HotLib.Bits;

namespace HotLib.IO
{
    /// <summary>
    /// Writes unmanaged values and collections of bytes to <see cref="Stream"/> instances.
    /// Can be set to use a specific endianness when writing values.
    /// </summary>
    public class ByteWriter : IDisposable
    {
        /// <summary>
        /// Gets the base stream to write to.
        /// </summary>
        protected Stream BaseStream { get; }

        /// <summary>
        /// Gets/Sets the position within the base stream.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The new position negative or too large.</exception>
        /// <exception cref="NotSupportedException">The writer's base stream does not support seeking.</exception>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual long Position
        {
            get => BaseStream.Position;
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);

                if (value < 0 || value >= BaseStream.Length)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be >= 0 and less than the base stream's length!");

                try
                {
                    BaseStream.Position = value;
                }
                catch (NotSupportedException e) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    throw new NotSupportedException("Base stream does not support seeking!", e);
                }
            }
        }

        /// <summary>
        /// Gets the writer's internal byte buffer.
        /// </summary>
        protected byte[] Buffer { get; set; } = new byte[8192];

        /// <summary>
        /// Gets/Sets the size of the writer's internal byte buffer.
        /// If the buffer is resized, <see cref="Flush(bool)"/> is called first so that nothing is lost.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The new buffer size is 0 or negative.</exception>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual int BufferSize
        {
            get => Buffer.Length;
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(null);

                if (Buffer.Length != value)
                {
                    if (value <= 0)
                        throw new ArgumentOutOfRangeException(nameof(value), "Must be > 0!");

                    Flush();

                    Buffer = new byte[value];
                }
            }
        }

        /// <summary>
        /// Gets/Sets the index in the buffer that will be written to next.
        /// </summary>
        protected int BufferIndex { get; set; } = 0;

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
        /// Gets/Sets whether the writer should automatically flush after every write.
        /// </summary>
        public bool AutoFlush { get; set; } = false;

        /// <summary>
        /// Gets/Sets whether to flush the base stream when the parameterless <see cref="Flush"/> is called.
        /// </summary>
        public bool FlushBaseStream { get; set; } = true;

        /// <summary>
        /// Instantiates a new <see cref="ByteWriter"/>.
        /// </summary>
        /// <param name="baseStream">The base stream to write to. Its <see cref="Stream.CanRead"/>
        ///     value must be <see langword="true"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="baseStream"/> cannot be written to,.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="baseStream"/> is null.</exception>
        public ByteWriter(Stream baseStream)
        {
            BaseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            if (!BaseStream.CanWrite)
                throw new ArgumentException("Base stream must be able to be written to!", nameof(baseStream));
        }
        
        /// <summary>
        /// Finalizes the <see cref="ByteWriter"/> by calling <see cref="Dispose(bool)"/>.
        /// </summary>
        ~ByteWriter() => Dispose(false);

        /// <summary>
        /// Writes a byte to the internal byte buffer to be later written to the base stream.
        /// Increments <see cref="BufferIndex"/>, and flushes if the buffer is full.
        /// Assumes no checking necessary for args or state (e.g. <see cref="IsDisposed"/>).
        /// Ignores <see cref="AutoFlush"/>.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        /// <returns>The number of bytes written. If no exceptions occur, this will always be 1.</returns>
        protected virtual void WriteByteInternal(byte b)
        {
            Buffer[BufferIndex] = b;

            BufferIndex++;
            if (BufferIndex >= Buffer.Length)
                Flush();
        }

        /// <summary>
        /// Writes a byte to the internal byte buffer to be later written to the base stream.
        /// If <see cref="AutoFlush"/> is <see langword="true"/>, the buffer will be flushed before returning.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        /// <returns>The number of bytes written. If no exceptions occur, this will always be 1.</returns>
        public virtual int Write(byte value)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            WriteByteInternal(value);

            if (AutoFlush)
                Flush();

            return 1;
        }

        /// <summary>
        /// Converts an unmanaged value to bytes, and then stores them in the
        /// internal byte buffer to be later written to the base stream.
        /// If <see cref="AutoFlush"/> is <see langword="true"/>, the buffer will be flushed before returning.
        /// </summary>
        /// <typeparam name="T">The type of value to write the bytes from. Must be unmanaged.</typeparam>
        /// <param name="value">The value to write the bytes from.</param>
        /// <param name="endianness">The endianness to use when writing the bytes from the value. If it
        ///     doesn't match the system's endianness, the given bytes will be used in reverse order to
        ///     create the value, such that the endianness of the created value will match the system's.</param>
        /// <returns>The total number of bytes written.</returns>
        /// <exception cref="ArgumentException"><paramref name="endianness"/> is not defined in <see cref="Endianness"/>.</exception>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public unsafe virtual int Write<T>(T value, Endianness endianness)
            where T : unmanaged
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            var ptr = (byte*)&value;

            bool MatchesSystemEndianness()
            {
                try
                {
                    return endianness.MatchesSystemEndianness();
                }
                catch (ArgumentException e) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    // Repackage so we can make sure that the parameter name is correct
                    throw new ArgumentException(e.Message, nameof(endianness), e);
                }
            }

            if (MatchesSystemEndianness())
            {
                for (var i = 0; i < sizeof(T); i++)
                    WriteByteInternal(ptr[i]);
            }
            else
            {
                for (var i = 0; i < sizeof(T); i++)
                    WriteByteInternal(ptr[sizeof(T) - i - 1]);
            }

            if (AutoFlush)
                Flush();

            return sizeof(T);
        }

        /// <summary>
        /// Writes the bytes from an array into the internal buffer to be later written to the base stream.
        /// If <see cref="AutoFlush"/> is <see langword="true"/>, the buffer will be flushed before returning.
        /// </summary>
        /// <param name="bytes">The array of bytes to write.</param>
        /// <param name="offset">The offset in the array from which to start writing.</param>
        /// <param name="count">The total number of bytes to write.</param>
        /// <returns>The number of bytes written.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is negative or too large for the byte array.
        ///     -or-<paramref name="count"/> is negative or too large for the byte array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual int Write(byte[] bytes, int offset, int count)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));
            if (offset < 0 || offset >= bytes.Length)
                throw new ArgumentOutOfRangeException("Must be within bounds of the byte array!", nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException("Must be >= 0!", nameof(count));
            if (offset + count > bytes.Length)
                throw new ArgumentOutOfRangeException("Offset + count must be within the bounds of the byte array!", nameof(count));

            for (var i = 0; i < count; i++)
                WriteByteInternal(bytes[offset + i]);

            if (AutoFlush)
                Flush();

            return count;
        }

        /// <summary>
        /// Writes the bytes from an array into the internal buffer to be later written to the base stream.
        /// If <see cref="AutoFlush"/> is <see langword="true"/>, the buffer will be flushed before returning.
        /// </summary>
        /// <param name="bytes">The array of bytes to write.</param>
        /// <returns>The number of bytes written.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual int Write(byte[] bytes) => Write(bytes, 0, bytes.Length);

        /// <summary>
        /// Writes the bytes from a span into the internal buffer to be later written to the base stream.
        /// If <see cref="AutoFlush"/> is <see langword="true"/>, the buffer will be flushed before returning.
        /// </summary>
        /// <param name="bytes">The span of bytes to write.</param>
        /// <returns>The number of bytes written.</returns>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual int Write(Span<byte> bytes)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            for (var i = 0; i < bytes.Length; i++)
                WriteByteInternal(bytes[i]);

            if (AutoFlush)
                Flush();

            return bytes.Length;
        }

        /// <summary>
        /// Writes the bytes from a enumerable into the internal buffer to be later written to the base stream.
        /// If <see cref="AutoFlush"/> is <see langword="true"/>, the buffer will be flushed before returning.
        /// </summary>
        /// <param name="bytes">The enumerable of bytes to write.</param>
        /// <returns>The number of bytes written.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual int Write(IEnumerable<byte> bytes)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            var count = 0;
            foreach (var b in bytes)
            {
                WriteByteInternal(b);
                count++;
            }

            if (AutoFlush)
                Flush();

            return count;
        }

        /// <summary>
        /// Writes the bytes from a pointer into the internal buffer to be later written to the base stream.
        /// If <see cref="AutoFlush"/> is <see langword="true"/>, the buffer will be flushed before returning.
        /// </summary>
        /// <param name="bytes">The pointer to the bytes to write.</param>
        /// <param name="count">The number of bytes to write from the pointer.</param>
        /// <returns>The number of bytes written.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public unsafe virtual int Write(byte* bytes, int count)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (count < 0)
                throw new ArgumentOutOfRangeException("Must be >= 0", nameof(count));

            for (var offset = 0; offset < count; offset++)
                WriteByteInternal(bytes[offset]);

            if (AutoFlush)
                Flush();

            return count;
        }

        /// <summary>
        /// Writes every byte from the given stream, starting at its current position, to the internal
        /// buffer to be later written to the base stream. Leaves the source stream at its end when done.
        /// </summary>
        /// <param name="stream">The stream to write the bytes from. Must support reading.</param>
        /// <returns>The number of bytes written.</returns>
        /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading,
        ///     getting its position, and/or getting its length.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public int Write(Stream stream)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("Must support reading!", nameof(stream));

            bool BytesLeftInStream()
            {
                try
                {
                    return stream.Position < stream.Length;
                }
                catch (NotSupportedException e) when (!System.Diagnostics.Debugger.IsAttached)
                {
                    // Either stream.Position.get threw, or stream.Length.get did
                    try
                    {
                        var x = stream.Position;

                        // If we got this far we know it's stream.Length.get
                        throw new ArgumentException("Does not support getting Length!", nameof(stream), e);
                    }
                    catch (NotSupportedException)
                    {
                        throw new ArgumentException("Does not support getting Position!", nameof(stream), e);
                    }
                }
            }

            int bytesRead = 0;
            while (BytesLeftInStream())
            {
                bytesRead = stream.Read(Buffer, BufferIndex, Buffer.Length - BufferIndex);
                BufferIndex += bytesRead;

                if (BufferIndex >= Buffer.Length)
                    Flush();
            }

            return bytesRead;
        }

        /// <summary>
        /// Flushes the writer's internal buffer to the base stream.
        /// If <see cref="FlushBaseStream"/>, flushes the base stream as well.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual void Flush() => Flush(FlushBaseStream);

        /// <summary>
        /// Flushes the writer's internal buffer to the base stream.
        /// </summary>
        /// <param name="flushBaseStream">If true, the base stream will be flushed as well.
        ///     Overrides <see cref="FlushBaseStream"/>.</param>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual void Flush(bool flushBaseStream)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            BaseStream.Write(Buffer, 0, BufferIndex);
            BufferIndex = 0;

            if (flushBaseStream)
                BaseStream.Flush();
        }

        /// <summary>
        /// Closes the writer and its base stream, even if <see cref="LeaveOpen"/> is <see langword="true"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The writer and/or base stream has been disposed.</exception>
        public virtual void Close()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(null);

            BaseStream.Close();
        }

        /// <summary>
        /// Disposes of the writer, closing the base stream if <see cref="LeaveOpen"/> is <see langword="false"/>.
        /// If <see cref="Dispose"/> has not already been called, flushes the internal buffer.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Flush(false);

                Dispose(true);
                GC.SuppressFinalize(this);
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Disposes of the stream, closing the base stream if requested.
        /// </summary>
        /// <param name="disposing">Whether this was called during disposal (<see langword="true"/>)
        ///     or finalization (<see langword="false"/>)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !LeaveOpen)
                BaseStream?.Dispose();
        }
    }
}
