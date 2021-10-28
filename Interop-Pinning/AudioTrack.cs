using ManagedBass;
using System.Runtime.InteropServices;


namespace Interop_Pinning
{
    public sealed class AudioTrack : IDisposable
    {
        private readonly object lockObject = new object();
        private const double MillisecondsPerSecond = 1000.0d;

        private bool disposedValue = false; // To detect redundant calls
        private bool endCueReached;

        public int Stream => _stream;

        int _stream;

        //Avoid GC of callback:
        private readonly SyncProcedure? _eofHandler;
        private readonly SyncProcedure? _endCueHandler;
        private GCHandle _gCHandle;

        public EventHandler? ReachedEndCue;
        public EventHandler? ReachedEndOfFile;

        public AudioTrack(Stream fileStream, int endCue=0)
        {
            var length = fileStream.Length;
            var buffer = new byte[length];
            fileStream.Read(buffer, 0, (int)length);
            fileStream.Close();
            _gCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            LoadStream(length);

            _eofHandler = new SyncProcedure(HandleEof);

            int eofSetResult = Bass.ChannelSetSync(_stream, SyncFlags.End | SyncFlags.Mixtime, 0, _eofHandler, IntPtr.Zero);
            if (eofSetResult == 0)
                throw new Exception("Could not set EOF");

            if (endCue > 0)
            {
                long endCuePosition = Bass.ChannelSeconds2Bytes(_stream, endCue / MillisecondsPerSecond);
                _endCueHandler = new SyncProcedure(HandleEndCue);

                int endCueResult = Bass.ChannelSetSync(_stream, SyncFlags.Position, endCuePosition, _endCueHandler, IntPtr.Zero);
                if (endCueResult == 0) {
                    throw new Exception("Could not set EndCue");
                }
            }
        }

        private bool LoadStream(long length)
        {
            var handle = _gCHandle.AddrOfPinnedObject();

            _stream = Bass.CreateStream(handle, 0, length, BassFlags.Decode);
            return CheckStream(length);
        }

        private bool CheckStream(long length)
        {
            if (_stream != 0)
            {
                var info = Bass.ChannelGetInfo(_stream);
                Console.WriteLine($"Track info: type={info.ChannelType} rate={info.Frequency} res={info.Resolution}");
                return true;
            }
            else
            {
                Console.WriteLine("Couldn't load stream ({Length})", length);
            }
            return false;
        }

        private void HandleEndCue(int handle, int channel, int data, IntPtr user)
        {
            lock (lockObject)
            {
                try
                {
                    if (disposedValue)
                    {
                        return;
                    }

                    endCueReached = true;
                    
                    if (ReachedEndCue != null)
                    {
                        Task.Run(() => ReachedEndCue(this, EventArgs.Empty));
                    }

                    Console.WriteLine("End-Cue was raised");
                }
                catch
                {
                    // ignore eventhandler/GC issues. Nothing we can do anyway
                    Console.WriteLine("Could not handle end-cue");
                }
            }
        }

        private void HandleEof(int handle, int channel, int data, IntPtr user)
        {
            lock (lockObject)
            {
                try
                {
                    if (disposedValue)
                    {
                        Console.WriteLine("End of file event on disposed track handled");
                        return;
                    }

                    if (endCueReached)
                    {
                        Console.WriteLine("End of file: already handled EndCue");
                    }
                    else
                    {
                        if (ReachedEndOfFile != null)
                        {
                            Task.Run(() => ReachedEndOfFile(this, new EventArgs()));
                        }
                        Console.WriteLine("End of file event raised");
                    }
                }
                catch
                {
                    // ignore eventhandler/GC issues. Nothing we can do anyway
                    Console.WriteLine("Could not handle EOF");
                }
            }
        }

        
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Console.WriteLine($"Track disposing");
                if (disposing)
                {
                    ReachedEndCue = null;
                    ReachedEndOfFile = null;
                }

                try
                {
                    Bass.StreamFree(_stream);
                    _gCHandle.Free();
                }
                catch
                {
                    //do not attempt to handle... 
                }

                disposedValue = true;
            }
        }

        ~AudioTrack()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
