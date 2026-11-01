namespace Rauschen;

using System.Numerics;

/// <summary>
/// Synthesizes sound the other way around: instead of generating samples per
/// note, it describes the sound in the frequency domain (a spectrum) and uses
/// FftSharp's inverse FFT to turn that spectrum into a time-domain signal.
/// </summary>
public static class SpectrumExample
{
    const int RATE = 44_100;
    const int N = 1 << 16;          // FFT size: 65536 samples ~ 1.49 s per block
    const double Volume = 0.25;

    public static void Main()
    {
        Console.WriteLine($"spectral resolution: {(double)RATE / N:F3} Hz/bin, block length {(double)N / RATE:F2} s");

        // the sound to describe in the frequency domain: a C major chord
        // var chord = Chord.Major(Pitch.C4);

        // var chord = Chord.Minor(Pitch.C4);

        // G B D F
        var chord = Chord.DominantSeventh(Pitch.C4);

        // BuildSpectrum adds the timbre: a few harmonics per pitch at 1/h^2 amplitude
        var spectrum = BuildSpectrum(chord, N, RATE);

        // convert the spectrum into time-domain samples
        var samples = Spectrum.ToSamples(spectrum, Volume);

        // bin-aligned frequencies are periodic over N samples,
        // so blocks can be repeated seamlessly
        const int blocks = 3;
        using var writer = new WavWriter("spectrum.wav", RATE);
        for (var block = 0; block < blocks; block++)
        {
            writer.Write(samples);
        }

        Console.WriteLine($"wrote spectrum.wav ({blocks * (double)N / RATE:F2} s)");
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

                Console.WriteLine($"  {pitch} h{harmonic}: {frequency,7:F2} Hz -> bin {bin,5} ({bin * binWidth:F2} Hz)");
            }
        }
        return spectrum;
    }
}
