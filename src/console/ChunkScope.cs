namespace Rauschen;


/// <summary>
/// Returned by the Begin* methods; ends the chunk when disposed.
/// Use with <c>using (w.BeginChunk(...)) { ... }</c>.
/// </summary>
public readonly struct ChunkScope : IDisposable
{
    private readonly RiffWriter? _writer;
    private readonly int _depth;

    internal ChunkScope(RiffWriter writer, int depth)
    {
        _writer = writer;
        _depth = depth;
    }

    public void Dispose() => _writer?.EndChunk(_depth);
}
