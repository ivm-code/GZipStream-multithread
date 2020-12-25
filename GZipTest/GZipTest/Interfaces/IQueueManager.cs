namespace GZipTest
{
    public interface IQueueManager
    {
        bool IsDone();

        void Enqueue(DataSegment task);

        DataSegment Dequeue();

        void Stop();
    }
}