namespace General
{
    public class PathNode
    {
        private Grid<PathNode> _grid;
        public int x;
        public int y;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool isWalkable;

        public PathNode PreviousNode;

        public PathNode(Grid<PathNode> grid, int x, int y)
        {
            _grid = grid;
            this.x = x;
            this.y = y;
            isWalkable = true;
        }

        public override string ToString()
        {
            return x + "," + y;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}