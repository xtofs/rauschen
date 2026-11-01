namespace Rauschen;

using System.Numerics;

/// <summary>Helpers for working with frequency-domain descriptions of sound.</summary>
public static class Spectrum
{
    /// <summary>
    /// Turns a spectrum into a track of samples in [-1, 1]: inverse FFT (in place)
    /// into the time domain, then the real part normalized to peak 1.
    /// </summary>
    public static double[] ToSamples(Complex[] spectrum)
    {
        FftSharp.FFT.Inverse(spectrum);

        var peak = spectrum.Aggregate(0.0, (max, c) => Math.Max(max, Math.Abs(c.Real)));
        var samples = new double[spectrum.Length];
        if (peak == 0) return samples; // silence stays silence

        for (var i = 0; i < samples.Length; i++)
        {
            samples[i] = spectrum[i].Real / peak;
        }
        return samples;
    }
}
