using System.IO;

namespace GZipTest
{
    public class CompressionManager : OperationManager
    {
        public CompressionManager(
            Stream inputStream, 
            Stream outputStream, 
            IProgress progressBar) : 
                base(
                    inputStream, 
                    outputStream, 
                    progressBar) { }

        protected override void Init()
        {
            _segmentReader = new SegmentReader(
                _inputStream, 
                _progressBar);
            _segmentWriter = new CompressedSegmentWriter(_outputStream);
            _dataTransformer = new Compressor();
        }
    }
}