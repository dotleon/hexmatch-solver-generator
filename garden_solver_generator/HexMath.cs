namespace garden_solver_generator
{
    static class HexMath
    {
        public static int XInDirection(int x, int y, int direction){
            switch(direction) {
                case 0:
                case 2: {
                    return x + y % 2;
                }
                case 1: {
                    return x + 1;
                }
                case 3:
                case 5: {
                    return x - 1 + y % 2;
                }
                case 4: {
                    return x - 1;
                }
                default: {
                    return x;
                }
            }
        }

        public static int YInDirection(int y, int direction){
            switch(direction) {
                case 0:
                case 5: {
                    return y - 1;
                }
                case 1:
                case 4: {
                    return y;
                }
                case 2:
                case 3: { 
                    return y + 1;
                }
                default: {
                    return y;
                }
            }
        }
    }
}
