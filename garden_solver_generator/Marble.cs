namespace garden_solver_generator
{
    class Marble
    {
        public readonly int Type;
        public readonly int X;
        public readonly int Y;

        public Marble(int type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }

        public void UpdateActive(Solver solver)
        {
            if (Type > 5 && Type < 12 && Type != solver.MetalLevel)
                return;
            var count = 0;
            for (var i = 0; i < 8; i++)
            {
                //going around
                var x = HexMath.XInDirection(X, Y, i % 6);
                var y = HexMath.YInDirection(Y, i % 6);

                if (solver.Marbles[x * 13 + y] != null)
                {
                    count = 0;
                }
                else
                {
                    count++;
                }
                if (count == 3)
                {
                    solver.ActiveMarbles.Add(this);
                    return;
                }
            }
        }

        public void UpdateNeighbours(Solver solver)
        {
            for (var i = 0; i < 6; i++)
            {
                solver.Marbles[HexMath.XInDirection(X, Y, i % 6) * 13 + HexMath.YInDirection(Y, i % 6)]?
                    .UpdateActive(solver);
            }
        }

        public bool CanBePairedWith(Marble marble)
        {
            if (Type < 5 || marble.Type < 5)
            {
                return Type < 5 && marble.Type < 5 &&
                       (Type == marble.Type || Type == 0 || marble.Type == 0);
            }
            if (Type == 5 || marble.Type == 5)
            {
                return Type == 5 && marble.Type > 5 && marble.Type < 11
                       || marble.Type == 5 && Type > 5 && Type < 11;
            }
            
            return Type == 12 && marble.Type == 13
                   || marble.Type == 12 && Type == 13;
        }
    }
}
