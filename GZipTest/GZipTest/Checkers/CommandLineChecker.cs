using System;
using System.IO;

namespace GZipTest
{
    public class CommandLineChecker : ICommandLineChecker
    {
        public OperationTypes WorkMode { get; private set; }

        public string InputFileName { get; private set; }

        public string OutputFileName { get; private set; }

        public bool CheckParameters(string[] args)
        {
            CheckArgumentsCount(args);
            SetCompressionMode(args);
            InputFileName = args[1].Trim();
            CheckInputFileExist(InputFileName);
            SetOutputFileName(args);
            CheckOutputFileExist(OutputFileName);
            return true;
        }

        private void CheckArgumentsCount(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                throw new Exception("Please enter arguments up to the following pattern:\n compress(decompress) [Source file] [Destination file].");
            }
        }

        private void SetCompressionMode(string[] args)
        {
            if (args[0].Trim().ToLower() == "compress")
            {
                WorkMode = OperationTypes.Compress;
                return;
            }
            if (args[0].Trim().ToLower() == "decompress")
            {
                WorkMode = OperationTypes.Decompress;
                return;
            }
            throw new Exception("Please enter arguments up to the following pattern:\n compress(decompress) [Source file] [Destination file].");
        }

        private void CheckInputFileExist(string inputFileName)
        {
            if (!File.Exists(inputFileName))
            {
                throw new Exception(string.Format("File '{0}' not found", OutputFileName));
            }
        }

        private void SetOutputFileName(string[] args)
        {
            if (args.Length == 2)
                OutputFileName = InputFileName + ".gz";
            else
                OutputFileName = args[2].Trim();
        }

        private void CheckOutputFileExist(string outputFileName)
        {
            if (File.Exists(outputFileName))
            {
                throw new Exception(string.Format("File '{0}' already exists", OutputFileName));
            }
        }     
    }
}