using System.IO;
using System.IO.Compression;

namespace CensusTools
{
    /// <summary>Deflate helper kept from the original tooling; the library no longer ships compressed resources.</summary>
    public static class FileCompressor
    {
        public static void CompressStringToFile(string sourceText, string outputFilePath)
        {
            using var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
            using var deflateStream = new DeflateStream(fileStream, CompressionMode.Compress);
            using var streamWriter = new StreamWriter(deflateStream);
            streamWriter.Write(sourceText);
        }

        public static string DecompressFileToString(string inputFilePath)
        {
            using var fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read);
            using var deflateStream = new DeflateStream(fileStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(deflateStream);
            return streamReader.ReadToEnd();
        }
    }
}
