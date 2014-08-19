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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Segunda : Page
    {

        int imgCount;

        // Lista de imagens que foi tirado o print
        string[] pngFiles;
        string pathExported;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public Segunda()
        {
            this.InitializeComponent();

            // Busca as imagens onde foi tirado os print e exibe no controle o primeito
            pathExported = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Images\\Exported\\Color");
            pngFiles = GetFileNames(pathExported, "*.png");

            imgCount = 0;
            if (pngFiles.Length > 0)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad; // this is key to destory the image with a new image coming in.
                Uri imageSource = new Uri(pathExported + @"\" + pngFiles[0], UriKind.Absolute);
                img.UriSource = imageSource;                
                img.EndInit();

                image.Source = img;
            }           
        }
        private string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);
            for (int i = 0; i < files.Length; i++)
                files[i] = System.IO.Path.GetFileName(files[i]);
            return files;
        }

        private void btnProximaImagem_Click(object sender, RoutedEventArgs e)
        {
            
            imgCount++;
            if (pngFiles.Length > 0 && pngFiles.Length != imgCount)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad; // this is key to destory the image with a new image coming in.
                Uri imageSource = new Uri(pathExported + @"\" + pngFiles[imgCount]);
                img.UriSource = imageSource;
                img.EndInit();

                image.Source = img;
            }
            else
                imgCount = 0;

        }


        public void iniciarKMeans() 
        {


            // Iniciando os centroides com as cores selecionadas
            // Inicia c1
            System.Windows.Media.Color c = (rec1.Fill as SolidColorBrush).Color;
            Application.Current.Properties["c1"] = c;
            Application.Current.Properties["fill1"] = c;    

            // Inicia c2
            c = (rec2.Fill as SolidColorBrush).Color;
            Application.Current.Properties["c2"] = c;
            Application.Current.Properties["fill2"] = c;    

            // Inicia c3
            c = (rec3.Fill as SolidColorBrush).Color;
            Application.Current.Properties["c3"] = c;
            Application.Current.Properties["fill3"] = c;    

            // Inicia c4
            c = (rec4.Fill as SolidColorBrush).Color;
            Application.Current.Properties["c4"] = c;
            Application.Current.Properties["fill4"] = c;    

            // Inicia c5
            c = (rec5.Fill as SolidColorBrush).Color;
            Application.Current.Properties["c5"] = c;
            Application.Current.Properties["fill5"] = c;    

            image.Source = null;

            // Navega para a terceira tela
            this.NavigationService.Navigate(new Terceira());

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

        private void btnSelecionar_Click(object sender, RoutedEventArgs e)
        {
            if (rec1.Fill == null)
            {
                rec1.Fill = rec6.Fill;
            }
            else if (rec2.Fill == null)
            {
                rec2.Fill = rec6.Fill;
            }
            else if (rec3.Fill == null)
            {
                rec3.Fill = rec6.Fill;
            }
            else if (rec4.Fill == null)
            {
                rec4.Fill = rec6.Fill;
            }
            else if (rec5.Fill == null)
            {
                rec5.Fill = rec6.Fill;
            }
                
        }

        private void btnIniciarKmeans_Click(object sender, RoutedEventArgs e)
        {
            iniciarKMeans();
        }

    }

}
