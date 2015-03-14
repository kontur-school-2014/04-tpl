namespace JapaneseCrossword
{
    class Program
    {
        static void Main(string[] args)
        {
            var solver = new CrosswordSolver();
            var status = solver.Solve(@"TestFiles\Car.txt", "output.txt");
        }
    }
}
