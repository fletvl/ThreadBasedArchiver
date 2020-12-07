using System;
using System.IO;

namespace ThreadBasedArchiver
{
    /// <summary>
    /// Check arguments on validation
    /// </summary>
    public class ValidationArgs
    {
        public static bool IsArgumentsValid(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Arguments are empty");
                return false;
            }

            if (args.Length != 3)
            {
                Console.WriteLine("Please enter arguments by next mask:\ncompress | decompress [Source file] [Output file]");
                return false;
            }

            if (args[0].ToUpper() != "COMPRESS" && args[0].ToUpper() != "DECOMPRESS")
            {
                Console.WriteLine("First argument should be \"compress\" or \"decompress\".");
                return false;
            }

            if (args[1].Length == 0)
            {
                Console.WriteLine("Source file not entered.");
                return false;
            }

            if (args[2].Length == 0)
            {
                Console.WriteLine("Output file not entered.");
                return false;
            }

            args[1] = args[1].Trim('\"');
            args[2] = args[2].Trim('\"');

            if (!File.Exists(args[1]))
            {
                Console.WriteLine("Source file is not found.");
                return false;
            }

            FileInfo _fileIn = new FileInfo(args[1]);
            FileInfo _fileOut;

            if (args[0].ToUpper() == "COMPRESS")
            {
                _fileOut = new FileInfo(Path.GetExtension(args[2]) == ".gz" ? args[2] : args[2] + ".gz");
            }
            else
                _fileOut = new FileInfo(args[2]);
            
            if (args[1] == args[2])
            {
                Console.WriteLine("Source and output files should be different.");
                return false;
            }

            if (_fileIn.Extension == ".gz" && args[0].ToUpper() == "COMPRESS")
            {
                Console.WriteLine("Source file already compressed.");
                return false;
            }

            if (_fileOut.Extension == ".gz" && _fileOut.Exists)
            {
                Console.WriteLine("Output file already exists.");
                return false;
            }

            if (_fileIn.Extension != ".gz" && args[0].ToUpper() == "DECOMPRESS")
            {
                Console.WriteLine("Source file must have .gz extension.");
                return false;
            }         

            return true;
        }
    }
}
