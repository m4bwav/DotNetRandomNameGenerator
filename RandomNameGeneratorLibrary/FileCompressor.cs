using System;
using System.IO;
using System.IO.Compression;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Deflate helper once used to shrink the census lists. It is not part of the name-generating API and will
    /// be removed in 3.0; a maintained copy lives in the repository's <c>tools/CensusTools</c> console project.
    /// </summary>
    [Obsolete("FileCompressor is a build-time tool, not part of the name-generating API, and will be removed in 3.0. The maintained copy is the tools/CensusTools console project in the repository (https://github.com/m4bwav/DotNetRandomNameGenerator).")]
    public class FileCompressor
    {
        /// <summary>Writes <paramref name="sourceText"/> to <paramref name="outputFilePath"/> as a Deflate stream.</summary>
        /// <param name="sourceText">The text to compress.</param>
        /// <param name="outputFilePath">The file to create or overwrite.</param>
        public void CompressStringToFile(string sourceText, string outputFilePath)
        {
            using (var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (var deflateStream = new DeflateStream(fileStream, CompressionMode.Compress))
            using (var streamWriter = new StreamWriter(deflateStream))
            {
                streamWriter.Write(sourceText);
                streamWriter.Flush();
            }
        }

        /// <summary>Reads a Deflate stream written by <see cref="CompressStringToFile"/> back into a string.</summary>
        /// <param name="inputFilePath">The compressed file.</param>
        public string DecompressFileToString(string inputFilePath)
        {
            using (var fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (var deflateStream = new DeflateStream(fileStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(deflateStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
