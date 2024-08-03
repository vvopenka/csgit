using System.Text;
using csgit.Lib.Model;

namespace csgit.Lib.Parser;

public class StreamReader : IDisposable, IAsyncDisposable
{
    private byte[] buffer = new byte[1024];
    private readonly Stream stream;
    
    public StreamReader(Stream stream)
    {
        this.stream = stream;
    }

    public Task<int> ReadUntilNul(byte[] buffer) => ReadUntilByte(buffer, 0);
    
    public async Task<int> ReadUntilByte(byte[] buffer, byte b)
    {
        int pos = 0;
        while (pos < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer, pos, 1);
            if (read != 1)
            {
                return -1;
            }

            if (buffer[pos] == b)
            {
                return pos;
            }

            pos++;
        }

        throw new ParserException("Not enough space in buffer");
    }

    public Task<string?> ReadStringUntilNul() => ReadStringUntilByte(0);
    
    public Task<string?> ReadStringUntilNl() => ReadStringUntilByte(0x0A);
    
    public async Task<string?> ReadStringUntilByte(byte b)
    {
        int size = await ReadUntilByte(buffer, b);
        if (size < 0)
            return null;
        return Encoding.UTF8.GetString(buffer, 0, size);
    }
    
    public async Task<Sha1> ReadSha1()
    {
        byte[] data = new byte[Sha1.Bytes];
        int read = await stream.ReadAsync(data, 0, Sha1.Bytes);
        if (read != Sha1.Bytes)
        {
            throw new ParserException("Not enough data in stream");
        }

        return new Sha1(data);
    }

    public async Task<byte[]> ReadToEnd()
    {
        using MemoryStream memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        return memory.ToArray();
    }

    public void Dispose()
    {
        stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await stream.DisposeAsync();
    }
}