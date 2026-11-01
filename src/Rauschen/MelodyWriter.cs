namespace Rauschen;

public record class MelodyWriter(WavWriter Writer, double Bpm, Waveform Waveform = Waveform.Sine)
{
    private readonly WavWriter _writer = Writer;
    private readonly double _bpm = Bpm;
    private readonly Waveform _waveform = Waveform;

    const double NoteVolume = 0.25;

    private static readonly System.Buffers.ArrayPool<double> Pool = System.Buffers.ArrayPool<double>.Shared;

    public void WriteNote(Note note)
    {
        var (frequency, duration) = note.FrequencyAndDuration(_bpm);
        var rate = _writer.Rate;
        var count = (int)(rate * duration);
        int fade = Math.Max(1, Math.Min(_writer.Rate / 100, count / 2));   // 10 ms fade in/out

        var wave = Pool.Rent(count);
        for (int i = 0; i < count; i++)
        {
            double env = Math.Min(1.0, Math.Min(i, count - 1 - i) / (double)fade);
            double phase = frequency * i / rate % 1.0; // position within the cycle, [0, 1)
            wave[i] = env * Oscillate(_waveform, phase);
        }

        _writer.Write(wave.AsSpan(0, count), NoteVolume);

        Pool.Return(wave);
    }

    public void WriteRest(double duration)
    {
        var count = (int)(_writer.Rate * duration);
        var wave = Pool.Rent(count); // may be larger than count
        wave.AsSpan(0, count).Clear();
        _writer.Write(wave.AsSpan(0, count));
        Pool.Return(wave);
    }


    /// <summary>
    /// Generates the value of the specified waveform at the given phase.
    /// The phase is a value between 0 and 1 representing the position within one cycle of the waveform.
    /// The output is a value between -1 and 1 representing the amplitude of the waveform at that phase.
    /// </summary>
    /// <param name="waveform"></param>
    /// <param name="phase"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static double Oscillate(Waveform waveform, double phase) => waveform switch
    {
        Waveform.Sine => Math.Sin(2 * Math.PI * phase),
        Waveform.Square => phase < 0.5 ? 1.0 : -1.0,
        Waveform.Sawtooth => 2 * phase - 1,
        // rises 0 -> 1, falls 1 -> -1, rises -1 -> 0 (same zero crossings as sine)
        Waveform.Triangle => phase < 0.25 ? 4 * phase
                           : phase < 0.75 ? 2 - 4 * phase
                           : 4 * phase - 4,
        Waveform.Gaussian => Math.Exp(-Math.Pow(phase - 0.5, 2) * 4),
        _ => throw new ArgumentOutOfRangeException(nameof(waveform), waveform, null)
    };

}
