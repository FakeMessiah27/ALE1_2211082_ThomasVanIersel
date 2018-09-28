using System.Collections.Generic;
using System.Text;

namespace ALE1_2211082_ThomasVanIersel
{
    /// <summary>
    /// SOURCE: https://github.com/Simsso/Quine-McCluskey-Algorithm
    /// </summary>
    class TruthTable
    {
        public string[] Titles;
        public List<List<LogicState>> InputStates;
        public List<LogicState> OutputStates;

        public TruthTable(string[] titles, List<List<LogicState>> inputStates, List<LogicState> outputStates)
        {
            this.Titles = titles;
            this.InputStates = inputStates;
            this.OutputStates = outputStates;
        }

        public void SetInputStates(List<List<LogicState>> newStates)
        {
            this.InputStates = newStates;
        }

        public void SetOutputStates(List<LogicState> newStates)
        {
            this.OutputStates = newStates;
        }

        // No longer in use
        public void SetOutputStatesToTrue()
        {
            int count = InputStates.Count;
            this.OutputStates = new List<LogicState>();
            for (int i = 0; i < count; i++)
            {
                this.OutputStates.Add(LogicState.True);
            }
        }

        /// <summary>
        /// Resets the truth values for the output of each row, after minimisation.
        /// (Added by Thomas van Iersel)
        /// </summary>
        public void ResetOutputStates()
        {
            int count = InputStates.Count;
            this.OutputStates = new List<LogicState>();

            // If a row has a star (LogicState.DontCare) in it, it has been minised and is therefore true.
            // If it doesn't have a star in it, it is one of the lines that was false.
            for (int i = 0; i < count; i++)
            {
                if (InputStates[i].Contains(LogicState.DontCare))
                    OutputStates.Add(LogicState.True);
                else
                    OutputStates.Add(LogicState.False);
            }
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < Titles.Length; i++)
            {
                output.Append(Titles[i] + ((i < Titles.Length - 1) ? "\t" : ""));
            }

            output.Append("\r\n");


            for (int i = 0; i < InputStates.Count; i++)
            {
                for (int j = 0; j < InputStates[i].Count; j++)
                {
                    output.Append(QuineMcCluskeyControl.LogicStateToString(InputStates[i][j]) + "\t");
                }
                output.Append(QuineMcCluskeyControl.LogicStateToString(OutputStates[i]) + ((i < InputStates.Count - 1) ? "\r\n" : ""));
            }

            return output.ToString();
        }
    }
}
