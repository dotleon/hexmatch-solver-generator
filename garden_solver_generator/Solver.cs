using System.Collections.Generic;
using System.Linq;

namespace garden_solver_generator
{
    class Solver
    {
        public readonly Marble[] Marbles;
        public List<Marble> ActiveMarbles;
        public int MetalLevel;
        public int MarbleCount;
        public readonly int[] MarbleTypeCount;

        public Solver(Marble[] marbles, bool quint)
        {
            Marbles = marbles;
            MetalLevel = 6;
            MarbleCount = 55;
            MarbleTypeCount = quint ? new[] {4, 8, 8, 8, 8, 5, 1, 1, 1, 1, 1, 1, 3, 3, 2} :
                new[] { 4, 8, 8, 8, 8, 5, 1, 1, 1, 1, 1, 1, 4, 4, 0 };
        }

        public bool IsSolvable()
        {
            var iteration = 0;
            var steps = new List<Step>();
            ActiveMarbles = new List<Marble>();

            foreach (var marble in Marbles.Where(m => m != null))
            {
                marble.UpdateActive(this);
            }

            steps.Add(new Step(this));

            var gameStates = new List<string>();

            while (MarbleCount > 1)
            {

                if (steps.Count == 0)
                {
                    return false;
                }
                    

                if (steps.Last().PairNextPair(this))
                {
                    var gameState = StateFromMarbles();
                    if (gameStates.Contains(gameState))
                    {
                        steps.Last().RevertLast(this);
                        steps.RemoveAt(steps.Count - 1);
                    }
                    else
                    {
                        gameStates.Add(gameState);
                        steps.Add(new Step(this));
                    }
                    
                }
                else
                {
                    steps.RemoveAt(steps.Count - 1);
                }
                
                iteration++;
                
                //magnitudes faster with early stopping, I cannot decide whether this creates earier maps, but I don't think so
                if (iteration == 500)
                    return false;
            }
            return true;
        }

        public string StateFromMarbles()
        {
            var str = string.Empty;
            for (int i = 0; i < Marbles.Length; i++)
                str += Marbles[i] != null ? 1 : 0;
            return str;
        }

    }
}
