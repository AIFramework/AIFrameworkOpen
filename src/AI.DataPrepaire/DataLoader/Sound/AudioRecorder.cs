using AI.DataStructs.Algebraic;
using NAudio.Wave;
using System;
using System.Text;

namespace AI.DataPrepaire.DataLoader.Sound
{
    /// <summary>
    /// A class that manages audio recording from a microphone input using NAudio library.
    /// </summary>
    [Serializable]
    public class AudioRecorder
    {
        private WaveInEvent waveSource = null;
        bool _isRunRec = false; // Начата ли запись
        /// <summary>
        /// Is start record
        /// </summary>
        public bool IsRecording => _isRunRec;
        /// <summary>
        /// Sample rate
        /// </summary>
        public int SampleRate { get; set; }


        /// <summary>
        /// Delegate for handling audio data received events.
        /// </summary>
        /// <param name="buffer">The buffer containing audio data.</param>
        /// <param name="bytesRecorded">The number of bytes recorded in the buffer.</param>
        public delegate void AudioDataReceivedEventHandler(byte[] buffer, int bytesRecorded);

        /// <summary>
        /// Event triggered when a buffer of audio data has been received.
        /// </summary>
        public event AudioDataReceivedEventHandler OnBufferReceived;

        /// <summary>
        /// Event triggered when a buffer of audio data has been received (Vector ret)
        /// </summary>
        public event Action<Vector> NewSignalPart;


        /// <summary>
        /// A class that manages audio recording from a microphone input using NAudio library.
        /// </summary>
        public AudioRecorder() 
        {
            OnBufferReceived += AudioRecorder_OnBufferReceived;
            NewSignalPart += AudioRecorder_NewSignalPart;
        }




        /// <summary>
        /// Starts recording audio from the default input device.
        /// </summary>
        public void StartRecording(int fd = 8000)
        {
            waveSource = new WaveInEvent();
            waveSource.WaveFormat = new WaveFormat(fd, 1); // CD quality audio, mono

            // Attach event handlers for data available and recording stopped events
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            // Start recording
            waveSource.StartRecording();
            _isRunRec = true;
            SampleRate = fd;
        }

        /// <summary>
        /// Handles the data available event from the WaveIn device and writes to the wave file.
        /// Also, raises the OnBufferReceived event with the audio data.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">WaveInEventArgs containing the buffer and bytes recorded.</param>
        private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
           
            // Raise the buffer received event
            OnBufferReceived?.Invoke(e.Buffer, e.BytesRecorded);
        }

        /// <summary>
        /// Handles the recording stopped event, cleans up resources.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">StoppedEventArgs containing event data.</param>
        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            // Dispose WaveIn and WaveFileWriter objects
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

        }

        /// <summary>
        /// Stops the recording process.
        /// </summary>
        public void StopRecording()
        {
            // Trigger stop recording
            waveSource.StopRecording();
            _isRunRec = false;
        }

        /// <summary>
        /// Если запись не идет - начинает ее, если идет - останавливает
        /// </summary>
        public bool StartAndStop(int sr = 8000) 
        {
            if(_isRunRec)
                StopRecording();
            else StartRecording(sr);
            return _isRunRec;
        }


        private void AudioRecorder_OnBufferReceived(byte[] buffer, int bytesRecorded)
        {
            var ints = AudioHelper.ConvertByteToShort(buffer);
            var floats = AudioHelper.ConvertShortToFloat(ints);
            NewSignalPart(floats);
        }



        private void AudioRecorder_NewSignalPart(Vector obj)
        {}
    }
}
