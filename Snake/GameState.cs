namespace Snake
{
    public class GameState
    {
        private readonly LinkedList<Position> snakePositions = new();
        private readonly Random random = new();
        private readonly LinkedList<Direction> dirChanges = new();
        internal readonly GridValue[,] grid;

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new GridValue[rows, columns];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        public int Rows { get; }

        public int Columns { get; }

        public Direction Dir { get; private set; }

        public int Score { get; private set; }

        public bool GameOver { get; private set; }

        public Position HeadPosition()
        {
            return snakePositions.First!.Value;
        }

        public Position TailPosition()
        {
            return snakePositions.Last!.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }

        public void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Position newHeadPos = HeadPosition().TranslateByDirection(Dir);
            GridValue hit = WillHit(newHeadPos);

            HandleSnakeMovement(newHeadPos, hit);
        }

        public GridValue GetGridValue(int row, int column)
        {
            return grid[row, column];
        }

        public void SetGridValue(int row, int column, GridValue value)
        {
            grid[row, column] = value;
        }

        private void HandleSnakeMovement(Position newHeadPos, GridValue hit)
        {
            if (hit == GridValue.Out || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddSnake()
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        private void AddFood()
        {
            List<Position> emptyPositions = new(EmptyPositions());

            if (emptyPositions.Count == 0)
            {
                return;
            }

            Position pos = emptyPositions[random.Next(emptyPositions.Count)];
            grid[pos.Row, pos.Column] = GridValue.Food;
        }

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            grid[pos.Row, pos.Column] = GridValue.Snake;
        }

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }

            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();

            return newDir != lastDir && newDir != lastDir.GetOppositeDirection();
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last!.Value;
            grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        private bool IsOut(Position pos)
        {
            return pos.Row < 0 ||
                   pos.Row >= Rows ||
                   pos.Column < 0 ||
                   pos.Column >= Columns;
        }

        private GridValue WillHit(Position newHeadPos)
        {
            if (IsOut(newHeadPos))
            {
                return GridValue.Out;
            }

            if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }

            return grid[newHeadPos.Row, newHeadPos.Column];
        }
    }
}
