


using static Note;

static class Scales
{
    private static readonly int[] MajorPattern = [2, 2, 1, 2, 2, 2, 1];
    private static readonly int[] MinorPattern = [2, 1, 2, 2, 1, 2, 2];

    private static int[] FromPattern(int root, int[] pattern)
    {
        int[] scale = new int[pattern.Length];
        scale[0] = root;
        for (int i = 1; i < pattern.Length; i++)
        {
            scale[i] = scale[i - 1] + pattern[i - 1];
        }
        return scale;
    }

    public static readonly int[] CMajor = FromPattern(C4, MajorPattern);
    public static readonly int[] GMajor = FromPattern(G4, MajorPattern);
    public static readonly int[] CMinor = FromPattern(C4, MinorPattern);
}

