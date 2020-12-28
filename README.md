## Description
Develop a C # console application to compress and decompress files block-by-block using System.IO.Compression.GzipStream.
For compression, the original file is divided into blocks of the same size, for example, 1 megabyte. Each block is compressed and written to the output file independently of the rest of the blocks.
The program must effectively parallelize and synchronize the processing of blocks in a multiprocessor environment and be able to process files that are larger than the amount of available RAM.
In case of exceptional situations, it is necessary to inform the user with a clear message that allows the user to correct the problem that has arisen, in particular, if the problems are related to the limitations of the operating system.
When working with threads, it is allowed to use only base classes and synchronization objects (Thread, Manual / AutoResetEvent, Monitor, Semaphor, Mutex) and it is not allowed to use async / await, ThreadPool, BackgroundWorker, TPL.
The program code must comply with the principles of OOP and OOD (readability, division into classes, etc.).
On success, the program should return 0, on error, return 1.
Note: the format of the archive does not matter for assessing the quality of the test, in particular, compliance with the GZIP format is optional.

## Commands
`GZipTest.exe compress / decompress [source file name] [result file name]`
