using System.IO;
using System.IO.Compression;

namespace RandomNameGeneratorLibrary
{
    public class FileCompressor
    {
        public void CompressStringToFile(string sourceText, string outputFilePath)
        {
            using (
                var streamWriter =
                    new StreamWriter(new DeflateStream(
                        new FileStream(outputFilePath, FileMode.Create, FileAccess.Write), CompressionMode.Compress)))
            {
                streamWriter.Write(sourceText);
                streamWriter.Flush();
            }
        }

        private static void WriteSourceStreamToTargetStream(Stream sourceStream, Stream targetStream)
        {
            var buffer = new byte[1024];
            int count;
            while ((count = sourceStream.Read(buffer, 0, 1024)) != 0)
                targetStream.Write(buffer, 0, count);
        }

        public string DecompressFileToString(string inputFilePath)
        {
            return
                new StreamReader(new DeflateStream(new FileStream(inputFilePath, FileMode.Open),
                    CompressionMode.Decompress))
                    .ReadToEnd();
        }
    }
}