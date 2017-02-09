﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Media;

namespace FlexAlert
{
    public class FlexAlertNoise
    {
        private SoundPlayer player = null;
        private BinaryWriter writer = null;

      

        /// <summary>
        /// Create a custom Alert Noise to play using FlexAlertNoise.Play();
        /// </summary>
        /// <param name="freq">Frequency in Hertz</param>
        /// <param name="tenthseconds">Duration in tenths of a second</param>
        /// <param name="vibrate">Allow pitch to vibrate up and down</param>
        public FlexAlertNoise(double freq, uint tenthseconds, bool vibrate)
        {
            string header_GroupID = "RIFF";  // RIFF
            uint header_FileLength = 0;      // total file length minus 8, which is taken up by RIFF
            string header_RiffType = "WAVE"; // always WAVE

            string fmt_ChunkID = "fmt "; // Four bytes: "fmt "
            uint fmt_ChunkSize = 16;     // Length of header in bytes
            ushort fmt_FormatTag = 1;        // 1 for PCM
            ushort fmt_Channels = 1;         // Number of channels, 2=stereo
            uint fmt_SamplesPerSec = 14000;  // sample rate, e.g. CD=44100
            ushort fmt_BitsPerSample = 8;   // bits per sample
            ushort fmt_BlockAlign =
                (ushort)(fmt_Channels * (fmt_BitsPerSample / 8)); // sample frame size, in bytes
            uint fmt_AvgBytesPerSec =
                fmt_SamplesPerSec * fmt_BlockAlign; // for estimating RAM allocation

            string data_ChunkID = "data";  // "data"
            uint data_ChunkSize;           // Length of header in bytes
            byte[] data_ByteArray;

            // Fill the data array with sample data

            // Number of samples = sample rate * channels * bytes per sample * duration in seconds
            uint numSamples = fmt_SamplesPerSec * fmt_Channels * tenthseconds / 10;
            data_ByteArray = new byte[numSamples];

            //int amplitude = 32760, offset=0; // for 16-bit audio
            int amplitude = 127, offset = 128; // for 8-audio
            double period = (2.0 * Math.PI * freq) / (fmt_SamplesPerSec * fmt_Channels);
            double amp;
            uint section = 1;

            for (uint i = 0; i < numSamples - 1; i += fmt_Channels)
            {
                if (vibrate)
                {
                    if (i < (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / numSamples; // amplitude decay
                    else if (i < 2 * (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / (numSamples * 2);
                    else if (i < 3 * (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / numSamples;
                    else if (i < 4 * (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / (numSamples * 2);
                    else if (i < 5 * (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / numSamples;
                    else if (i < 6 * (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / (numSamples * 2);
                    else if (i < 7 * (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / numSamples;
                    else if (i < 8 * (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / (numSamples * 2);
                    else if (i < 9 * (numSamples / 10))
                        amp = amplitude * (double)(numSamples - i) / numSamples;
                    else
                        amp = amplitude * (double)(numSamples - i) / (numSamples * 2);

                }
                else
                    amp = amplitude * (double)(numSamples - i) / numSamples;
                                                                                 
                for (int channel = 0; channel < fmt_Channels; channel++)
                {
                    data_ByteArray[i + channel] = Convert.ToByte(amp * Math.Sin(i * period) + offset);
                }
            }

            // Calculate file and data chunk size in bytes
            data_ChunkSize = (uint)(data_ByteArray.Length * (fmt_BitsPerSample / 8));
            header_FileLength = 4 + (8 + fmt_ChunkSize) + (8 + data_ChunkSize);

            // write data to a MemoryStream with BinaryWriter
            MemoryStream audioStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(audioStream);

            // Write the header
            writer.Write(header_GroupID.ToCharArray());
            writer.Write(header_FileLength);
            writer.Write(header_RiffType.ToCharArray());

            // Write the format chunk
            writer.Write(fmt_ChunkID.ToCharArray());
            writer.Write(fmt_ChunkSize);
            writer.Write(fmt_FormatTag);
            writer.Write(fmt_Channels);
            writer.Write(fmt_SamplesPerSec);
            writer.Write(fmt_AvgBytesPerSec);
            writer.Write(fmt_BlockAlign);
            writer.Write(fmt_BitsPerSample);

            // Write the data chunk
            writer.Write(data_ChunkID.ToCharArray());
            writer.Write(data_ChunkSize);
            foreach (byte dataPoint in data_ByteArray)
            {
                writer.Write(dataPoint);
            }
            player = new SoundPlayer(audioStream);
        }

        public void Play()
        {
            if (player != null)
            {
                player.Stream.Seek(0, SeekOrigin.Begin);
                player.Play();
            }
            else
            { throw new Exception("Player not initialized."); }
        }

        public void Dispose()
        {
            if (writer != null)
                writer.Close();
            if (player != null)
                player.Dispose();

            writer = null;
            player = null;
        }
    }
}
