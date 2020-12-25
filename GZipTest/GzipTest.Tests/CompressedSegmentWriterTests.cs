using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System.IO;
using System.Linq;

namespace GZipUnitTests
{
    [TestClass]
    public class CompressedSegmentWriterTests
    {
        private IDataSegmentWriter _dataSegmentWriter;

        private byte[] _outputData;

        private byte[] _gzipSegment;

        private byte[] _gzipModifiedSegment;

        [TestMethod]
        public void WriteSegment_Call__AddsExtraDataToOutputStream()
        {
            byte extraSegmentFlag = 0x04;
            byte compressedSegmentSize = 0x16 - 0x0a;
            byte _segmentSizeFieldLength = 0x08;
            _gzipSegment = new byte[] { 0x1f, 0x8b, 0x08, extraSegmentFlag, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0xf0, 0xf1 };
            _gzipModifiedSegment = new byte[] { 0x1f, 0x8b, 0x08, extraSegmentFlag, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, _segmentSizeFieldLength, 0x00, compressedSegmentSize, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0xf1 };
            _outputData = new byte[22];

            using Stream outputStream = new MemoryStream(_outputData);
                _dataSegmentWriter = new CompressedSegmentWriter(outputStream);
                _dataSegmentWriter.WriteSegment(_gzipSegment);

            Assert.IsTrue(Enumerable.SequenceEqual(_gzipModifiedSegment, _outputData));
        }
    }
}