using TestClient;

namespace TestCoordinate
{
    [TestClass]
    public class TestCoordinate
    {
        [TestMethod]
        public void TestConstructorAndRaw()
        {
            // Arrange
            int[] dim = { 1, 1, 3 };
            var coord = new Coordinate(dim);

            Assert.AreEqual(String.Join(",", dim), String.Join(",", coord.raw));
        }

        [TestMethod]
        public void TestCoodinateDistances()
        {
            // Arrange
            int[] dim = { 1, 1, 3 };
            var food = new Coordinate(dim);
            int[] dim2 = { 1, 2, 2 };
            var snakeHead = new Coordinate(dim2);

            // Act 
            var dists = food.distancePerCoord(snakeHead);

            // Assert
            int[] expectedDists = { 0, -1, 1 };
            Assert.AreEqual(String.Join(",", expectedDists), String.Join(",", dists.raw));
        }

        [TestMethod]
        public void TestCoordinateCarthesianDistance()
        {
            // Arrange
            int[] dim = { 1, 1, 3 };
            var food = new Coordinate(dim);
            int[] dim2 = { 1, 2, 2 };
            var snakeHead = new Coordinate(dim2);

            // Act 
            int dist = food.cathesianDistance(snakeHead);

            // Assert
            Assert.AreEqual(2, dist);
        }

        [TestMethod]
        public void TestCoordinateOperatorBrackets()
        {
            // Arrange
            int[] dim = { 1, 2, 3, 4, 5 };
            var food = new Coordinate(dim);

            for (int i = 0; i < dim.Length; i++)
            {
                food[i]++;
            }

            // Assert
            int[] expected = { 2, 3, 4, 5, 6 };
            Assert.AreEqual(String.Join(",", expected), food.ToString());
        }

        [TestMethod]
        public void TestIsSane()
        {
            int[] dim = { 5, 5, 5, 5 };
            Coordinate.setLimits(dim);
            {
                int[] coor = { 1, 2, 3, 4 };
                var food = new Coordinate(coor);
                Assert.IsTrue(food.IsSane);
            }
            {
                int[] coor = { 0, 2, 0, 4 };
                var food = new Coordinate(coor);
                Assert.IsTrue(food.IsSane);
            }
            {
                int[] coor = { 1, -2, 3, 4 };
                var food = new Coordinate(coor);
                Assert.IsFalse(food.IsSane);
            }
            {
                int[] coor = { 1, 2, 5, 4 };
                var food = new Coordinate(coor);
                Assert.IsFalse(food.IsSane);
            }
        }

        [TestMethod]
        public void TestOperatorEquals()
        {
            int[] b = { 5, 5, 5, 5 };
            var bC = new Coordinate(b);
            {
                int[] c = { 5, 5, 5, 5 };
                var cC = new Coordinate(c);
                Assert.IsTrue(bC == cC);
                Assert.IsFalse(bC != cC);
            }
        }
        [TestMethod]
        public void TestOperatorNotEquals()
        {
            int[] b = { 5, 5, 5, 5 };
            var bC = new Coordinate(b);
            {
                int[] c = { 1, 2, 3, 4 };
                var cC = new Coordinate(c);
                Assert.IsTrue(bC != cC);
                Assert.IsFalse(bC == cC);
            }
        }

    }
}