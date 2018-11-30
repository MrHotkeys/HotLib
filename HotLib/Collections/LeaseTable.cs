using System;
using System.Collections.Generic;

namespace HotLib.Collections
{
    /// <summary>
    /// A table to which items can be added and then leased, so that it can be known if those items are in use.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LeaseTable<T> : ILeaseTable<T>
    {
        /// <summary>
        /// An <see cref="ILeaseTableItemLease{T}"/> implementation for <see cref="LeaseTable{T}"/>.
        /// Represents a lease to an item in the table.
        /// </summary>
        protected class ItemLease : ILeaseTableItemLease<T>
        {
            /// <summary>
            /// Backs the <see cref="ID"/> property.
            /// </summary>
            private readonly int _id;
            /// <summary>
            /// Gets the ID of the leased item.
            /// </summary>
            /// <exception cref="ObjectDisposedException">The lease has been disposed.</exception>
            public int ID => !IsDisposed ? _id : throw new ObjectDisposedException(null);

            /// <summary>
            /// Backs the <see cref="Item"/> property.
            /// </summary>
            private readonly T _item;
            /// <summary>
            /// Gets the leased item.
            /// </summary>
            /// <exception cref="ObjectDisposedException">The lease has been disposed.</exception>
            public T Item => !IsDisposed ? _item : throw new ObjectDisposedException(null);

            /// <summary>
            /// Gets the table containing the leased item.
            /// </summary>
            protected LeaseTable<T> Table { get; }

            /// <summary>
            /// Gets whether or not the lease has been disposed. Once disposed, the lease is no longer valid.
            /// </summary>
            public bool IsDisposed { get; protected set; }

            /// <summary>
            /// Instantiates a new <see cref="ItemLease"/>.
            /// </summary>
            /// <param name="id">The ID of the leased item.</param>
            /// <param name="table">The table containing the leased item.</param>
            public ItemLease(int id, LeaseTable<T> table)
            {
                _id = id;
                Table = table ?? throw new ArgumentNullException(nameof(table));

                _item = Table[ID];
            }

            /// <summary>
            /// Finalizes the lease, disposing it.
            /// </summary>
            ~ItemLease() => Dispose(false);

            /// <summary>
            /// Disposes the lease.
            /// </summary>
            public void Dispose()
            {
                if (!IsDisposed)
                {
                    Dispose(true);
                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }

            /// <summary>
            /// Disposes the lease.
            /// </summary>
            /// <param name="disposing">Whether this was called during disposal (<see langword="true"/>)
            ///     or finalization (<see langword="false"/>).</param>
            protected virtual void Dispose(bool disposing)
            {
                Table.Release(ID);
            }
        }

        /// <summary>
        /// Indicates whether a spot in the table is filled with an item, that
        /// specific item, and how many active leases exist to that item.
        /// </summary>
        protected struct ItemContainer
        {
            /// <summary>
            /// The item in the container.
            /// </summary>
            public T Item;
            /// <summary>
            /// Whether or not the container is filled with an item.
            /// </summary>
            public bool Filled;
            /// <summary>
            /// The number of active leases to the item.
            /// </summary>
            public int ActiveLeases;
        }

        /// <summary>
        /// Gets the list of item containers that make up the table.
        /// </summary>
        protected List<ItemContainer> Containers { get; } = new List<ItemContainer>();

        /// <summary>
        /// Gets a stack of IDs that were used for items that have since been removed from the table.
        /// </summary>
        protected Stack<int> RecycledIDs { get; } = new Stack<int>();

        /// <summary>
        /// Gets/Sets whether to remove items when their active leases drop to 0.
        /// </summary>
        public bool RemoveWhenZeroLeases { get; set; } = false;

        /// <summary>
        /// Backs the <see cref="MaxLeases"/> property.
        /// </summary>
        private int? _maxLeases;
        /// <summary>
        /// Gets/Sets the max number of leases an item may have. If null, there is no max.
        /// Cannot be &lt; 1.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The new value is &lt; 1.</exception>
        public int? MaxLeases
        {
            get => _maxLeases;
            set
            {
                if (_maxLeases.HasValue && _maxLeases.Value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be > 0!");
                else
                    _maxLeases = value;
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="LeaseTable{T}"/>.
        /// </summary>
        public LeaseTable()
        { }

        /// <summary>
        /// Gets the item from the table with the given ID.
        /// </summary>
        /// <param name="id">The ID of the item to get.</param>
        /// <returns>Gets the item with the given ID.</returns>
        /// <exception cref="ArgumentException">No item exists in the table under the given ID.</exception>
        public virtual T this[int id] => GetContainer(id, true).Item;

        /// <summary>
        /// Adds an item to the table. Returns the ID of the item in the table.
        /// </summary>
        /// <param name="item">The item to add to the table.</param>
        /// <returns>The ID of the item in the table.</returns>
        public virtual int Add(T item)
        {
            var id = GetUnusedID();
            var container = new ItemContainer { Item = item, Filled = true, ActiveLeases = 1 };

            if (id < Containers.Count)
                Containers[id] = container;
            else
                Containers.Add(container);

            return id;
        }

        /// <summary>
        /// Adds an item to the table, and immediately creates a lease to that item.
        /// </summary>
        /// <param name="item">The item to add to the table and lease.</param>
        /// <returns>The created lease to the item.</returns>
        public virtual ILeaseTableItemLease<T> AddAndLease(T item)
        {
            var id = Add(item);
            return new ItemLease(id, this);
        }

        /// <summary>
        /// Leases an item in the table.
        /// </summary>
        /// <param name="id">the ID of the item to lease.</param>
        /// <returns>The created lease to the item.</returns>
        /// <exception cref="ArgumentException">No item exists in the table under the given ID.</exception>
        /// <exception cref="InvalidOperationException">Maximum leases exceeded for the item with the given ID.</exception>
        public virtual ILeaseTableItemLease<T> Lease(int id)
        {
            var container = GetContainer(id, true);
            container.ActiveLeases++;

            if (MaxLeases.HasValue && container.ActiveLeases > MaxLeases.Value)
                throw new InvalidOperationException($"Max leases of {MaxLeases.Value} exceeded for item {id} ({container.Item})!");

            Containers[id] = container;

            return new ItemLease(id, this);
        }

        /// <summary>
        /// Releases an active lease to the item with the given ID, decrementing its active leases count by 1.
        /// </summary>
        /// <param name="id">The ID of the item to release.</param>
        /// <exception cref="ArgumentException">No item exists in the table under the given ID.</exception>
        protected virtual void Release(int id)
        {
            var container = GetContainer(id, true);
            container.ActiveLeases--;

            if (RemoveWhenZeroLeases && container.ActiveLeases == 0)
            {
                container.Filled = false;
                container.Item = default;

                RecycledIDs.Push(id);
            }

            Containers[id] = container;
        }

        /// <summary>
        /// Removes all items in the table with zero uses.
        /// </summary>
        public virtual void RemoveItemsWithZeroLeases()
        {
            for (var id = 0; id < Containers.Count; id++)
            {
                var container = Containers[id];

                if (container.ActiveLeases == 0)
                {
                    container.Filled = false;
                    container.Item = default;

                    Containers[id] = container;

                    RecycledIDs.Push(id);
                }
            }
        }

        /// <summary>
        /// Gets an unused item ID, either from <see cref="RecycledIDs"/> or whatever
        /// the next ID in <see cref="Containers"/> would be if one was added.
        /// </summary>
        /// <returns>The unused ID.</returns>
        protected virtual int GetUnusedID()
        {
            if (RecycledIDs.Count > 0)
                return RecycledIDs.Pop();
            else
                return Containers.Count;
        }

        /// <summary>
        /// Gets the container for the given ID.
        /// </summary>
        /// <param name="id">The ID to get the container for.</param>
        /// <param name="enforceFilled">If true, a <see cref="ArgumentException"/>
        ///     will be thrown if the container is empty.</param>
        /// <returns>The container for the given ID.</returns>
        /// <exception cref="ArgumentException">No item exists in the table under the given ID.</exception>
        protected virtual ItemContainer GetContainer(int id, bool enforceFilled)
        {
            if (id < 0 || id >= Containers.Count)
                throw new ArgumentException($"No item in table with ID {id} (out of table range)!", nameof(id));

            var container = Containers[id];

            if (enforceFilled && !container.Filled)
                throw new ArgumentException($"No item in table with ID {id} (no longer exists)!", nameof(id));

            return container;
        }
    }
}
