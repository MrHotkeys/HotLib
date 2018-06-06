using System;
using System.Runtime.InteropServices;

namespace HotLib
{
    /// <summary>
    /// Wraps a pointer to a single value and allows accessing the individual bytes at the
    /// pointer via the indexer. Intended for making pointers usable within expression trees.
    /// </summary>
    /// <typeparam name="T">The type of the pointer. Must be unmanaged.</typeparam>
    public unsafe struct WrappedPointer<T>
        where T : unmanaged
    {
        /// <summary>
        /// Gets a <see cref="WrappedPointer{T}"/> that points to a single value of the
        /// wrapped pointer type (<typeparamref name="T"/>) on the unmanaged heap.
        /// </summary>
        /// <returns>The created <see cref="WrappedPointer{T}"/>.</returns>
        public static WrappedPointer<T> FromHAllocGlobal()
        {
            var pointer = (T*)Marshal.AllocHGlobal(sizeof(T));
            return new WrappedPointer<T>(pointer);
        }

        /// <summary>
        /// The wrapped pointer.
        /// </summary>
        public readonly T* Pointer;

        /// <summary>
        /// Gets the wrapped pointer as an <see cref="System.IntPtr"/>.
        /// </summary>
        public IntPtr IntPtr => (IntPtr)Pointer;

        /// <summary>
        /// Gets the value at the pointer.
        /// </summary>
        public T Value => *Pointer;

        /// <summary>
        /// Instantiates a new <see cref="WrappedPointer{T}"/>.
        /// </summary>
        /// <param name="pointer">The pointer to wrap.</param>
        public WrappedPointer(IntPtr pointer)
            : this((T*)pointer)
        { }
        
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

        /// <summary>
        /// Gets the value at the pointer as the given unmanaged type. 
        ///The pointer is cast to the given type and then dereferenced.
        /// </summary>
        /// <typeparam name="TAs">The type to get the value as.</typeparam>
        /// <returns>The value at the pointer as the given type.</returns>
        public TAs As<TAs>()
            where TAs : unmanaged
        {
            return *(TAs*)Pointer;
        }

        /// <summary>
        /// Implicitly casts the pointer as an <see cref="System.IntPtr"/>.
        /// </summary>
        /// <param name="wrapped">The wrapped pointer to cast.</param>
        public static implicit operator IntPtr(WrappedPointer<T> wrapped) => wrapped.IntPtr;
    }
}
