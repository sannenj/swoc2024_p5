using PlayerInterface;
using System.Diagnostics.CodeAnalysis;

namespace TestClient
{
    internal partial class Program
    {
        public class Snake
        {
            public string Name { get; set; }
            public int Length { get; set; }
            public List<Coordinate> Segments { get; set; }
            public Coordinate Head { get; set; }

            private int _kidCount;
            public string NextKidName
            {
                get
                {
                    return $"{Name}_{_kidCount++}";
                }
            }

            public Snake(string name, List<Coordinate> segments)
            {
                Name = name;
                Length = segments.Count;
                Head = segments.Last();
                Segments = segments;
            }
        }
    }
}