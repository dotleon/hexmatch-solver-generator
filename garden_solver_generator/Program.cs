using System;
using System.Diagnostics;
using System.IO;

namespace garden_solver_generator
{
    class Program
    {
        static void Main(string[] args)
        {
            const bool hard = false;
            const bool quint = true;
            const int amountOfMapsGenerated = 10000;
            
            //FileCombiner.CombineFiles();

            var random = new Random();

            /*
            //look at pretty map patters (check if it works correctly)
            while (true)
            {
                MapGenerator.DrawMap(MapGenerator.GenerateRandomMapPattern(random));
                Console.ReadKey();
            }*/
            
            
            //some map patterns has a much higher change to be generated, so it pregenerates 2000 unique map patterns to randomize from them
            //sadly, there are still repetitions, like rotations of 2 or 3 sided patterns, and mirrors.
            var mapPatternList = MapGenerator.GenerateMapPatternsList(random, hard);
            
            int[] maxMarbleCountPerType = quint ? new[] { 4, 8, 8, 8, 8, 5, 1, 1, 1, 1, 1, 1, 3, 3, 2 } 
                                               : new [] { 4, 8, 8, 8, 8, 5, 1, 1, 1, 1, 1, 1, 4, 4};
            

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int generatedMapsCount = 0;

            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(amountOfMapsGenerated + (hard ? "hard" : "easy") + (quint ? "quint" : "")+".dat")))
            {
                var generatedSolvableMapCount = 0;
                while (generatedSolvableMapCount < amountOfMapsGenerated)
                {
                    var marbles = MapGenerator.GenerateRandomMap(random, maxMarbleCountPerType, mapPatternList[random.Next(0,2000)], quint, out var mapByteArray);
                    generatedMapsCount++;
                    var solver = new Solver(marbles, quint);

                    if (solver.IsSolvable())
                    {
                        bw.Write(mapByteArray);
                        generatedSolvableMapCount++;
                        if (generatedSolvableMapCount % 100 == 0)
                        {
                            watch.Stop();
                            Console.WriteLine("maps created so far: " + generatedSolvableMapCount + ", 100 maps in: " + watch.ElapsedMilliseconds + "ms");
                            Console.WriteLine("Generated maps solvability rate: " + (double)generatedSolvableMapCount / (double)generatedMapsCount * 100 +"%");
                            bw.Flush();
                            watch.Restart();
                        }
                    }
                    //Console.WriteLine("solvable: " + solvable + ", time elapsed: " + watch.ElapsedMilliseconds + "ms, iterations: " + iteration );
                }
            }
          

            Console.WriteLine("DONE");
            Console.ReadKey();
        }
    }
}
