using System.IO;
using System.Threading;

namespace GZipTest
{
    public class SegmentReader : IDataSegmentReader
    {
        private readonly IProgress _progressBar;

        private readonly Stream _inputStream;

        private long _currentSegmentNumber;

        private readonly object _locker = new object();

        private const int _buffSize = 1024 * 1024;

        private long _bytesReaded;

        public SegmentReader(
            Stream inputStream, 
            IProgress progressBar)
        {
            _progressBar = progressBar;
            _inputStream = inputStream;
            _currentSegmentNumber = 0;
        }

        public DataSegment ReadSegment()
        {
            lock (_locker)
            {
                if (_inputStream.Position == _inputStream.Length)
                {
                    return null;
                }
                byte[] buffer = new byte[_buffSize];
                int readLength = _inputStream.Read(
                    buffer, 
                    0, 
                    _buffSize);
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
    }
}