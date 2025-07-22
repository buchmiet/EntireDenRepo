namespace denPusher;

public class ProgressFileStream : FileStream
{
    public event EventHandler<long> ProgressChanged;
    private long totalRead = 0;

    public ProgressFileStream(string path, FileMode mode) : base(path, mode)
    {
    }

    public override int Read(byte[] array, int offset, int count)
    {
        var bytesRead = base.Read(array, offset, count);
        totalRead += bytesRead;
        ProgressChanged?.Invoke(totalRead, this.Length);
        return bytesRead;
    }

    public override void Write(byte[] array, int offset, int count)
    {
        base.Write(array, offset, count);
        OnProgressChanged(count);
    }


    protected virtual void OnProgressChanged(long bytes)
    {
        ProgressChanged?.Invoke(this, bytes);
    }
}