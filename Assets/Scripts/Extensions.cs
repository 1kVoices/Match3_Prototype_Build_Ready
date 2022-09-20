using System.Collections;
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

        /// <summary>
        /// Для избежания повторий кода этот метод был сделан расширением
        /// т.к его используют два класса Cell и MatchHelper
        /// </summary>
        public static bool CompareChips(Cell cell, Cell comparativeCell)
        {
            return cell.NotNull() &&
                   comparativeCell.NotNull() &&
                   comparativeCell.CurrentChip.NotNull() &&
                   comparativeCell.CurrentChip != cell.CurrentChip &&
                   comparativeCell.CurrentChip.Type != ChipType.None &&
                   comparativeCell.CurrentChip.Type == cell.CurrentChip.Type;
        }
        /// <summary>
        /// Здесь используется List вместо LinkedList только из-за того,
        /// что Cell необходимо иметь именно List из-за метода AddRange
        /// </summary>
        public static void FindMatches(List<Cell> fillingList, Cell cell, Cell caller = null)
        {
            if (cell.IsNull()) return;
            if (caller.NotNull())
            {
                cell.SetTemporaryChip(cell.CurrentChip);
                cell.SetCurrentChip(caller.CurrentChip);
            }

            Cell top = cell.Top;
            Cell bot = cell.Bot;
            Cell left = cell.Left;
            Cell right = cell.Right;
            Cell topTop = cell.TopTop;
            Cell botBot = cell.BotBot;
            Cell leftLeft = cell.LeftLeft;
            Cell rightRight = cell.RightRight;

            #region Horizontal
            if (CompareChips(cell, left) && CompareChips(cell, right))
            {
                //00_00
                if (CompareChips(cell, leftLeft) && CompareChips(cell, rightRight))
                {
                    fillingList.Add(leftLeft);
                    fillingList.Add(rightRight);
                }
                //00_0
                if (CompareChips(cell, leftLeft)) fillingList.Add(leftLeft);
                //0_00
                if (CompareChips(cell, rightRight)) fillingList.Add(rightRight);
                //0_0
                fillingList.Add(left);
                fillingList.Add(right);
            }
            //00_
            if (CompareChips(cell, left) && CompareChips(cell, leftLeft))
            {
                fillingList.Add(left);
                fillingList.Add(leftLeft);
            }
            //_00
            if (CompareChips(cell, right) && CompareChips(cell, rightRight))
            {
                fillingList.Add(right);
                fillingList.Add(rightRight);
            }
            #endregion

            #region Vertical
            if (CompareChips(cell, top) && CompareChips(cell, bot)) //top is left
            {
                //00_00
                if (CompareChips(cell, topTop) && CompareChips(cell, botBot))
                {
                    fillingList.Add(topTop);
                    fillingList.Add(botBot);
                }
                //00_0
                if (CompareChips(cell, topTop)) fillingList.Add(topTop);
                //0_00
                if (CompareChips(cell, botBot)) fillingList.Add(botBot);
                //0_0
                fillingList.Add(top);
                fillingList.Add(bot);
            }
            //00_
            if (CompareChips(cell, top) && CompareChips(cell, topTop))
            {
                fillingList.Add(top);
                fillingList.Add(topTop);
            }
            //_00
            if (CompareChips(cell, bot) && CompareChips(cell, botBot))
            {
                fillingList.Add(bot);
                fillingList.Add(botBot);
            }
            #endregion

            if (cell.TemporaryChip.IsNull()) return;
            cell.SetCurrentChip(cell.TemporaryChip);
            cell.SetTemporaryChip(null);
        }

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

        public static bool HasChip(this Cell cell) => cell.CurrentChip.NotNull();

        public static bool PosYIdentical(this Cell[] array) =>
            array.ToList().TrueForAll(z => z.transform.position.y.Equals(array.First().transform.position.y));

        public static bool PosXIdentical(this Cell[] array) =>
            array.ToList().TrueForAll(z => z.transform.position.x.Equals(array.First().transform.position.x));

        public static bool NotNull(this object obj) => obj != null;
        public static bool IsNull(this object obj) => obj == null;
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