using PlayerInterface;
using static TestClient.Program;

namespace TestClient
{
    public class Map
    {
        public Map(int[] dimensions)
        {
            _map = Array.CreateInstance(typeof(Cell), dimensions);
            _map.Initialize();
            _dimensions = dimensions;
        }

        public void updateCells(UpdatedCell[] cellList)
        {
            foreach (var cell in cellList)
            {
                var coord = new Coordinate(cell.Address.ToArray());
                var cell2adapt = GetCell(coord);
                cell2adapt.HasPlayer = !string.IsNullOrWhiteSpace(cell.Player);
                cell2adapt.HasFood = (cell.FoodValue > 0);
            }
        }

        public bool IsSafe(Coordinate addr)
        {
            return !GetCell(addr).HasPlayer;
        }

        public Cell GetCell(Coordinate addr)
        {
            var cell = _map.GetValue(addr.raw) as Cell;
            if (cell == null)
            {
                cell = new Cell();
                _map.SetValue(cell, addr.raw);
            }
            return cell;
        }

        public Cell GetCell(int[] addr)
        {
            var cell = _map.GetValue(addr) as Cell;
            if (cell == null)
            {
                cell = new Cell();
                _map.SetValue(cell, addr);
            }
            return cell;
        }

        public bool HasPlayer(Coordinate addr)
        {
            return GetCell(addr).HasPlayer;
        }

        public bool HasFood(Coordinate addr)
        {
            return GetCell(addr).HasFood;
        }

        //public void Print()
        //{
        //    if (_dimensions.Length > 2)
        //        throw new Exception();
        //    for(int i=0; i< _dimensions[0]; i++)
        //    {
        //        string line = "";
        //        for (int j = 0; j < _dimensions[1]; j++)
        //        {
        //            var c = new int[i, j];
        //            var coord = new Coordinate(c);
        //            if (HasPlayer(coord)) { line += "%"; }
        //            else
        //            {
        //                if (HasFood(coord)) { line += "#"; }
        //                else { line += " "; }
        //            }
        //        }
        //        Console.WriteLine(line);
        //    }
        //}

        private Array _map;
        private int[] _dimensions;

    }
}
