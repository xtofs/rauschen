

static class Play
{

    // A_4 frequency, 
    // this is the note with index 0 in the semitone system
    const double A4 = 440;

    public static short[] Note(int rate, double duration, int note)
    {
        return Note(rate, duration, A4 * Math.Pow(2, note / 12.0));
    }

    const double NoteVolume = 0.25;

    public static short[] Note(int rate, double duration, double frequency)
    {
        var samples = new short[(int)(rate * duration)];
        int fade = Math.Max(1, Math.Min(rate / 100, samples.Length / 2));   // 10 ms fade in/out
        for (int i = 0; i < samples.Length; i++)
        {
            double env = Math.Min(1.0, Math.Min(i, samples.Length - 1 - i) / (double)fade);
            samples[i] = (short)(short.MaxValue * NoteVolume * env * Math.Sin(2 * Math.PI * frequency * i / rate));
        }
        return samples;
    }
}