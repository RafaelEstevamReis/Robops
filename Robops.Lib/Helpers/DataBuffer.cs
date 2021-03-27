using System;
using System.Collections.Generic;
using System.Text;

namespace Robops.Lib.Helpers
{
    [Obsolete]
    public class DataBuffer<T>
    {
        public int Quantity { get; }
        public Action<IEnumerable<T>> FlushData { get; }

        List<T> queue;

        public DataBuffer(int quantity, Action<IEnumerable<T>> flushData)
        {
            Quantity = quantity;
            FlushData = flushData ?? throw new ArgumentNullException(nameof(flushData));
            queue = new List<T>(quantity + 2);
        }

        public void Add(T value)
        {
            queue.Add(value);

            if (queue.Count > Quantity)
            {
                Flush();
            }
        }

        public void Flush()
        {
            lock (queue)
            {
                FlushData(queue);
                queue.Clear();
            }
        }

    }
}
