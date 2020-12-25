using System;
using System.IO;
using System.Threading;

namespace GZipTest
{
    public class CompressedSegmentReader : IDataSegmentReader
    {
        private const byte _fextraFlag = 0x04;

        private const int _flagByteOffset = 3;

        private const int _headerLength = 20;

        private const int _extraSegmentSize = 10;

        private const int _segmentSizeFieldLengthOffset = 10;

        private const int _segmentSizeFieldLength = 8;

        private const int _compressedSegmentSizeOffset = 12;

        private readonly IProgress _progressBar;

        private readonly Stream _inputStream;

        private long _currentSegmentNumber = 0;

        private long _bytesReaded;

        private readonly object _locker = new object();

        public CompressedSegmentReader(
            Stream inputStream, 
            IProgress progressBar)
        {
            _progressBar = progressBar;
            _inputStream = inputStream;
        }

        public DataSegment ReadSegment()
        {
            lock (_locker)
            {
                if (_inputStream.Position == _inputStream.Length)
                {
                    return null;
                }
                int buffSize = GetCompressedSegmentLength();
                byte[] buffer = new byte[buffSize];
                _inputStream.Seek(-20, SeekOrigin.Current);
                int readLength = _inputStream.Read(buffer, 0, buffSize);
                DataSegment currentSegment = new DataSegment(
                    _currentSegmentNumber, 
                    readLength)
                {
                    Data = buffer
                };
                Interlocked.Increment(ref _currentSegmentNumber);
                Interlocked.Add(
                    ref _bytesReaded, 
                    readLength);
                if (_progressBar != null) 
                    _progressBar.ShowProgress(
                        _bytesReaded, 
                        _inputStream.Length);
                return currentSegment;
            }
        }

        private int GetCompressedSegmentLength()
        {
            byte[] header = ReadHeader();
            if (CheckFexraFlagSet(header[_flagByteOffset]) && CheckExtraSegmentLength(header))
                return (BitConverter.ToInt32(header, _compressedSegmentSizeOffset) + _extraSegmentSize);
            return (int)_inputStream.Length;
        }

        private byte[] ReadHeader()
        {
            byte[] header = new byte[_headerLength];
            _inputStream.Read(
                header, 
                0, 
                _headerLength);
            return header;
        }

        private bool CheckFexraFlagSet(byte fextraByte)
        {
            bool result = ((byte)(fextraByte & _fextraFlag) == _fextraFlag);
            return result;
        }

        private bool CheckExtraSegmentLength(byte[] header)
        {
            bool result = (BitConverter.ToInt16(header, _segmentSizeFieldLengthOffset) == _segmentSizeFieldLength);
            return result;
        }
    }
}