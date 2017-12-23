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
            const bool quint = false;
            const int amountOfMapsGenerated = 10000;
            const int mapPatternCount = 2000; //if it's too high, it can get into an infinite loop
            
            //FileCombiner.CombineFiles(amountOfMapsGenerated);

            var random = new Random();
            
            //some map patterns has a much higher change to be generated, so it pregenerates 2000 unique map patterns to randomize from them
            //sadly, there are still repetitions, like rotations of 2 or 3 sided patterns, and mirrors.
            var mapPatternList = MapGenerator.GenerateMapPatternsList(random, hard, mapPatternCount);

            //look at pretty map patters (check if it works correctly)
            /*while (true)
            {
                MapGenerator.DrawMap(mapPatternList[random.Next(0,mapPatternCount)]);
                Console.ReadKey();
            }*/

            int[] maxMarbleCountPerType = quint ? new[] { 4, 8, 8, 8, 8, 5, 1, 1, 1, 1, 1, 1, 3, 3, 2 } 
                                               : new [] { 4, 8, 8, 8, 8, 5, 1, 1, 1, 1, 1, 1, 4, 4};
            

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int generatedMapsCount = 0;

            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(amountOfMapsGenerated + (hard ? "hard" : "easy") + (quint ? "quint" : "")+".dat")))
            {
                var generatedSolvableMapCount = 0;
                var patternRotations = 1;
                while (generatedSolvableMapCount < amountOfMapsGenerated)
                {
                    var marbles = MapGenerator.GenerateRandomMap(random,
                        maxMarbleCountPerType,
                        mapPatternList[random.Next((patternRotations - 1) * (mapPatternCount / 3),patternRotations * (mapPatternCount / 3))],
                        quint, out var mapByteArray);
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

                        if (generatedSolvableMapCount == amountOfMapsGenerated / 3
                            || generatedSolvableMapCount == amountOfMapsGenerated / 3 * 2)
                        {
                            patternRotations++;
                            Console.WriteLine("Switching pattern rotations.");
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
