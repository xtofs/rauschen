namespace Rauschen;

using System.Numerics;

/// <summary>
/// Plays a melody over a chord progression. The chords are synthesized the
/// other way around: described in the frequency domain (a spectrum) and turned
/// into a time-domain signal with FftSharp's inverse FFT. The melody is
/// rendered in the time domain, and the two tracks are mixed by addition.
/// </summary>
public static class MelodyExample
{
    const int RATE = 44_100;
    const int N = 1 << 16;          // FFT size: 65536 samples ~ 1.49 s per block
    const double Volume = 0.25;
    const double BPM = 120;

    public static void Main()
    {
        // the first phrase of Ode to Joy
        var melody = """
            E4/4 E4/4  F4/4 G4/4 |
            G4/4 F4/4  E4/4 D4/4 |
            C4/4 C4/4  D4/4 E4/4 |
            E4/4. D4/8 D4/2
            """.ParseMelody();

        // one chord per bar (the "harmonic rhythm").
        // Textbook harmonization of this phrase in C major: I | V7 | I | V7
        // (ending on V7 = half cadence, which is why the phrase feels open).
        Chord[] progression = [
            Chord.Major(Pitch.C4),            // I   (E E F G)
            Chord.DominantSeventh(Pitch.G3),  // V7  (G F E D)
            Chord.Major(Pitch.C4),            // I   (C C D E)
            Chord.DominantSeventh(Pitch.G3),  // V7  (E. D D)
        ];

        var melodyTrack = CreateMelodyTrack(melody, BPM, RATE);
        var chordTrack = CreateChordProgressionTrack(progression, BPM, RATE);
        var mix = melodyTrack.Mix(chordTrack);
        WriteMix("ode-to-joy.wav", mix, Volume);
    }

    /// <summary>
    /// Renders a melody into a track: one sine per note with a short fade
    /// in/out, placed one after another on the timeline.
    /// </summary>
    public static Track CreateMelodyTrack(Note[] melody, double bpm, int rate)
    {
        var total = melody.Sum(note => (int)(rate * note.ToSeconds(bpm)));
        var track = new double[total];
        var position = 0;
        foreach (var note in melody)
        {
            var count = (int)(rate * note.ToSeconds(bpm));
            var fade = Math.Max(1, Math.Min(rate / 100, count / 2)); // 10 ms fade in/out
            for (var i = 0; i < count; i++)
            {
                var env = Math.Min(1.0, Math.Min(i, count - 1 - i) / (double)fade);
                track[position + i] = env * Math.Sin(2 * Math.PI * note.Pitch.Frequency * i / rate);
            }
            position += count;
        }
        return new Track(track, rate);
    }

    /// <summary>
    /// Renders a chord progression (one chord per 4/4 bar) into a track:
    /// each chord is synthesized as a spectrum, inverse-FFT'd
    /// into a looping block, and faded in/out over 10 ms at the bar boundaries
    /// (switching spectra mid-phase would otherwise click).
    /// </summary>
    public static Track CreateChordProgressionTrack(Chord[] progression, double bpm, int rate, int beatsPerBar = 4)
    {
        var samplesPerBar = (int)(rate * beatsPerBar * 60.0 / bpm);
        var fade = Math.Max(1, rate / 100); // 10 ms declick at bar changes
        var track = new double[progression.Length * samplesPerBar];

        for (var bar = 0; bar < progression.Length; bar++)
        {
            // the bar's chord as a seamlessly looping block, normalized to [-1, 1]
            var block = Spectrum.ToSamples(BuildSpectrum(progression[bar], N, rate));
            for (var i = 0; i < samplesPerBar; i++)
            {
                var env = Math.Min(1.0, Math.Min(i, samplesPerBar - 1 - i) / (double)fade);
                track[bar * samplesPerBar + i] = env * block[i % block.Length];
            }
        }
        return new Track(track, rate);
    }

    /// <summary>
    /// Writes a track to a WAV file at the given volume — the double-to-PCM
    /// conversion happens inside the WavWriter.
    /// </summary>
    public static void WriteMix(string path, Track mix, double volume)
    {
        using var writer = new WavWriter(path, mix.Rate);
        writer.Write(mix, volume);
        Console.WriteLine($"wrote {path} ({mix.Duration.TotalSeconds:F2} s)");
    }

    /// <summary>
    /// Describes a chord in the frequency domain: for each pitch, its first few
    /// harmonics are placed on the nearest FFT bins with 1/h^2 amplitude rolloff.
    /// </summary>
    private static Complex[] BuildSpectrum(Chord chord, int size, int rate, int harmonics = 4)
    {
        var binWidth = (double)rate / size;
        var spectrum = new Complex[size];
        foreach (var pitch in chord.Pitches)
        {
            for (var harmonic = 1; harmonic <= harmonics; harmonic++)
            {
                var frequency = pitch.Frequency * harmonic;
                var amplitude = 1.0 / (harmonic * harmonic);
                var bin = (int)Math.Round(frequency / binWidth);

                // a conjugate-symmetric pair of bins yields a purely real signal;
                // the -i phase makes each component a sine, starting at zero
                spectrum[bin] += new Complex(0, -amplitude * size / 2);
                spectrum[size - bin] += new Complex(0, amplitude * size / 2);
            }
        }
        return spectrum;
    }
}
