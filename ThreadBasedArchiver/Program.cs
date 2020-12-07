using ThreadBasedArchiver.Utils;
using System;
using System.Diagnostics;
using System.IO;

namespace ThreadBasedArchiver
{
    public static class Program
    {
        private static GZipBase _gzipBase;
        public static int Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            try
            {
                Console.WriteLine("Please enter arguments by next mask:\ncompress [Source file] [Output file]\n\n");
                
                if (!ValidationArgs.IsArgumentsValid(args))
                    return 1;

                if (args[0].ToUpper() == "COMPRESS")
                    _gzipBase = new Compressor(args[1], args[2]);

                _gzipBase.Run();
                var result = _gzipBase.GetResult();
                timer.Stop();

                if (result == 1)
                {
                    Console.WriteLine("\nFailed result.");
                    Console.WriteLine("\nPress any key to exit.");
                    Console.ReadKey();
                    return 1;
                }

                ResultFileBuilder.BuildFile(args[2]);

                string elapsedTime =
                    $"{timer.Elapsed.Hours:00}:{timer.Elapsed.Minutes:00}:{timer.Elapsed.Seconds:00}.{timer.Elapsed.Milliseconds / 100:00}";

                Console.WriteLine(result == 0
                    ? $"\nSUCCESS in {elapsedTime}"
                    : $"\n{_gzipBase.GetType().Name} FAILED");

                return 0;        
            }
            catch (Exception ex)
            {
                timer.Stop();
                Console.WriteLine($"Error! App down in method: {ex.TargetSite} \n Message: {ex.Message}");
                Console.ReadKey();
                return 1;
            }
        }
    }
}
