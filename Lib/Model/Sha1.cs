namespace csgit.Lib.Model;

public struct Sha1 : IEquatable<Sha1>
{
    public const int Bits = 160;
    public const int Bytes = Bits / 8;
    
    private readonly byte[] data;
    
    public Sha1(byte[] data)
    {
        if (data.Length != Bytes)
        {
            throw new ArgumentException($"Data must be {Bytes} bytes long");
        }
        
        this.data = data;
    }

    public bool Equals(Sha1 other)
    {
        for (int pos = 0; pos < Bytes; pos++)
        {
            if (data[pos] != other.data[pos])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Sha1 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return data.GetHashCode();
    }
    
    public static bool operator ==(Sha1 left, Sha1 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Sha1 left, Sha1 right)
    {
        return !(left == right);
    }
    
    public override string ToString()
    {
        return Convert.ToHexString(data).ToLowerInvariant();
    }
    
    public static Sha1 Parse(string s)
    {
        if (s.Length != Bytes * 2)
        {
            throw new ArgumentException($"String must be {Bytes * 2} characters long");
        }

        byte[] data = Convert.FromHexString(s);

        return new Sha1(data);
    }
}