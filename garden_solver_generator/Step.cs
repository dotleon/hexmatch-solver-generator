using System.Collections.Generic;
using System.Linq;

namespace garden_solver_generator
{
    class Step
    {
        public readonly List<List<Marble>> MarblePairs;
        public List<Marble> LastPaired;

        public Step(Solver solver)
        {
            MarblePairs = new List<List<Marble>>();
            CheckForPairs(solver);
        }



        public void CheckForPairs(Solver solver)
        {
            // if there is active gold
            if (solver.MarbleTypeCount[11] != 0)
            {
                var gold = solver.ActiveMarbles.SingleOrDefault(m => m.Type == 11);
                if (gold != null)
                {
                    MarblePairs.Add(new List<Marble> { gold });
                    return;
                }
                    
            }

            var oddelements = solver.MarbleTypeCount[1] % 2 +
                            solver.MarbleTypeCount[2] % 2 +
                            solver.MarbleTypeCount[3] % 2 +
                            solver.MarbleTypeCount[4] % 2;

            
            for (var i = 0; i < solver.ActiveMarbles.Count; i++)
            {
                //I'm a failure in life :(
                if (solver.ActiveMarbles[i].Type == 14)
                {
                    var type1Marbles = solver.ActiveMarbles.Where(m => m.Type == 1).ToList();
                    var type2Marbles = solver.ActiveMarbles.Where(m => m.Type == 2).ToList();
                    var type3Marbles = solver.ActiveMarbles.Where(m => m.Type == 3).ToList();
                    var type4Marbles = solver.ActiveMarbles.Where(m => m.Type == 4).ToList();
                    if (type1Marbles.Count > 0 
                        && type2Marbles.Count > 0 
                        && type3Marbles.Count > 0 
                        && type4Marbles.Count > 0)
                    {
                        foreach (var m1 in type1Marbles)
                        {
                            foreach (var m2 in type2Marbles)
                            {
                                foreach (var m3 in type3Marbles)
                                {
                                    foreach (var m4 in type4Marbles)
                                    {
                                        MarblePairs.Add(new List<Marble> { solver.ActiveMarbles[i], m1, m2, m3, m4 });
                                    }
                                }
                            }
                        }
                    }
                }
                else for (var j = i + 1; j < solver.ActiveMarbles.Count; j++)
                {
                    var m1 = solver.ActiveMarbles[i];
                    var m2 = solver.ActiveMarbles[j];
                    if (m1.CanBePairedWith(m2))
                    {
                        //don't even add as a pair if there are as much odd elements as salt, and it't trying to pair with an even element
                        //if one is salt and other is not
                        //and there are as much odd elements as salt
                        //and the paired element is not odd
                        //then continue
                        if ((m1.Type == 0 && m2.Type != 0
                             || m1.Type != 0 && m2.Type == 0)
                            && oddelements == solver.MarbleTypeCount[0]
                            && !(m1.Type == 0 && solver.MarbleTypeCount[m2.Type] % 2 == 1)
                            && !(m2.Type == 0 && solver.MarbleTypeCount[m1.Type] % 2 == 1)
                            )
                            continue;
                        MarblePairs.Add(new List<Marble> {m1, m2});
                    }
                }
            }
        }

        //true if there are still pairs, false if no pairs left
        public bool PairNextPair(Solver solver)
        {
            //no pairs left
            if (MarblePairs.Count == 0)
            {
                //restore last pair if there was one before getting deleted
                RevertLast(solver);
                return false;
            }

            //restore last pair if there was one before pairing next one
            RevertLast(solver);

            Pair(solver, MarblePairs[0]);
            LastPaired = MarblePairs[0];
            MarblePairs.RemoveAt(0);

            return true;
        }
        
        public void RevertLast(Solver solver)
        {
            if (LastPaired != null)
                Restore(solver, LastPaired);
        }
        
        public void Pair(Solver solver, List<Marble> marbles)
        {
            foreach (var marble in marbles)
            {
                //delete marble
                solver.Marbles[marble.X * 13 + marble.Y] = null;
                solver.MarbleTypeCount[marble.Type]--;

                //increase metallevel if quicksilver, also try to activate next metal
                if (marble.Type == 5)
                {
                    solver.MetalLevel++;
                }

                solver.MarbleCount--;

                solver.UpdateAll();
            }
        }

        public void Restore(Solver solver, List<Marble> marbles)
        {
            foreach (var marble in marbles)
            {
                //delete and deactivate
                solver.Marbles[marble.X * 13 + marble.Y] = marble;
                solver.MarbleTypeCount[marble.Type]++;

                //increase metallevel if quicksilver, also try to activate next metal
                if (marble.Type == 5)
                {
                    solver.MetalLevel--;
                }

                solver.MarbleCount++;

                solver.UpdateAll();
            }
        }
    }
}
