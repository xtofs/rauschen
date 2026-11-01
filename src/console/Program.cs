namespace Rauschen;

using static Monophonic;

public static class WavExample
{
    const int RATE = 44_100;

    public static void Main(string[] args)
    {
        // check if there is a --waveform argument
        Waveform selectedWaveform = ParseArgs(args);

        var odeToJoy = """
            E4/4 E4/4 F4/4 G4/4 |
            G4/4 F4/4 E4/4 D4/4 |
            C4/4 C4/4 D4/4 E4/4 |
            E4/4. D4/8 D4/2     |

            E4/4 E4/4 F4/4 G4/4 |
            G4/4 F4/4 E4/4 D4/4 |
            C4/4 C4/4 D4/4 E4/4 |
            D4/4. C4/8 C4/2     |

            D4/4 D4/4 E4/4 C4/4      |
            D4/4 E4/8 F4/8 E4/4 C4/4 |
            D4/4 E4/8 F4/8 E4/4 D4/4 |
            C4/4 D4/4 G3/2           |

            E4/4 E4/4 F4/4 G4/4 |
            G4/4 F4/4 E4/4 D4/4 |
            C4/4 C4/4 D4/4 E4/4 |
            D4/4. C4/8 C4/2     |
        """.ParseMelody();

        // foreach (var note in odeToJoy)
        // {
        //     Console.WriteLine(note);
        // }

        using var writer = new WavWriter("audio.wav", RATE);

        // 60 / 240 = 0.25. 
        // that is 0.25 seconds per beat or 4 beats per second
        var melodyWriter = new MelodyWriter(writer, 180, selectedWaveform);

        foreach (var note in odeToJoy)
        {
            melodyWriter.WriteNote(note);
        }
        melodyWriter.WriteRest(6);

    }

    private static Waveform ParseArgs(string[] args)
    {
        Waveform selectedWaveform = Waveform.Sine; // default waveform
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--waveform" && i + 1 < args.Length)
            {
                // parse the waveform argument
                if (Enum.TryParse<Waveform>(args[i + 1], true, out var waveform))
                {
                    // set the waveform to use
                    selectedWaveform = waveform;
                }
            }
        }

        return selectedWaveform;
    }
}
