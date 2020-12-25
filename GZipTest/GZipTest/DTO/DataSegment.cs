namespace GZipTest
{
    public class DataSegment
    {
        public long Id { get; }

        public int Size { get; }

        public byte[] Data { get; set; }

        public DataSegment(long id, int size)
        {
            Id = id;
            Size = size;
        }
    }
}