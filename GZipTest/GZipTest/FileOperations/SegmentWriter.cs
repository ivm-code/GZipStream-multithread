using System.IO;

namespace GZipTest
{
    public class SegmentWriter : IDataSegmentWriter
    {
        private readonly Stream _outputStream;

        public SegmentWriter(Stream outputStream)
        {
            _outputStream = outputStream;
        }

        public void WriteSegment(byte[] block)
        {
            _outputStream.Write(
                block,
                0,
                block.Length);
        }
    }
}