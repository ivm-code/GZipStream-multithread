using System.IO;

namespace GZipTest
{
    public interface IFileParametersChecker
    {
        void CheckParameters(Stream filestream, OperationTypes workMode);
    }
}
