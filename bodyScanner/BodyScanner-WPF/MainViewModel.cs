namespace Microsoft.Samples.Kinect.KinectFusionExplorer
{
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using HelixToolkit.Wpf;
    using System.IO;
    using System;

    /// <summary>
    /// Provides a ViewModel for the Main window.
    /// </summary>
    public class MainViewModel
    {

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

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            // Create a model group
            modelGroup = new Model3DGroup();
            Material insideMaterial = MaterialHelper.CreateMaterial(Colors.Gray);
            Material g1Material = null;
            Material g2Material = null;
            Material g3Material = null;
            Material g4Material = null;
            Material g5Material = null;

            // Create a mesh builder and add a box to it
            var meshBuilder1 = new MeshBuilder(false, false);
            var meshBuilder2 = new MeshBuilder(false, false);
            var meshBuilder3 = new MeshBuilder(false, false);
            var meshBuilder4 = new MeshBuilder(false, false);
            var meshBuilder5 = new MeshBuilder(false, false);

            string line;
            int count = 0;
            System.Collections.Generic.List<Point3D> pontos = new System.Collections.Generic.List<Point3D>();
            // Lê o arquivo ply
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Projetos\Unicamp\unicamp.mestradocompleto\bodyScanner\BodyScanner-WPF\Models\Model2.ply");
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
                             Color.FromRgb(
                                 Byte.Parse(coord[4]),
                                 Byte.Parse(coord[5]),
                                 Byte.Parse(coord[6])));

                    }

                    if (Int32.Parse(coord[7]) == 2)
                    {
                        meshBuilder2.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g2Material)
                            g2Material = MaterialHelper.CreateMaterial(
                            Color.FromRgb(
                                Byte.Parse(coord[4]),
                                Byte.Parse(coord[5]),
                                Byte.Parse(coord[6])));
                    }

                    if (Int32.Parse(coord[7]) == 3)
                    {
                        meshBuilder3.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g3Material)
                            g3Material = MaterialHelper.CreateMaterial(
                            Color.FromRgb(
                                Byte.Parse(coord[4]),
                                Byte.Parse(coord[5]),
                                Byte.Parse(coord[6])));
                    }

                    if (Int32.Parse(coord[7]) == 4)
                    {
                        meshBuilder4.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g4Material)
                            g4Material = MaterialHelper.CreateMaterial(
                            Color.FromRgb(
                                Byte.Parse(coord[4]),
                                Byte.Parse(coord[5]),
                                Byte.Parse(coord[6])));
                    }

                    if (Int32.Parse(coord[7]) == 5)
                    {
                        meshBuilder5.AddTriangle(
                            pontos[Int32.Parse(coord[1])],
                            pontos[Int32.Parse(coord[2])],
                            pontos[Int32.Parse(coord[3])]);

                        if (null == g5Material)
                            g5Material = MaterialHelper.CreateMaterial(
                            Color.FromRgb(
                                Byte.Parse(coord[4]),
                                Byte.Parse(coord[5]),
                                Byte.Parse(coord[6])));
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

            modelGroup.Children.Add(new GeometryModel3D
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

            modelGroup.Children.Add(new GeometryModel3D
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

            this.Model = modelGroup;
        }

        public void AtualizarModel(bool cb1, bool cb2, bool cb3, bool cb4, bool cb5) 
        {
            // Limapa o grupo
            modelGroup = new Model3DGroup();

            if(cb1)
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

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public Model3D Model { get; set; }
    }
}