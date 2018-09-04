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
        Node firstNodeInTree;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            // Get the initial input from the prefix textbox and remove spaces.
            string input = tbPrefix.Text;
            input = input.Replace(" ", "");

            // Create the internal tree op objects, and store the first node in the tree to keep access to it.
            firstNodeInTree = CreateTree(input);

            // Create the infix version of the formula.
            string infixFormula = firstNodeInTree.ToString();
            // Get all unique variables in the formula.
            string variables = GetVariablesFromPrefix(infixFormula);

            // Apply the various outputs found above to the appropriate textboxes.
            tbInfix.Text = infixFormula;
            tbVariables.Text = variables;

            // Create a GraphvizHelper object and use it to create the Graph's .dot and .png files.
            GraphvizHelper gh = new GraphvizHelper();
            gh.CreateGraph(firstNodeInTree);

            // Apply the created .png file to the image element as a bitmap.
            graph.Source = gh.GetBitmapFromPng();
        }


       
        /// <summary>
        /// Takes a formula in prefix notation and returns all variables, separated by commas.
        /// </summary>
        /// <param name="prefixFormula"></param>
        /// <returns></returns>
        private string GetVariablesFromPrefix(string prefixFormula)
        {
            List<char> variables = new List<char>();

            for (int i = 0; i < prefixFormula.Length; i++)
            {
                if (Char.IsLetter(prefixFormula[i]))
                    variables.Add(prefixFormula[i]);
            }

            return String.Join(",", variables.Distinct());
        }

        /// <summary>
        /// Creates a tree of internal objects representing a formula in infix notation.
        /// </summary>
        /// <param name="input">Formula in infix notation.</param>
        /// <returns>First node of the tree.</returns>
        private Node CreateTree(string input)
        {
            // Create three nodes. 
            //
            // Operator node = first character (input[0]).
            // Left node = first operand, this can be another sub-formula.
            // Right node = second operand, this can be another sub-formula or nothing in case of negation.
            //
            // Return first node (the operator).

            // The operator node is always the first character in the strong, due to the infix notation used.
            Node operatorNode = new Node
            {
                Characters = input[0].ToString()
            };

            // Create an empty node object for the left node.
            Node leftNode = new Node();
            // Find the position of the middle comma in the formula, which separates the left and right operands.
            // Each of these operands can be a formula onto themselves.
            int middleCommaPosition = FindMiddleCommaPosition(input);
            string leftOperand = "";
            string rightOperand = "";

            // If the middle comma position could not be found, it will be 0. This is the case with negation.
            if (middleCommaPosition == 0)
            {
                // In case of negation (or any other operator that only has one operand), take whatever is in between the brackets.
                leftOperand = Slice(input, 2, input.IndexOf(')'));
            }
            else
            {
                // If a comma was found in the middle of the formula (in between any possible sub-formulas), divide the input string into two new strings;
                // one for each side of the formula (the two operands).
                leftOperand = Slice(input, 2, middleCommaPosition);
                rightOperand = Slice(input, middleCommaPosition + 1, input.LastIndexOf(')'));
            }
            
            
            if (leftOperand.Contains("("))
            {
                // If an opening bracket is found, it means the operand is a formula itself, and we will use recursion to dig down to the lowest level
                // of the formula in order to build the internal tree of objects.
                leftNode = CreateTree(leftOperand);
            }
            else
            {
                // Once a left operand is found that is not another sub-formula, store its characters.
                leftNode.Characters = leftOperand;
            }
            // Link the first found node (the operator) to its left operand as the first child.
            operatorNode.FirstChild = leftNode;

            // For the right operand, first check if the operator is a negation. If so, there is no right operand.
            if (operatorNode.Characters != "~")
            {
                Node rightNode = new Node();

                // Check if the right operand is a sub-formula.
                if (rightOperand.Contains("("))
                {
                    // If an opening bracket is found, it means the operand is a formula itself, and we will use recursion to dig down to the lowest level
                    // of the formula in order to build the internal tree of objects.
                    rightNode = CreateTree(rightOperand);
                }
                else
                {
                    // Once a right operand is found that is not another sub-formula, store its characters.
                    rightNode.Characters = rightOperand;
                }
                // Link the first found node (the operator) to its right operand as the second child.
                operatorNode.SecondChild = rightNode;
            }

            return operatorNode;
        }

        /// <summary>
        /// Finds the index position of the middle comma, separating the left and right operands of the formula.
        /// </summary>
        /// <param name="input">Formula in infix notation.</param>
        /// <returns></returns>
        private int FindMiddleCommaPosition(string input)
        {
            int nrOfOpeningBrackets = 0;
            int nrOfClosingBrackets = 0;
            int middleCommaPosition = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                    nrOfOpeningBrackets++;
                else if (input[i] == ')')
                    nrOfClosingBrackets++;
                else if (input[i] == ',')
                {
                    if (nrOfOpeningBrackets == nrOfClosingBrackets + 1)
                        middleCommaPosition = i;
                }
            }

            return middleCommaPosition;
        }

        /// <summary>
        /// Get the string slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// Source: https://www.dotnetperls.com/string-slice
        /// </summary>
        private string Slice(string source, int start, int end)
        {
            if (end < 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            int len = end - start;               // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }
    }
}
