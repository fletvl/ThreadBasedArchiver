using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace ThreadBasedArchiver
{
    /// <summary>
    /// Compress class
    /// </summary>
    public class Compressor : GZipBase
    {
        private Thread[] _threads;

        public Compressor(string inFileName, string outFileName) : base(inFileName, outFileName)
        {
        }

        public override void Run()
        {
            try
            {
                FillStartQueue();
                Console.Write("Start compressing...\n");

                int threadNumber = Math.Min(queueManager.Count(), Environment.ProcessorCount);
                _threads = new Thread[threadNumber];
                for (int partCount = 0; partCount < threadNumber; partCount++)
                {
                    var thread = _threads[partCount] = new Thread(Process);
                    Console.WriteLine($"Starting a compressing thread {thread.ManagedThreadId}");
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR:\n" + ex.Message);
                isFailed = true;
            }
        }

        private void FillStartQueue()
        {
            int chunkNumber = 0;
            using (var inFile = new FileStream(sourceFile, FileMode.Open))
            {
                while (chunkNumber * chunkDataSize < inFile.Length)
                {
                    queueManager.Enqueue(chunkNumber);
                    chunkNumber++;                    
                }
            }            
        }

        private void Process()
        {
            try
            {
                while (true)
                {
                    var chunk = queueManager.Dequeue();
                    if (chunk == -1)
                    {
                        if (CheckThreadsExceptSelf(Thread.CurrentThread.ManagedThreadId))
                            isSuccess = true;

                        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} stopped");
                        return;
                    }
                    var readChunk = ReadChunk(chunk);
                    CompressBlock(readChunk);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n{ex.Message}");
                isFailed = true;
            }
        }

        private Chunk ReadChunk(int chunkId)
        {
            Console.WriteLine($"ReadChunk invoking: {chunkId} in thread {Thread.CurrentThread.ManagedThreadId}");

            using (var inFile = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long filePosition = chunkId * chunkDataSize;
                int bytesRead;

                if (inFile.Length - filePosition <= chunkDataSize)
                {
                    bytesRead = (int)(inFile.Length - filePosition);
                }
                else
                {
                    bytesRead = chunkDataSize;
                }

                var lastBuffer = new byte[bytesRead];
                inFile.Seek(filePosition, SeekOrigin.Current);
                inFile.Read(lastBuffer, 0, bytesRead);

                Chunk chunk = new Chunk()
                {
                    ChunkId = chunkId,
                    Buffer = lastBuffer
                };
                return chunk;
            }
        }  

        private void CompressBlock(Chunk chunk)
        {         
            Console.WriteLine($"CompressBlock invoking: {chunk.ChunkId} in thread {Thread.CurrentThread.ManagedThreadId}");
            
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress))
                {
                    gs.Write(chunk.Buffer, 0, chunk.Buffer.Length);
                }
                byte[] compressedData = ms.ToArray();

                var chunkOut = new Chunk()
                {
                    ChunkId = chunk.ChunkId,
                    Buffer = compressedData
                };

                WriteChunkFile(chunkOut);                 
            }
        }

        private void WriteChunkFile(Chunk chunkOut)
        {
            Console.WriteLine($"WriteChunkFile invoking: {chunkOut.ChunkId} in thread {Thread.CurrentThread.ManagedThreadId}");
            var chunkFileName = resultFile + $"_{chunkOut.ChunkId}";

            using (var outFile = new FileStream(chunkFileName, FileMode.Append))
            {
                outFile.Write(chunkOut.Buffer, 0, chunkOut.Buffer.Length);
            }
        }

        private bool CheckThreadsExceptSelf(int threadId)
        {
            return _threads.Where(x => x.ManagedThreadId != threadId).All(x => x.ThreadState == ThreadState.Stopped);
        }
    }
}
