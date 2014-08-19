//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.KinectFusionExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;
    using ColorPickerControls;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Kinect.Toolkit.Fusion;
    using Wpf3DTools;
    using System.Windows.Shapes;
    using ColorMine.ColorSpaces;
    using ColorMine.ColorSpaces.Comparisons;
    using HelixToolkit.Wpf;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System.Windows.Controls.DataVisualization.Charting;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Quarta : Page
    {
        private MainViewModel mv;
        private MeshGeometry3D mesh1;
        private MeshGeometry3D mesh2;
        private MeshGeometry3D mesh3;
        private MeshGeometry3D mesh4;
        private MeshGeometry3D mesh5;

        private Material g1Material;
        private Material g2Material;
        private Material g3Material;
        private Material g4Material;
        private Material g5Material;

        private Material insideMaterial;

        private Model3DGroup modelGroup;
        private Model3DGroup modelGroup1;
        private Model3DGroup modelGroup2;
        private Model3DGroup modelGroup3;
        private Model3DGroup modelGroup4;
        private Model3DGroup modelGroup5;
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public Model3D Model { get; set; }
        public Model3D Model1 { get; set; }
        public Model3D Model2 { get; set; }
        public Model3D Model3 { get; set; }
        public Model3D Model4 { get; set; }
        public Model3D Model5 { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public Quarta()
        {
            this.InitializeComponent();
            ((PieSeries)this.grPorcentagem.Series[0]).ItemsSource = 
                (KeyValuePair<string, int>[])Application.Current.Properties["arrayPorcentagem"];
           
            ((LineSeries)this.grCentros.Series[0]).ItemsSource =
                (Dictionary<DateTime, double>)Application.Current.Properties["arrayCentros"];

            var converter = new System.Windows.Media.BrushConverter();
            rec1.Fill = (System.Windows.Media.Brush)converter.ConvertFrom(Application.Current.Properties["fill1"].ToString());
            rec2.Fill = (System.Windows.Media.Brush)converter.ConvertFrom(Application.Current.Properties["fill2"].ToString());
            rec3.Fill = (System.Windows.Media.Brush)converter.ConvertFrom(Application.Current.Properties["fill3"].ToString());
            rec4.Fill = (System.Windows.Media.Brush)converter.ConvertFrom(Application.Current.Properties["fill4"].ToString());
            rec5.Fill = (System.Windows.Media.Brush)converter.ConvertFrom(Application.Current.Properties["fill5"].ToString());

            // Create a model group
            modelGroup = new Model3DGroup();
            modelGroup1 = new Model3DGroup();
            modelGroup2= new Model3DGroup();
            modelGroup3 = new Model3DGroup();
            modelGroup4 = new Model3DGroup();
            modelGroup5 = new Model3DGroup();
            insideMaterial = MaterialHelper.CreateMaterial(Colors.Gray);
            g1Material = null;
            g2Material = null;
            g3Material = null;
            g4Material = null;
            g5Material = null;

            // Create a mesh builder and add a box to it
            var meshBuilder1 = new MeshBuilder(false, false);
            var meshBuilder2 = new MeshBuilder(false, false);
            var meshBuilder3 = new MeshBuilder(false, false);
            var meshBuilder4 = new MeshBuilder(false, false);
            var meshBuilder5 = new MeshBuilder(false, false);

            string line;
            int count = 0;
            System.Collections.Generic.List<Point3D> pontos = new System.Collections.Generic.List<Point3D>();
            
            string path = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Models", "Model2.ply");   
            // Lê o arquivo ply
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                // Ignoro o header
                if (count <= 13)
                {
                    count++;
                    continue;
                }

                // Separa pelo espaço
                string[] coord = line.Split(' ');
                // Se iniciar com 3 é face se não é ponto
                if (Double.Parse(coord[0]) != 3)
                {
                    pontos.Add(
                        new Point3D(
                            Double.Parse(coord[0]),
                            Double.Parse(coord[1]),
                            Double.Parse(coord[2]))
                        );
                }
                else
                {
                    if (Int32.Parse(coord[7]) == 1)
                    {
                        meshBuilder1.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g1Material)
                            g1Material = MaterialHelper.CreateMaterial(
                             System.Windows.Media.Color.FromRgb(0,0,255));

                    }

                    if (Int32.Parse(coord[7]) == 2)
                    {
                        meshBuilder2.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g2Material)
                            g2Material = MaterialHelper.CreateMaterial(
                                System.Windows.Media.Color.FromRgb(0, 255, 0));
                    }

                    if (Int32.Parse(coord[7]) == 3)
                    {
                        meshBuilder3.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g3Material)
                            g3Material = MaterialHelper.CreateMaterial(
                                System.Windows.Media.Color.FromRgb(255, 0, 0));
                    }

                    if (Int32.Parse(coord[7]) == 4)
                    {
                        meshBuilder4.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g4Material)
                            g4Material = MaterialHelper.CreateMaterial(
                                System.Windows.Media.Color.FromRgb(255, 255, 0));
                    }

                    if (Int32.Parse(coord[7]) == 5)
                    {
                        meshBuilder5.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g5Material)
                            g5Material = MaterialHelper.CreateMaterial(
                                System.Windows.Media.Color.FromRgb(255, 0, 255));
                    }
                }

                count++;
            }


            // Cria mesh para cada grupo
            mesh1 = meshBuilder1.ToMesh(true);
            mesh2 = meshBuilder2.ToMesh(true);
            mesh3 = meshBuilder3.ToMesh(true);
            mesh4 = meshBuilder4.ToMesh(true);
            mesh5 = meshBuilder5.ToMesh(true);

            // Adiciona cada mesh no grupo
            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh1,
                Material = g1Material,
                BackMaterial = insideMaterial
            });
            modelGroup1.Children.Add(new GeometryModel3D
            {
                Geometry = mesh1,
                Material = g1Material,
                BackMaterial = insideMaterial
            });


            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh2,
                Material = g2Material,
                BackMaterial = insideMaterial
            });
            modelGroup2.Children.Add(new GeometryModel3D
            {
                Geometry = mesh2,
                Material = g2Material,
                BackMaterial = insideMaterial
            });

            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh3,
                Material = g3Material,
                BackMaterial = insideMaterial
            });
            modelGroup3.Children.Add(new GeometryModel3D
            {
                Geometry = mesh3,
                Material = g3Material,
                BackMaterial = insideMaterial
            });

            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh4,
                Material = g4Material,
                BackMaterial = insideMaterial
            });
            modelGroup4.Children.Add(new GeometryModel3D
            {
                Geometry = mesh4,
                Material = g4Material,
                BackMaterial = insideMaterial
            });

            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh5,
                Material = g5Material,
                BackMaterial = insideMaterial
            });
            modelGroup5.Children.Add(new GeometryModel3D
            {
                Geometry = mesh5,
                Material = g5Material,
                BackMaterial = insideMaterial
            });


            this.Model = modelGroup;
            this.Model1 = modelGroup1;
            this.Model2 = modelGroup2;
            this.Model3 = modelGroup3;
            this.Model4 = modelGroup4;
            this.Model5 = modelGroup5;            
          
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {



        }

        public double Lowest(params double[] inputs)
        {
            return inputs.Min();
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        public void AtualizarModel(bool cb1, bool cb2, bool cb3, bool cb4, bool cb5)
        {
            // Limapa o grupo
            modelGroup = new Model3DGroup();

            if (cb1)
                modelGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = mesh1,
                    Material = g1Material,
                    BackMaterial = insideMaterial
                });

            if (cb2)
                modelGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = mesh2,
                    Material = g2Material,
                    BackMaterial = insideMaterial
                });

            if (cb3)
                modelGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = mesh3,
                    Material = g3Material,
                    BackMaterial = insideMaterial
                });

            if (cb4)
                modelGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = mesh4,
                    Material = g4Material,
                    BackMaterial = insideMaterial
                });

            if (cb5)
                modelGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = mesh5,
                    Material = g5Material,
                    BackMaterial = insideMaterial
                });

            this.Model = modelGroup;
        }
        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {
            vp1.Visibility = Visibility.Visible;
            vp2.Visibility = Visibility.Hidden;
            vp3.Visibility = Visibility.Hidden;
            vp4.Visibility = Visibility.Hidden;
            vp5.Visibility = Visibility.Hidden;
            vp6.Visibility = Visibility.Hidden;

            cb1.IsChecked = false;
            cb2.IsChecked = false;
            cb3.IsChecked = false;
            cb4.IsChecked = false;
            cb5.IsChecked = false;

        }

        private void cb_Checked(object sender, RoutedEventArgs e)
        {
            if (cb1.IsChecked == true)
            {
                vp1.Visibility = Visibility.Hidden;
                vp2.Visibility = Visibility.Visible;
                vp3.Visibility = Visibility.Hidden;
                vp4.Visibility = Visibility.Hidden;
                vp5.Visibility = Visibility.Hidden;
                vp6.Visibility = Visibility.Hidden;
            }
            if (cb2.IsChecked == true)
            {
                vp1.Visibility = Visibility.Hidden;
                vp2.Visibility = Visibility.Hidden;
                vp3.Visibility = Visibility.Visible;
                vp4.Visibility = Visibility.Hidden;
                vp5.Visibility = Visibility.Hidden;
                vp6.Visibility = Visibility.Hidden;
            }
            if (cb3.IsChecked == true)
            {
                vp1.Visibility = Visibility.Hidden;
                vp2.Visibility = Visibility.Hidden;
                vp3.Visibility = Visibility.Hidden;
                vp4.Visibility = Visibility.Visible;
                vp5.Visibility = Visibility.Hidden;
                vp6.Visibility = Visibility.Hidden;
            }
            if (cb4.IsChecked == true)
            {
                vp1.Visibility = Visibility.Hidden;
                vp2.Visibility = Visibility.Hidden;
                vp3.Visibility = Visibility.Hidden;
                vp4.Visibility = Visibility.Hidden;
                vp5.Visibility = Visibility.Visible;
                vp6.Visibility = Visibility.Hidden;
            }
            if (cb5.IsChecked == true)
            {
                vp1.Visibility = Visibility.Hidden;
                vp2.Visibility = Visibility.Hidden;
                vp3.Visibility = Visibility.Hidden;
                vp4.Visibility = Visibility.Hidden;
                vp5.Visibility = Visibility.Hidden;
                vp6.Visibility = Visibility.Visible;
            }
        }

        private void gerarGraficoCentros()
        {
            // gera os gráficos            
            Rect bounds = VisualTreeHelper.GetDescendantBounds(this.grCentros);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual isolatedVisual = new DrawingVisual();
            using (DrawingContext drawing = isolatedVisual.RenderOpen())
            {
                drawing.DrawRectangle(System.Windows.Media.Brushes.White, null, new Rect(new System.Windows.Point(), bounds.Size)); // Optional Background
                drawing.DrawRectangle(new VisualBrush(this.grCentros), null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            renderBitmap.Render(isolatedVisual);
            string path = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Graficos", "Centros.png");
            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }

        }


        private void salvarPrint3D()
        {
            // gera os gráficos            
            Rect bounds = VisualTreeHelper.GetDescendantBounds(this.vp3D);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual isolatedVisual = new DrawingVisual();
            using (DrawingContext drawing = isolatedVisual.RenderOpen())
            {
                drawing.DrawRectangle(System.Windows.Media.Brushes.White, null, new Rect(new System.Windows.Point(), bounds.Size)); // Optional Background
                drawing.DrawRectangle(new VisualBrush(this.vp3D), null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            renderBitmap.Render(isolatedVisual);
            string path = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Images\\Exported\\Print3D", "print3D" + DateTime.Now.ToBinary() + ".png");
            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }
        }

        private void gerarGraficoPorcentagem() 
        {
            // gera os gráficos            
            Rect bounds = VisualTreeHelper.GetDescendantBounds(this.grPorcentagem);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual isolatedVisual = new DrawingVisual();
            using (DrawingContext drawing = isolatedVisual.RenderOpen())
            {
                drawing.DrawRectangle(System.Windows.Media.Brushes.White, null, new Rect(new System.Windows.Point(), bounds.Size)); // Optional Background
                drawing.DrawRectangle(new VisualBrush(this.grPorcentagem), null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            renderBitmap.Render(isolatedVisual);
            string pathPorcentagem = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Graficos", "Porcentagem.png");
            using (FileStream outStream = new FileStream(pathPorcentagem, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }

        }

        private void btnRelatorio_Click(object sender, RoutedEventArgs e)
        {
            gerarGraficoPorcentagem();
            gerarGraficoCentros();
            //string path = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Pdf", "BodyScanner.pdf");   
            //Document pdfDoc = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(path, FileMode.OpenOrCreate));

            //pdfDoc.Open();
            //pdfDoc.Add(new Paragraph("Some data"));
            //PdfContentByte cb = writer.DirectContent;
            //cb.MoveTo(pdfDoc.PageSize.Width / 2, pdfDoc.PageSize.Height / 2);
            //cb.LineTo(pdfDoc.PageSize.Width / 2, pdfDoc.PageSize.Height);
            //cb.Stroke();

            //pdfDoc.Close();
            this.NavigationService.Navigate(new Relatorio());            
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            this.salvarPrint3D();
        }

    }

}
