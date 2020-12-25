## Description
To write a console program in C #, designed for block-by-block compression and decompression of files using System.IO.Compression.GzipStream.

For compression, the source file is divided into blocks of the same size, for example, 1 megabyte. Each block is compressed and written to the output file, regardless of the other blocks.

The program should effectively parallelize and synchronize block processing in a multiprocessor environment and be able to handle files that are larger than the amount of available RAM.
In case of exceptional situations, it is necessary to inform the user with a clear message that allows the user to correct the problem, in particular if the problems are related to the limitations of the operating system.
When working with threads, it is allowed to use only standard classes and libraries from .Net 3.5 (excluding ThreadPool, BackgroundWorker, TPL). Expected to be implemented using Threads.
The program code must comply with the principles of OOP and OOD (readability, division into classes, etc.).

## Commands
`GZipTest.exe compress / decompress [source file name] [result file name]`
