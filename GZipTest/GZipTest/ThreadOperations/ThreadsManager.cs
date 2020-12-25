using System.Threading;
using System;

namespace GZipTest
{
    public class ThreadsManager : IThreadsManager
    {
        private readonly IOperationManager _operationManager;

        private readonly IQueueManager _queueManager;

        private readonly object _locker = new object();

        private Thread _writer;

        private Thread[] _readers;

        private readonly int _maxThreadsCount;

        public ThreadsManager(
            IOperationManager operationManager, 
            IQueueManager queueManager, 
            int maxThreadsCount)
        {
            _queueManager = queueManager;
            _operationManager = operationManager;
            _maxThreadsCount = maxThreadsCount;
        }

        public void Start()
        {
            try
            {
                _readers = new Thread[_maxThreadsCount];
                for (int i = 0; i < _maxThreadsCount; i++)
                {
                    _readers[i] = new Thread(GetData);
                    _readers[i].Start();
                }
                _writer = new Thread(SaveData);
                _writer.Start();
                foreach (Thread reader in _readers)
                    reader.Join();
                _queueManager.Stop();
                _writer.Join();
            }
            catch (Exception exception) 
            {
                while (exception.InnerException != null)
                    exception = exception.InnerException;
                throw new Exception(String.Format("Error while start threads\nInner exception:", exception.Message));
            }
        }

        public void Stop()
        {
            _queueManager.Stop();
        }

        private void GetData()
        {
            DataSegment currentSegment = _operationManager.Get();
            while (currentSegment != null)
            {
                DataSegment compressedData = _operationManager.Operate(currentSegment);
                _queueManager.Enqueue(compressedData);
                currentSegment = _operationManager.Get();
            }
        }

        private void SaveData()
        {
            while (!_queueManager.IsDone())
            {
                DataSegment currentSegment = _queueManager.Dequeue();
                if (currentSegment != null)
                    _operationManager.Save(currentSegment);
            }
        }
    }
}