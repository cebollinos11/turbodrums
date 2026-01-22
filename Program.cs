using NAudio.Midi;
using NAudio.Wave;

class Program
{
    static MidiIn midiIn;


static void PlaySound(string file)
{
    var audioFile = new AudioFileReader(file);
    var output = new WaveOutEvent();

    output.Init(audioFile);
    output.Play();

    output.PlaybackStopped += (s, e) =>
    {
        output.Dispose();
        audioFile.Dispose();
    };
}

    static Dictionary<int, string> drumMap = new()
{
    { 36, "Samples/kick.wav" },
    { 38, "Samples/snare.wav" },
    { 42, "Samples/hihat_closed.wav" },
    { 46, "Samples/hihat_open.wav" },
    { 49, "Samples/crash.wav" }
};



    static void Main()
    {
        midiIn = new MidiIn(0); // change index if needed
        midiIn.MessageReceived += MidiIn_MessageReceived;
        midiIn.Start();

        Console.WriteLine("Listening for drum hits...");
        Console.ReadLine();

        midiIn.Stop();
        midiIn.Dispose();
    }


        static void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteOnEvent noteOn && noteOn.Velocity > 0)
            {
                Console.WriteLine(
                        $"MISSING Note: {noteOn.NoteNumber}, Velocity: {noteOn.Velocity}");

                if (drumMap.TryGetValue(noteOn.NoteNumber, out var sound))
                {
                    PlaySound(sound);
                }
                else
                {
                    
                }
            }
        }


    
}
