namespace GZipTest
{
    public interface IDataTransformer
    {
        DataSegment TransformData(DataSegment dataSegment);
    }
}