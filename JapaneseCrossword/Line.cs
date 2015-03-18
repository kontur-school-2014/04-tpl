using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JapaneseCrossword
{
    public class Line
    {
        public int Number;
        public bool IsHorizontal;
        public int CountOfBlocks;
        public int[] BlocksLength;
        public bool NeedRefresh;

        public Line(int number, bool isHorizontal,int countOfBlocks,int[] blocksLength,bool needRefresh)
        {
            Number = number;
            IsHorizontal = isHorizontal;
            CountOfBlocks = countOfBlocks;
            BlocksLength = blocksLength;
            NeedRefresh = needRefresh;
        }
    }
}
