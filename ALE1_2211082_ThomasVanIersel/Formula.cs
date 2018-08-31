using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE1_2211082_ThomasVanIersel
{
    class Formula
    {
        public string Operator { get; set; }
        public Operand FirstOperand { get; set; }
        public Operand SecondOperand { get; set; }

        public override string ToString()
        {
            if (SecondOperand != null)
                return String.Format("{0}({1}, {2})", Operator, FirstOperand, SecondOperand);
            else
                return String.Format("{0}({1})", Operator, FirstOperand);
        }
    }
}
