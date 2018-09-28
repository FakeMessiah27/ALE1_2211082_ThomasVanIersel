using System;
using System.Collections.Generic;
using System.Windows;

namespace ALE1_2211082_ThomasVanIersel
{
    /// <summary>
    /// SOURCE: https://github.com/Simsso/Quine-McCluskey-Algorithm
    /// </summary>
    class QuineMcCluskeyControl
    {
        MainWindow mainWindow;

        public QuineMcCluskeyControl()
        {
        }

        public QuineMcCluskeyControl(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public string ProcessTruthTableString(List<string> truthTable)
        {
            try
            {
                TruthTable newTruthTable = StringToTruthTable(truthTable);
                List<List<LogicState>> minimized = QuineMcCluskeyAlgorithm.MinimizeTruthTable(newTruthTable);
                newTruthTable.SetInputStates(minimized);
                newTruthTable.ResetOutputStates();

                return newTruthTable.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured while processing the truth table. \n\n" + e.Message);
            }

            return null;
        }

        public static TruthTable StringToTruthTable(List<string> lines)
        {
            string[][] cells = new string[lines.Count][];
            for (int i = 0; i < lines.Count; i++)
            {
                cells[i] = lines[i].Split('\t');
            }

            string[] titles = new string[cells[0].Length];
            List<List<LogicState>> inputStates = new List<List<LogicState>>();
            List<LogicState> outputStates = new List<LogicState>();;

            for (int i = 0; i < cells.Length; i++)
            {
                if (i > 0)
                {
                    inputStates.Add(new List<LogicState>());
                }

                for (int j = 0; j < cells[i].Length; j++)
                {
                    if (i == 0)
                    {
                        titles[j] = cells[i][j];
                    }
                    else
                    {
                        if (j < cells[i].Length - 1)
                        {
                            inputStates[i - 1].Add(StringToLogicState(cells[i][j]));
                        }
                        else
                        {
                            outputStates.Add(StringToLogicState(cells[i][j]));
                        }
                    }
                }
            }

            return new TruthTable(titles, inputStates, outputStates);
        }

        public static LogicState StringToLogicState(string input)
        {
            switch (input)
            {
                case "0":
                    return LogicState.False;
                case "1":
                    return LogicState.True;
                case "*":
                    return LogicState.DontCare;
                default:
                    throw new ArgumentException("The truth table is damaged.");
            }
        }

        public static string LogicStateToString(LogicState state)
        {
            switch (state)
            {
                case LogicState.False:
                    return "0";
                case LogicState.True:
                    return "1";
                case LogicState.DontCare:
                    return "*";
                default:
                    throw new Exception("An unknown error occured.");
            }
        }
    }

    static class Extensions
    {
        public static List<T> Clone<T>(this List<T> list)
        {
            List<T> clone = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                clone.Add(list[i]);
            }

            return clone;
        }

    }
}
