// File: StringCompress.cs

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Posh2Exe
{
    public class TextCompress
    {

        /// <summary>
        /// Compresses the content of a file and returns the path of the compressed file
        /// </summary>
        /// <param name="UncompressedString"></param>
        /// <returns></returns>
        public static string CompressFile(string Ps1Path)
        {
            string tmpFilePath = string.Empty;

            using (StreamReader sr = new StreamReader(Ps1Path))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(sr.ReadToEnd());
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    gZipStream.Write(buffer, 0, buffer.Length);
                memoryStream.Position = 0;

                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);
                var gZipBuffer = new byte[compressedData.Length + 4];
                // First 4 bytes are the size of the buffer
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                // Now copy the rest of the buffer
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                // Get a tmp file path
                tmpFilePath = Path.GetTempFileName();
                // Open a StreamWriter to write everything into that file
                using (StreamWriter sw = new StreamWriter(tmpFilePath))
                    sw.WriteLine(Convert.ToBase64String(gZipBuffer));
            }
            return tmpFilePath;
        }

        /// <summary>
        /// Compresses the content of a stream and returns the path of the compressed file
        /// </summary>
        /// <param name="Ps1Stream"></param>
        /// <returns></returns>
        public static string CompressStream(Stream Ps1Stream)
        {
            string tmpFilePath = string.Empty;

            using (StreamReader sr = new StreamReader(Ps1Stream))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(sr.ReadToEnd());
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    gZipStream.Write(buffer, 0, buffer.Length);
                memoryStream.Position = 0;

                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);
                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                tmpFilePath = Path.GetTempFileName();
                using (StreamWriter sw = new StreamWriter(tmpFilePath))
                    sw.WriteLine(Convert.ToBase64String(gZipBuffer));
            }
            return tmpFilePath;
        }

        /// <summary>
        /// Returns the path of the decompressed file
        /// </summary>
        /// <param name="CompressPath"></param>
        /// <returns></returns>
        public static string DecompressFile(string CompressPath)
        {
            string tmpPath = String.Empty;
            // Open the compressed file
            using (StreamReader sr = new StreamReader(CompressPath))
            {
                // read the file content into a buffer
                byte[] gZipBuffer = Convert.FromBase64String(sr.ReadToEnd());
                // the first 4 bytes are the buffer size
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                // the the rest into a buffer
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                    var buffer = new byte[dataLength];
                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }
                    // write the buffer into a file
                    tmpPath = Path.GetTempFileName();
                    using (StreamWriter sw = new StreamWriter(tmpPath))
                    {
                        sw.WriteLine(Encoding.UTF8.GetString(buffer));
                    }
                }
            }
            return tmpPath;
        }

        /// <summary>
        /// Decompresses a compressed stream and returns the decompressed text
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string DecompressStream(Stream stream)
        {
            string tmpPath = String.Empty;
            using (StreamReader sr = new StreamReader(stream))
            {
                byte[] gZipBuffer = Convert.FromBase64String(sr.ReadToEnd());
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                    var buffer = new byte[dataLength];
                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }
                    return Encoding.UTF8.GetString(buffer);
                }
            }
        }
    }
}
