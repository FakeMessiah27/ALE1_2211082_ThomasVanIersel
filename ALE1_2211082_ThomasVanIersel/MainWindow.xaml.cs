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
        public MainWindow()
        {
            InitializeComponent();
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
            truthTable.ItemsSource = formula.GenerateTruthTable();

            // ToString("X")
            // Generate hash code from truth table.
            //lblHashCode.Content = "Hash code: " + GetHashFromTable();
        }

        //private string GetHashFromTable()
        //{
            
        //}
    }
}
