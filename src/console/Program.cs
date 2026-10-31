namespace Rauschen;

using static Chromatic;

public static class WavExample
{
    const int RATE = 44_100;

    public static void Main()
    {


        (int n, double beats)[] odeToJoy =
        [
            // mm. 1-4
            (E4,1), (E4,1), (F4,1), (G4,1),
            (G4,1), (F4,1), (E4,1), (D4,1),
            (C4,1), (C4,1), (D4,1), (E4,1),
            (E4,1.5), (D4,0.5), (D4,2),

            // mm. 5-8
            (E4,1), (E4,1), (F4,1), (G4,1),
            (G4,1), (F4,1), (E4,1), (D4,1),
            (C4,1), (C4,1), (D4,1), (E4,1),
            (D4,1.5), (C4,0.5), (C4,2),

            // mm. 9-12
            (D4,1), (D4,1), (E4,1), (C4,1),
            (D4,1), (E4,0.5), (F4,0.5), (E4,1), (C4,1),
            (D4,1), (E4,0.5), (F4,0.5), (E4,1), (D4,1),
            (C4,1), (D4,1), (G3,2),

            // mm. 13-16
            (E4,1), (E4,1), (F4,1), (G4,1),
            (G4,1), (F4,1), (E4,1), (D4,1),
            (C4,1), (C4,1), (D4,1), (E4,1),
            (D4,1.5), (C4,0.5), (C4,2)
        ];

        using var writer = new WavWriter("audio.wav", RATE);

        const double secondsPerBeat = 0.3; // 0.5 => 60 / 0.5 = 120 BPM
        foreach (var (n, beats) in odeToJoy)
        {
            writer.WriteNote(beats * secondsPerBeat, n - 48);
        }
        writer.WriteNote(6 * secondsPerBeat, -100);



//         double[] signal = FftSharp.SampleData.SampleAudio1();

// // Shape the signal using a Hanning window
// var window = new FftSharp.Windows.Hanning();
// window.ApplyInPlace(signal);

// // Calculate the FFT as an array of complex numbers
// System.Numerics.Complex[] spectrum = FftSharp.FFT.Forward(signal);

// // or get the magnitude (units²) or power (dB) as real numbers
// double[] magnitude = FftSharp.FFT.Magnitude(spectrum);
// double[] power = FftSharp.FFT.Power(spectrum);
    }
}
