namespace Rauschen;

/// <summary>A set of pitches sounding together.</summary>
public readonly struct Chord(params Pitch[] pitches)
{
    /// <summary>The pitches of the chord (empty for the default instance).</summary>
    public ReadOnlySpan<Pitch> Pitches => pitches;



    // common triads, built by transposing the root by semitones
    public static Chord Major(Pitch root) => new(root, root + 4, root + 7);
    public static Chord Minor(Pitch root) => new(root, root + 3, root + 7);

    // G7 = G B D F
    // G → root
    // B → major third
    // D → perfect fifth
    // F → minor seventh

    /// <summary>
    /// Returns the dominant seventh chord built on the given root pitch.
    /// the chord built on the 5th scale degree, plus a seventh
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    // public static Chord DominantSeventh(Pitch g) => new(g, g + 4, g + 7, g + 10);
    public static Chord DominantSeventh(Pitch g) => new(g, g + Interval.M3, g + Interval.P5, g + Interval.m7);

    // C7    = C E G Bb   (dominant seventh)
    // Cmaj7 = C E G B    (major seventh)
    // Cm7   = C Eb G Bb  (minor seventh)

    public override string ToString() => pitches is null ? "" : string.Join(" ", pitches);
}


public readonly struct Interval(int semitones)
{
    public readonly int Semitones = semitones;

    public static Pitch operator +(Pitch p, Interval i) => p + i.Semitones;

#pragma warning disable IDE1006 // Naming Styles

    public static Interval P1 { get; } = new(0);
    public static Interval m2 { get; } = new(1);
    public static Interval M2 { get; } = new(2);
    public static Interval m3 { get; } = new(3);
    public static Interval M3 { get; } = new(4);
    public static Interval P4 { get; } = new(5);
    public static Interval TT { get; } = new(6); //  (tritone)
    public static Interval P5 { get; } = new(7);
    public static Interval m6 { get; } = new(8);
    public static Interval M6 { get; } = new(9);
    public static Interval m7 { get; } = new(10);
    public static Interval M7 { get; } = new(11);
    public static Interval P8 { get; } = new(12);

#pragma warning restore IDE1006 // Naming Styles
}
