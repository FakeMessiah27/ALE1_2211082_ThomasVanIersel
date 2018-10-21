using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALE1_2211082_ThomasVanIersel;

namespace FormulaTesting
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void InputValidation()
        {
            Dictionary<string, string> inputs = new Dictionary<string, string>();
            inputs.Add("correctInput", ">(=(A,~(C)),|(B,C))");
            inputs.Add("emptyInput", "");
            inputs.Add("missingOpeningBracket", ">(A,|B,C))");
            inputs.Add("missingClosingBracket", ">(=(A,~(C),|(B,C))");
            inputs.Add("missingComma", ">(=(A~(C)),|(B,C))");
            inputs.Add("missingOperator", ">((A,~(C)),|(B,C))");

            Formula formula = new Formula();
            
            foreach (var input in inputs)
            {
                if (input.Key == "correctInput")
                    Assert.AreEqual(formula.FormulaIsValid(input.Value), true);
                else
                    Assert.AreEqual(formula.FormulaIsValid(input.Value), false);
            }
        }

        [TestMethod]
        public void FormulaTest1()
        {
            // Simple formula.
            string input = ">(A,B)";

            Assert.AreEqual(TestFormula(input), true);
        }

        [TestMethod]
        public void FormulaTest2()
        {
            // Mediocre formula.
            string input = ">(=(A,~(C)),|(B,C))";

            Assert.AreEqual(TestFormula(input), true);
        }

        [TestMethod]
        public void FormulaTest3()
        {
            // Formula with all operators.
            string input = "=(>(A,B),|(~(A),=(B,>(C,D))))";

            Assert.AreEqual(TestFormula(input), true);
        }

        [TestMethod]
        public void FormulaTestContradiction()
        {
            // Contradiction.
            string input = "&(~(p),p)";

            Assert.AreEqual(TestFormula(input), true);
        }

        [TestMethod]
        public void FormulaTestTautology()
        {
            // Tautology.
            string input = "|(>(p,q),>(q,p))";

            Assert.AreEqual(TestFormula(input), true);
        }

        private bool TestFormula(string input)
        {
            // Create a formula object.
            Formula formula = new Formula(input);

            // Store various outputs of the formula's creation.
            string infixNotation = formula.InfixNotation;
            List<string> variables = formula.Variables;
            string initialHashValue = GetHashFromTable(formula.GenerateTruthTable());
            string disjunctiveNormalForm = formula.GetDisjunctiveNormalForm();
            string disjunctiveHashValue = "";

            // We need to test if the disjunctive normal form was created in the first place. If a formula is always false, the return
            // value of the GetDisjunctiveNormalForm method wouldn't be a valid formula, but an error message instead.
            if (formula.FormulaIsValid(disjunctiveNormalForm))
            {
                Formula disjunctiveFormula = new Formula(formula.GetDisjunctiveNormalForm());
                disjunctiveHashValue = GetHashFromTable(disjunctiveFormula.GenerateTruthTable());
            }
            
            Formula nandifiedFormula = new Formula(formula.GetNandifiedForm(formula.FirstNode));
            string nandifiedHashValue = GetHashFromTable(nandifiedFormula.GenerateTruthTable());

            if (initialHashValue == nandifiedHashValue)
            {
                if (initialHashValue == disjunctiveHashValue)
                    return true;
                else if (disjunctiveNormalForm == "Disjunctive normal form not possible. Formula is always false.")
                    return true;
            }

            return false;
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
