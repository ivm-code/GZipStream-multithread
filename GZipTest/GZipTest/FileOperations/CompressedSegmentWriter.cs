using System;
using System.IO;

namespace GZipTest
{
    
    public class CompressedSegmentWriter : IDataSegmentWriter
    {
        private readonly Stream _outputStream;

        private const int _headerSize = 20;

        private const int _gzipSignatureSize = 10;

        private const int _extraFieldSizeOffset = 10;

        private const int _segmentSizeFieldLength = 8;

        private const int _segmentSizeFieldOffset = 12;

        private readonly byte[] _header; 

        public CompressedSegmentWriter(Stream outputStream)
        {
            _outputStream = outputStream;
            _header = new byte[_headerSize];
            _header[_extraFieldSizeOffset] = _segmentSizeFieldLength;
        }

        public void WriteSegment(byte[] block)
        {
            SetHeader(block);
            _outputStream.Write(_header, 0, _headerSize);
            _outputStream.Write(block, _gzipSignatureSize, block.Length - _gzipSignatureSize);
        }

        private void SetHeader(byte[] block)
        {
            block[3] = (byte)(block[3] | (byte)0x04);
            Array.Copy(block, 0, _header, 0, _gzipSignatureSize);
            byte[] bytes = BitConverter.GetBytes((long)block.Length);
            Array.Copy(bytes, 0, _header, _segmentSizeFieldOffset, _segmentSizeFieldLength);
        }
    }
}