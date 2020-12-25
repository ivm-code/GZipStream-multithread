using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System;
using System.IO;

namespace GZipUnitTests
{
    [TestClass]
    public class CommandLineReaderTest
    {
        [TestInitialize]
        public void TestInit()
        {
            using (FileStream outputStream = File.OpenWrite("fileWithGzipSignature.gz"))
            {
                outputStream.Write(new byte[] { 0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 20);
            }
            using (FileStream outputStream = File.OpenWrite("fileWithoutGzipSignature.doc"))
            {
                outputStream.Write(new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff }, 0, 22);
            }
        }

        [TestCleanup()]
        public void Cleanup()
        {
            File.Delete("fileWithGzipSignature.gz");
            File.Delete("fileWithoutGzipSignature.doc");
        }

        [TestMethod]
        [DataRow(new string[] { "decompress" })]
        [DataRow(new string[] { "des", "fileWithoutGzipSignature.doc" })]
        [DataRow(new string[] { "decompress", "notExistingFile.gz", "notExistingFile.xyz" })]
        [DataRow(new string[] { "decompress", "fileWithGzipSignature.gz", "fileWithoutGzipSignature.doc" })]
        [DataRow(new string[] { "compress", "fileWithoutGzipSignature.doc", "fileWithGzipSignature.gz" })]
        public void CheckParameters_WrongFormat_ThrowsException(string[] args)
        {
            CommandLineChecker commandLineReader = new CommandLineChecker();

            Assert.ThrowsException<Exception>(() => commandLineReader.CheckParameters(args));
        }


        [TestMethod]
        [DataRow(new string[] { "decompress", "fileWithGzipSignature.gz", "notExistingFile.xyz" })]
        [DataRow(new string[] { "compress", "fileWithoutGzipSignature.doc" })]
        [DataRow(new string[] { "compress", "fileWithoutGzipSignature.doc", "notExistingFile.gz" })]
        public void CheckParameters_RightFormat_ReturnsTrue(string[] args)
        {
            CommandLineChecker commandLineReader = new CommandLineChecker();

            Assert.IsTrue(commandLineReader.CheckParameters(args));
        }
    }
}