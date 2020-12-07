using System;
using System.Collections.Generic;

namespace ThreadBasedArchiver
{
    public class QueueManager
    {
        private readonly Queue<int> _mainCompressQueue = new Queue<int>();
        private readonly object _myLocker = new object();

        internal int Count()
        {
            return _mainCompressQueue.Count;
        }

        public void Enqueue(int chunkNumber)
        {
            _mainCompressQueue.Enqueue(chunkNumber);
        }

        public int Dequeue()
        {
            lock (_myLocker)
            {
                if (_mainCompressQueue.Count == 0)
                    return -1;
                else
                    return _mainCompressQueue.Dequeue();
            }
        }
    }
}
