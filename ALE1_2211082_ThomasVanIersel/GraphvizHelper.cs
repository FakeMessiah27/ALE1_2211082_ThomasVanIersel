using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ALE1_2211082_ThomasVanIersel
{
    class GraphvizHelper
    {
        string datetimeSeed;

        public string DotProcessFileName { get; set; }

        public GraphvizHelper()
        {
            // Create a new unique datetimeSeed to act as a filename each time a new helper is created.
            datetimeSeed = CreateDatetimeSeed();
        }

        public bool CreateGraph(Node firstNodeInTree)
        {
            // Get the path to application's directory.
            string currentDirectoryPath = Directory.GetCurrentDirectory();

            // Create a string for the path to the new .dot file that will be created.
            string dotfilePath = currentDirectoryPath + "\\" + datetimeSeed + ".dot";

            // Create the new file.
            var file = File.Create(dotfilePath);
            file.Close();

            // Write contents of .dot file for the graph.
            using (TextWriter tw = new StreamWriter(dotfilePath))
            {
                // Write standard header for .dot graph file.
                tw.WriteLine("graph logic {");
                tw.WriteLine("node [ fontname = \"Arial\" ]");

                // Set initial node counter to 0.
                int nodeCounter = 0;

                // Use recursion to write correct lines to .dot graph file.
                DrawTreeGraph(firstNodeInTree, ref nodeCounter, tw);

                // Write standard footer line for .dot graph file.
                tw.WriteLine("}");
            }

            // Create a png file from the above created .dot graph file.
            return CreatePng(currentDirectoryPath);
        }

        /// <summary>
        /// Creates a PNG image from a .dot file using the Graphviz application.
        /// </summary>
        /// <param name="currentDirectoryPath">Path to the folder containing the .dot file. This will also be the destination for the newly created image.</param>
        private bool CreatePng(string currentDirectoryPath)
        {
            Process dot = new Process();

            if (String.IsNullOrWhiteSpace(DotProcessFileName) == false)
            {
                dot.StartInfo.FileName = DotProcessFileName;
            }
            else
            {
                dot.StartInfo.FileName = @"dot.exe";
            }
            
            dot.StartInfo.Arguments = String.Format("-Tpng -o{0}.png {0}.dot", currentDirectoryPath + "\\" + datetimeSeed);

            try
            {
                dot.Start();
                dot.WaitForExit();
                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a string of numbers from the current system date and time.
        /// </summary>
        /// <returns></returns>
        private string CreateDatetimeSeed()
        {
            string datetime = DateTime.Now.ToString();
            string datetimeSeed = "";

            // Remove any special characters.
            foreach (char c in datetime)
            {
                if (Char.IsLetterOrDigit(c) == true)
                    datetimeSeed += c;
            }

            return datetimeSeed;
        }

        /// <summary>
        /// Starts at the first node in the tree and uses recursion to run through all the nodes. For each node, it writes the appropriate line
        /// neccesary for the graph to the .dot file.
        /// </summary>
        /// <param name="n">First node in the (sub) tree</param>
        /// <param name="nodeCounter">Counter to keep track of the number of nodes created. Used to name new nodes.</param>
        /// <param name="tw">TextWriter for .dot file.</param>
        /// <returns></returns>
        private int DrawTreeGraph(Node n, ref int nodeCounter, TextWriter tw)
        {
            // Increment the nodeCounter and store the result.
            int nodeNumber = ++nodeCounter;
            // Writes a line to the .dot file to create a new node in the graph.
            tw.WriteLine(String.Format("node{0} [ label = \"{1}\" ] ", nodeNumber, n.Characters));

            // Checks for both children if they exist, and if so, recursively searches for further children of these children.
            // After reaching the bottom of the tree, the recursion will return the nodeNumber of the previous child in the tree
            // in order to write a line to .dot file that will create the connection between parent and child node.
            if (n.FirstChild != null)
            {
                int childNodeNumber = DrawTreeGraph(n.FirstChild, ref nodeCounter, tw);
                tw.WriteLine(String.Format("node{0} -- node{1}", nodeNumber, childNodeNumber));
            }
            if (n.SecondChild != null)
            {
                int childNodeNumber = DrawTreeGraph(n.SecondChild, ref nodeCounter, tw);
                tw.WriteLine(String.Format("node{0} -- node{1}", nodeNumber, childNodeNumber));
            }

            // Returns the number of this node. Used to let previous step/node in recursion know what number this node had
            // in order to create the connecting lines in the graph.
            return nodeNumber;
        }

        /// <summary>
        /// Gets the bitmap from the previously created png file.
        /// </summary>
        /// <returns></returns>
        public BitmapImage GetBitmapFromPng()
        {
            var imageUri = new Uri(Directory.GetCurrentDirectory() + "\\" + datetimeSeed + ".png");
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = imageUri;
            bitmap.EndInit();

            return bitmap;
        }
    }
}
