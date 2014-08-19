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
    using iTextSharp.text;
    using iTextSharp.text.pdf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Relatorio : Page
    {
        string[] pngFiles;
        string[] pngFiles3d;
        string[] pngFilesReal;
        string[] graphFiles;
        string pathExported;
        string pathExportedReal;
        string pathExported3d;
        string pathExportedGraph;

        private string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);
            for (int i = 0; i < files.Length; i++)
                files[i] = System.IO.Path.GetFileName(files[i]);
            return files;
        }

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public Relatorio()
        {
            this.InitializeComponent();
            string path = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Pdf", "BodyScanner.pdf");
            string pathLog = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Logs", "BodyScannerLog.txt");
            String log = "";
            using (StreamReader sr = new StreamReader(pathLog))
            {
                log = sr.ReadToEnd();
            }

            // Declaro as fontes para título
            BaseFont courier = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
            iTextSharp.text.Font tituloFonte = new iTextSharp.text.Font(courier, 14, iTextSharp.text.Font.BOLD, new BaseColor(255, 255, 255));
            iTextSharp.text.Font titulo = new iTextSharp.text.Font(courier, 16, iTextSharp.text.Font.BOLD, new BaseColor(0, 0, 0));

            Document pdfDoc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(path, FileMode.OpenOrCreate));
            pdfDoc.SetMargins(0f, 0f, 0f, 0f);

            pdfDoc.Open();
            //pdfDoc.Add(new Paragraph("Body Scanner Report " + DateTime.Now.ToString()));
            pathExportedReal = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Images\\Exported\\Color");
            pathExported3d = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Images\\Exported\\3D");
            pathExported = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Images\\Exported\\Print3D");
            pngFiles = GetFileNames(pathExported, "*.png");
            pngFiles3d = GetFileNames(pathExported3d, "*.png");
            pngFilesReal = GetFileNames(pathExported3d, "*.png");
            pathExportedGraph = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Graficos");
            graphFiles = GetFileNames(pathExportedGraph, "*.png");
            // var converter = new System.Windows.Media.BrushConverter();
            //System.Drawing.Pen blackPen = new System.Drawing.Pen((System.Drawing.Brush)converter.ConvertFrom(Application.Current.Properties["fill1"].ToString()), 5);
            //e.Graphics.DrawRectangle(blackPen, 10, 10, 100, 50);

            // Tabela pai
            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            //rec1.Fill = (System.Windows.Media.Brush)converter.ConvertFrom(Application.Current.Properties["fill1"].ToString());
            //System.Drawing.Color c1 = (System.Drawing.Color)Application.Current.Properties["fill1"];


            // Header
            PdfPCell cell = new PdfPCell(new Phrase("Body Scanner Report", titulo));
            cell.Colspan = 4;
            cell.PaddingBottom = 20;
            cell.PaddingTop = 20;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell);

            // Subtítulo
            PdfPCell cellSubtitulo = new PdfPCell(new Phrase("Dados das cores", tituloFonte));
            cellSubtitulo.PaddingBottom = 5;
            cellSubtitulo.PaddingTop = 5;
            cellSubtitulo.BackgroundColor = new BaseColor(0, 0, 0);
            cellSubtitulo.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellSubtitulo.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cellSubtitulo);


            // Tabela de cores
            PdfPTable tableImg = new PdfPTable(3);
            tableImg.WidthPercentage = 95;

            System.Windows.Media.Color c1 = (System.Windows.Media.Color)Application.Current.Properties["fill1"];
            // Primeira celula
            PdfPCell cell11 = new PdfPCell(new Phrase("Grupo 1:"));
            cell11.BackgroundColor = new BaseColor(c1.R, c1.G, c1.B);
            cell11.HorizontalAlignment = 1;
            tableImg.AddCell(cell11);
            
            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));            
            PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString() + "%"));
            cell12.HorizontalAlignment = 1;
            tableImg.AddCell(cell12);

            // Constante de área por 10cm2
            int a = 4900;

            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            float area1 = (100 * (int)Application.Current.Properties["px1"]) / a;
            PdfPCell cell13 = new PdfPCell(new Phrase(area1.ToString() + " cm²"));
            cell13.HorizontalAlignment = 1;
            tableImg.AddCell(cell13);

            System.Windows.Media.Color c2 = (System.Windows.Media.Color)Application.Current.Properties["fill2"];
            // Primeira celula
            PdfPCell cell21 = new PdfPCell(new Phrase("Grupo 2:"));
            cell21.BackgroundColor = new BaseColor(c2.R, c2.G, c2.B);
            cell21.HorizontalAlignment = 1;
            tableImg.AddCell(cell21);

            // Primeira celula
            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            PdfPCell cell22 = new PdfPCell(new Phrase(Application.Current.Properties["p2"].ToString() + "%"));
            cell22.HorizontalAlignment = 1;
            tableImg.AddCell(cell22);

            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            float area2 = (100 * (int)Application.Current.Properties["px2"]) / a;
            PdfPCell cell23 = new PdfPCell(new Phrase(area2.ToString() + " cm²"));
            cell23.HorizontalAlignment = 1;
            tableImg.AddCell(cell23);

            System.Windows.Media.Color c3 = (System.Windows.Media.Color)Application.Current.Properties["fill3"];
            // Primeira celula
            PdfPCell cell31 = new PdfPCell(new Phrase("Grupo 3:"));
            cell31.BackgroundColor = new BaseColor(c3.R, c3.G, c3.B);
            cell31.HorizontalAlignment = 1;
            tableImg.AddCell(cell31);

            // Primeira celula
            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            PdfPCell cell32 = new PdfPCell(new Phrase(Application.Current.Properties["p3"].ToString() + "%"));
            cell32.HorizontalAlignment = 1;
            tableImg.AddCell(cell32);

            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            float area3 = (100 * (int)Application.Current.Properties["px3"]) / a;
            PdfPCell cell33 = new PdfPCell(new Phrase(area3.ToString() + " cm²"));
            cell33.HorizontalAlignment = 1;
            tableImg.AddCell(cell33);

            //System.Drawing.Color c2 = (System.Drawing.Color)Application.Current.Properties["fill2"];
            //System.Drawing.Color c4 = System.Drawing.Color.FromArgb(255, 0, 255, 255);
            System.Windows.Media.Color c4 = (System.Windows.Media.Color)Application.Current.Properties["fill4"];
            // Primeira celula
            PdfPCell cell41 = new PdfPCell(new Phrase("Grupo 4:"));
            cell41.BackgroundColor = new BaseColor(c4.R, c4.G, c4.B);
            cell41.HorizontalAlignment = 1;
            tableImg.AddCell(cell41);

            // Primeira celula
            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            PdfPCell cell42 = new PdfPCell(new Phrase(Application.Current.Properties["p4"].ToString() + "%"));
            cell42.HorizontalAlignment = 1;
            tableImg.AddCell(cell42);

            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            float area4 = (100 * (int)Application.Current.Properties["px4"]) / a;
            PdfPCell cell43 = new PdfPCell(new Phrase(area4.ToString() + " cm²"));
            cell43.HorizontalAlignment = 1;
            tableImg.AddCell(cell43);

            //System.Drawing.Color c5 = System.Drawing.Color.FromArgb(100, 120, 44, 255);
            System.Windows.Media.Color c5 = (System.Windows.Media.Color)Application.Current.Properties["fill5"];
            // Primeira celula
            PdfPCell cell51 = new PdfPCell(new Phrase("Grupo 5:"));
            cell51.BackgroundColor = new BaseColor(c5.R, c5.G, c5.B);
            cell51.HorizontalAlignment = 1;
            tableImg.AddCell(cell51);

            // Primeira celula
            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            PdfPCell cell52 = new PdfPCell(new Phrase(Application.Current.Properties["p5"].ToString() + "%"));
            cell52.HorizontalAlignment = 1;
            tableImg.AddCell(cell52);

            //PdfPCell cell12 = new PdfPCell(new Phrase(Application.Current.Properties["p1"].ToString()));
            float area5 = (100 * (int)Application.Current.Properties["px5"]) / a;
            PdfPCell cell53 = new PdfPCell(new Phrase(area5.ToString() + " cm²"));
            cell53.HorizontalAlignment = 1;
            tableImg.AddCell(cell53);

            PdfPCell celImg = new PdfPCell(tableImg);
            celImg.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            celImg.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(celImg);

            // Titulo das fotos
            PdfPCell cellTituloLogs = new PdfPCell(new Phrase("Logs do Processamento", tituloFonte));
            cellTituloLogs.PaddingBottom = 5;
            cellTituloLogs.BackgroundColor = new BaseColor(0, 0, 0);
            cellTituloLogs.PaddingTop = 5;
            cellTituloLogs.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellTituloLogs.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cellTituloLogs);

            PdfPCell cellLogs = new PdfPCell(new Phrase(log));
            cellLogs.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellLogs.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cellLogs.PaddingBottom = 10;
            cellLogs.PaddingTop = 10;
            table.AddCell(cellLogs);

            PdfPCell cellGraficos = new PdfPCell(new Phrase("Gráficos", tituloFonte));
            cellGraficos.PaddingBottom = 5;
            cellGraficos.BackgroundColor = new BaseColor(0, 0, 0);
            cellGraficos.PaddingTop = 5;
            cellGraficos.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellGraficos.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cellGraficos);
            
            // Tabela de fotos
            PdfPTable tableGraphs = new PdfPTable(1);
            tableGraphs.WidthPercentage = 95;



            foreach (string image in graphFiles)
            {
                System.Drawing.Image img =
                    System.Drawing.Image.FromFile(pathExportedGraph + "\\" + image);

                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);

                pic.ScalePercent(70);

                PdfPCell cellx = new PdfPCell(pic);
                cellx.Border = 1;
                cellx.BorderColor = new BaseColor(0, 0, 0); ;
                cellx.HorizontalAlignment = 1;
                tableGraphs.AddCell(cellx);

                //pdfDoc.Add(pic);
                //pdfDoc.NewPage();
            }


            PdfPCell celGraphs = new PdfPCell(tableGraphs);
            celGraphs.PaddingBottom = 5;
            celGraphs.PaddingTop = 5;
            celGraphs.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            celGraphs.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(celGraphs);



            // Titulo imagens capturadas
            PdfPCell cellTituloFotosReal = new PdfPCell(new Phrase("Imagens capturadas (Reais)", tituloFonte));
            cellTituloFotosReal.PaddingBottom = 5;
            cellTituloFotosReal.BackgroundColor = new BaseColor(0, 0, 0);
            cellTituloFotosReal.PaddingTop = 5;
            cellTituloFotosReal.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellTituloFotosReal.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cellTituloFotosReal);

            // Tabela de fotos
            PdfPTable tableFotosReal = new PdfPTable(2);
            tableFotosReal.WidthPercentage = 95;
            int i = 0;
            foreach (string image in pngFilesReal)
            {
                if (i == 2)
                    continue;

                System.Drawing.Image img =
                    System.Drawing.Image.FromFile(pathExportedReal + "\\" + image);

                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);

                pic.ScalePercent(45);

                PdfPCell cellx = new PdfPCell(pic);
                cellx.Border = 1;
                cellx.BorderColor = new BaseColor(0, 0, 0); ;
                cellx.HorizontalAlignment = 0;
                tableFotosReal.AddCell(cellx);
                i++;

                //pdfDoc.Add(pic);
                //pdfDoc.NewPage();
            }


            PdfPCell celFotosReal = new PdfPCell(tableFotosReal);
            celFotosReal.PaddingBottom = 5;
            celFotosReal.PaddingTop = 5;
            celFotosReal.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            celFotosReal.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(celFotosReal);



            // Titulo imagens capturadas
            PdfPCell cellTituloFotos3d = new PdfPCell(new Phrase("Imagens capturadas (3D)", tituloFonte));
            cellTituloFotos3d.PaddingBottom = 5;
            cellTituloFotos3d.BackgroundColor = new BaseColor(0, 0, 0);
            cellTituloFotos3d.PaddingTop = 5;
            cellTituloFotos3d.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellTituloFotos3d.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cellTituloFotos3d);

            // Tabela de fotos
            PdfPTable tableFotos3d = new PdfPTable(2);
            tableFotos3d.WidthPercentage = 95;
            int count1 = 0;
            foreach (string image in pngFiles3d)
            {
                if (count1 == 2)
                    continue;

                System.Drawing.Image img =
                    System.Drawing.Image.FromFile(pathExported3d + "\\" + image);

                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);

                pic.ScalePercent(45);

                PdfPCell cellx = new PdfPCell(pic);                
                cellx.Border = 1;
                cellx.BorderColor = new BaseColor(0, 0, 0); ;
                cellx.HorizontalAlignment = 0;
                tableFotos3d.AddCell(cellx);
                count1++;

                //pdfDoc.Add(pic);
                //pdfDoc.NewPage();
            }


            PdfPCell celFotos3d = new PdfPCell(tableFotos3d);
            celFotos3d.PaddingBottom = 5;
            celFotos3d.PaddingTop = 5;
            celFotos3d.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            celFotos3d.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(celFotos3d);

            // Titulo imagens capturadas
            PdfPCell cellTituloFotos = new PdfPCell(new Phrase("Imagens capturadas (Segmentação)", tituloFonte));
            cellTituloFotos.PaddingBottom = 5;
            cellTituloFotos.BackgroundColor = new BaseColor(0, 0, 0);
            cellTituloFotos.PaddingTop = 5;
            cellTituloFotos.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            cellTituloFotos.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cellTituloFotos);

            // Tabela de fotos
            PdfPTable tableFotos = new PdfPTable(2);
            tableFotos.WidthPercentage = 95;
            int count = 0;
            foreach (string image in pngFiles)
            {
                if (count == 2)
                    continue;

                System.Drawing.Image img =
                    System.Drawing.Image.FromFile(pathExported + "\\" + image);

                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);

                pic.ScalePercent(50);

                PdfPCell cellx = new PdfPCell(pic);
                cellx.Border = 1;
                cellx.BorderColor = new BaseColor(0, 0, 0); ;
                cellx.HorizontalAlignment = 0;
                tableFotos.AddCell(cellx);
                count++;

                //pdfDoc.Add(pic);
                //pdfDoc.NewPage();
            }


            PdfPCell celFotos = new PdfPCell(tableFotos);
            celFotos.PaddingBottom = 5;
            celFotos.PaddingTop = 5;
            celFotos.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            celFotos.VerticalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(celFotos);

            pdfDoc.Add(table);


            pdfDoc.Close();
            string path2 = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Pdf", "BodyScanner.pdf");
            this.pdfViewer.Navigate(path2);

        }


        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {



        }

        public void DeleteAllExportedImages(string pasta)
        {

            string path = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + "\\Images\\Exported\\" + pasta;
            (new System.Threading.Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(path);

                        foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                        {
                            dir.Delete(true);
                        }
                    }
                    catch { }
                }
            })).Start();
        }


        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DeleteAllExportedImages("3D");
            this.DeleteAllExportedImages("Color");
            this.DeleteAllExportedImages("Print3D");
        }

    }

}
