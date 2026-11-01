namespace Rauschen;

using System.Numerics;

/// <summary>Helpers for working with frequency-domain descriptions of sound.</summary>
public static class Spectrum
{
    /// <summary>
    /// Turns a spectrum into 16-bit PCM samples: inverse FFT (in place) into the
    /// time domain, then the real part normalized to the given peak volume.
    /// </summary>
    public static short[] ToSamples(Complex[] spectrum, double volume)
    {
        FftSharp.FFT.Inverse(spectrum);

        var peak = spectrum.Aggregate(0.0, (max, c) => Math.Max(max, Math.Abs(c.Real)));
        var samples = new short[spectrum.Length];
        if (peak == 0) return samples; // silence stays silence

        for (var i = 0; i < samples.Length; i++)
        {
            samples[i] = (short)(short.MaxValue * volume * spectrum[i].Real / peak);
        }
        return samples;
    }
}
