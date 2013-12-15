using System;
namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    interface IMainWindow
    {
        void InitializeComponent();
        Microsoft.Kinect.KinectSensor Sensor { get; set; }
    }
}
