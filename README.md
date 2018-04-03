# unicamp.mestrado.bodyScan

This is a first version of my work developing a body scanner using Kinect SDK and K-means algorithm to classify the body colors. It was used in a Thermoregulatory Sweat Test in hospitals with real models.

## Running and Debugging

### .NET Framework Version

4.5

### Kinect SDK Version

Latest

### Environment

This project requires Visual Studio 2010 (or newer), all libs were included.

### Running

Import the project file in VS 2010 (bodyScanner/BodyScanner-WPF/BodyScanner-WPF), probably it will complain about missing libs in references. Use the dlls in bodyScanner/DLLs to import those libs.

Some projects can be missing too. Import the missing projects using the source code inside bodyScanner (like ColorMine for example) if you need to debug it or change it. If you just want to run, try only importing the DDLs inside.