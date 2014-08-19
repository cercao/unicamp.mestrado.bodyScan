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
    public partial class Terceira : Page
    {
        // Centróides
        private Lab c1;
        private Lab c2;
        private Lab c3;
        private Lab c4;
        private Lab c5;

        // Centróides anteriores
        private Lab ac1;
        private Lab ac2;
        private Lab ac3;
        private Lab ac4;
        private Lab ac5;

        // Diferencas
        double dif1;
        double dif2;
        double dif3;
        double dif4;
        double dif5;

        // Grupos dos centróides
        Dictionary<int, Lab> grupoC1;
        Dictionary<int, Lab> grupoC2;
        Dictionary<int, Lab> grupoC3;
        Dictionary<int, Lab> grupoC4;
        Dictionary<int, Lab> grupoC5;

        private Dictionary<DateTime, double> arrayCentros = new Dictionary<DateTime, double>();

        // Mesh inicial
        ColorMesh mesh;
        int imgCount;

        // Lista de imagens que foi tirado o print
        string[] pngFiles;
        string pathExported;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public Terceira()
        {
            this.InitializeComponent();

            mesh = (ColorMesh)Application.Current.Properties["mesh"];

            Thread kMeans = new Thread(iniciarKMeans);
            kMeans.Start();

        }

        public void iniciarKMeans() 
        {
            //Bitmap image2 = new Bitmap(@"C:\Users\Lucas\Pictures\KinectSnapshot.png");
            List<Lab> pixels = new List<Lab>();

            if (null != mesh)
            {
                Dispatcher.Invoke((Action)(() =>
                    this.txtLog.AppendText("Convertendo informação de cores para LAB... \n")
                ));

                var vertices = mesh.GetVertices();
                var indices = mesh.GetTriangleIndexes();
                var colors = mesh.GetColors();

                int faces = indices.Count / 3;

                // Sequentially write the 3 vertices of the triangle, for each triangle
                for (int i = 0; i < vertices.Count; i++)
                {
                    var vertex = vertices[i];

                    string vertexString = vertex.X.ToString(CultureInfo.InvariantCulture) + " ";

                   // if (flipAxes)
                    //{
                        vertexString += (-vertex.Y).ToString(CultureInfo.InvariantCulture) + " " + (-vertex.Z).ToString(CultureInfo.InvariantCulture);
                   // }
                   // else
                   // {
                        //vertexString += vertex.Y.ToString(CultureInfo.InvariantCulture) + " " + vertex.Z.ToString(CultureInfo.InvariantCulture);
                   // }

                    int red = (colors[i] >> 16) & 255;
                    int green = (colors[i] >> 8) & 255;
                    int blue = colors[i] & 255;

                    // Adiciona a cor no array
                    Rgb colorRgb = new Rgb { R = red, G = green, B = blue };

                    pixels.Add(colorRgb.To<Lab>());

                }

                // Sequentially write the 3 vertex indices of the triangle face, for each triangle, 0-referenced in PLY files
                //for (int i = 0; i < faces; i++)
                //{
                //    // Lucas - Enconta o indice de cada vertice para uni-los e gerar a face
                //    int baseIndex0 = (i * 3);
                //    int baseIndex1 = ((i * 3) + 1);
                //    int baseIndex2 = ((i * 3) + 2);

                //    int red0 = (colors[baseIndex0] >> 16) & 255;
                //    int green0 = (colors[baseIndex0] >> 8) & 255;
                //    int blue0 = colors[baseIndex0] & 255;

                //    int red1 = (colors[baseIndex1] >> 16) & 255;
                //    int green1 = (colors[baseIndex1] >> 8) & 255;
                //    int blue1 = colors[baseIndex1] & 255;

                //    int red2 = (colors[baseIndex2] >> 16) & 255;
                //    int green2 = (colors[baseIndex2] >> 8) & 255;
                //    int blue2 = colors[baseIndex2] & 255;

                //    // Lucas - Alterando a geração do ply para salvar a media da cor dos 3 vertices na face
                //    string faceString = "3 " +
                //        baseIndex0.ToString() + " " + baseIndex1.ToString() + " " + baseIndex2.ToString() + " "
                //        + ((red0 + red1 + red2) / 3) + " " + // Red
                //        ((green0 + green1 + green2) / 3) + " " + // Green
                //        ((blue0 + blue1 + blue2) / 3); // Blue
                //}

            }
            else 
            {
                // Converte tudo para LAB
                //for (int y = 0; y < image2.Height; y++)
                //{
                //    for (int x = 0; x < image2.Width; ++x)
                //    {
                //        System.Drawing.Color cor = image2.GetPixel(x, y);
                //        Rgb colorRgb = new Rgb { R = cor.R, G = cor.G, B = cor.B };

                //        pixels.Add(colorRgb.To<Lab>());
                //    }
                //}
            }

            /*************************************
             * Iniciando K-Means                 * 
             * ***********************************/
            Dispatcher.Invoke((Action)(() =>
                this.txtLog.AppendText("Iniciando K-Means... \n")
            ));
            // Iniciando os centroides com as cores selecionadas
            // Inicia c1
            System.Windows.Media.Color c = (System.Windows.Media.Color)Application.Current.Properties["c1"];
            Rgb c1rgb = new Rgb { R = c.R, G = c.G, B = c.B };
            c1 = c1rgb.To<Lab>();
            ac1 = c1;
            grupoC1 = new Dictionary<int, Lab>();

            // Inicia c2
            c = (System.Windows.Media.Color)Application.Current.Properties["c2"];
            Rgb c2rgb = new Rgb { R = c.R, G = c.G, B = c.B };
            c2 = c2rgb.To<Lab>();
            ac2 = c2;
            grupoC2 = new Dictionary<int, Lab>();

            // Inicia c3
            c = (System.Windows.Media.Color)Application.Current.Properties["c3"];
            Rgb c3rgb = new Rgb { R = c.R, G = c.G, B = c.B };
            c3 = c3rgb.To<Lab>();
            ac3 = c3;
            grupoC3 = new Dictionary<int, Lab>();

            // Inicia c4
            c = (System.Windows.Media.Color)Application.Current.Properties["c4"];
            Rgb c4rgb = new Rgb { R = c.R, G = c.G, B = c.B };
            c4 = c4rgb.To<Lab>();
            ac4 = c4;
            grupoC4 = new Dictionary<int, Lab>();

            // Inicia c5
            c = (System.Windows.Media.Color)Application.Current.Properties["c5"];
            Rgb c5rgb = new Rgb { R = c.R, G = c.G, B = c.B };
            c5 = c5rgb.To<Lab>();
            ac5 = c5;
            grupoC5 = new Dictionary<int, Lab>();

            // Distribui as cores LUV pelos grupos de acordo com a proximidade de coloração dos centróides
            separarPorGrupos(pixels);

            // Calcular médias de cada centróide e atribuir ao novo centroide
            if (grupoC1.Count > 0)
                c1 = calcularLabMedio(grupoC1);

            if (grupoC2.Count > 0)
                c2 = calcularLabMedio(grupoC2);

            if (grupoC3.Count > 0)
                c3 = calcularLabMedio(grupoC3);

            if (grupoC4.Count > 0)
                c4 = calcularLabMedio(grupoC4);

            if (grupoC5.Count > 0)
                c5 = calcularLabMedio(grupoC5);

            // Calcula as diferenças
            dif1 = (ac1.L - c1.L) + (ac1.A - c1.A) + (ac1.B - c1.B);
            dif2 = (ac2.L - c2.L) + (ac2.A - c2.A) + (ac2.B - c2.B);
            dif3 = (ac3.L - c3.L) + (ac3.A - c3.A) + (ac3.B - c3.B);
            dif4 = (ac4.L - c4.L) + (ac4.A - c4.A) + (ac4.B - c4.B);
            dif5 = (ac5.L - c5.L) + (ac5.A - c5.A) + (ac5.B - c5.B);

            // Se for diferente dos centróides anteriores, atribui o novo valor para osanteriores e calcula novamente
            int cont = 1;
            while (dif1 > 0 || dif2 > 0 || dif3 > 0 || dif4 > 0 || dif5 > 0 )
            {
                // Calcula as diferenças
                dif1 = (ac1.L - c1.L) + (ac1.A - c1.A) + (ac1.B - c1.B);
                dif2 = (ac2.L - c2.L) + (ac2.A - c2.A) + (ac2.B - c2.B);
                dif3 = (ac3.L - c3.L) + (ac3.A - c3.A) + (ac3.B - c3.B);
                dif4 = (ac4.L - c4.L) + (ac4.A - c4.A) + (ac4.B - c4.B);
                dif5 = (ac5.L - c5.L) + (ac5.A - c5.A) + (ac5.B - c5.B);

                // Iguala aos anteriores
                ac1 = c1;
                ac2 = c2;
                ac3 = c3;
                ac4 = c4;
                ac5 = c5;

                //using (StreamWriter w = File.AppendText(@"C:\temp\logKinect.txt"))
                //{
                //    // Loga os dados anteriores e atuais
                //    //double s1 = (double)(ac1.L / c1.L + ac1.A / c1.A + ac1.B / c1.B);
                //    //double s2 = (double)(ac2.L / c2.L + ac2.A / c2.A + ac2.B / c2.B);
                //    //double s3 = (double)(ac3.L / c3.L + ac3.A / c3.A + ac3.B / c3.B);
                //    //double s4 = (double)(ac4.L / c4.L + ac4.A / c4.A + ac4.B / c4.B);
                //    //double s5 = (double)(ac5.L / c5.L + ac5.A / c5.A + ac5.B / c5.B);

                //    //w.WriteLine(
                //    //   ac1.L / c1.L + " + " + ac1.A / c1.A + " + " + ac1.B / c1.B + " = " +
                //    //   s1);
                //    //w.WriteLine(
                //    //   ac2.L / c2.L + " + " + ac2.A / c2.A + " + " + ac2.B / c2.B + " = " +
                //    //   s2);
                //    //w.WriteLine(
                //    //   ac3.L / c3.L + " + " + ac3.A / c3.A + " + " + ac3.B / c3.B + " = " +
                //    //   s3);
                //    //w.WriteLine(
                //    //   ac4.L / c4.L + " + " + ac4.A / c4.A + " + " + ac4.B / c4.B + " = " +
                //    //   s4);
                //    //w.WriteLine(
                //    //   ac5.L / c5.L + " + " + ac5.A / c5.A + " + " + ac5.B / c5.B + " = " +
                //    //   s5);

                //    // Soma
                //    //double st = s1 + s2 + s3 + s4 + s5;
                //    //Diferenca
                //    //double df = dif1 + dif2 + dif3 + dif4 + dif5;
                //    //this.txtLog.AppendText("Diferença: " + df + " Hora:" + DateTime.Now.Minute + ":" + DateTime.Now.Second.ToString());
                //    //w.WriteLine("Diferença: " + df + " Hora:" + DateTime.Now.Minute + ":" + DateTime.Now.Second.ToString());

                //}

                // Loga na tela
                double df = Math.Abs(dif1 + dif2 + dif3 + dif4 + dif5);
                Dispatcher.Invoke((Action)(() => 
                    this.txtLog.AppendText("Diferença: " + df + " Hora:" + DateTime.Now.ToString() + "\n")
                ));

                Dispatcher.Invoke((Action)(() =>
                    this.txtLog.ScrollToEnd()
                ));

                arrayCentros.Add(DateTime.Now, df);

                // Inicializa os  grupos novamente
                grupoC1 = new Dictionary<int, Lab>();
                grupoC2 = new Dictionary<int, Lab>();
                grupoC3 = new Dictionary<int, Lab>();
                grupoC4 = new Dictionary<int, Lab>();
                grupoC5 = new Dictionary<int, Lab>();

                // Distribui as cores LUV pelos grupos de acordo com a proximidade de coloração dos centróides
                separarPorGrupos(pixels);

                // Calcular médias de cada centróide e atribuir ao novo centroide
                if(grupoC1.Count > 0)
                    c1 = calcularLabMedio(grupoC1);

                if (grupoC2.Count > 0)
                    c2 = calcularLabMedio(grupoC2);

                if (grupoC3.Count > 0)
                    c3 = calcularLabMedio(grupoC3);

                if (grupoC4.Count > 0)
                    c4 = calcularLabMedio(grupoC4);

                if (grupoC5.Count > 0)
                    c5 = calcularLabMedio(grupoC5);

                cont++;
            }

            Dispatcher.Invoke((Action)(() =>
                this.txtLog.AppendText("Salvando um novo modelo com as novas informações de cores... \n")
            ));

            Dispatcher.Invoke((Action)(() =>
                this.txtLog.ScrollToEnd()
            ));

            // Dps que terminar, é necessário atualizar o mesh com as novas colorações
            // a atualização deverá ser feita por grupo (dicionario) 
            string path = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Models", "Model2.ply");            
            using (StreamWriter writer = new StreamWriter(path))
            {
                // Default to flip Y,Z coordinates on save
                this.SalvarPlyMeshAtualiado(writer);

            }

            Dispatcher.Invoke((Action)(() => finaliza(pixels) ));
        }


        private void finaliza(List<Lab> pixels)
        {
            double p1 = (grupoC1.Count * 100) / pixels.Count;
            double p2 = (grupoC2.Count * 100) / pixels.Count;
            double p3 = (grupoC3.Count * 100) / pixels.Count;
            double p4 = (grupoC4.Count * 100) / pixels.Count;
            double p5 = (grupoC5.Count * 100) / pixels.Count;

            this.txtLog.AppendText("Processamento finalizado com sucesso (" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "): \n");
            this.txtLog.AppendText("Grupo 1:" + p1 + "% \n");
            this.txtLog.AppendText("Grupo 2:" + p2 + "% \n");
            this.txtLog.AppendText("Grupo 3:" + p3 + "% \n");
            this.txtLog.AppendText("Grupo 4:" + p4 + "% \n");
            this.txtLog.AppendText("Grupo 5:" + p5 + "% \n");

            Application.Current.Properties["p1"] = p1;
            Application.Current.Properties["p2"] = p2;
            Application.Current.Properties["p3"] = p3;
            Application.Current.Properties["p4"] = p4;
            Application.Current.Properties["p5"] = p5;

            Application.Current.Properties["px1"] = grupoC1.Count;
            Application.Current.Properties["px2"] = grupoC2.Count;
            Application.Current.Properties["px3"] = grupoC3.Count;
            Application.Current.Properties["px4"] = grupoC4.Count;
            Application.Current.Properties["px5"] = grupoC5.Count;

            Application.Current.Properties["arrayPorcentagem"] = new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Grupo 1", (int)p1),
                new KeyValuePair<string,int>("Grupo 2", (int)p2),
                new KeyValuePair<string,int>("Grupo 3", (int)p3),
                new KeyValuePair<string,int>("Grupo 4", (int)p4),
                new KeyValuePair<string,int>("Grupo 5", (int)p5) };

            Application.Current.Properties["arrayCentros"] = arrayCentros;

            Dispatcher.Invoke((Action)(() =>
                this.txtLog.ScrollToEnd()
            ));

            Application.Current.Properties["log"] = this.txtLog.Text;

            string path = System.IO.Path.Combine(System.IO.Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName) + "\\Logs", "BodyScannerLog.txt");
            // Gravo a LOg em arquivo
            using (StreamWriter w = File.AppendText(path))
            {
                w.Write(this.txtLog.Text);
            }            

            this.lbStatus.Foreground = System.Windows.Media.Brushes.Green;
            this.lbStatus.Content = "Processamento finalizado.";
            MessageBox.Show("Processamento concluído com sucesso! \n");
        }

        private void separarPorGrupos(List<Lab> pixels)
        {
            // Compara esta cor com cada centroide

            for (int i = 0; i < pixels.Count; i++)
            {
                double[] comparacoes = new double[5];
                double cValorMenor = double.MaxValue;
                int centroide = 0;

                comparacoes[0] = pixels[i].Compare(c1, new CieDe2000Comparison());
                if (comparacoes[0] <= cValorMenor)
                {
                    cValorMenor = comparacoes[0];
                    centroide = 1;
                }

                comparacoes[1] = pixels[i].Compare(c2, new CieDe2000Comparison());
                if (comparacoes[1] <= cValorMenor)
                {
                    cValorMenor = comparacoes[1];
                    centroide = 2;
                }

                comparacoes[2] = pixels[i].Compare(c3, new CieDe2000Comparison());
                if (comparacoes[2] <= cValorMenor)
                {
                    cValorMenor = comparacoes[2];
                    centroide = 3;
                }

                comparacoes[3] = pixels[i].Compare(c4, new CieDe2000Comparison());
                if (comparacoes[3] <= cValorMenor)
                {
                    cValorMenor = comparacoes[3];
                    centroide = 4;
                }
                comparacoes[4] = pixels[i].Compare(c5, new CieDe2000Comparison());
                if (comparacoes[4] <= cValorMenor)
                {
                    cValorMenor = comparacoes[4];
                    centroide = 5;
                }

                if (centroide == 1)
                    grupoC1.Add(i, pixels[i]);

                if (centroide == 2)
                    grupoC2.Add(i, pixels[i]);

                if (centroide == 3)
                    grupoC3.Add(i, pixels[i]);

                if (centroide == 4)
                    grupoC4.Add(i, pixels[i]);

                if (centroide == 5)
                    grupoC5.Add(i, pixels[i]);
            }
        }

        public Lab calcularLabMedio(Dictionary<int, Lab> cores) 
        {
            double somaR = 0;
            double somaG = 0;
            double somaB = 0;

            foreach (KeyValuePair<int, Lab> cor in cores)
            {
                somaR += cor.Value.L;
                somaG += cor.Value.A;
                somaB += cor.Value.B;
            }

            return new Lab {
                L = somaR / cores.Count,
                A = somaG / cores.Count,
                B = somaB / cores.Count 
                //L = (Math.Round((somaR / cores.Count), 2)),
                //A = (Math.Round((somaG / cores.Count), 2)),
                //B = (Math.Round((somaB / cores.Count), 2)) 
            };
        }

        public void SalvarPlyMeshAtualiado(TextWriter writer)
        {
            if (null == mesh || null == writer)
            {
                return;
            }

            var vertices = mesh.GetVertices();
            var indices = mesh.GetTriangleIndexes();
            var colors = mesh.GetColors();

            // Check mesh arguments
            if (0 == vertices.Count || 0 != vertices.Count % 3 || vertices.Count != indices.Count )
            {
                throw new ArgumentException(Unicamp.Kinect.BodyScanner.Properties.Resources.InvalidMeshArgument);
            }

            int faces = indices.Count / 3;

            // Write the PLY header lines
            writer.WriteLine("ply");
            writer.WriteLine("format ascii 1.0");
            writer.WriteLine("comment file created by Microsoft Kinect Fusion");

            writer.WriteLine("element vertex " + vertices.Count.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("property float x");
            writer.WriteLine("property float y");
            writer.WriteLine("property float z");

            writer.WriteLine("property uchar red");
            writer.WriteLine("property uchar green");
            writer.WriteLine("property uchar blue");
            writer.WriteLine("property uchar alpha");
            
            writer.WriteLine("element face " + faces.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine("property list uchar int vertex_indices");
            writer.WriteLine("end_header");

            // Sequentially write the 3 vertices of the triangle, for each triangle
            for (int i = 0; i < vertices.Count; i++)
            {
                var vertex = vertices[i];

                string vertexString = vertex.X.ToString(CultureInfo.InvariantCulture) + " ";

                //vertexString += (-vertex.Y).ToString(CultureInfo.InvariantCulture) + " " + (-vertex.Z).ToString(CultureInfo.InvariantCulture);
                vertexString += vertex.Y.ToString(CultureInfo.InvariantCulture) + " " + vertex.Z.ToString(CultureInfo.InvariantCulture);
                int red = 0;
                int green = 0;
                int blue = 0;

                if (grupoC1.ContainsKey(i))
                {
                    Rgb cor = (Rgb)c1.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                }
                else if (grupoC2.ContainsKey(i)) 
                {
                    Rgb cor = (Rgb)c2.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                }
                else if (grupoC3.ContainsKey(i))
                {
                    Rgb cor = (Rgb)c3.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                }
                else if (grupoC4.ContainsKey(i))
                {
                    Rgb cor = (Rgb)c4.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                }
                else if (grupoC5.ContainsKey(i))
                {
                    Rgb cor = (Rgb)c5.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                }

                vertexString += " " + red.ToString(CultureInfo.InvariantCulture) + " " + green.ToString(CultureInfo.InvariantCulture) + " "
                                + blue.ToString(CultureInfo.InvariantCulture) + " " + "255";
                

                writer.WriteLine(vertexString);
            }

            // Sequentially write the 3 vertex indices of the triangle face, for each triangle, 0-referenced in PLY files
            for (int i = 0; i < faces; i++)
            {
                // Lucas - Enconta o indice de cada vertice para uni-los e gerar a face
                int baseIndex0 = (i * 3);
                int baseIndex1 = ((i * 3) + 1);
                int baseIndex2 = ((i * 3) + 2);

                int red = 0;
                int green = 0;
                int blue = 0;

                // Forca do grupo por triagulo
                int g1 = 0;
                int g2 = 0;
                int g3 = 0;
                int g4 = 0;
                int g5 = 0;

                if (grupoC1.ContainsKey(baseIndex0))
                {
                    Rgb cor = (Rgb)c1.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g1++;
                }
                else if (grupoC2.ContainsKey(baseIndex0))
                {
                    Rgb cor = (Rgb)c2.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g2++;
                }
                else if (grupoC3.ContainsKey(baseIndex0))
                {
                    Rgb cor = (Rgb)c3.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g3++;
                }
                else if (grupoC4.ContainsKey(baseIndex0))
                {
                    Rgb cor = (Rgb)c4.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g4++;
                }
                else if (grupoC5.ContainsKey(baseIndex0))
                {
                    Rgb cor = (Rgb)c5.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g5++;
                }

                int red0 = red;
                int green0 = green;
                int blue0 = blue;


                if (grupoC1.ContainsKey(baseIndex1))
                {
                    Rgb cor = (Rgb)c1.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g1++;
                }
                else if (grupoC2.ContainsKey(baseIndex1))
                {
                    Rgb cor = (Rgb)c2.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g2++;
                }
                else if (grupoC3.ContainsKey(baseIndex1))
                {
                    Rgb cor = (Rgb)c3.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g3++;
                }
                else if (grupoC4.ContainsKey(baseIndex1))
                {
                    Rgb cor = (Rgb)c4.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g4++;
                }
                else if (grupoC5.ContainsKey(baseIndex1))
                {
                    Rgb cor = (Rgb)c5.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g5++;
                }

                int red1 = red;
                int green1 = green;
                int blue1 = blue;

                if (grupoC1.ContainsKey(baseIndex2))
                {
                    Rgb cor = (Rgb)c1.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g1++;
                }
                else if (grupoC2.ContainsKey(baseIndex2))
                {
                    Rgb cor = (Rgb)c2.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g2++;
                }
                else if (grupoC3.ContainsKey(baseIndex2))
                {
                    Rgb cor = (Rgb)c3.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g3++;
                }
                else if (grupoC4.ContainsKey(baseIndex2))
                {
                    Rgb cor = (Rgb)c4.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g4++;
                }
                else if (grupoC5.ContainsKey(baseIndex2))
                {
                    Rgb cor = (Rgb)c5.ToRgb();
                    red = (int)cor.R;
                    green = (int)cor.G;
                    blue = (int)cor.B;
                    g5++;
                }

                int red2 = red;
                int green2 = green;
                int blue2 = blue;

                string grupo = "";
                int maior;
                int[] array = { g1, g2, g3, g4, g5 };
                maior = array.Max();

                // Adiciono a qual grupo pertence este triangulo. Isto será necessário para melhorar a performance da renderização no 
                // helix
                if (maior == g1)
                    grupo = "1";
                if (maior == g2)
                    grupo = "2";
                if (maior == g3)
                    grupo = "3";
                if (maior == g4)
                    grupo = "4";
                if (maior == g5)
                    grupo = "5";

                // Lucas - Alterando a geração do ply para salvar a media da cor dos 3 vertices na face
                string faceString = "3 " +
                    baseIndex0.ToString() + " " + baseIndex1.ToString() + " " + baseIndex2.ToString() + " "
                    + ((red0 + red1 + red2) / 3) + " " + // Red
                    ((green0 + green1 + green2) / 3) + " " + // Green
                    ((blue0 + blue1 + blue2) / 3); // Blue
                writer.WriteLine(faceString + " " + grupo);
            }
        }

        public Lab atualizaMeshGrupo(Dictionary<int, Lab> cores)
        {
            double somaR = 0;
            double somaG = 0;
            double somaB = 0;

            foreach (KeyValuePair<int, Lab> cor in cores)
            {
                somaR += cor.Value.L;
                somaG += cor.Value.A;
                somaB += cor.Value.B;
            }

            return new Lab
            {
                L = somaR / cores.Count,
                A = somaG / cores.Count,
                B = somaB / cores.Count
                //L = (Math.Round((somaR / cores.Count), 2)),
                //A = (Math.Round((somaG / cores.Count), 2)),
                //B = (Math.Round((somaB / cores.Count), 2)) 
            };
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

        private void btnIniciarKmeans_Click(object sender, RoutedEventArgs e)
        {
            iniciarKMeans();
        }

        public void DeleteAllExportedImages() 
        {
            string path = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + "\\Images\\Exported\\Color";
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

        private void btnProximo_Click(object sender, RoutedEventArgs e)
        {
            // Navega para a quarta tela
            this.NavigationService.Navigate(new Quarta());
        }

    }

}
