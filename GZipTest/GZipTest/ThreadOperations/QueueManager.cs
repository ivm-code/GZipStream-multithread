using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{

    public class QueueManager : IQueueManager
    { 
        private readonly object _locker = new object();

        private readonly Queue<DataSegment> _queue = new Queue<DataSegment>();

        private bool _isStopRequired = false;

        private long _lastEnqueuedSegmentId;

        private readonly int _maxThreadsCount;

        public QueueManager(int maxThreadsCount)
        {
            _lastEnqueuedSegmentId = 0L;
            _maxThreadsCount = maxThreadsCount;
        }

        public bool IsDone()
        {
            lock (_locker)
            {
                return (_isStopRequired && (_queue.Count == 0));
            }
        }
        public void Enqueue(DataSegment task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            lock (_locker)
            {
                if (_isStopRequired)
                    throw new InvalidOperationException("Queue already stopped");

                while (_lastEnqueuedSegmentId != task.Id)
                {
                    Monitor.Wait(_locker);
                }
                while (_queue.Count > _maxThreadsCount)
                {
                    Monitor.Wait(_locker);
                }
                _queue.Enqueue(task);
                Interlocked.Increment(ref _lastEnqueuedSegmentId);
                Monitor.PulseAll(_locker);
            }
        }

        public DataSegment Dequeue()
        {
            lock (_locker)
            {
                while (_queue.Count == 0 && !_isStopRequired)
                    Monitor.Wait(_locker);
                if (_queue.Count == 0)
                    return null;

                DataSegment dataSegment = _queue.Dequeue();
                Monitor.PulseAll(_locker);
                return dataSegment;
            }
        }

        public void Stop()
        {
            lock (_locker)
            {
                _isStopRequired = true;
                Monitor.PulseAll(_locker);
            }
        }
    }
}