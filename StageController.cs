using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Midi;
using System.Windows.Media;
using System.Windows;
using System.Collections;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{

    // 
    public class StageController : kinectInterpreter
    {

        // Skalierungskonstanten für die Anpassung der Sensordaten auf Midiwerte (0-128)
        int SCALING_FACTOR_KCT_MIDI = 80;
        float HIGHT_OF_HANDS_OFFSET = (float)0.1;
        private float HEIGHT_VALUE_NORM_FACTOR = (float)1.5;
        private float PERCUSSION_RADIUS = (float)0.45;
        private int POS_HIST_LEN = 5;

        // Zusätzliche Visualisierung in eigenem Fenster...
        Visualisation visualisation;
        
        List<SkeletonPoint> positionHistory;
        List<SkeletonPoint> positionHistory2;
        private double HIGH_VEL_DIST = 0.1;
        private bool freshHit;
        private bool freshHit2;
        private MainWindow mainWindow;
        


        public StageController(MainWindow newMainWindow)
        {
            positionHistory = new List<SkeletonPoint>();
            positionHistory2 = new List<SkeletonPoint>();
            freshHit = true;
            freshHit2 = true;
            mainWindow = newMainWindow;
            /*SkeletonPoint defaultPoint = new SkeletonPoint();
            defaultPoint.X = 0;
            defaultPoint.Y = 0;
            defaultPoint.Z = 0;
            for (int i = 0; i < POS_HIST_LEN; i++)
                positionHistory.Add(defaultPoint);*/
        }

        // Eventhandler, wird aufgerufen wenn der Sensor neue Daten bereitstellt. 
        // Die 3d Position verschiedener Gelenke können aus dem "Skeleton"-Objekt ausgelesen werden.
        public override void SensorSkeletonFrameReady(Skeleton skeleton, DrawingContext dc)
        {
            processSkeleton(skeleton, dc);
        }

        // Daten werden verarbeitet und durch verschiedene Methoden ausgewertet.
        private void processSkeleton(Skeleton skeleton, DrawingContext dc)
        {

           

           // dc.DrawEllipse(Brushes.Green, new Pen(Brushes.Green, 20), mainWindow.SkeletonPointToScreen(skeleton.Joints[JointType.ShoulderCenter].Position), 20,20);


            positionHistory.Add(skeleton.Joints[JointType.HandRight].Position);
            positionHistory2.Add(skeleton.Joints[JointType.HandLeft].Position);


            ArrayList newValues = panAndHeightToMidiOutput(skeleton, dc);

            newValues.AddRange(horizontalAndVerticalDistanceToMidiOutput(skeleton, dc));
            //newValues.Add(rightHandToPitch(skeleton.Joints[JointType.HandRight]));

            if (positionHistory.Count == POS_HIST_LEN)
            {

                newValues.AddRange(percussionToMidiOutput(skeleton, dc, skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderRight], 0, ref positionHistory, ref freshHit));
                newValues.AddRange(percussionToMidiOutput(skeleton, dc, skeleton.Joints[JointType.HandLeft], skeleton.Joints[JointType.ShoulderLeft], 4, ref positionHistory2, ref freshHit2));
                positionHistory.RemoveAt(0);
                positionHistory2.RemoveAt(0);
            }

            // Zusätzliche Visualisierung in eigenem Fenster...
            if (visualisation != null)
            {
                visualisation.updateVisuals((int)newValues[2], (int)newValues[3], (int)newValues[1]);
            }

            



        }

        private ICollection percussionToMidiOutput(Skeleton skeleton, DrawingContext dc, Joint hittingJoint, Joint referenceJoint, int midiOffset, ref List<SkeletonPoint> posHist, ref bool frshHt)
        {

            Joint hitHand = hittingJoint;
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            Joint bodyCenter = referenceJoint;
            

            bool isInPercRadius = Distance2D(hitHand.Position.X, hitHand.Position.Y, bodyCenter.Position.X, bodyCenter.Position.Y) > PERCUSSION_RADIUS;
            if (isInPercRadius)
            {

                if (isHighVelocity(hitHand.Position, dc, posHist) && frshHt)
                {
                    int midiValue;

                    if (hitHand.Position.Y - bodyCenter.Position.Y < 0)
                    {

                        if (hitHand.Position.Y - bodyCenter.Position.Y < -PERCUSSION_RADIUS / 2)
                        {
                            midiValue = 1 + midiOffset;
                            dc.DrawEllipse(Brushes.Yellow, new Pen(Brushes.Yellow, 20), mainWindow.SkeletonPointToScreen(hittingJoint.Position), 20, 20);
                        }
                        else
                        {
                            midiValue = 2 + midiOffset;
                            dc.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 20), mainWindow.SkeletonPointToScreen(hittingJoint.Position), 20, 20);
                        }
                    }
                    else
                    {
                        if (hitHand.Position.Y - bodyCenter.Position.Y < PERCUSSION_RADIUS / 2)
                        {
                            midiValue = 3 + midiOffset;
                            dc.DrawEllipse(Brushes.Blue, new Pen(Brushes.Blue, 20), mainWindow.SkeletonPointToScreen(hittingJoint.Position), 20, 20);
                        }
                        else
                        {
                            midiValue = 4 + midiOffset;
                            dc.DrawEllipse(Brushes.Green, new Pen(Brushes.Green, 20), mainWindow.SkeletonPointToScreen(hittingJoint.Position), 20, 20);
                        }
                    }
                    

                    Clock clock = new Clock(120);  // beatsPerMinute=120
                    clock.Start();
                    Midi.Pitch midiPitch = (Midi.Pitch)midiValue;
                    clock.Schedule(new NoteOnOffMessage(currentOutputDevice, midiChannel, midiPitch, 50, 0, clock, 3));
                    frshHt = false;
                    // Console.WriteLine("Bang!" + bodyCenter.Position.X);
                    drawToScreen(dc, Distance2D(hitHand.Position.X, hitHand.Position.Y, bodyCenter.Position.X, bodyCenter.Position.Y), 6);
                    


                }

            }
            else
                frshHt = true;

            ArrayList newValues = new ArrayList();
            return newValues;
            
        }

        private bool isHighVelocity(SkeletonPoint currentPos, DrawingContext dc, List<SkeletonPoint> posHist)
        {
            bool isHigh = false;
            if (Distance2D(currentPos.X, currentPos.Y, posHist[0].X, posHist[0].Y) > HIGH_VEL_DIST)
            {
                isHigh = true;
                //drawToScreen(dc, Distance2D(currentPos.X, currentPos.Y, posHist[0].X, posHist[0].Y), 7);
                //drawToScreen(dc, positionHistory[0].X, 9);
                //drawToScreen(dc, positionHistory[0].Y, 10);
                //drawToScreen(dc, HIGH_VEL_DIST, 12);
            }
            return isHigh;
        }

        private SkeletonPoint getHistoryPos()
        {
            return positionHistory[0];
        }

        // Wertet das "Skeleton"-Objekt betreffend der horizontalen und vertikalen Distanz der Hände aus
        // und sendet Mididaten an das Output-Device.
        private ArrayList horizontalAndVerticalDistanceToMidiOutput(Skeleton skeleton, DrawingContext dc)
        {

            Joint rightHand = skeleton.Joints[JointType.HandRight];
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            ArrayList newValues = new ArrayList();

            // Horizontale Distanz
            double distanceHorizontal = Distance2D(rightHand.Position.X, rightHand.Position.Z, leftHand.Position.X, leftHand.Position.Z);

            int midiDistanceHorizontal = scaleDistanceOfHandsToMidiCC(distanceHorizontal, 0);

            currentOutputDevice.SendControlChange(midiChannel, Midi.Control.ModulationWheel, midiDistanceHorizontal);

            newValues.Add(midiDistanceHorizontal);

            drawToScreen(dc, midiDistanceHorizontal, 0);
            


            // Vertikale Distanz
            double distanceVertical = Math.Abs(rightHand.Position.Y - leftHand.Position.Y);

            int midiDistanceVertical = scaleDistanceOfHandsToMidiCC(distanceVertical, 10);

            currentOutputDevice.SendControlChange(midiChannel, Midi.Control.Volume, midiDistanceVertical);

            drawToScreen(dc, midiDistanceVertical, 1);

            newValues.Add(midiDistanceVertical);


            return newValues;

        }

        // Wertet das "Sekleton"-Objekt betreffend der horizontalen und vertikalen Durchschnittsposition
        // beider Hände aus.
        private ArrayList panAndHeightToMidiOutput(Skeleton skeleton, DrawingContext dc)
        {

            Joint rightHand = skeleton.Joints[JointType.HandRight];
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            ArrayList newValues = new ArrayList();


            // Panorama
            // Berechnet Mittelpunkt der Hände
            float positionOnXAxis = (rightHand.Position.X + leftHand.Position.X) / 2;

            // Wandelt Position in MidiCC
            int midiValuePanorama = Math.Abs((int)(normalizePanValue(positionOnXAxis) * 64)) % 63;

            if (rightHand.Position.X >= 0)
                midiValuePanorama = midiValuePanorama + 63;
            else
                midiValuePanorama = 63 - midiValuePanorama;

            // Sendet MidiCC
            currentOutputDevice.SendControlChange(midiChannel, Midi.Control.Pan, midiValuePanorama);

            drawToScreen(dc, midiValuePanorama, 3);

            newValues.Add(midiValuePanorama);


            // Höhe
            // Berechnet mittlere Höhe beider Hände
            float yAxis = (rightHand.Position.Y + leftHand.Position.Y) / 2 - HIGHT_OF_HANDS_OFFSET;

            // Scalieren zu Midi
            int midiValueHeight = (int)(normalizeHightValue(yAxis) * 127); // Math.Abs((int)(yAxis * 200));

            currentOutputDevice.SendControlChange(midiChannel, Midi.Control.Expression, midiValueHeight);

            newValues.Add(midiValueHeight);

            drawToScreen(dc, midiValueHeight, 4);


            return newValues;
        }

        private static void drawToScreen(DrawingContext dc, double valueToPrint, int position)
        {
            dc.DrawText(
                new FormattedText(valueToPrint.ToString(),
                System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                30, System.Windows.Media.Brushes.White),
                new System.Windows.Point(50, 50 + position * 30));
        }

        private int scaleDistanceOfHandsToMidiCC(double valueToScale, int gate)
        {
            int midiDistance = (int)(valueToScale * SCALING_FACTOR_KCT_MIDI);
            if (midiDistance > 127)
                midiDistance = 127;
            if (valueToScale < gate)
                valueToScale = 0;
            return midiDistance;
        }

        private float normalizeHightValue(float yAxis)
        {
            if (yAxis < 0)
                yAxis = 0;

            yAxis = yAxis * HEIGHT_VALUE_NORM_FACTOR;

            if (yAxis > 1)
                yAxis = 1;

            return yAxis;
        }

        private float normalizePanValue(float positionOnXAxis)
        {
            if (positionOnXAxis > 1)
                positionOnXAxis = 1;
            if (positionOnXAxis < -1)
                positionOnXAxis = -1;

            return positionOnXAxis;
        }

        private Joint rightHandToPitch(Joint rightHand)
        {
            float yAxis = rightHand.Position.Y;



            int midiValueY = Math.Abs((int)(yAxis * 50)) % 50 + 50;
            //Pitch pitchSend = (Pitch)(positionY);

            Clock clock = new Clock(120);  // beatsPerMinute=120
            clock.Start();
            Midi.Pitch midiPitch = (Midi.Pitch)midiValueY;
            clock.Schedule(new NoteOnOffMessage(currentOutputDevice, midiChannel, midiPitch, 50, 0, clock, 1));
            return rightHand;
        }

        public static double Distance3D(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            //     __________________________________
            //d = &#8730; (x2-x1)^2 + (y2-y1)^2 + (z2-z1)^2
            //

            //Our end result
            double result = 0;
            //Take x2-x1, then square it
            double part1 = Math.Pow((x2 - x1), 2);
            //Take y2-y1, then sqaure it
            double part2 = Math.Pow((y2 - y1), 2);
            //Take z2-z1, then square it
            double part3 = Math.Pow((z2 - z1), 2);
            //Add both of the parts together
            double underRadical = part1 + part2 + part3;
            //Get the square root of the parts
            result = Math.Sqrt(underRadical);
            //Return our result
            return result;
        }

        public static double Distance2D(double x1, double y1, double x2, double y2)
        {
            //     __________________________________
            //d = &#8730; (x2-x1)^2 + (y2-y1)^2 + (z2-z1)^2
            //

            //Our end result
            double result = 0;
            //Take x2-x1, then square it
            double part1 = Math.Pow((x2 - x1), 2);
            //Take y2-y1, then sqaure it
            double part2 = Math.Pow((y2 - y1), 2);

            //Add both of the parts together
            double underRadical = part1 + part2;
            //Get the square root of the parts
            result = Math.Sqrt(underRadical);
            //Return our result
            return result;
        }

        public Visualisation startVisualisation()
        {
            visualisation = new Visualisation();
            return visualisation;

        }
    }
}

