const int deviceId = 1;

// Build very basic audio pipeline
if (!Bass.Init(deviceId))
{
    throw new InvalidOperationException("Could not get access to audio device, is device configured properly?");
}

var selectedDevice = Bass.GetDeviceInfo(deviceId);
Console.WriteLine($"Selected device:{selectedDevice.Name}");

var audioFile = File.OpenRead("demo.mp3");
var track = new AudioTrack(audioFile);

var audioMixerStream = BassMix.CreateMixerStream(44100, 2, BassFlags.Default);
BassMix.MixerAddChannel(audioMixerStream, track.Stream, BassFlags.MixerChanDownMix);

Bass.ChannelPlay(audioMixerStream);

//Poor mans waiting for track to end
Console.WriteLine("Press enter to exit");
Console.ReadLine();
