using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public static class Extensions
    {
        public static readonly int Show = Animator.StringToHash("showUp");
        public static readonly int Fade = Animator.StringToHash("fadeOut");
        public static readonly int FastFade = Animator.StringToHash("fastFade");
        public static readonly int SunFade = Animator.StringToHash("sunFade");
        public static readonly int FastShow = Animator.StringToHash("fastShowUp");
        public static readonly int Fall = Animator.StringToHash("fall");
        public static readonly int EndFall = Animator.StringToHash("endFall");
        public static readonly int ActionTrigger = Animator.StringToHash("action");

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

        public static bool HasChip(this Cell cell)
        {
            return cell.CurrentChip.NotNull();
        }

        public static void Enqueue(this List<Cell> list, Cell cell)
        {
            if (!list.Contains(cell)) list.Add(cell);
        }

        public static Cell RemoveFirst(this List<Cell> list)
        {
            Cell cell = list.First();
            list.RemoveAt(0);
            return cell;
        }

        public static bool PosYIdentical(this Cell[] array) =>
            array.ToList().TrueForAll(z => z.transform.position.y.Equals(array.First().transform.position.y));

        public static bool PosXIdentical(this Cell[] array) =>
            array.ToList().TrueForAll(z => z.transform.position.x.Equals(array.First().transform.position.x));

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
        None,
        Apple,
        Avocado,
        Kiwi,
        Banana,
        Orange,
        Peach
    }

    public enum SpecialChipType : byte
    {
        None,
        Sun,
        M18,
        BlasterH,
        BlasterV
    }
}