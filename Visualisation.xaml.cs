using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Timers;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    /// <summary>
    /// Interaktionslogik für Visualisation.xaml
    /// </summary>
    public partial class Visualisation : Window
    {

        Model3DGroup triangle;
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;
        Timer timer;
        int x;
        

        public Visualisation()
        {
            InitializeComponent();

            //startTriangle();

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            /*
            x = 0;
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.X1 = 1 + x;
            myLine.X2 = 50;
            myLine.Y1 = 1;
            myLine.Y2 = 50;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            myCanvas.Children.Add(myLine);
            */
            //timer = new Timer(1000);
            //timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            //timer.Start();




 

            //BeachBallSphere();

           
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Add a Line Element

            
        }

        public void updateVisuals(double variance, double vertical, double hightOfTrees)
        {
            if (variance <= 1.2)
                variance = 1.2;

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                
                double circleRadius = vertical / 40 +2;
    
                for (int i = 0; i < 11; i++)
                {

                    double peakHight = 1 / Math.Sqrt(2 * Math.PI * variance) * Math.Exp(-Math.Pow(i - 5, 2) / (2 * variance));

                    double heightOfTreesForRadius = hightOfTrees / 70;

                    dc.DrawEllipse(null, new Pen(Brushes.LawnGreen, 1), new Point(10 * i, -100 * peakHight - hightOfTrees), circleRadius + heightOfTreesForRadius, circleRadius + heightOfTreesForRadius);
                    dc.DrawLine(new Pen(Brushes.LawnGreen, 1), new Point(10* i, -(100 * peakHight - circleRadius - heightOfTreesForRadius + hightOfTrees)), new Point(10 * i, 0));

                }

                //dc.DrawEllipse(null, new Pen(Brushes.LawnGreen, 6), new Point(0,0), 5, 5);
                //dc.DrawEllipse(Brushes.LawnGreen, null, new Point(10, 0), 5, 5);
                //dc.DrawLine(new Pen(Brushes.LawnGreen, 3), new Point(0, 0), new Point(0, 20));
                //dc.DrawLine(new Pen(Brushes.LawnGreen, 3), new Point(10, 0), new Point(10, 20));
            }
        }


        public void BeachBallSphere()
        {
            //Title = "Beachball Sphere";

            // Create Viewport3D as content of window.
            Viewport3D viewport = new Viewport3D();
            Content = viewport;

            // Get the MeshGeometry3D from the GenerateSphere method.
            MeshGeometry3D mesh = 
                GenerateSphere(new Point3D(0, 0, 0), 1, 36, 18);
            mesh.Freeze();

            // Define a brush for the sphere.
            Brush[] brushes = new Brush[6] { Brushes.Red, Brushes.Blue, 
                                             Brushes.Yellow, Brushes.Orange, 
                                             Brushes.Green, Brushes.White };
            DrawingGroup drawgrp = new DrawingGroup();

            for (int i = 0; i < brushes.Length ; i++)
            {
                RectangleGeometry rectgeo = 
                    new RectangleGeometry(new Rect(10 * i, 0, 10, 60));

                GeometryDrawing geodraw = 
                    new GeometryDrawing(brushes[i], null, rectgeo);

                drawgrp.Children.Add(geodraw);
            }
            DrawingBrush drawbrsh = new DrawingBrush(drawgrp);
            drawbrsh.Freeze();

            // Define the GeometryModel3D.
            GeometryModel3D geomod = new GeometryModel3D();
            geomod.Geometry = mesh;
            geomod.Material = new DiffuseMaterial(drawbrsh);

            // Create a ModelVisual3D for the GeometryModel3D.
            ModelVisual3D modvis = new ModelVisual3D();
            modvis.Content = geomod;
            viewport.Children.Add(modvis);

            // Create another ModelVisual3D for light.
            Model3DGroup modgrp = new Model3DGroup();
            modgrp.Children.Add(new AmbientLight(Color.FromRgb(128, 128, 128)));
            modgrp.Children.Add(
                new DirectionalLight(Color.FromRgb(128, 128, 128),
                                     new Vector3D(2, -3, -1)));

            modvis = new ModelVisual3D();
            modvis.Content = modgrp;
            viewport.Children.Add(modvis);

            // Create the camera.
            PerspectiveCamera cam = new PerspectiveCamera(new Point3D(0, 0, 8), 
                            new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 45);
            viewport.Camera = cam;

            // Create a transform for the GeometryModel3D.
            AxisAngleRotation3D axisangle = 
                new AxisAngleRotation3D(new Vector3D(1, 1, 0), 0);
            RotateTransform3D rotate = new RotateTransform3D(axisangle);
            geomod.Transform = rotate;
            
            // Animate the RotateTransform3D.
            DoubleAnimation anima = 
                new DoubleAnimation(360, new Duration(TimeSpan.FromSeconds(5)));
            anima.RepeatBehavior = RepeatBehavior.Forever;
            axisangle.BeginAnimation(AxisAngleRotation3D.AngleProperty, anima);
        }

        MeshGeometry3D GenerateSphere(Point3D center, double radius,
                                      int slices, int stacks)
        {
            // Create the MeshGeometry3D.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Fill the Position, Normals, and TextureCoordinates collections.
            for (int stack = 0; stack <= stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / stacks;
                double y = radius * Math.Sin(phi);
                double scale = -radius * Math.Cos(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    double theta = slice * 2 * Math.PI / slices;
                    double x = scale * Math.Sin(theta);
                    double z = scale * Math.Cos(theta);

                    Vector3D normal = new Vector3D(x, y, z);
                    mesh.Normals.Add(normal);
                    mesh.Positions.Add(normal + center);
                    mesh.TextureCoordinates.Add(
                            new Point((double)slice / slices,
                                      (double)stack / stacks));
                }
            }

            // Fill the TriangleIndices collection.
            for (int stack = 0; stack < stacks; stack++)
                for (int slice = 0; slice < slices; slice++)
                {
                    int n = slices + 1; // Keep the line length down.

                    if (stack != 0)
                    {
                        mesh.TriangleIndices.Add((stack + 0) * n + slice);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice);
                        mesh.TriangleIndices.Add((stack + 0) * n + slice + 1);
                    }
                    if (stack != stacks - 1)
                    {
                        mesh.TriangleIndices.Add((stack + 0) * n + slice + 1);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice + 1);
                    }
                }
            return mesh;
        }
    

        public Model3DGroup CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2)
        {
            MeshGeometry3D mymesh = new MeshGeometry3D();

            mymesh.Positions.Add(p0);
            mymesh.Positions.Add(p1);
            mymesh.Positions.Add(p2);
            mymesh.TriangleIndices.Add(0);
            mymesh.TriangleIndices.Add(1);
            mymesh.TriangleIndices.Add(2);
            Vector3D Normal = CalculateTraingleNormal(p0, p1, p2);
            mymesh.Normals.Add(Normal);
            mymesh.Normals.Add(Normal);
            mymesh.Normals.Add(Normal);

            Material Material = new DiffuseMaterial(
                new SolidColorBrush(Colors.Orange));
            GeometryModel3D model = new GeometryModel3D(
                mymesh, Material);
            Model3DGroup Group = new Model3DGroup();
            Group.Children.Add(model);
            return Group; 
        }

        //private void startTriangle()
        //{


        //    triangle = new Model3DGroup();
        //    Point3D p0 = new Point3D(0, 0, 0);
        //    Point3D p1 = new Point3D(5, 0, 0);
        //    Point3D p2 = new Point3D(5, 0, 5);
        //    Point3D p3 = new Point3D(0, 0, 5);
        //    Point3D p4 = new Point3D(0, 5, 0);
        //    Point3D p5 = new Point3D(5, 5, 0);
        //    Point3D p6 = new Point3D(5, 5, 5);

        //    triangle.Children.Add(CreateTriangleModel(p1, p4, p3));
        //    triangle.Children.Add(CreateTriangleModel(p1, p4, p6));
        //    triangle.Children.Add(CreateTriangleModel(p3, p1, p6));

        //    ModelVisual3D Model = new ModelVisual3D();
        //    Model.Content = triangle;
        //    this.MainViewPort.Children.Add(Model);

        //    rotateTriangle();
        //}

        private Vector3D CalculateTraingleNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(
                p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(
                p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        private void rotateTriangle()
        {
            //Define a transformation
            RotateTransform3D myRotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 2, 1), 1));
            //Define an animation for the transformation
            DoubleAnimation myAnimation = new DoubleAnimation();
            myAnimation.From = 1;
            myAnimation.To = 361;
            myAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(5000));
            myAnimation.RepeatBehavior = RepeatBehavior.Forever;
            //Add animation to the transformation
            myRotateTransform.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, myAnimation);

            //Add transformation to the model
            triangle.Transform = myRotateTransform;
        }

    }


}
