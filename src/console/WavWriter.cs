namespace Riff;


using System;
using System.IO;


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
    private readonly ChunkScope _wav;
    private readonly ChunkScope _chunk;

    private bool _disposed;

    public WavWriter(string filePath, int rate)
    {
        var stream = File.Create(filePath);
        _riffWriter = new RiffWriter(stream);

        _wav = _riffWriter.BeginRiff("WAVE");

        // Fixed-size chunk from a struct
        _riffWriter.WriteChunk("fmt ", WaveFormat.CreatePcm(rate, 1, 16));

        // Variable-length chunk: size patched when the scope ends
        _chunk = _riffWriter.BeginChunk("data");
    }

    public void Write(ReadOnlySpan<short> data)
    {
        _riffWriter.Write<short>(data);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _chunk.Dispose();
        _wav.Dispose();
        _riffWriter.Dispose();
    }
}