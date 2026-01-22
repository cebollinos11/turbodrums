using NAudio.Midi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Diagnostics;
using NAudio.Wave.Asio;

class Program
{
    static MidiIn midiIn;

    static IWavePlayer outputDevice;
    static MixingSampleProvider mixer;

    static Dictionary<int, CachedSound> drumSamples = new();

    static Dictionary<int, string> drumMap = new()
    {
        { 36, "Samples/kick.wav" },
        { 38, "Samples/snare.wav" },
        { 42, "Samples/hihat_closed.wav" },
        { 46, "Samples/hihat_open.wav" },
        { 44, "Samples/hihat_pedal.wav" },
        { 51, "Samples/ride.wav" },
        { 48, "Samples/tom1.wav" },
        { 45, "Samples/tom2.wav" },
        { 43, "Samples/tom3.wav" },
        { 49, "Samples/crash.wav" }
    };
    static WaveFormat mixerFormat =    WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
    [STAThread]

    static void Main()
    {

        Console.WriteLine("asio");
        for (int i = 0; i < AsioOut.GetDriverNames().Length; i++)
        {
            Console.WriteLine(AsioOut.GetDriverNames()[i]);
        }

        //ASIO4ALL v2


        // 🎧 Create audio engine ONCE
        mixer = new MixingSampleProvider(mixerFormat)
        {
            ReadFully = true
        };

        outputDevice = new AsioOut("ASIO4ALL v2");
        outputDevice.Init(mixer);
        outputDevice.Play();

        // 🥁 Load samples into RAM
        foreach (var kvp in drumMap)
        {
            drumSamples[kvp.Key] =
                new CachedSound(kvp.Value, mixerFormat);
        }


        // 🎹 MIDI
        midiIn = new MidiIn(0);
        midiIn.MessageReceived += MidiIn_MessageReceived;
        midiIn.Start();

        Console.WriteLine("Drum sampler running...");
        Console.ReadLine();

        midiIn.Stop();
        midiIn.Dispose();
        outputDevice.Dispose();
    }

    static void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
    {
        if (e.MidiEvent is NoteOnEvent noteOn && noteOn.Velocity > 0)
        {
            Console.WriteLine(noteOn.NoteNumber);
            if (drumSamples.TryGetValue(noteOn.NoteNumber, out var sound))
            {
                var provider = new CachedSoundSampleProvider(sound);
                mixer.AddMixerInput(provider);
            }
        }
    }
}
