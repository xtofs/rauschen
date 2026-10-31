namespace Rauschen;

public static class WriterExtensions
{
    const double NoteVolume = 0.25;


    extension(WavWriter w)
    {
        public void WriteNote(double duration, Pitch pitch)
        {
            var count = (int)(w.Rate * duration);
            var wave = Pool.Rent(count); // may be larger than count
            FillNote(wave.AsSpan(0, count), w.Rate, pitch.Frequency);
            w.Write(wave.AsSpan(0, count));
            Pool.Return(wave);
        }

        public void WriteRest(double duration)
        {
            var count = (int)(w.Rate * duration);
            var wave = Pool.Rent(count); // may be larger than count
            wave.AsSpan(0, count).Clear();
            w.Write(wave.AsSpan(0, count));
            Pool.Return(wave);
        }
    }

    private static readonly System.Buffers.ArrayPool<short> Pool = System.Buffers.ArrayPool<short>.Shared;

    private static void FillNote(Span<short> samples, int rate, double frequency)
    {
        int fade = Math.Max(1, Math.Min(rate / 100, samples.Length / 2));   // 10 ms fade in/out
        for (int i = 0; i < samples.Length; i++)
        {
            double env = Math.Min(1.0, Math.Min(i, samples.Length - 1 - i) / (double)fade);
            samples[i] = (short)(short.MaxValue * NoteVolume * env * Math.Sin(2 * Math.PI * frequency * i / rate));
        }
    }
}