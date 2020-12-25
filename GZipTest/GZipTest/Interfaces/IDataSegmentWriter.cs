namespace GZipTest
{
    public interface IDataSegmentWriter
    {
        void WriteSegment(byte[] block);
    }
}