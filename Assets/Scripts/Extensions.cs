using System.Collections.Generic;
using System.Linq;

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

        public static bool PosYIdentical(this List<CellComponent> list) =>
            list.TrueForAll(z => z.transform.position.y.Equals(list.First().transform.position.y));

        public static bool PosXIdentical(this List<CellComponent> list) =>
            list.TrueForAll(z => z.transform.position.x.Equals(list.First().transform.position.x));

        public static bool NotNull(this object obj) => obj != null;
        public static bool IsNull(this object obj) => obj == null;
    }

    public enum MatchType : byte
    {
        Default,
        Horizontal4,
        Vertical4,
        T_type,
        Match5
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
        Peach,
        SpecialSun,
        SpecialM18,
        SpecialBlasterH,
        SpecialBlasterV
    }
}