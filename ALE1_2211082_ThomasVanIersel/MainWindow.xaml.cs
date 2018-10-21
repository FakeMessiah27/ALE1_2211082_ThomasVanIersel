using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace ALE1_2211082_ThomasVanIersel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GraphvizHelper gh;
        string previousHashValue;

        public MainWindow()
        {
            InitializeComponent();
            
            gh = new GraphvizHelper();
            previousHashValue = null;
        }
        
        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            // Disable controls.
            DisableControls();
            // Select the first tab.
            this.Dispatcher.BeginInvoke((Action)(() => tcMain.SelectedIndex = 0));

            // Get the initial input from the prefix textbox and remove spaces.
            string input = tbPrefix.Text;

            // Don't do anything if the textbox is empty.
            if (String.IsNullOrWhiteSpace(input))
                return;

            // Remove spaces.
            input = input.Replace(" ", "");

            // Create Formula object to hold the formula.
            Formula formula = new Formula(input);

            // Show an error message if something goes wrong trying to read the formula.
            if (formula.FirstNode == null)
            {
                System.Windows.MessageBox.Show("Something went wrong while trying to read the formula. Please ensure it is in a syntactically correct prefix notation.", "Error!");
                return;
            }

            // Apply the various outputs found above to the appropriate textboxes.
            tbInfix.Text = formula.InfixNotation;
            tbVariables.Text = String.Join(",", formula.Variables);

            // Use the GraphvizHelper object to generate the graph and display it in the ui.
            bool graphCreated = gh.CreateGraph(formula.FirstNode);

            if (graphCreated == true)
            {
                // Apply the created .png file to the image element as a bitmap.
                graph.Source = gh.GetBitmapFromPng();
            }
            else
            {
                // If the graph was not created successfully, the system could not find GraphViz' "dot.exe".
                string errorMessage = "Couldn't find GraphViz' \"dot.exe\"! Please ensure you have it installed on your computer and have your PATH variables set up correctly.\n\n" +
                    "Alternatively, use the button on the Graph tab to set the path to your GraphViz  \"dot.exe\".";
                System.Windows.MessageBox.Show(errorMessage, "Error!");
            }

            // Generate truth table.
            List<string> truthTableSource = AddTabs(new List<string>(formula.GenerateTruthTable()));
            truthTable.ItemsSource = truthTableSource;

            // Generate hash code from truth table.
            string hashValue = GetHashFromTable(truthTableSource);
            
            // Set the hash code labels.
            if (previousHashValue != null)
            {
                lblPreviousHashCode.Content = "Previous hash code:\t" + previousHashValue;
            }
            lblHashCode.Content = "Hash code:\t\t" + hashValue;
            previousHashValue = hashValue;

            // Generate minimised truth table.
            Task.Factory.StartNew(() => 
            {
                QuineMcCluskeyControl qmControl = new QuineMcCluskeyControl();
                string simplifiedTable = qmControl.ProcessTruthTableString(truthTableSource);
                this.Dispatcher.Invoke(() =>
                {
                    tbMinimised.Text = simplifiedTable;
                    tabItemSimplify.IsEnabled = true;
                });
            });

            //tbMinimised.Text = qmControl.ProcessTruthTableString(truthTableSource);

            // Generate Disjunctive normal form.
            tbDisNormal.Text = formula.GetDisjunctiveNormalForm();

            // Generate Nandified form.
            if (formula.FirstNode.Characters == "%")
            {
                tbNandified.Text = input;
            }
            else
            {
                tbNandified.Text = formula.GetNandifiedForm(formula.FirstNode);
            }

            // Re-enable the execute button.
            btnExecute.IsEnabled = true;
        }

        private void btnSetPath_Click(object sender, RoutedEventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select your GraphViz dot.exe";
                ofd.Filter = "exe files (*.exe)|*.exe";
                DialogResult result = ofd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    gh.DotProcessFileName = ofd.FileName;
                }
            }
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

        /// <summary>
        /// Disables the simplify tabitem and the execute button.
        /// </summary>
        private void DisableControls()
        {
            tabItemSimplify.IsEnabled = false;

            btnExecute.IsEnabled = false;
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpMessage = "For this application, a valid formula must adhere to the following rules:";
            helpMessage += "\n\n";
            helpMessage += "\n- The numbers of opening and closing brackets must match.";
            helpMessage += "\n- An opening bracket must be preceded by a valid operator (see below).";
            helpMessage += "\n- The formula must have at least one operator.";
            helpMessage += "\n- The formula must have at least one variable.";
            helpMessage += "\n- Variables cannot be whitespace";
            helpMessage += "\n- The negation operator requires brackets.";
            helpMessage += "\n\t- Good: \t\"~(A)\"";
            helpMessage += "\n\t- Bad: \t\"~A\"";
            helpMessage += "\n\n";
            helpMessage += "\n Valid operators:";
            helpMessage += "\n\t- Negation (NOT): \t\t\"~\"";
            helpMessage += "\n\t- Implication: \t\t\">\"";
            helpMessage += "\n\t- Biconditional (XNOR): \t\"=\"";
            helpMessage += "\n\t- Conjunction (AND): \t\"&\"";
            helpMessage += "\n\t- Disjunction (OR): \t\"|\"";
            helpMessage += "\n\t- Not And (NAND): \t\"%\"";

            System.Windows.MessageBox.Show(helpMessage, "Help");
        }
    }
}
