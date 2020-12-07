using System;
using System.IO;

namespace ThreadBasedArchiver.Utils
{
    /// <summary>
    /// Build result file by chunks
    /// </summary>
    public class ResultFileBuilder
    {
        public static void BuildFile(string path)
        {
            int chunkNumber = 0;
            while (true)
            {
                var outFileName = Path.GetExtension(path) == ".gz" ? path : path + ".gz";
                var chunkFileName = outFileName + $"_{chunkNumber}";
                if (!File.Exists(chunkFileName))
                    break;

                using (var outFile = new FileStream(outFileName, FileMode.Append))
                {
                    var chunk = File.ReadAllBytes(chunkFileName);
                    outFile.Write(chunk, 0, chunk.Length);
                }
                Console.WriteLine($"Deleting chunk {Path.GetFileName(chunkFileName)}");
                File.Delete(chunkFileName);
                chunkNumber++;
            }
            Console.WriteLine("The resulting file was created");
        }
    }
}
