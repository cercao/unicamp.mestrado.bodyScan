//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.KinectFusionExplorer
{
    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Fusion;
using Wpf3DTools;
    using Unicamp.Kinect.BodyScanner;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Frame : Page
    {

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public Frame()
        {
            this.InitializeComponent();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var navWindow = Window.GetWindow(this) as NavigationWindow;
                if (navWindow != null) navWindow.ShowsNavigationUI = false;
            }));

            Thread.Sleep(1000);
            this.InicialFrame.Navigate(new Primeira());
            
        }

        /// <summary>
        /// Finalizes an instance of the MainWindow class.
        /// This destructor will run only if the Dispose method does not get called.
        /// </summary>
        ~Frame()
        {
            
        }         
    }      

}
