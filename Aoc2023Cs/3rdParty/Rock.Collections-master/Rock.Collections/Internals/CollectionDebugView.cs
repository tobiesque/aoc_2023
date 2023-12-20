using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rock.Collections.Internals
{
    public sealed class CollectionDebugView<T>
    {
        private readonly ICollection<T> m_collection;

        public CollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            m_collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] items = new T[m_collection.Count];
                m_collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}