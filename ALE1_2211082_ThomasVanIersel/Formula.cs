using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE1_2211082_ThomasVanIersel
{
    class Formula
    {
        #region ---------------------------------- PROPERTIES ------------------------------------

        public Node FirstNode { get; set; }

        public List<String> Variables { get; set; }

        public string InfixNotation { get; set; }

        public string PrefixNotation { get; set; }

        public string HashCode { get; set; }

        #endregion
        #region --------------------------------- CONSTRUCTORS -----------------------------------

        public Formula(string prefixNotation)
        {
            PrefixNotation = prefixNotation ?? throw new ArgumentNullException(nameof(prefixNotation));

            // Get all unique variables in the formula.
            Variables = GetVariablesFromPrefix(PrefixNotation);

            // Generate the tree and store its first node.
            FirstNode = CreateTree(PrefixNotation);

            // Create the infix version of the formula.
            InfixNotation = FirstNode.ToString();
        }

        #endregion
        #region ------------------------------------ METHODS -------------------------------------
        
        /// <summary>
        /// Takes a formula in prefix notation and returns all variables as a list of strings.
        /// </summary>
        /// <param name="prefixFormula"></param>
        /// <returns></returns>
        private List<string> GetVariablesFromPrefix(string prefixFormula)
        {
            List<string> variables = new List<string>();

            for (int i = 0; i < prefixFormula.Length; i++)
            {
                if (Char.IsLetter(prefixFormula[i]))
                    variables.Add(prefixFormula[i].ToString());
            }

            variables.Sort();
            return variables.Distinct().ToList();
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

        public List<string> GenerateTruthTable()
        {
            List<string> textLines = new List<string>();
            string topLine = "";

            for (int i = 0; i < Variables.Count; i++)
            {
                if (i != 0)
                    topLine += "\t";

                topLine += Variables[i];
            }
            topLine += "\tFormula";
            textLines.Add(topLine);

            // Generate lines with all possible 0/1 combinations.
            for (int i = 0; i <= Math.Pow(2, Variables.Count) - 1; i++)
            {
                textLines.Add(Convert.ToString(i, 2).PadLeft(Variables.Count, '0'));
            }

            List<string> TESTLines = new List<string>
            {
                "A\tB\tC",
                "0\t0\t0",
                "0\t0\t1",
                "0\t1\t0",
                "0\t1\t1",
                "1\t0\t0",
                "1\t0\t1",
                "1\t1\t0",
                "1\t1\t1",
            };

            for (int i = 1; i < textLines.Count; i++)
            {
                Dictionary<string, bool> truthValues = new Dictionary<string, bool>();
                string cleanLine = textLines[i].Replace("\t", "");

                for (int j = 0; j < cleanLine.Count(); j++)
                {
                    if (cleanLine[j] == '0')
                        truthValues.Add(Variables[j], false);
                    else
                        truthValues.Add(Variables[j], true);
                }

                textLines[i] += "\t" + GetTruthValue(FirstNode, truthValues);
            }
            
            return textLines;
        }

        private bool GetTruthValue(Node node, Dictionary<string, bool> truthValues)
        {
            if (node.FirstChild == null && node.SecondChild == null)
            {
                foreach (KeyValuePair<string, bool> tv in truthValues)
                {
                    if (tv.Key == node.Characters)
                        return tv.Value;
                }
            }

            bool firstChildTruthValue = false;
            bool secondChildTruthValue = false;

            if (node.FirstChild != null)
            {
                firstChildTruthValue = GetTruthValue(node.FirstChild, truthValues);
            }
            if (node.SecondChild != null)
            {
                secondChildTruthValue = GetTruthValue(node.SecondChild, truthValues);
            }

            switch (node.Characters)
            {
                case "~":
                    return !firstChildTruthValue;
                case ">":
                    if (firstChildTruthValue == true && secondChildTruthValue == true)
                        return true;
                    else if (firstChildTruthValue == false && secondChildTruthValue == true)
                        return true;
                    else if (firstChildTruthValue == false && secondChildTruthValue == false)
                        return true;
                    break;
                case "=":
                    if (firstChildTruthValue == secondChildTruthValue)
                        return true;
                    break;
                case "&":
                    if (firstChildTruthValue == true && secondChildTruthValue == true)
                        return true;
                    break;
                case "|":
                    if (firstChildTruthValue == true || secondChildTruthValue == true)
                        return true;
                    break;
                default:
                    break;
            }

            return false;
        }

        #endregion
    }
}
