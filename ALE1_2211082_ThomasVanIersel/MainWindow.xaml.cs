using System;
using System.Collections.Generic;
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
        GraphvizHelper gh;

        public MainWindow()
        {
            InitializeComponent();

            gh = new GraphvizHelper();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            string input = tbPrefix.Text;
            input = input.Replace(" ", "");

            firstNodeInTree = CreateTree(input);

            string infixFormula = firstNodeInTree.ToString();
            string variables = GetVariablesFromPrefix(infixFormula);

            tbInfix.Text = infixFormula;
            tbVariables.Text = variables;

            gh.CreateTextFile(firstNodeInTree);
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
            int openingBracketIndex = input.IndexOf('(');
            int closingBracketIndex = input.LastIndexOf(')');

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
                leftOperand = Slice(input, 2, middleCommaPosition);
                rightOperand = Slice(input, middleCommaPosition + 1, closingBracketIndex);
            }
            
            if (leftOperand.Contains("("))
            {
                leftNode = CreateTree(leftOperand);
            }
            else
            {
                leftNode.Characters = leftOperand;
            }
            operatorNode.FirstChild = leftNode;

            if (operatorNode.Characters != "~")
            {
                Node rightNode = new Node();
                if (rightOperand.Contains("("))
                {
                    rightNode = CreateTree(rightOperand);
                }
                else
                {
                    rightNode.Characters = rightOperand;
                }
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
