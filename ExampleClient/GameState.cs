using PlayerInterface;
using static TestClient.Program;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace TestClient
{
    internal partial class Program
    {
        public class GameState
        {
            private Map _map;
            private List<Snake> _snakes;
            private List<Coordinate> _food;
            private List<Coordinate> _others;
            private Coordinate _dimensions;
            private string _playerName;
            private string _playerIdentifier;
            private Coordinate _startAddress;

            public GameState(int[] dimensions, int[] startAddress, string playerName, string playerIdentifier)
            {
                _map = new Map(dimensions);
                _startAddress = new Coordinate(startAddress);
                Console.WriteLine($"start address: {_startAddress.ToString()}");
                _snakes = new List<Snake>
                {
                    new Snake(playerName, new List<Coordinate> { _startAddress })
                };
                _dimensions = new Coordinate(dimensions);
                Coordinate.setLimits(dimensions);
                _playerIdentifier = playerIdentifier;
                _playerName = playerName;

                _food = new List<Coordinate>();
                _others = new List<Coordinate>();
            }

            public void setStartState(UpdatedCell[] startCells)
            {
                foreach (var cell in startCells)
                {
                    var coord = new Coordinate(cell.Address.ToArray());
                    var cell2adapt = _map.GetCell(coord);
                    if (cell.FoodValue > 0)
                    {
                        _food.Add(coord);
                        cell2adapt.HasFood = true;
                    }
                    if (cell.Player != String.Empty)
                    {
                        _others.Add(coord);
                        cell2adapt.HasPlayer = true;
                    }
                }
            }

            private List<Snake> _enemySnakes = new List<Snake>();

            public void UpdateEnemySnakes(List<UpdatedCell> cells)
            {
                foreach (UpdatedCell update in cells)
                {
                    if (update.Player == _playerName)
                        continue;
                    var addr = new Coordinate(update.Address.ToArray());
                    bool updateHasPlayer = !string.IsNullOrWhiteSpace(update.Player);
                    if (updateHasPlayer)
                    {
                        var playerSnakes = _enemySnakes.FindAll(x => x.Name == update.Player);
                        if (playerSnakes.Count > 0)
                        {
                            foreach (var snk in playerSnakes)
                            {
                                if (addr.cathesianDistance(snk.Head) == 1)
                                {
                                    snk.Segments.Add(addr);
                                    snk.Head = addr;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            List<Coordinate> coordList = new List<Coordinate>();
                            coordList.Add(addr);
                            _enemySnakes.Add(new Snake(update.Player, coordList));
                            //Console.WriteLine($"New snake from {update.Player}");
                        }
                    }
                    else
                    {
                        // need to remove the tail from a snake
                        var playerSnakes = _enemySnakes.FindAll(x => x.Name == update.Player);
                        foreach (var snk in playerSnakes)
                        {
                            var item = snk.Segments.First();
                            if (addr.cathesianDistance(item) == 1)
                            {
                                snk.Segments.RemoveAt(0);
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine($"now {_enemySnakes.Count} enemy snakes.");
                //foreach(var snake in _enemySnakes)
                //{
                //    Console.WriteLine($"snake from {snake.Name} has length {snake.Segments.Count}");
                //}
            }


            public void UpdateGameState(List<UpdatedCell> cells)
            {
                UpdateMap(cells);
                UpdateEnemySnakes(cells);
            }
            public void UpdateMap(List<UpdatedCell> cells)
            {
                //                Console.WriteLine($"need to process {cells.Count} items");
                foreach (UpdatedCell update in cells)
                {
                    var addr = new Coordinate(update.Address.ToArray());
                    var cell = _map.GetCell(addr);

                    bool updateHasPlayer = !string.IsNullOrWhiteSpace(update.Player);
                    cell.HasPlayer = updateHasPlayer;
                    if (updateHasPlayer)
                    {
                        _others.Add(addr);
                    }
                    else
                    {
                        var item = _others.Find(x => x == addr);
                        _others.Remove(item);
                    }

                    cell.HasFood = update.FoodValue > 0;
                    if (update.FoodValue == 0)
                    {
                        var item = _others.Find(x => x == addr);
                        _food.Remove(item);
                    }
                }
                Console.WriteLine($"after update: {_food.Count} food and {_others.Count} snake elements left");
            }

            public List<SplitRequest> GetSplits()
            {
                var list = new List<SplitRequest>();
                var newSnakes = new List<Snake>();
                var snake = _snakes.First();
                if (snake.Length > 2 && _snakes.Count < 11)
                {
                    snake.Length -= 1;
                    var newSnakeName = snake.NextKidName;
                    Console.WriteLine($"Split!, new snake={newSnakeName}");
                    var newHead = snake.Segments[0];
                    snake.Segments.RemoveAt(0);
                    var newSnake = new Snake(newSnakeName, new List<Coordinate> { newHead });
                    newSnakes.Add(newSnake);

                    var address = GetNextAddress(newHead);
                    newSnake.Head = address;
                    var cell = _map.GetCell(address);
                    newSnake.Segments.Add(address);
                    var split = new SplitRequest
                    {
                        SnakeSegment = 1,
                        PlayerIdentifier = _playerIdentifier,
                        NewSnakeName = newSnakeName,
                        OldSnakeName = snake.Name
                    };
                    split.NextLocation.AddRange(address.raw);
                    list.Add(split);
                }
            _snakes.AddRange(newSnakes);
            return list;
            }

            public List<Move> GetMoves()
            {
                var moves = new List<Move>();
                foreach (var snake in _snakes)
                {
                    var nextLocation = GetNextTowardsFoodAddress(snake.Head);
                    if (nextLocation == Coordinate.CannotNotMove)
                        continue;
                    Move move = HandleSnake(snake, nextLocation);
                    moves.Add(move);
                }
                return moves;
            }

            private Move HandleSnake(Snake snake, Coordinate nextLocation)
            {
                var cell = _map.GetCell(nextLocation);
                snake.Segments.Add(nextLocation);
                snake.Head = nextLocation;
                if (cell.HasFood)
                {
                    snake.Length += 1;
                    Console.WriteLine($"    HAP !!!  Length={snake.Segments.Count}");
                }
                else
                {
                    snake.Segments.RemoveAt(0);
                }
                var move = new Move();
                move.PlayerIdentifier = _playerIdentifier;
                move.SnakeName = snake.Name;
                move.NextLocation.AddRange(nextLocation.raw);
                return move;
            }

            public bool IsNotMe(Coordinate newAddress)
            {
                foreach (Snake snake in _snakes)
                {
                    foreach (Coordinate c in snake.Segments)
                    {
                        if (c == newAddress) return false;
                    }
                }
                return true;
            }

            public Coordinate GetNextTowardsFoodAddress(Coordinate head)
            {
                List<Coordinate> sortedFoods = _food.OrderBy(x => x.cathesianDistance(head)).ToList();

                while (sortedFoods.Count > 0)
                {
                    var closestFood = sortedFoods.First();
                    var closestFoodDistance = closestFood.distancePerCoord(head);
                    for (int i = 0; i < head.Length; i++)
                    {
                        Coordinate newAddress = new Coordinate(head.Length);
                        Array.Copy(head.raw, newAddress.raw, head.Length);
                        if (closestFoodDistance[i] > 0)
                        {
                            newAddress[i]++;
                            if (_map.IsSafe(newAddress) && IsNotMe(newAddress))
                            {
                                Console.WriteLine($"Head: {head.ToString()}, food: {closestFood.ToString()}, clfdist: {closestFoodDistance.ToString()} = {closestFood.cathesianDistance(head)}, next: {newAddress.ToString()}");
                                return newAddress;
                            }
                        }
                        if (closestFoodDistance[i] < 0)
                        {
                            newAddress[i]--;
                            if (_map.IsSafe(newAddress) && IsNotMe(newAddress))
                            {
                                Console.WriteLine($"Head: {head.ToString()}, food: {closestFood.ToString()}, clfdist: {closestFoodDistance.ToString()} = {closestFood.cathesianDistance(head)}, next: {newAddress.ToString()}");
                                return newAddress;
                            }
                        }
                    }
                    Console.WriteLine($"Head: {head.ToString()}, food: {closestFood.ToString()}, clfdist: {closestFoodDistance.ToString()} = {closestFood.cathesianDistance(head)}, next: NO MOVE");
                    sortedFoods.RemoveAt(0);
                }

                Console.WriteLine("Truly NO MOVE");
                return Coordinate.CannotNotMove;
            }

            public Coordinate GetNextAddress(Coordinate address)
            {
                var rand = new Random();
                while (true)
                {

                    Coordinate newAddress = new Coordinate(address.Length);
                    Array.Copy(address.raw, newAddress.raw, address.Length);
                    var dim = rand.Next(address.Length);
                    var dir = rand.Next(2) == 1;
                    if (dir)
                    {
                        if ((newAddress[dim] + 1) != _dimensions[dim])
                        {
                            newAddress[dim]++;
                            if (_map.IsSafe(newAddress) && IsNotMe(newAddress))
                            {
                                return newAddress;
                            }
                        }
                    }
                    else
                    {
                        if (newAddress[dim] != 0)
                        {
                            newAddress[dim]--;
                            if (_map.IsSafe(newAddress))
                            {
                                return newAddress;
                            }
                        }
                    }
                }
            }
        }
    }
}