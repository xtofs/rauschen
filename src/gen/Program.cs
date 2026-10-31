Console.WriteLine("""
static class Chromatic
{
    publicconst int
""");
// "C, C♯/D♭, D, D♯/E♭, E, F, F♯/G♭, G, G♯/A♭, A, A♯/B♭, B";
string[] names = ["C", "D", "E", "F", "G", "A", "B"];


var ix = -57; // starting index for C0 wich makes C4 = 0
for (var i = 0; i < 6; i++)
{
    for (var n = 0; n < names.Length; n++)
    {
        var name = names[n];

        // Add the natural note itself
        Console.WriteLine($"        {name}{i}  = {ix,3}, // {name}");
        ix += 1;

        // if it is not E or B
        // add sharp for the current note and a flat for the next note 
        if (name is not "E" and not "B") // add sharp for non E and B notes
        {
            Console.WriteLine($"        {name}s{i} = {ix,3}, // {name}♯");
            var next = names[(n + 1) % 7];
            Console.WriteLine($"        {next}f{i} = {ix,3}, // {next}♭");
            ix += 1;
        }
    }
    Console.WriteLine();
}


