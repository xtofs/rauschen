namespace Rauschen;

using System.Numerics;

/// <summary>
/// Generates the classic "colors" of noise from the frequency domain.
/// Noise is a spectrum with *random phase* in every bin — the color is just the
/// spectral envelope: power ∝ 1/f^α, i.e. magnitude ∝ f^(−α/2). A flat envelope
/// with coherent phases would be a click; the random phases make it noise.
/// </summary>
public static class NoiseExample
{
    const int RATE = 44_100;
    const int N = 1 << 16;          // FFT size: 65536 samples ~ 1.49 s per block
    const double Volume = 0.25;
    const double LowCutoff = 20.0;  // skip sub-audible bins; keeps 1/f^2 low end
                                    // from dominating the normalization

    // power ∝ 1/f^α
    enum NoiseColor { White = 0, Pink = 1, Brown = 2, Blue = -1, Violet = -2 }

    public static void Main()
    {
        var rng = new Random(Seed: 42); // seeded: reproducible output files

        foreach (var color in (NoiseColor[])[NoiseColor.White, NoiseColor.Pink, NoiseColor.Brown, NoiseColor.Blue, NoiseColor.Violet])
        {
            var alpha = (int)color;
            var spectrum = BuildNoiseSpectrum(N, RATE, alpha, rng);
            var samples = Spectrum.ToSamples(spectrum, Volume);

            var name = color.ToString().ToLowerInvariant();
            const int blocks = 2;
            using var writer = new WavWriter($"noise-{name}.wav", RATE);
            for (var block = 0; block < blocks; block++)
            {
                writer.Write(samples);
            }

            Console.WriteLine($"noise-{name}.wav: power ~ 1/f^{alpha} ({alpha * -3:+0;-0;0} dB/octave, {blocks * (double)N / RATE:F2} s)");
        }
    }

    /// <summary>
    /// Builds a noise spectrum: every audible bin gets a magnitude following the
    /// 1/f^α power envelope and a uniformly random phase. Conjugate-symmetric
    /// bin pairs keep the inverse FFT purely real.
    /// </summary>
    private static Complex[] BuildNoiseSpectrum(int size, int rate, double alpha, Random rng)
    {
        var binWidth = (double)rate / size;
        var spectrum = new Complex[size];

        for (var bin = 1; bin < size / 2; bin++) // DC and Nyquist stay 0
        {
            var frequency = bin * binWidth;
            if (frequency < LowCutoff) continue;

            // power ∝ f^-α, so magnitude ∝ f^(-α/2)
            var magnitude = Math.Pow(frequency, -alpha / 2.0);
            var phase = 2 * Math.PI * rng.NextDouble();

            var component = Complex.FromPolarCoordinates(magnitude, phase);
            spectrum[bin] = component;
            spectrum[size - bin] = Complex.Conjugate(component);
        }
        return spectrum;
    }
}
