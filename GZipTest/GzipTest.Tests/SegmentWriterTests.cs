using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System.IO;
using System.Linq;

namespace GZipUnitTests
{
    [TestClass]
    public class SegmentWriterTests
    {
        private IDataSegmentWriter _dataSegmentWriter;

        private byte[] _outputData;

        private byte[] _inputData;

        [TestMethod]
        public void WriteSegment_Call_WritesSameDataToOutputStream()
        {
            _inputData =  new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; ;
            _outputData = new byte[20];

            using Stream outputStream = new MemoryStream(_outputData);
            _dataSegmentWriter = new SegmentWriter(outputStream);
            _dataSegmentWriter.WriteSegment(_inputData);

            Assert.IsTrue(Enumerable.SequenceEqual(_inputData, _outputData));
        }
    }
}