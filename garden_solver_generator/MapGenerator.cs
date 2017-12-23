using System;
using System.Collections.Generic;
using System.Linq;

namespace garden_solver_generator
{
    static class MapGenerator
    {
        //fill map pattern with random hexes, also create a compact saveable version
        //if it's always written and read in the same sequence, then hex types do not have to be saved
        //as x and y position has a maximum value of 12, it fits into half byte, so one hex only takes one byte
        public static Marble[] GenerateRandomMap(Random random, int[] maxMarbleCountPerType, int[] mapPattern, bool quint,  out byte[] saveableMap)
        {
            saveableMap = new byte[54];
            var marbleCountPerType = new int[quint ? 15 : 14];
            var marbles = new Marble[169];
            var marbleCount = 0;

            var mapPoints = mapPattern
                .Select((value, index) => new { value, index })
                .Where(x => x.value == 1)
                .Select(x => x.index)
                .ToList();

            marbles[6 * 13 + 6] = new Marble(11, 6, 6);
            marbleCountPerType[11]++;

            //for each marble type
            for (var i = 0; i < (quint ? 15 : 14); i++)
            {
                //until enough marbles are put by type
                while (maxMarbleCountPerType[i] != marbleCountPerType[i])
                {
                    //put into random map point
                    var mapPoint = mapPoints[random.Next(0, mapPoints.Count)];
                    mapPoints.Remove(mapPoint);
                    var x = mapPoint / 13;
                    var y = mapPoint % 13;

                    marbles[x * 13 + y] = new Marble(i,x,y);
                    marbleCountPerType[i]++;
                    saveableMap[marbleCount] = byte.Parse(((x << 4) + y).ToString());
                    marbleCount++;
                }
            }

            return marbles;
        }

        //generate unique map patterns
        public static List<int[]> GenerateMapPatternsList(Random random, bool hard, int patternCount)
        {
            var mapPatternList = new List<int[]>();
            int[] mapPattern;
            int rotations = 1;
            while (mapPatternList.Count < patternCount)
            {
                int activeCount;
                do
                {
                    mapPattern = GenerateRandomMapPattern(random, rotations);
                    activeCount = CountActive(mapPattern);
                } while (activeCount > (hard ? 9 : 20) || activeCount < (hard ? 3 : 11));
                
                if (!mapPatternList.Any(m => m.SequenceEqual(mapPattern))
                    || hard && rotations == 1) //there are only around 35 hard mode 6 sided patterns that exist...
                {
                    mapPatternList.Add(mapPattern);
                }
                if ( patternCount / 3 == mapPatternList.Count
                    || hard && mapPatternList.Count == (int) (patternCount * 0.05)) //too many repetitions of 6 sided on hard mode
                {
                    if (rotations != 2)
                    {
                        rotations = 2;
                        Console.WriteLine("6 sided patterns generated.");
                    }
                }
                if ((hard ? patternCount / 2 : patternCount / 3 * 2) == mapPatternList.Count)
                {
                    rotations = 3;
                    Console.WriteLine("3 sided patterns generated.");
                }
            }
            Console.WriteLine("2 sided patterns generated.\n" + patternCount + " map patterns generated.");
            return mapPatternList;
        }

        //creates pretty patterns by mirroring spreading holes in 2, 3 or 6 directions
        public static int[] GenerateRandomMapPattern(Random random, int rotations)
        {
            //default game map
            int[] generatedMap = {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0,
                0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                0, 1, 1, 1, 1, 1, 8, 1, 1, 1, 1, 1, 0,
                0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0,
                0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
            var marbleCount = 90;

            //how many points to start spreading holes from
            var numberOfPoints = random.Next(0 + rotations, 3 + rotations);

            var points = new List<Point>();

            for (var i = 0; i < numberOfPoints; i++)
            {
                var p = new Point(0,0);
                while (generatedMap[p.X * 13 + p.Y] != 1)
                {
                    p.X = random.Next(1, 13);
                    p.Y = random.Next(1, 13);
                }
                DoRotations(generatedMap, rotations, p);
                marbleCount -= 6 / rotations;
                points.Add(p);
                //DrawMap(generatedMap);
            }


            var fullbreak = false;
            var iteration = 0;
            while (!fullbreak)
            {
                foreach (var p in points)
                {
                    var direction = random.Next(0, 6);
                    var x = HexMath.XInDirection(p.X, p.Y, direction);
                    var y = HexMath.YInDirection(p.Y, direction);
                    if (x > 0 && x < 13 && y > 0 && y < 13 
                        && generatedMap[x * 13 + y] == 1)
                        {
                            p.X = x;
                            p.Y = y;
                            DoRotations(generatedMap, rotations, p);
                            marbleCount -= 6 / rotations;
                            //DrawMap(generatedMap);
                            if (marbleCount < 55)
                            {
                                fullbreak = true;
                                break;
                            }
                        }

                    //if it goes for too long, it's stuck so rerandomize points
                    iteration++;
                    if (iteration == 100)
                    {
                        iteration = 0;
                        foreach (var p2 in points)
                        {
                            p2.X = random.Next(1, 13);
                            p2.Y = random.Next(1, 13);
                        }
                    }
                }
            }

            //Console.WriteLine(generatedMap.Where(m => m == 1).Count() + "-" + marbleCount);
            
            //DrawMap(generatedMap);

            return generatedMap;

        }

        //draws the map into the console
        public static void DrawMap(int[] map)
        {
            for (var y = 0; y < 13; y++)
            {
                if (y % 2 == 1)
                    Console.Write(" ");
                for (var x = 0; x < 13; x++)
                {
                    Console.Write(map[x * 13 + y] > 0 ? map[x * 13 + y] + " " : "  ");
                    //Console.Write(map[y * 13 + x]);
                }
                Console.WriteLine();
            }
        }
        
        //rotates "rotations" times and removes a position, does this to all sides, so eventually it becomes a pretty pattern
        public static void DoRotations(int[] map, int rotations, Point p)
        {
            for (var i = 0; i < 6 / rotations; i++)
            {
                for(var j = 0; j < rotations; j++)
                    Rotate(p);
                map[p.X * 13 + p.Y] = 0;
            }
        }

        //black magic, rotates 60 degrees clockwise from center
        public static void Rotate(Point p)
        {
            var cubePx = 9 - p.Y;
            var cubePy = p.X - (p.Y - (p.Y & 1)) / 2 + p.Y - 3;
            
            p.X = cubePx + (cubePy - (cubePy & 1)) / 2;
            p.Y = cubePy;
        }

        //counts potentially active marble positions
        public static int CountActive(int[] map)
        {
            var actives = 0;

            for (var x = 0; x < 13; x++)
            {
                for (var y = 0; y < 13; y++)
                {
                    if (map[x * 13 + y] != 1)
                        continue;

                    var count = 0;

                    for (var i = 0; i < 8; i++)
                    {
                        //going around
                        var x2 = HexMath.XInDirection(x, y, i % 6);
                        var y2 = HexMath.YInDirection(y, i % 6);

                        if (map[x2 * 13 + y2] != 0)
                        {
                            count = 0;
                        }
                        else
                        {
                            count++;
                        }
                        if (count == 3)
                        {
                            actives++;
                            break;
                        }
                    }
                }
            }
            return actives;
        }
    }
}
