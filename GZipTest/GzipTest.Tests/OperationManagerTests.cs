using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using Moq;
using System.IO;
using System.Linq;
using System;

namespace GZipUnitTests
{
    [TestClass]
    public class OperationManagerTests
    {
        private IOperationManager _compressor;

        private IOperationManager _decompressor;

        private Mock<Stream> _inputStream;

        private Mock<Stream> _outputStream;

        private Mock<IProgress> _progressBar;

        private byte[] _gzipHeader;

        private byte[] _initialData;

        private byte[] _outputData;

        private byte[] _gzipSegment;

        [TestInitialize]
        public void Init()
        {
            byte extraSegmentFlag = 0x04;
            byte compressedSegmentSize = 0x16 - 0x0a;
            byte _segmentSizeFieldLength = 0x08;
            _gzipHeader =  new byte[] { 0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            _initialData = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff };
            _gzipSegment = new byte[] { 0x1f, 0x8b, 0x08, extraSegmentFlag, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, _segmentSizeFieldLength, 0x00, compressedSegmentSize, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0xf1 };
            _outputData = new byte[30];
            _inputStream = new Mock<Stream>();
            _inputStream
                .Setup(stream => stream.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);
            _inputStream
                .Setup(stream => stream.Length)
                .Returns(1);
            _outputStream = new Mock<Stream>();
            _outputStream
                .Setup(a => a.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()));
            _progressBar = new Mock<IProgress>();
        }


        [TestMethod]
        public void Save_CallfromDecompressor_WritesSameDataToOutputStream()
        {
            DataSegment dataSegment = new DataSegment(0, 22) { Data = _initialData };

            using Stream outputStream = new MemoryStream(_outputData);
            _decompressor = new DeCompressionManager(
                _inputStream.Object,
                outputStream,
                _progressBar.Object);
            _decompressor.Save(dataSegment);

            Assert.AreEqual(_initialData[0], _outputData[0]);
        }


        [TestMethod]
        public void Save_CallfromCompressor_SetFextraFlagToOutputStream()
        {
            DataSegment dataSegment = new DataSegment(0, 22) { Data = _gzipHeader };

            using Stream outputStream = new MemoryStream(_outputData);
            _compressor = new CompressionManager(
                _inputStream.Object,
                outputStream,
                _progressBar.Object);
            _compressor.Save(dataSegment);

            Assert.AreEqual(0x04, _outputData[3]);
            Assert.AreEqual(0x08, _outputData[10]);
        }


        [TestMethod]
        public void Get_CallfromCompressor_ReturnBytesFromStream()
        {
            using Stream inputStream = new MemoryStream(_initialData);
            _compressor = new CompressionManager(
                inputStream,
                _outputStream.Object,
                _progressBar.Object);

            DataSegment seg = _compressor.Get();
            byte[] output = new byte[22];
            Array.Copy(seg.Data, output, 22);

            Assert.AreEqual(22, seg.Size);
            Assert.IsTrue(Enumerable.SequenceEqual(output, _initialData));
        }

        [TestMethod]
        public void Get_CallfromDecompressor_ReturnBytesFromStream()
        {
            using Stream inputStream = new MemoryStream(_gzipSegment);
            _decompressor = new DeCompressionManager(
                inputStream,
                _outputStream.Object,
                _progressBar.Object);

            DataSegment seg = _decompressor.Get();

            Assert.AreEqual(22, seg.Size);
            Assert.IsTrue(Enumerable.SequenceEqual(_gzipSegment, seg.Data));
        }

        [TestMethod]
        public void Operate_CompressDecompress_AreEqual()
        {
            _compressor = new CompressionManager(
                _inputStream.Object,
                _outputStream.Object,
                _progressBar.Object);
            _decompressor = new DeCompressionManager(
                _inputStream.Object,
                _outputStream.Object,
                _progressBar.Object);

            DataSegment compressed = _compressor.Operate(
                new DataSegment(0, 22)
                {
                    Data = _initialData
                });
            DataSegment dataSegment = new DataSegment(0, compressed.Data.Length)
            {
                Data = compressed.Data
            };
            DataSegment decompressed = _decompressor.Operate(dataSegment);

            Assert.IsNotNull(decompressed);
            Assert.IsTrue(Enumerable.SequenceEqual(_initialData, decompressed.Data));
        }

    }
}