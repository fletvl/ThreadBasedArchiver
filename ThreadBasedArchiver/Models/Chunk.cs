
namespace ThreadBasedArchiver
{
    public class Chunk
    {
        public int ChunkId { get; set; }

        public byte[] Buffer { get; set; }

        public byte[] CompressedBuffer { get; set; }
    }
}
