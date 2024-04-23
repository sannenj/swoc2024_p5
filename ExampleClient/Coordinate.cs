namespace TestClient
{
    public class Coordinate
    {
        public Coordinate(int size)
        {
            _coords = new int[size];
        }

        public Coordinate(int[] coords)
        {
            _coords = coords;
        }

        static public void setLimits(int[] dimensions)
        {
            _dimensions = dimensions;
        }

        public int[] raw {  get { return _coords; } set { _coords = value; } }

        public int Length { get { return _coords.Length; } }

        public static Coordinate CannotNotMove {  get { return new Coordinate(_dimensions); } }

        public override string ToString()
        {
            return String.Join(",", _coords);
        }

        public static bool operator ==(Coordinate left, Coordinate right)
        {
            if (left._coords.Length != right._coords.Length)
                return false;
            for(int i=0; i< left._coords.Length; i++)
            {
                if (left[i] != right[i])
                    return false;
            }
            return true;
        }
        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !(left==right);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj is Coordinate)
                return this == (Coordinate)obj;
            return false;
        }

        public int this[int index] { get { return _coords[index]; } set { _coords[index] = value; } }

        public bool IsSane { get 
            { 
                if (_coords.Length == 0)    return false;
                if (_dimensions == null) return false;
                for(int i=0; i< _dimensions.Length; i++)
                {
                    if (_coords[i] < 0 || _coords[i] >= _dimensions[i])
                        return false;
                }
                return true;
            } 
        }

        public int cathesianDistance(Coordinate to)
        {
            int dist = 0;
            Coordinate allDists = distancePerCoord(to);
            int dimensionCount = allDists.Length;
            for (int i = 0; i < dimensionCount; i++)
            {
                dist += Math.Abs(allDists[i]);
            }
            return dist;
        }

        public Coordinate distancePerCoord(Coordinate to)
        {
            Coordinate dist = new Coordinate(to.Length);
            to._coords.CopyTo(dist.raw, 0);
            int dimensionCount = to.Length;
            for (int i = 0; i < dimensionCount; i++)
            {
                dist.raw[i] = _coords[i] - to[i];
            }
            return dist;
        }

        private int[] _coords;
        static private int[]? _dimensions;
    }
}
