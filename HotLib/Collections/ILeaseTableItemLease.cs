using System;

namespace HotLib.Collections
{
    /// <summary>
    /// Defines public members for a lease to an item in an <see cref="ILeaseTable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item leased.</typeparam>
    public interface ILeaseTableItemLease<T> : IDisposable
    {
        /// <summary>
        /// Gets the ID of the leased item.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The lease has been disposed.</exception>
        int ID { get; }

        /// <summary>
        /// Gets the leased item.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The lease has been disposed.</exception>
        T Item { get; }

        /// <summary>
        /// Gets whether or not the lease has been disposed. Once disposed, the lease is no longer valid.
        /// </summary>
        bool IsDisposed { get; }
    }
}
