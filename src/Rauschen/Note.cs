namespace Rauschen;

/// <summary>
/// A note in a melody: a <see cref="Rauschen.Pitch"/> plus a duration measured in
/// integer ticks, with 960 ticks per quarter note (960 PPQN). The fixed-point
/// resolution 960 = 2^6 * 3 * 5 keeps dotted notes, triplets and quintuplets exact.
/// </summary>
public readonly record struct Note(Pitch Pitch, int Ticks) : IParsable<Note>
{
    public const int TicksPerQuarter = 960;
    public const int TicksPerWhole = 4 * TicksPerQuarter;

    /// <summary>Duration in floating point beats, where a quarter note is one beat.</summary>
    public double Beats => Ticks / (double)TicksPerQuarter;

    /// <summary>Duration in seconds at the given tempo.</summary>
    public double ToSeconds(double beatsPerMinute) => Beats * 60.0 / beatsPerMinute;

    public override string ToString()
    {
        // find the note value (plain or dotted) that matches the tick count exactly
        for (var denominator = 1; denominator <= 64; denominator *= 2)
        {
            var plain = TicksPerWhole / denominator;
            if (Ticks == plain) return $"{Pitch}/{denominator}";
            if (Ticks == plain + plain / 2) return $"{Pitch}/{denominator}.";
        }
        return $"{Pitch}/{Ticks}t"; // not notatable as a (dotted) power-of-two value
    }

    public static Note Parse(string s, IFormatProvider? provider) => TryParse(s, provider, out var result) ? result : throw new FormatException($"Invalid note format: {s}");

    public static bool TryParse(string? str, IFormatProvider? provider, out Note result)
    {
        // parsing a string like "E4/4" into a pitch and duration
        // E4/4 E4/4 F4/4 G4/4 
        // E4/4. D4/8 D4/2
        // a pitch (see Pitch.TryParse), e.g. "E4", "F#3", "Bb2"
        // followed by a duration (e.g., /4, /8, /2)
        // optionally followed by a dot for dotted notes (e.g., /4.)
        result = default;
        var span = str.AsSpan();

        var slashIx = span.IndexOf('/');
        if (slashIx < 0) return false;

        if (!Pitch.TryParse(span[..slashIx], out var pitch))
        {
            return false;
        }
        span = span[(slashIx + 1)..]; // keep only the duration part

        // parse the (possibly multi-digit) denominator, e.g. /4 or /16
        var denominator = 0;
        while (!span.IsEmpty && char.IsAsciiDigit(span[0]))
        {
            denominator = denominator * 10 + (span[0] - '0');
            span = span[1..];
        }
        if (denominator == 0 || TicksPerWhole % denominator != 0) return false;

        var ticks = TicksPerWhole / denominator;

        if (!span.IsEmpty && span[0] == '.')
        {
            if (ticks % 2 != 0) return false; // dotted value must stay an integer
            ticks += ticks / 2;
            span = span[1..]; // remove the dot for further parsing
        }

        if (!span.IsEmpty) return false; // reject trailing characters

        result = new Note(pitch, ticks);
        return true;
    }

    internal (double Frequency, double Duration) FrequencyAndDuration(double bpm)
    {
        var frequency = this.Pitch.Frequency;
        var duration = this.ToSeconds(bpm);
        return (frequency, duration);
    }
}


