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

        const double BPM = 200; // 60 / 200 = 0.3 seconds per beat
        foreach (var note in odeToJoy)
        {
            writer.WriteNote(note.ToSeconds(BPM), note.Pitch);
        }
        writer.WriteRest(6 * 60.0 / BPM);

    }
}
