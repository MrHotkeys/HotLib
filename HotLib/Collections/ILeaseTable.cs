using System;

namespace HotLib.Collections
{
    /// <summary>
    /// Defines public members for a lookup table where items can be added and leased, so that it can be known if those items are in use.
    /// </summary>
    /// <typeparam name="T">The type of item in the table.</typeparam>
    public interface ILeaseTable<T>
    {
        /// <summary>
        /// Gets the item from the table with the given ID.
        /// </summary>
        /// <param name="id">The ID of the item to get.</param>
        /// <returns>Gets the item with the given ID.</returns>
        /// <exception cref="ArgumentException">No item exists in the table under the given ID.</exception>
        T this[int id] { get; }

        /// <summary>
        /// Adds an item to the table. Returns the ID of the item in the table.
        /// </summary>
        /// <param name="item">The item to add to the table.</param>
        /// <returns>The ID of the item in the table.</returns>
        int Add(T item);

        /// <summary>
        /// Adds an item to the table, and immediately creates a lease to that item.
        /// </summary>
        /// <param name="item">The item to add to the table and lease.</param>
        /// <returns>The created lease to the item.</returns>
        ILeaseTableItemLease<T> AddAndLease(T item);

        /// <summary>
        /// Leases an item in the table.
        /// </summary>
        /// <param name="id">the ID of the item to lease.</param>
        /// <returns>The created lease to the item.</returns>
        /// <exception cref="ArgumentException">No item exists in the table under the given ID.</exception>
        ILeaseTableItemLease<T> Lease(int id);
    }
}