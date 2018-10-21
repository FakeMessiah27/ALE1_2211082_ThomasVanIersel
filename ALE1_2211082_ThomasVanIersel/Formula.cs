using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALE1_2211082_ThomasVanIersel
{
    public class Formula
    {
        #region ---------------------------------- PROPERTIES ------------------------------------

        public Node FirstNode { get; set; }

        public List<String> Variables { get; set; }

        public string InfixNotation { get; set; }

        public string PrefixNotation { get; set; }

        public string HashCode { get; set; }

        public List<String> TruthTable { get; set; }

        #endregion
        #region --------------------------------- CONSTRUCTORS -----------------------------------

        // Empty contructor for unit testing.
        public Formula() { }

        public Formula(string prefixNotation)
        {
            PrefixNotation = prefixNotation ?? throw new ArgumentNullException(nameof(prefixNotation));

            // Get all unique variables in the formula.
            //Variables = GetVariablesFromPrefix(PrefixNotation);
            Variables = new List<string>();
            
            try
            {
                if (FormulaIsValid(prefixNotation))
                {
                    // Generate the tree and store its first node.
                    FirstNode = CreateTree(PrefixNotation);

                    // Get all unique variables.
                    Variables.Sort();
                    Variables = Variables.Distinct().ToList();

                    if (Variables.Count == 0)
                    {
                        FirstNode = null;
                    }

                    // Create the infix version of the formula.
                    InfixNotation = FirstNode.ToString();
                }
                else
                {
                    FirstNode = null;
                }
            }
            catch (Exception)
            {
                FirstNode = null;
            }
        }

        #endregion
        #region ------------------------------------ METHODS -------------------------------------

        /// <summary>
        /// Checks if the formula is in a syntactically correct, prefix notation. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool FormulaIsValid(string input)
        {
            // Store the number of brackets, commas, and negations.
            MatchCollection openingBrackets = Regex.Matches(input, Regex.Escape("("));
            int nrOfClosingBrackets = Regex.Matches(input, Regex.Escape(")")).Count;
            int nrOfNegations = Regex.Matches(input, Regex.Escape("~")).Count;
            int nrOfCommas = Regex.Matches(input, Regex.Escape(",")).Count;
            string[] operators = {"~", ">", "=", "&", "|", "%" };
            
            // Check if there are brackets in the first place, and if so, if there's an equal number of opening and closing brackets. 
            if (openingBrackets.Count > 0 && nrOfClosingBrackets > 0 && openingBrackets.Count == nrOfClosingBrackets)
            {
                // For every set of brackets, there needs to be a comma, EXCEPT if it's a negation.
                if (openingBrackets.Count - nrOfNegations == nrOfCommas)
                {
                    // Assume every set of brackets has an operator.
                    bool hasOperators = true;

                    // Loop through the opening brackets.
                    foreach (Capture openingBracket in openingBrackets)
                    {
                        // If an opening bracket is missing an operator, set the hasOperators variable to false.
                        if (operators.Contains(input[openingBracket.Index - 1].ToString()) == false)
                            hasOperators = false;
                    }

                    return hasOperators;
                }
            }

            return false;
        }

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
        
        #region Internal formula tree
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
                leftOperand = Utilities.Slice(input, 2, input.LastIndexOf(')'));
            }
            else
            {
                // If a comma was found in the middle of the formula (in between any possible sub-formulas), divide the input string into two new strings;
                // one for each side of the formula (the two operands).
                leftOperand = Utilities.Slice(input, 2, middleCommaPosition);
                rightOperand = Utilities.Slice(input, middleCommaPosition + 1, input.LastIndexOf(')'));
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
                if (String.IsNullOrWhiteSpace(leftOperand) == false)
                    Variables.Add(leftOperand);
                else
                    throw new Exception();
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
                    if (String.IsNullOrWhiteSpace(rightOperand) == false)
                        Variables.Add(rightOperand);
                    else
                        throw new Exception();
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
        #endregion
        #region Truth table
        /// <summary>
        /// Generates a truth table based on the internal tree of objects that make up the formula.
        /// </summary>
        /// <returns></returns>
        public List<string> GenerateTruthTable()
        {
            TruthTable = new List<string>();
            string topLine = "";

            // First add the top line which has the variables on it.
            for (int i = 0; i < Variables.Count; i++)
            {
                if (i != 0)
                    topLine += "\t";

                topLine += Variables[i];
            }
            topLine += "\tFormula";
            TruthTable.Add(topLine);

            // Generate lines with all possible 0/1 combinations.
            // By calculating the amount of variables to the power 2, you get the number of different combinations of 0 and 1 they can make.
            // By then counting up to that number and converting each of those decimal numbers to binary, you write all possible 0/1 combinations.
            // (For this code, I received help from Cezar Savin.)
            for (int i = 0; i <= Math.Pow(2, Variables.Count) - 1; i++)
            {
                TruthTable.Add(Convert.ToString(i, 2).PadLeft(Variables.Count, '0'));
            }

            for (int i = 1; i < TruthTable.Count; i++)
            {
                // For each line, create a dictionary holding the truth values for each variable.
                Dictionary<string, bool> truthValues = new Dictionary<string, bool>();

                // Read the truth value of each variable and store it in the dictionary.
                for (int j = 0; j < TruthTable[i].Count(); j++)
                {
                    if (TruthTable[i][j] == '0')
                        truthValues.Add(Variables[j], false);
                    else
                        truthValues.Add(Variables[j], true);
                }

                // Calculate, for this line, what the formula's outcome is, and append it onto the string to act as the final column.
                TruthTable[i] += Convert.ToInt32(GetTruthValue(FirstNode, truthValues));
            }
            
            return TruthTable;
        }

        /// <summary>
        /// Calculates the truth value (or outcome) of the formula based on a dictionary with truth values for each variable in the formula.
        /// </summary>
        /// <param name="node">The first node in the internal object of trees, representing the formula.</param>
        /// <param name="truthValues">The Dictionary holding the truth value, for each variable in the formula.</param>
        /// <returns></returns>
        private bool GetTruthValue(Node node, Dictionary<string, bool> truthValues)
        {
            // If this node has no further child nodes, simply return whatever truth value the variable has.
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

            // Use recursion to get to the bottom of the tree, on both sides.
            if (node.FirstChild != null)
            {
                firstChildTruthValue = GetTruthValue(node.FirstChild, truthValues);
            }
            if (node.SecondChild != null)
            {
                secondChildTruthValue = GetTruthValue(node.SecondChild, truthValues);
            }

            // Once a leaf in the tree is found, return the truth value of this (sub-)formula
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
                case "%":
                    if (firstChildTruthValue == false || secondChildTruthValue == false)
                        return true;
                    break;
                default:
                    break;
            }

            return false;
        }

        #endregion
        #region Disjunctive normal form

        /// <summary>
        /// Generates the disjunctive normal form of the formula using the pre-calculated truth table.
        /// </summary>
        /// <returns>Disjunctive normal form of the formula in infix notation.</returns>
        public string GetDisjunctiveNormalForm()
        {
            string disjunctiveNormalForm = "";
            List<string> trueRows = TruthTable.Where(r => r.Last() == '1').ToList();

            if (trueRows.Count == 0)
                return "Disjunctive normal form not possible. Formula is always false.";
            
            foreach (string row in trueRows)
            {
                if (row != trueRows.Last())
                {
                    disjunctiveNormalForm += "|(";
                }

                disjunctiveNormalForm += WriteAsDisjunctiveNormalForm(row);
                
                if (row != trueRows.Last())
                {
                    disjunctiveNormalForm += ",";
                }
                else
                {
                    for (int i = 1; i < trueRows.Count; i++)
                    {
                        disjunctiveNormalForm += ")";
                    }
                }
            }

            return disjunctiveNormalForm;
        }

        /// <summary>
        /// Takes one row of the truth table and converts it to its disjunctive normal form.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string WriteAsDisjunctiveNormalForm(string row)
        {
            string disjunctiveNormalForm = "&(";

            for (int i = 0; i < row.Length - 1; i++)
            {
                if (row[i]== '1')
                {
                    disjunctiveNormalForm += Variables[i];
                }
                else
                {
                    disjunctiveNormalForm += String.Format("~({0})", Variables[i]);
                }

                if (i == row.Length - 3)
                {
                    disjunctiveNormalForm += ",";
                }
                else if (i == row.Length - 2)
                {
                    for (int j = 0; j < Variables.Count - 1; j++)
                    {
                        disjunctiveNormalForm += ")";
                    }
                }
                else
                {
                    disjunctiveNormalForm += ",&(";
                }
            }
            
            return disjunctiveNormalForm;
        }

        #endregion
        #region Nandified form

        /// <summary>
        /// Generates the nandified form of the formula.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string GetNandifiedForm(Node node)
        {
            string nandifiedForm = "";

            string firstChildVariable = "";
            string secondChildVariable = "";

            // Use recursion to get to the bottom of the tree, on both sides.
            if (node.FirstChild != null)
            {
                firstChildVariable = GetNandifiedForm(node.FirstChild);
            }
            if (node.SecondChild != null)
            {
                secondChildVariable = GetNandifiedForm(node.SecondChild);
            }
            
            // Convert to nandified form, based on the operator.
            switch (node.Characters)
            {
                case "~":
                    nandifiedForm = String.Format("%({0},{0})", firstChildVariable);
                    break;
                case ">":
                    nandifiedForm = String.Format("%({0},%({1},{1}))", firstChildVariable, secondChildVariable);
                    break;
                case "=":
                    nandifiedForm = String.Format("%(%(%({0},{0}),%({1},{1})),%({0},{1}))", firstChildVariable, secondChildVariable);
                    break;
                case "&":
                    nandifiedForm = String.Format("%(%({0},{1}),%({0},{1}))", firstChildVariable, secondChildVariable);
                    break;
                case "|":
                    nandifiedForm = String.Format("%(%({0},{0}),%({1},{1}))", firstChildVariable, secondChildVariable);
                    break;
                default:
                    // If the current node is a leaf, it does not have an operator but a variable, which needs to be returned 
                    // to the previous iteration of the recursive loop.
                    return node.Characters;
            }

            return nandifiedForm;
        }

        #endregion

        // End of Methods region
        #endregion
    }
}
