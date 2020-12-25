using System;
using System.IO;

namespace GZipTest
{
    public class FileParametersChecker : IFileParametersChecker
    {
        private const int _minGzipHeaderSize = 10;

        private const byte _fextraFlag = 0x04;

        private const int _flagByteOffset = 3;

        private const int _segmentSizeFieldLengthOffset = 10;

        private const int _segmentSizeFieldLength = 8;

        public void CheckParameters(Stream filestream, OperationTypes workMode)
        {
            if (workMode != OperationTypes.Decompress)
            {
                return;
            }
            try
            {
                byte[] header = new byte[20];
                int bytesRead = filestream.Read(header, 0, 20);
                CheckIsFileInGzipFormat(
                    header,
                    bytesRead);
                CheckFileSize(
                    filestream,
                    header);
            }
            finally 
            {
                filestream.Seek(0, SeekOrigin.Begin);
            }
            
        }

        private void CheckIsFileInGzipFormat(
            byte[] header,
            int bytesRead)
        {
            if (bytesRead < _minGzipHeaderSize || !CheckGzipSignature(header))
            {
                throw new Exception("Input file is not in a gzip format");
            }
        }

        private void CheckFileSize(
            Stream stream,
            byte[] header)
        {
            if (stream.Length > 0x7FFFFFC7)
            {
                if (!CheckFEXTRAFlagSet(header[_flagByteOffset]) || !(CheckExtraSegmentLength(header)))
                {
                    throw new Exception("Input file can't be decompressed in multithread mode");
                }
            }
        }

        private bool CheckGzipSignature(byte[] header)
        {
            bool result = (header[0] == 0x1F && header[1] == 0x8B);
            return result;
        }

        private bool CheckFEXTRAFlagSet(byte value)
        {
            bool result = ((byte)(value & _fextraFlag) == _fextraFlag);
            return result;
        }

        private bool CheckExtraSegmentLength(byte[] header)
        {
            bool result = (BitConverter.ToInt16(header, _segmentSizeFieldLengthOffset) == _segmentSizeFieldLength);
            return result;
        }
    }
}
