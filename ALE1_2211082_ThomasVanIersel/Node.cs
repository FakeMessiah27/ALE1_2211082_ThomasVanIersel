using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE1_2211082_ThomasVanIersel
{
    class Node
    {
        public string Characters { get; set; }

        public Node FirstChild { get; set; }

        public Node SecondChild { get; set; }

        public override string ToString()
        {
            string op = "";

            switch (Characters)
            {
                case "~":
                    op = "¬";
                    break;
                case ">":
                    op = "⇒";
                    break;
                case "=":
                    op = "⇔";
                    break;
                case "&":
                    op = "⋀";
                    break;
                case "|":
                    op = "⋁";
                    break;
                default:
                    return Characters;
            }

            if (FirstChild != null && SecondChild != null)
            {
                return String.Format("({0}{1}{2})", FirstChild.ToString(), op, SecondChild.ToString());
            }
            else if (FirstChild != null)
            {
                return String.Format("({0}{1})", op, FirstChild.ToString());
            }
            else
            {
                return op;
            }
        }
    }
}
