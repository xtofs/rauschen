// WavExample.cs — writes a 1-second 440 Hz mono 16-bit WAV with an INFO list.

using System;
using System.IO;

using Riff;

public static class WavExample

{
    const int RATE = 44_100;

    const int
        C3 = -21, D3 = -19, E3 = -17, F3 = -16, G3 = -14,
        C4 = -9, D4 = -7, E4 = -5, F4 = -4, G4 = -2;

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

        const double secondsPerBeat = 0.5;   // 120 BPM
        foreach (var (n, beats) in odeToJoy)
        {
            var note = Play.Note(RATE, beats * secondsPerBeat, n);
            writer.Write(note);
        }
    }
}

