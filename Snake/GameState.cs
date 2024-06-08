namespace Snake
{
    public class GameState
    {
        private readonly LinkedList<Position> snakePositions = new();
        private readonly Random random = new();

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[rows, columns];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        public int Rows { get; }

        public int Columns { get; }

        public GridValue[,] Grid { get; }

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
            Dir = dir;
        }

        public void Move()
        {
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

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

        private void AddSnake()
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> emptyPositions = new (EmptyPositions());

            if (emptyPositions.Count == 0)
            {
                return;
            }

            Position pos = emptyPositions[random.Next(emptyPositions.Count)];
            Grid[pos.Row, pos.Column] = GridValue.Food;
        }

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Column] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last!.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
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

            return Grid[newHeadPos.Row, newHeadPos.Column];
        }
    }
}
