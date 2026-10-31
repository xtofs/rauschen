namespace Rauschen;

using static Monophonic;

public static class WavExample
{
    const int RATE = 44_100;

    public static void Main()
    {


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

        foreach (var note in odeToJoy)
        {
            Console.WriteLine(note);
        }

        using var writer = new WavWriter("audio.wav", RATE);

        // 60 / 240 = 0.25. 
        // that is 0.25 seconds per beat or 4 beats per second
        const double bpm = 240;

        foreach (var note in odeToJoy)
        {
            writer.WriteNote(note.ToSeconds(bpm), note.Pitch);
        }
        writer.WriteRest(6 * 60.0 / bpm);

    }
}
