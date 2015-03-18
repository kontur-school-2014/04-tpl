using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword
{
    public class Crossword
    {
        public int[,] CrosswordField;
        public List<Line> Lines;
        public int CountOfRows;
        public int CountOfColumns;
        public int ErrorLevel = 0;

        public Crossword GetCrosswordCopy()
        {
            return (Crossword) MemberwiseClone();
        }
    }
}
