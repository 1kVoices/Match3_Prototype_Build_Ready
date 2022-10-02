using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public static class Extensions
    {
        public static readonly int Show = Animator.StringToHash("showUp");
        public static readonly int Fade = Animator.StringToHash("fadeOut");
        public static readonly int Hide = Animator.StringToHash("hide");
        public static readonly int Coloring = Animator.StringToHash("coloring");
        public static readonly int FastFade = Animator.StringToHash("fastFade");
        public static readonly int SunFade = Animator.StringToHash("sunFade");
        public static readonly int FastShow = Animator.StringToHash("fastShowUp");
        public static readonly int Fall = Animator.StringToHash("fall");
        public static readonly int EndFall = Animator.StringToHash("endFall");
        public static readonly int ActionTrigger = Animator.StringToHash("action");
        public static readonly int ExtraActionTrigger = Animator.StringToHash("extraAction");
        public static readonly int HalfDarken = Animator.StringToHash("halfDarken");
        public static readonly int Whitening = Animator.StringToHash("halfWhitening");

        public static StandardChip[] ChipsOnMap()
        {
            return LevelManager.Singleton.AllCells
                .Where(cell => cell.HasChip())
                .Where(cell => cell.CurrentChip.Type != ChipType.None)
                .Select(cell => cell.CurrentChip)
                .ToArray();
        }

        private static bool CompareChips(Cell cell, Cell comparativeCell)
        {
            return cell is not null &&
                   comparativeCell is not null &&
                   comparativeCell.HasChip() &&
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
            if (cell is null) return;
            if (caller is not null)
            {
                cell.SetTemporaryChip(cell.CurrentChip);
                cell.SetCurrentChip(caller.CurrentChip);
            }

            #region Horizontal
            if (CompareChips(cell, cell.Left) && CompareChips(cell, cell.Right))
            {
                //00_00
                if (CompareChips(cell, cell.Left.Left) && CompareChips(cell, cell.Right.Right))
                {
                    fillingList.Add(cell.Left.Left);
                    fillingList.Add(cell.Right.Right);
                }
                //00_0
                if (CompareChips(cell, cell.Left.Left)) fillingList.Add(cell.Left.Left);
                //0_00
                if (CompareChips(cell, cell.Right.Right)) fillingList.Add(cell.Right.Right);
                //0_0
                fillingList.Add(cell.Left);
                fillingList.Add(cell.Right);
            }
            //00_
            if (CompareChips(cell, cell.Left) && CompareChips(cell, cell.Left.Left))
            {
                fillingList.Add(cell.Left);
                fillingList.Add(cell.Left.Left);
            }
            //_00
            if (CompareChips(cell, cell.Right) && CompareChips(cell, cell.Right.Right))
            {
                fillingList.Add(cell.Right);
                fillingList.Add(cell.Right.Right);
            }
            #endregion

            #region Vertical
            if (CompareChips(cell, cell.Top) && CompareChips(cell, cell.Bot)) //top is left
            {
                //00_00
                if (CompareChips(cell, cell.Top.Top) && CompareChips(cell, cell.Bot.Bot))
                {
                    fillingList.Add(cell.Top.Top);
                    fillingList.Add(cell.Bot.Bot);
                }
                //00_0
                if (CompareChips(cell, cell.Top.Top)) fillingList.Add(cell.Top.Top);
                //0_00
                if (CompareChips(cell, cell.Bot.Bot)) fillingList.Add(cell.Bot.Bot);
                //0_0
                fillingList.Add(cell.Top);
                fillingList.Add(cell.Bot);
            }
            //00_
            if (CompareChips(cell, cell.Top) && CompareChips(cell, cell.Top.Top))
            {
                fillingList.Add(cell.Top);
                fillingList.Add(cell.Top.Top);
            }
            //_00
            if (CompareChips(cell, cell.Bot) && CompareChips(cell, cell.Bot.Bot))
            {
                fillingList.Add(cell.Bot);
                fillingList.Add(cell.Bot.Bot);
            }
            #endregion

            if (cell.TemporaryChip is null) return;
            cell.SetCurrentChip(cell.TemporaryChip);
            cell.SetTemporaryChip(null);
        }

        public static DirectionType OppositeDirection(this DirectionType direction)
        {
            return direction switch
            {
                DirectionType.Top => DirectionType.Bot,
                DirectionType.Bot => DirectionType.Top,
                DirectionType.Left => DirectionType.Right,
                DirectionType.Right => DirectionType.Left,
                _ => DirectionType.None
            };
        }

        public static bool HasChip(this Cell cell)
        {
            return cell.CurrentChip is not null;
        }

        public static bool PosYIdentical(this Cell[] array) =>
            array.ToList().TrueForAll(cell => cell.transform.position.y.Equals(array.First().transform.position.y));

        public static bool PosXIdentical(this Cell[] array) =>
            array.ToList().TrueForAll(cell => cell.transform.position.x.Equals(array.First().transform.position.x));
    }

    public enum DirectionType : byte
    {
        None,
        Top,
        Bot,
        Left,
        Right,
    }

    [System.Flags]
    public enum ChipType : byte
    {
        None = 0,
        Apple = 1,
        Avocado = 2,
        Kiwi = 4,
        Banana = 8,
        Orange = 16,
        Peach = 32
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