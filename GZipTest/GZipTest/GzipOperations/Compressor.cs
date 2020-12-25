using System.IO;
using System.IO.Compression;

namespace GZipTest
{

    public class Compressor : IDataTransformer
    {
        public DataSegment TransformData(DataSegment dataSegment)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(
                    outputStream,
                    CompressionMode.Compress))
                {
                    gZipStream.Write(
                    dataSegment.Data,
                    0,
                    dataSegment.Size);
                }
                dataSegment.Data = outputStream.ToArray();
            }
            return dataSegment;
        }
    }
}