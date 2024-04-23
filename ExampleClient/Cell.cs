namespace TestClient
{
    public class Cell
    {
        public bool HasPlayer { get; set; }
        public bool HasFood { get; set; }

        public Cell()
        {
            HasFood= false;
            HasPlayer= false;
        }
    }
}