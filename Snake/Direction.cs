namespace Snake
{
    public class Direction
    {
        public static readonly Direction Up = new(-1, 0);
        public static readonly Direction Down = new(1, 0);
        public static readonly Direction Left = new(0, -1);
        public static readonly Direction Right = new(0, 1);

        private Direction(int rowOffSet, int columnOffSet)
        {
            RowOffSet = rowOffSet;
            ColumnOffSet = columnOffSet;
        }

        public int RowOffSet { get; }

        public int ColumnOffSet { get; }

        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is Direction direction &&
                   RowOffSet == direction.RowOffSet &&
                   ColumnOffSet == direction.ColumnOffSet;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffSet, ColumnOffSet);
        }

        public Direction GetOppositeDirection() => new Direction(-RowOffSet, -ColumnOffSet);
    }
}
