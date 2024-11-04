using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
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
            string imagePath = "test1.jpg";
            //string imagePath = "test2.jpg";
            //string imagePath = "test3.jpg";
            Bitmap image = new Bitmap(imagePath);

            var midiFile = new MidiFile();
            var trackChunk = new TrackChunk();
            midiFile.Chunks.Add(trackChunk);

            int tempo = 500000; // 120 BPM
            trackChunk.Events.Add(new SetTempoEvent(tempo));

            long noteDuration = 180; // Short duration for each note
            long deltaTimeBetweenNotes = 60; // Short pause between notes

            // Only process every 10th pixel to reduce length and file size
            for (int y = 0; y < image.Height; y += 10)
            {
                for (int x = 0; x < image.Width; x += 10)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int brightness = (int)(pixelColor.GetBrightness() * 127);

                    // Map brightness to a MIDI note range (e.g., C4 to C5)
                    int noteNumber = 60 + brightness % 12;

                    // Start the note
                    trackChunk.Events.Add(new NoteOnEvent((SevenBitNumber)noteNumber, (SevenBitNumber)90) { DeltaTime = (int)deltaTimeBetweenNotes });

                    // End the note
                    trackChunk.Events.Add(new NoteOffEvent((SevenBitNumber)noteNumber, (SevenBitNumber)90) { DeltaTime = (int)noteDuration });
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

        static void TwinkleMidi()
        {
            var notes = new (NoteName note, int octave, MusicalTimeSpan duration)[]
            {
            (NoteName.C, 4, MusicalTimeSpan.Quarter), (NoteName.C, 4, MusicalTimeSpan.Quarter),
            (NoteName.G, 4, MusicalTimeSpan.Quarter), (NoteName.G, 4, MusicalTimeSpan.Quarter),
            (NoteName.A, 4, MusicalTimeSpan.Quarter), (NoteName.A, 4, MusicalTimeSpan.Quarter),
            (NoteName.G, 4, MusicalTimeSpan.Half),

            (NoteName.F, 4, MusicalTimeSpan.Quarter), (NoteName.F, 4, MusicalTimeSpan.Quarter),
            (NoteName.E, 4, MusicalTimeSpan.Quarter), (NoteName.E, 4, MusicalTimeSpan.Quarter),
            (NoteName.D, 4, MusicalTimeSpan.Quarter), (NoteName.D, 4, MusicalTimeSpan.Quarter),
            (NoteName.C, 4, MusicalTimeSpan.Half),

            (NoteName.G, 4, MusicalTimeSpan.Quarter), (NoteName.G, 4, MusicalTimeSpan.Quarter),
            (NoteName.F, 4, MusicalTimeSpan.Quarter), (NoteName.F, 4, MusicalTimeSpan.Quarter),
            (NoteName.E, 4, MusicalTimeSpan.Quarter), (NoteName.E, 4, MusicalTimeSpan.Quarter),
            (NoteName.D, 4, MusicalTimeSpan.Half),

            (NoteName.G, 4, MusicalTimeSpan.Quarter), (NoteName.G, 4, MusicalTimeSpan.Quarter),
            (NoteName.F, 4, MusicalTimeSpan.Quarter), (NoteName.F, 4, MusicalTimeSpan.Quarter),
            (NoteName.E, 4, MusicalTimeSpan.Quarter), (NoteName.E, 4, MusicalTimeSpan.Quarter),
            (NoteName.D, 4, MusicalTimeSpan.Half),

            (NoteName.C, 4, MusicalTimeSpan.Quarter), (NoteName.C, 4, MusicalTimeSpan.Quarter),
            (NoteName.G, 4, MusicalTimeSpan.Quarter), (NoteName.G, 4, MusicalTimeSpan.Quarter),
            (NoteName.A, 4, MusicalTimeSpan.Quarter), (NoteName.A, 4, MusicalTimeSpan.Quarter),
            (NoteName.G, 4, MusicalTimeSpan.Half),

            (NoteName.F, 4, MusicalTimeSpan.Quarter), (NoteName.F, 4, MusicalTimeSpan.Quarter),
            (NoteName.E, 4, MusicalTimeSpan.Quarter), (NoteName.E, 4, MusicalTimeSpan.Quarter),
            (NoteName.D, 4, MusicalTimeSpan.Quarter), (NoteName.D, 4, MusicalTimeSpan.Quarter),
            (NoteName.C, 4, MusicalTimeSpan.Half)
            };

            var midiFile = new MidiFile();
            var trackChunk = new TrackChunk();
            midiFile.Chunks.Add(trackChunk);


            int tempo = 500000; // Set tempo (120 BPM)
            trackChunk.Events.Add(new SetTempoEvent(tempo));
            long currentTime = 0;

            foreach (var (noteName, octave, duration) in notes)
            {
                long noteLength = LengthConverter.ConvertFrom(duration, currentTime, TempoMap.Default);
                trackChunk.Events.Add(new NoteOnEvent((SevenBitNumber)NoteUtilities.GetNoteNumber(noteName, octave), (SevenBitNumber)90) { DeltaTime = (int)currentTime });

                currentTime = noteLength;  // move time to next note
                trackChunk.Events.Add(new NoteOffEvent((SevenBitNumber)NoteUtilities.GetNoteNumber(noteName, octave), (SevenBitNumber)90) { DeltaTime = (int)currentTime });
            }

            string fileName = "TwinkleTwinkle.mid";
            if (File.Exists(fileName))
                File.Delete(fileName);

            midiFile.Write(fileName);

            Console.WriteLine($"MIDI file created and saved: {fileName}");
        }
    }
}