namespace Rauschen;


/// <summary>
/// Provides utility methods for working with monophonic melodies, represented as sequences of notes.
/// </summary>
public static class Monophonic
{
    /// <summary>
    /// Parses a melody like "E4/4 E4/4 F4/4 G4/4 | ..." into notes (pitch + ticks,
    /// 960 ticks per quarter note).
    /// </summary>
    public static Note[] ParseMelody(this string s)
    {
        var notes = new List<Note>();
        var parts = s.Split([' ', '\t', '|', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (Note.TryParse(part, null, out var note))
            {
                notes.Add(note);
            }
            else
            {
                throw new FormatException($"Invalid note format: {part}");
            }
        }
        return notes.ToArray();
    }
}

