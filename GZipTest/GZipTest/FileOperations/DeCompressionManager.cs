using System.IO;

namespace GZipTest
{
    public class DeCompressionManager : OperationManager
    {
        public DeCompressionManager(
            Stream inputStream, 
            Stream outputStream, 
            IProgress progressBar) : 
                base(
                    inputStream, 
                    outputStream, 
                    progressBar) { }

        protected override void Init()
        {
            _segmentReader = new CompressedSegmentReader(
                _inputStream,
                _progressBar);
            _segmentWriter = new SegmentWriter(_outputStream);
            _dataTransformer = new Decompressor();
        }
    }
}