using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class CachedSound
{
    public float[] AudioData { get; }
    public WaveFormat WaveFormat { get; }

    public CachedSound(string fileName, WaveFormat targetFormat)
    {
        WaveFormat = targetFormat;

        using var reader = new AudioFileReader(fileName);

        ISampleProvider provider = reader;

        // 🔁 Resample if needed
        if (provider.WaveFormat.SampleRate != targetFormat.SampleRate)
        {
            provider = new WdlResamplingSampleProvider(
                provider,
                targetFormat.SampleRate);
        }

        // 🔊 Channel conversion
        if (provider.WaveFormat.Channels == 1 &&
            targetFormat.Channels == 2)
        {
            provider = new MonoToStereoSampleProvider(provider);
        }
        else if (provider.WaveFormat.Channels == 2 &&
                 targetFormat.Channels == 1)
        {
            provider = new StereoToMonoSampleProvider(provider);
        }

        // ✅ Already float — no conversion needed

        var sampleList = new List<float>();
        var buffer = new float[targetFormat.SampleRate * targetFormat.Channels];

        int samplesRead;
        while ((samplesRead = provider.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < samplesRead; i++)
                sampleList.Add(buffer[i]);
        }

        AudioData = sampleList.ToArray();
    }
}
