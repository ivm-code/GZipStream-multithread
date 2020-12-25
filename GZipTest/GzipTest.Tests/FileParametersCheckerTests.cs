using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System;

namespace GZipUnitTests
{
    [TestClass]
    public class FileParametersCheckerTests
    {
        readonly byte[] _gzipHeader = new byte[] { 0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        readonly byte[] _notGzipHeader = new byte[] { 0x11, 0x81, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        [TestMethod]
        public void CheckParameters_FileConsistsOf1HugeSegment_ThrowException()
        {
            using MemoryStreamStub fileStreamStub = new MemoryStreamStub(_gzipHeader);
            FileParametersChecker fileChecker = new FileParametersChecker();

            Assert.ThrowsException<Exception>(() => fileChecker.CheckParameters(fileStreamStub, OperationTypes.Decompress));
        }

        [TestMethod]
        public void CheckParameters_HasNotGzipSignature_ThrowException()
        {
            using MemoryStreamStub fileStreamStub = new MemoryStreamStub(_notGzipHeader);
            FileParametersChecker fileChecker = new FileParametersChecker();

            Assert.ThrowsException<Exception>(() => fileChecker.CheckParameters(fileStreamStub, OperationTypes.Decompress));
        }
    }
}