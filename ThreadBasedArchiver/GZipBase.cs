using System;
using System.IO;
using System.Threading;

namespace ThreadBasedArchiver
{
    /// <summary>
    /// Base GZip class
    /// </summary>
    public abstract class GZipBase
    {
        protected const int chunkDataSize = 1024 * 1024 * 5;
        
        protected QueueManager queueManager = new QueueManager();
        protected string sourceFile, resultFile;
        protected bool isSuccess = false;
        protected bool isFailed = false;

        public GZipBase(string input, string output)
        {
            sourceFile = input;
            resultFile = Path.GetExtension(output) == ".gz" ? output : output + ".gz";
        }

        public abstract void Run();

        public int GetResult()
        {
            while (true)
            {
                Thread.Sleep(100);

                if (isFailed)
                    return 1;
                if (isSuccess)
                    return 0;
            }            
        }
    }
}
