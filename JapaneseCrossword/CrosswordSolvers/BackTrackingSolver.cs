using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword.CrosswordSolvers
{
    public class BackTrackingSolver : ICrosswordSolver
    {
        private Crossword crossword;
        private StreamWriter outputStream;
        private bool solutionFound;
        private SolutionStatus solutionStatus;

        public SolutionStatus Solve(string inputFilePath, string outputFilePath)
        {
            if (!File.Exists(inputFilePath))
                return SolutionStatus.BadInputFilePath;
            try
            {
                var fileStream = File.Create(outputFilePath);
                fileStream.Close();
                outputStream = new StreamWriter(outputFilePath);
            }
            catch (Exception)
            {
                return SolutionStatus.BadOutputFilePath;
            }
            crossword = CrosswordLoader.Load(new StreamReader(inputFilePath));
            solutionFound = false;
            TrySolve(0, 0);
            outputStream.Close();
            return solutionStatus;
        }

        private void TrySolve(int y, int x)
        {
            var copyOfCrosswordField = crossword.GetCrosswordCopy();
            crossword.ErrorLevel = 0;
            IterateLineAnalize();
            if (crossword.ErrorLevel != 0)
            {
                solutionStatus = SolutionStatus.IncorrectCrossword;
                return;
            }
            int j = y, i = x;
            while (j < crossword.CountOfRows && crossword.CrosswordField[j, i] != 2)
                if (i == crossword.CountOfColumns - 1)
                {
                    i = 0;
                    ++j;
                }
                else
                    ++i;
            if (j >= crossword.CountOfRows)
            {
                OutputSolution();
                solutionStatus = SolutionStatus.Solved;
            }
            else
            {
                for (var row = 0; row < crossword.CountOfRows; row++)
                    for (var column = 0; column < crossword.CountOfColumns; column++)
                        copyOfCrosswordField.CrosswordField[row, column] = crossword.CrosswordField[row, column];
                crossword.CrosswordField[j, i] = 0;
                var lineForRefresh = crossword.Lines.Find(otherLine => otherLine.Number == j && otherLine.IsHorizontal);
                var indexOfLineForRefresh = crossword.Lines.IndexOf(lineForRefresh);
                crossword.Lines[indexOfLineForRefresh].NeedRefresh = true;
                lineForRefresh = crossword.Lines.Find(otherLine => otherLine.Number == i && !otherLine.IsHorizontal);
                indexOfLineForRefresh = crossword.Lines.IndexOf(lineForRefresh);
                crossword.Lines[indexOfLineForRefresh].NeedRefresh = true;
                TrySolve(j, i);
                for (var row = 0; row < crossword.CountOfRows; row++)
                    for (var column = 0; column < crossword.CountOfColumns; column++)
                        crossword.CrosswordField[row, column] = copyOfCrosswordField.CrosswordField[row, column];
                crossword.CrosswordField[j, i] = 1;
                lineForRefresh = crossword.Lines.Find(otherLine => otherLine.Number == j && otherLine.IsHorizontal);
                indexOfLineForRefresh = crossword.Lines.IndexOf(lineForRefresh);
                crossword.Lines[indexOfLineForRefresh].NeedRefresh = true;
                lineForRefresh = crossword.Lines.Find(otherLine => otherLine.Number == i && !otherLine.IsHorizontal);
                indexOfLineForRefresh = crossword.Lines.IndexOf(lineForRefresh);
                crossword.Lines[indexOfLineForRefresh].NeedRefresh = true;
                TrySolve(j, i);
            }
            solutionStatus = SolutionStatus.Solved;
        }

        private void OutputSolution()
        {
            if (solutionFound)
                outputStream.WriteLine();
            solutionFound = true;
            for (var i = 0; i < crossword.CountOfRows; i++)
            {
                for (var j = 0; j < crossword.CountOfColumns; j++)
                    outputStream.Write(crossword.CrosswordField[i, j] == 1 ? "*" : crossword.CrosswordField[i, j] == 0 ? "." : "?");
                outputStream.WriteLine();
            }
        }

        private void IterateLineAnalize()
        {
            do
            {
                for (var i = 0; i < crossword.CountOfRows + crossword.CountOfColumns; i++)
                    if (crossword.Lines[i].NeedRefresh)
                        AnalizeLine(crossword.Lines[i]);
            }
            while (crossword.Lines.Any(x => x.NeedRefresh));
        }

        private void AnalizeLine(Line line)
        {
            line.NeedRefresh = false;
            var lengthOfSelectedLine = line.IsHorizontal ? crossword.CountOfColumns : crossword.CountOfRows;
            var canOne = new List<bool> { false, false };
            var canZero = new List<bool> { false, false };
            var cells = new List<int> { 1, 0 };
            var blocks = new List<int>();
            var resultsTable = new List<List<int>>();
            if (line.IsHorizontal)
                for (var i = 1; i <= lengthOfSelectedLine; i++)
                {
                    cells.Add(crossword.CrosswordField[line.Number, i - 1]);
                    canOne.Add(false);
                    canZero.Add(false);
                }
            else
            {
                for (var i = 1; i <= lengthOfSelectedLine; i++)
                {
                    cells.Add(crossword.CrosswordField[i - 1, line.Number]);
                    canOne.Add(false);
                    canZero.Add(false);
                }
            }
            var blocksIntoLine = line.CountOfBlocks;
            blocks.Add(1);
            for (var i = 1; i <= blocksIntoLine; i++)
                blocks.Add(line.BlocksLength[i - 1]);
            for (var i = 0; i < blocksIntoLine; i++)
            {
                resultsTable.Add(new List<int>());
                for (var j = 0; j < lengthOfSelectedLine; j++)
                    resultsTable[i].Add(-1);
            }
            if (TryBlock(0, -1, lengthOfSelectedLine, blocksIntoLine, blocks, ref cells, ref resultsTable, ref canOne,
                ref canZero))
            {
                for (var i = 2; i <= lengthOfSelectedLine + 1; i++)
                {
                    if (cells[i] == 2 && (canOne[i] ^ canZero[i]))
                    {
                        var lineForRefresh =
                            crossword.Lines.Find(
                                otherLine => otherLine.Number == i - 2 && otherLine.IsHorizontal != line.IsHorizontal);
                        var indexOfLineForRefresh = crossword.Lines.IndexOf(lineForRefresh);
                        crossword.Lines[indexOfLineForRefresh].NeedRefresh = true;
                        cells[i] = canOne[i] ? 1 : 0;
                        if (line.IsHorizontal)
                            crossword.CrosswordField[line.Number, i - 2] = cells[i];
                        else
                            crossword.CrosswordField[i - 2, line.Number] = cells[i];
                    }
                }
            }
            else
                crossword.ErrorLevel = 1;
        }

        private bool TryBlock(int theBlock, int theStart, int countOfSelectedLine, int n, List<int> blocks, ref List<int> cells, ref List<List<int>> resultsTable, ref List<bool> canOne, ref List<bool> canZero)
        {
            bool res;
            if (theBlock > 0 && resultsTable[theBlock - 1][theStart - 1] != -1)
                return resultsTable[theBlock - 1][theStart - 1] == 1;
            for (var i = theStart; i <= theStart + blocks[theBlock] - 1; i++)
            {
                if (cells[i + 1] == 0)
                {
                    resultsTable[theBlock - 1][theStart - 1] = 0;
                    return false;
                }
            }
            if (theBlock < n)
            {
                res = false;
                for (var startNext = theStart + blocks[theBlock] + 1;
                    startNext <= countOfSelectedLine - blocks[theBlock + 1] + 1;
                    ++startNext)
                {
                    if (cells[startNext] == 1)
                        break;
                    if (TryBlock(theBlock + 1, startNext, countOfSelectedLine, n, blocks, ref cells,
                        ref resultsTable, ref canOne, ref canZero))
                    {
                        res = true;
                        for (var i = theStart; i < theStart + blocks[theBlock]; i++)
                            canOne[i + 1] = true;
                        for (var i = theStart + blocks[theBlock]; i < startNext; i++)
                            canZero[i + 1] = true;
                    }
                }
                return res;
            }
            for (var i = theStart + blocks[theBlock]; i <= countOfSelectedLine; i++)
                if (cells[i + 1] == 1)
                    return false;
            for (var i = theStart; i < theStart + blocks[theBlock]; i++)
                canOne[i + 1] = true;
            for (var i = theStart + blocks[theBlock]; i <= countOfSelectedLine; i++)
                canZero[i + 1] = true;
            return true;
        }
    }
}
