using System;
using System.IO;

namespace GZipTest
{
    class Program
    {
        static readonly IQueueManager queueManager = new QueueManager(Environment.ProcessorCount / 2);

        static int Main(string[] args)
        {
            IThreadsManager threadsManager;
            IProgress progressBar = new ProgressBar();
            IOperationManager operationManager;
            ICommandLineChecker commandLineReader = new CommandLineChecker();
            IFileParametersChecker fileParametersChecker = new FileParametersChecker();
            try
            {
                commandLineReader.CheckParameters(args);
                using FileStream inputStream = File.OpenRead(commandLineReader.InputFileName);
                fileParametersChecker.CheckParameters(inputStream, commandLineReader.WorkMode);
                using FileStream outputStream = File.OpenWrite(commandLineReader.OutputFileName);
                if (commandLineReader.WorkMode == OperationTypes.Compress)
                    operationManager = new CompressionManager(
                        inputStream,
                        outputStream,
                        progressBar);
                else
                    operationManager = new DeCompressionManager(
                        inputStream,
                        outputStream,
                        progressBar);
                threadsManager = new ThreadsManager(
                    operationManager,
                    queueManager,
                    Environment.ProcessorCount);
                threadsManager.Start();
                return 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine(
                    string.Format(
                        "Programm terminated\nMethod: {0}\n Error description: {1}",
                        exception.TargetSite,
                        exception.Message));
                return 1;
            }
        }
    }
}