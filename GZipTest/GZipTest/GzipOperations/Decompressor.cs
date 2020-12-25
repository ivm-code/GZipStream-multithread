using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public class Decompressor : IDataTransformer
    {
        public DataSegment TransformData(DataSegment dataSegment)
        {
            using MemoryStream inputStream = new MemoryStream(dataSegment.Data);
            using MemoryStream outputStream = new MemoryStream();
            using GZipStream gZipStream = new GZipStream(
                    inputStream,
                    CompressionMode.Decompress);
            gZipStream.CopyTo(outputStream);
            dataSegment.Data = outputStream.ToArray();
            return dataSegment;
        }
    }
}