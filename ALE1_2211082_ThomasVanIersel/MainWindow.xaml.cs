using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ALE1_2211082_ThomasVanIersel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        QuineMcCluskeyControl qmControl;

        public MainWindow()
        {
            InitializeComponent();

            qmControl = new QuineMcCluskeyControl();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            // Get the initial input from the prefix textbox and remove spaces.
            string input = tbPrefix.Text;
            input = input.Replace(" ", "");

            // Create Formula object to hold the formula.
            Formula formula = new Formula(input);

            // Apply the various outputs found above to the appropriate textboxes.
            tbInfix.Text = formula.InfixNotation;
            tbVariables.Text = String.Join(",", formula.Variables);

            // Create a GraphvizHelper object and use it to create the Graph's .dot and .png files.
            GraphvizHelper gh = new GraphvizHelper();
            gh.CreateGraph(formula.FirstNode);

            // Apply the created .png file to the image element as a bitmap.
            graph.Source = gh.GetBitmapFromPng();

            // Generate truth table.
            List<string> truthTableSource = AddTabs(new List<string>(formula.GenerateTruthTable()));
            truthTable.ItemsSource = truthTableSource;
            
            // Generate minimised truth table.
            tbMinimised.Text = qmControl.ProcessTruthTableString(truthTableSource);
            
            // Generate hash code from truth table.
            string hashValue = GetHashFromTable(truthTableSource);
            lblHashCode.Content = "Hash code: " + hashValue;
        }

        /// <summary>
        /// Takes a list of strings and adds tabs inbetween each character of each string.
        /// </summary>
        /// <param name="textLines"></param>
        /// <returns></returns>
        private List<string> AddTabs(List<string> textLines)
        {
            for (int i = 1; i < textLines.Count; i++)
            {
                string newLine = "";

                for (int j = 0; j < textLines[i].Count(); j++)
                {
                    newLine += textLines[i][j];

                    if (j != textLines[i].Count() - 1)
                    {
                        newLine += "\t";
                    }
                }

                textLines[i] = newLine;
            }

            return textLines;
        }

        /// <summary>
        /// Generates a hexadecimal hash code based on the right most column of the truth table (the last digit of each line in the list, starting at the back).
        /// </summary>
        /// <param name="truthTableSource"></param>
        /// <returns></returns>
        private string GetHashFromTable(List<string> truthTableSource)
        {
            string formulaTruthValues = "";

            for (int i = truthTableSource.Count - 1; i > 0; i--)
            {
                formulaTruthValues += truthTableSource[i].Last();
            }

            try
            {
                // Convert to binary first, then to decimal, then to hexadecimal.
                long binaryValue = Convert.ToInt64(formulaTruthValues, 2);
                return binaryValue.ToString("X");
            }
            catch (OverflowException)
            {
                return "NUMBER TOO LARGE";
            }
        }
    }
}
