using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE1_2211082_ThomasVanIersel
{
    class GraphvizHelper
    {
        public void CreateTextFile(Node firstNodeInTree)
        {
            string datetime = DateTime.Now.ToString();
            string datetimeSeed = "";
            foreach (char c in datetime)
            {
                if (Char.IsLetterOrDigit(c) == true)
                    datetimeSeed += c;
            }

            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Images\\" + datetimeSeed;

            string dotPath = path + ".dot";
            var file = File.Create(dotPath);
            file.Close();

            using (TextWriter tw = new StreamWriter(path))
            {
                tw.WriteLine("graph logic {");
                tw.WriteLine("node [ fontname = \"Arial\" ]");

                int nodeCounter = 0;

                DrawTreeGraph(firstNodeInTree, ref nodeCounter, tw);

                tw.WriteLine("}");
            }

            CreatePng(path, datetimeSeed);
        }

        private void CreatePng(string path, string datetimeSeed)
        {
            string dotPath = path + ".dot";
            Process dot = new Process();
            dot.StartInfo.FileName = @"dot.exe";
            dot.StartInfo.Arguments = String.Format("-Tpng -o{0}.png {1}", path, dotPath);

            dot.Start();
            dot.WaitForExit();


        }

        private int DrawTreeGraph(Node n, ref int nodeCounter, TextWriter tw)
        {
            int nodeNumber = ++nodeCounter;
            tw.WriteLine(String.Format("node{0} [ label = \"{1}\" ] ", nodeNumber, n.Characters));

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

            return nodeNumber;
        }
    }
}
