using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Unicamp.Kinect.BodyScanner
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
            LoadColumnChartData();

            
        }

        private void LoadColumnChartData()
        {
            ((ColumnSeries)mcChart.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("G1", 12),
                new KeyValuePair<string,int>("G2", 25),
                new KeyValuePair<string,int>("G3", 5),
                new KeyValuePair<string,int>("G4", 6),
                new KeyValuePair<string,int>("G5", 10),
                new KeyValuePair<string,int>("G6", 4) };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (mcChart.Series[0] == null)
            {
                MessageBox.Show("there is nothing to export");
            }
            else
            {
                Rect bounds = VisualTreeHelper.GetDescendantBounds(mcChart);

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);

                DrawingVisual isolatedVisual = new DrawingVisual();
                using (DrawingContext drawing = isolatedVisual.RenderOpen())
                {
                    drawing.DrawRectangle(Brushes.White, null, new Rect(new Point(), bounds.Size)); // Optional Background
                    drawing.DrawRectangle(new VisualBrush(mcChart), null, new Rect(new Point(), bounds.Size));
                }

                renderBitmap.Render(isolatedVisual);

                using (FileStream outStream = new FileStream(@"C:/Temp/graph.png", FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(outStream);
                }
                
            }
        }
    }
}
