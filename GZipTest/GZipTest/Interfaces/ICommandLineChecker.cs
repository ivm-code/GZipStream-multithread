namespace GZipTest
{
    interface ICommandLineChecker
    {
        OperationTypes WorkMode { get; }
        
        string InputFileName { get;  }
        
        string OutputFileName { get; }

        bool CheckParameters(string[] args);
    }
}