using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Midi;
using System.Windows.Media;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{

    //Basisklasse für alle Klassen, die Skelettdaten verarbeiten und Midi-Daten ausgeben.
    //Der kinectInterpreter muss mit einem Midi-Outputdevice verbunden werden und die 
    //SensorSkeletonFrameReady-Methode dem Kinectsensor übergeben werden.
    public class kinectInterpreter
    {

        private int MIDI_CHANNEL = 0;
        protected OutputDevice currentOutputDevice;
        protected Midi.Channel midiChannel;

        public kinectInterpreter()
        {
            // Midikanal wird gesetzt (Standard 0)
            midiChannel = (Midi.Channel)MIDI_CHANNEL;
        }

        // Verbindet mit Midi-Outputdevice
        public void connectToMidiMachine(OutputDevice new_outputDevice)
        {

            currentOutputDevice = new_outputDevice;

        }

        // Eventhandler, der dem Kinect Sensor übergeben wird
        public virtual void SensorSkeletonFrameReady(Skeleton skeleton, DrawingContext dc)
        {

        }
    }
}
