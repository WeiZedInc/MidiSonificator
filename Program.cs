using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.Drawing;

namespace MidiSonificator
{
    internal class Program
    {
        //https://cifkao.github.io/html-midi-player/
        static void Main(string[] args)
        {
            SonificationByPixelsBrightness();
        }

        static void SonificationByPixelsBrightness()
        {
            //string imagePath = "test1.jpg";
            string imagePath = "test2.jpg";
            //string imagePath = "test3.jpg";
            Bitmap image = new Bitmap(imagePath);

            var midiFile = new MidiFile();
            var trackChunk = new TrackChunk();
            midiFile.Chunks.Add(trackChunk);

            long pauseBetweenNotes = 60;
            int tempo = 500000; // 120 BPM
            trackChunk.Events.Add(new SetTempoEvent(tempo));


            for (int y = 0; y < image.Height; y += 10)
            {
                for (int x = 0; x < image.Width; x += 10)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int brightness = (int)(pixelColor.GetBrightness() * 127);

                    int noteNumber = 60 + brightness % 12;
                    int velocity = (int)(pixelColor.GetSaturation() * 127);
                    int hue = (int)(pixelColor.GetHue() / 360 * 127);
                    var programChangeEvent = new ProgramChangeEvent((SevenBitNumber)(hue % 128));
                    trackChunk.Events.Add(programChangeEvent);

                    // min 120, max 480
                    long noteDuration = Random.Shared.Next(120, 280);

                    // Start the note with volume affected by saturation
                    trackChunk.Events.Add(new NoteOnEvent((SevenBitNumber)noteNumber, (SevenBitNumber)velocity) { DeltaTime = (int)pauseBetweenNotes });

                    // End the note
                    trackChunk.Events.Add(new NoteOffEvent((SevenBitNumber)noteNumber, (SevenBitNumber)velocity) { DeltaTime = (int)noteDuration });
                }
            }

            string fileName = "ImageSonification.mid";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            midiFile.Write(fileName);

            Console.WriteLine($"MIDI file created and saved: {fileName}");
        }
    }
}