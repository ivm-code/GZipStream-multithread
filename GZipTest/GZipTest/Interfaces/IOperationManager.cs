namespace GZipTest
{
    public interface IOperationManager 
    {
        DataSegment Get();

        void Save(DataSegment block);

        DataSegment Operate(DataSegment dataSegment);
    }
}