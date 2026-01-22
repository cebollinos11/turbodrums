using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class CachedSoundSampleProvider : ISampleProvider
{
    private readonly CachedSound cachedSound;
    private long position;

    public CachedSoundSampleProvider(CachedSound cachedSound)
    {
        this.cachedSound = cachedSound;
    }

    public WaveFormat WaveFormat => cachedSound.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        var availableSamples = cachedSound.AudioData.Length - position;
        var samplesToCopy = Math.Min(availableSamples, count);

        Array.Copy(
            cachedSound.AudioData,
            position,
            buffer,
            offset,
            samplesToCopy);

        position += samplesToCopy;
        return (int)samplesToCopy;
    }
}
