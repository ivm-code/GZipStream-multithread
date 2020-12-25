using System;
using System.IO;

namespace GZipUnitTests
{
    public class MemoryStreamStub : Stream
    {
        private readonly byte[] _bytesForReturn;

        public MemoryStreamStub(byte[] bytesForReturn)
            : base()
        {
            _bytesForReturn = bytesForReturn;
        }
        public override long Length => 0x7FFFFFC7 + 1;
        public override bool CanRead => true;

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int size)
        {
            Array.Copy(_bytesForReturn, offset, buffer, 0, size);
            return 20;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return offset;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}