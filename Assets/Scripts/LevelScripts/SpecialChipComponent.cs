using System;
using UnityEngine;

namespace Match3
{
    public class SpecialChipComponent : ChipComponent
    {
        [SerializeField]
        private SpecialChipType _specialType;
        private static readonly int ActionTrigger = Animator.StringToHash("action");

        public void Action()
        {
            SetAnimationState(true);
            _animator.SetTrigger(ActionTrigger);
            MarkNeighbours();
        }

        private void MarkNeighbours()
        {
            switch (_specialType)
            {
                case SpecialChipType.None:
                    break;
                case SpecialChipType.SpecialSun:
                    break;
                case SpecialChipType.SpecialM18:
                    CellComponent top = CurrentCell.GetNeighbour(DirectionType.Top);
                    CellComponent bot = CurrentCell.GetNeighbour(DirectionType.Bot);
                    CellComponent left = CurrentCell.GetNeighbour(DirectionType.Left);
                    CellComponent right = CurrentCell.GetNeighbour(DirectionType.Right);
                    CurrentCell.Pulling(new []
                    {
                        top,bot,left,right,
                        top.GetNeighbour(DirectionType.Left),
                        top.GetNeighbour(DirectionType.Right),
                        bot.GetNeighbour(DirectionType.Left),
                        bot.GetNeighbour(DirectionType.Right)
                    });
                    break;
                case SpecialChipType.SpecialBlasterH:
                    break;
                case SpecialChipType.SpecialBlasterV:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}