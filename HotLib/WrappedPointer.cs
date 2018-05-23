namespace HotLib
{
    /// <summary>
    /// Wraps a pointer to a single value and allows accessing
    /// the individual bytes at the pointer via the indexer.
    /// </summary>
    /// <typeparam name="T">The type of the pointer. Must be unmanaged.</typeparam>
    public unsafe struct WrappedPointer<T>
        where T : unmanaged
    {
        /// <summary>
        /// The wrapped pointer.
        /// </summary>
        public readonly T* Pointer;
        
        /// <summary>
        /// Instantiates a new <see cref="WrappedPointer{T}"/>.
        /// </summary>
        /// <param name="pointer">The pointer to wrap.</param>
        public WrappedPointer(T* pointer)
        {
            Pointer = pointer;
        }

        /// <summary>
        /// Gets/Sets the byte at the given offset from the pointer.
        /// No range checking is performed to reduce overhead.
        /// </summary>
        /// <param name="offset">The offset from the pointer of the target byte.</param>
        /// <returns>The target byte.</returns>
        public byte this[int offset]
        {
            get => ((byte*)Pointer)[offset];
            set => ((byte*)Pointer)[offset] = value;
        }
    }
}
