namespace CS5410
{
    public class Cell
    {
        public int posX;
        public int posY;
        public Cell topNeighbor;
        public Cell bottomNeighbor;
        public Cell leftNeighbor;
        public Cell rightNeighbor;
        public bool visited;
        public Cell prior;

        public Cell(int posX, int posY)
        {
            visited = false;
            this.posX = posX;
            this.posY = posY;
        }

        public override string ToString()
        {
            return "Cell: " + posX + ", " + posY;
        }
    }
}