using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword
{
    public static class CrosswordLoader
    {

        public static Crossword Load(StreamReader reader)
        {
            var inputData = reader.ReadToEnd().Split(new [] {"rows:","columns:","\r","\n"},StringSplitOptions.RemoveEmptyEntries);
            var crossword = new Crossword
            {
                CrosswordField = new int[int.Parse(inputData[0]), int.Parse(inputData[int.Parse(inputData[0]) + 1])],
                Lines = new List<Line>(),
                CountOfRows = int.Parse(inputData[0]),
                CountOfColumns = int.Parse(inputData[int.Parse(inputData[0]) + 1])       
            };
            for (var i = 0; i < crossword.CountOfRows; i++)
                for (var j = 0; j < crossword.CountOfColumns; j++)
                    crossword.CrosswordField[i, j] = 2;
            for (var i = 0; i < crossword.CountOfRows; i++)
            {
                var blocks = inputData[i+1].Split(' ');
                var line = new Line(i, true, blocks.Length, Enumerable.Range(0, blocks.Length).Select(x => int.Parse(blocks[x])).ToArray(), true);
                crossword.Lines.Add(line);
            }
            for (var i = 0; i < crossword.CountOfColumns; i++)
            {
                var blocks = inputData[i + int.Parse(inputData[0]) + 2].Split(' ');
                var line = new Line(i, false, blocks.Length, Enumerable.Range(0, blocks.Length).Select(x => int.Parse(blocks[x])).ToArray(), true);
                crossword.Lines.Add(line);
            }
            return crossword;
        }
    }
}
