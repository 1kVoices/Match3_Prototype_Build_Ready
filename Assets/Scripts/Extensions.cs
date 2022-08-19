namespace Match3
{
    public static class Extensions
    {
        public static DirectionType OppositeDirection(this DirectionType direction)
        {
            switch (direction)
            {
                case DirectionType.Top:
                    return DirectionType.Bot;
                case DirectionType.Bot:
                    return DirectionType.Top;
                case DirectionType.Left:
                    return DirectionType.Right;
                case DirectionType.Right:
                    return DirectionType.Left;
                default: return DirectionType.None;
            }
        }
    }

    public enum DirectionType : byte
    {
        None,
        Top,
        Bot,
        Left,
        Right,
    }

    public enum ChipType : byte
    {
        Apple,
        Avocado,
        Kiwi,
        Banana,
        Orange,
        Peach
    }
}