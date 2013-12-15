using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Midi;
using System.Collections;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    public class MidiMachine
    {

        OutputDevice currentOutputDevice;

        Clock clock;

        public MidiMachine()
        { 
        }
      
        public ArrayList getMidiDeviceNames()
        {

            ArrayList listOfNames = new ArrayList();

            foreach (OutputDevice outputDevice in OutputDevice.InstalledDevices)
            {

                listOfNames.Add(outputDevice.Name);

            }

            return listOfNames;

        }

        public bool openMidiDeviseByIndex(int deviceIndex)
        {

            //close open devices

            if (currentOutputDevice != null)
            {
                if (currentOutputDevice.IsOpen)
                    currentOutputDevice.Close();
            }

            //opens new device geändert!

            currentOutputDevice = OutputDevice.InstalledDevices[1];
            currentOutputDevice.Open();

            if (currentOutputDevice.IsOpen)
                return true;
            else
                return false;
        }

        public void sendMidiCC(int midiChannel, int midiControl, float value)
        {

            if (currentOutputDevice != null)
            {
                Midi.Channel channel = (Midi.Channel)midiChannel;
                Midi.Control control = (Midi.Control)midiControl;

                currentOutputDevice.SendControlChange(channel, control, (int)value * 127);
            }

        }

        public void sendMidiNoteOn(int midiChannel, int notePitch, int velocity)
        {

            if (currentOutputDevice != null)
            {
                Pitch pitch = (Pitch)notePitch;
                Channel channel = (Channel)midiChannel;

                currentOutputDevice.SendNoteOn(channel, pitch, velocity);
            }
        }

        public void sendMidiNoteOff(int midiChannel, int notePitch, int velocity)
        {

            if (currentOutputDevice != null)
            {
                Pitch pitch = (Pitch)notePitch;
                Channel channel = (Channel)midiChannel;

                currentOutputDevice.SendNoteOff(channel, pitch, velocity);
            }
        }

        public Midi.OutputDevice getOutputDevice()
        {
            return currentOutputDevice;
        }
        //public void setMidiClock(float bpm)
        //{
        //           clock = new Clock(bpm);
        //}

        //public void sendMidiOnOff(int midiChannel, int pitch, int velocity, float time, float duration)
        //{

        //    NoteOnOffMessage sdfa = new NoteOnOffMessage(
        //    Midi.Channel channel = (Midi.Channel)midiChannel;
        //    Midi.Pitch new_pitch = (Midi.Pitch)pitch;


        //    clock.Schedule(new NoteOnOffMessage(currentOutputDevice, midiChannel, new_pitch, 80, (float)1, clock, 1));
        //}
    }
}
