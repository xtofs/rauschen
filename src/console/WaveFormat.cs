// WavExample.cs — writes a 1-second 440 Hz mono 16-bit WAV with an INFO list.

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct WaveFormat            // PCMWAVEFORMAT, 16 bytes
{
    public static WaveFormat CreatePcm(int rate, int channels = 1, int bitsPerSample = 16)
    {
        return new WaveFormat
        {
            FormatTag = 1,
            Channels = (ushort)channels,
            SamplesPerSec = (uint)rate,
            AvgBytesPerSec = (uint)(rate * channels * bitsPerSample / 8),
            BlockAlign = (ushort)(channels * bitsPerSample / 8),
            BitsPerSample = (ushort)bitsPerSample
        };
    }

    public ushort FormatTag;        // 1 = PCM
    public ushort Channels;
    public uint SamplesPerSec;
    public uint AvgBytesPerSec;
    public ushort BlockAlign;
    public ushort BitsPerSample;
}
