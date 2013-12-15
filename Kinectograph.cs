using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Kinect;
using Microsoft.Kinect;
using Midi;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    public class Kinectograph
    {

        private KinectSensor sensor;

        public Kinectograph() { }

        public void makeSensorReady()
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                //this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                //this.statusBarText.Text = Properties.Resources.NoKinectReady;

            }

        }

        public KinectSensor getSensor()
        {
            return sensor;
        }

        //public void addMathodAsListenerToSkeletonFrameReady(object drawSkeleton)
        //{

        //    //unbedingt ;) v
        //    throw new NotImplementedException();


        //}

        public bool isSensorReady()
        {
            if (sensor == null)
                return false;
            else
                return true;
        }

        public void seatedModeOn(bool state)
        {
            if(sensor != null)
            {
            if(state)
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            else
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }
            
        }
    }
}
