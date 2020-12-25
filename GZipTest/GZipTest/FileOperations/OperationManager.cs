using System.IO;

namespace GZipTest
{
    public abstract class OperationManager : IOperationManager
    {
        protected IDataSegmentReader _segmentReader;

        protected IDataSegmentWriter _segmentWriter;

        protected IDataTransformer _dataTransformer;

        protected Stream _inputStream;

        protected Stream _outputStream;

        protected IProgress _progressBar;

        public DataSegment Get()
        {
            return _segmentReader.ReadSegment();
        }

        public DataSegment Operate(DataSegment dataSegment)
        {
            return _dataTransformer.TransformData(dataSegment);
        }

        public void Save(DataSegment block)
        {
            _segmentWriter.WriteSegment(block.Data);
        }

        public OperationManager(
            Stream inputStream, 
            Stream outputStream, 
            IProgress progressBar)
        {
            _inputStream = inputStream;

            _outputStream = outputStream;

            _progressBar = progressBar;

            Init();
        }

        protected abstract void Init();
    }
}