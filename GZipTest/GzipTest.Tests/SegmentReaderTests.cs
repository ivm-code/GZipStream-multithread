using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System.IO;
using System.Threading;

namespace GZipUnitTests
{
    [TestClass]
    public class SegmentReaderTests
    {
        private readonly Thread[] _readers = new Thread[10];

        private readonly byte[] _simpleSegments  = new byte[1024*1024*3 + 512]; //3 segments 1Mb and 1 segment 512 bytes

       [TestMethod]
        public void ReadSegment_Read4SegmentsBy10Threads_4SegmentsReaded6ReturnsNull()
        {
            int segmentsReaded = 0;
            int nullResultReturned = 0;
            int errorCount = 0;
            
            using Stream inputStream = new MemoryStream(_simpleSegments);
            IDataSegmentReader segmentReader = new SegmentReader(inputStream, null);

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
