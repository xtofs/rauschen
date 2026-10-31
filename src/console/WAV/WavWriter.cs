namespace Rauschen;



/// <summary>
/// Writes WAV files using the RIFF format.
/// Supports writing PCM audio data to a WAV file.
/// Example usage:
/// <code>
/// using var writer = new WavWriter("output.wav", 44100);
/// writer.Write(samples);
/// </code>
/// </summary>
public class WavWriter : IDisposable
{
    private readonly RiffWriter _riffWriter;
    private readonly Stack<ChunkScope?> _scopes;
    private bool _disposed;

    public int Rate { get; }

    public WavWriter(string filePath, int rate)
    {
        Rate = rate;
        _scopes = new Stack<ChunkScope?>();

        var stream = File.Create(filePath);
        _riffWriter = new RiffWriter(stream);

        _scopes.Push(_riffWriter.BeginRiff("WAVE"));

        // Fixed-size chunk from a struct
        _riffWriter.WriteChunk("fmt ", WaveFormat.CreatePcm(rate, 1, 16));

        // Variable-length chunk: size patched when the scope ends
        _scopes.Push(_riffWriter.BeginChunk("data"));
    }

    public void Write(ReadOnlySpan<short> data)
    {
        _riffWriter.Write(data);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            while (_scopes.TryPop(out var scope))
            {
                scope?.Dispose();
            }
            _riffWriter.Dispose();
        }
        _disposed = true;
    }
}