using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System.IO;
using System.Threading;

namespace GZipUnitTests
{
    [TestClass]
    public class CompressedSegmentReaderTests
    {
        private readonly byte _extraSegmentFlag = 0x04;

        private readonly byte _compressedSegmentSize = 0x16 - 0x0a;

        private readonly byte _segmentSizeFieldLength = 0x08;

        private byte[] _gzip4Segments;

        private readonly Thread[] _readers = new Thread[10];

        [TestMethod]
        public void ReadSegment_Read4SegmentsBy10Threads_4SegmentsReaded6ReturnsNull()
        {
            int segmentsReaded = 0;
            int nullResultReturned = 0;
            int errorCount = 0;
            _gzip4Segments = new byte[]
            {
                0x1f, 0x8b, 0x08, _extraSegmentFlag, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, _segmentSizeFieldLength, 0x00, _compressedSegmentSize, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0xf1,
                0x1f, 0x8b, 0x08, _extraSegmentFlag, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, _segmentSizeFieldLength, 0x00, _compressedSegmentSize, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf2, 0xf3,
                0x1f, 0x8b, 0x08, _extraSegmentFlag, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, _segmentSizeFieldLength, 0x00, _compressedSegmentSize, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf4, 0xf5,
                0x1f, 0x8b, 0x08, _extraSegmentFlag, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, _segmentSizeFieldLength, 0x00, _compressedSegmentSize, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf6, 0xf7
            };

            using Stream inputStream = new MemoryStream(_gzip4Segments);
            IDataSegmentReader segmentReader = new CompressedSegmentReader(inputStream, null);

            for (int i = 0; i < 10; i++)
            {
                _readers[i] = new Thread(() =>
                {
                    DataSegment segment = segmentReader.ReadSegment();
                    if (segment == null)
                        Interlocked.Increment(ref nullResultReturned);
                    else if (segment.Size == 0)
                        Interlocked.Increment(ref errorCount);
                    else
                        Interlocked.Increment(ref segmentsReaded);
                });
                _readers[i].Start();
            }
            foreach (Thread reader in _readers)
                reader.Join();

            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(4, segmentsReaded);
            Assert.AreEqual(6, nullResultReturned);
        }
    }
}
