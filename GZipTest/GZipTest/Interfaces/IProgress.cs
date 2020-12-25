namespace GZipTest
{
    public interface IProgress
    {
        void ShowProgress(long current, long whole);
    }
}