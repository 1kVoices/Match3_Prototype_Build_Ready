using System;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class SpecialChip : StandardChip
    {
        [SerializeField]
        private SpecialChipType _specialType;
        public SpecialChipType SpecialType => _specialType;
        public bool IsExecuted { get; private set; }

        public void Action()
        {
            SetAnimationState(true);
            ChipAnimator.SetTrigger(Extensions.ActionTrigger);
            MarkNeighbours();
        }

        public void ExecutionStart()
        {
            SetExecutionState(false);
            MarkNeighbours();
        }

        public void Executed()
        {
            SetExecutionState(true);
            CurrentCell.Pooling();
        }

        private void MarkNeighbours() //намеренно вызывается несколько раз для отслеживания фишек на поле
        {
            switch (_specialType)
            {
                case SpecialChipType.None:
                    break;
                case SpecialChipType.SpecialSun:
                    ChipType targetChipType = CurrentCell.PreviousChip.Type;
                    CurrentCell.Pulling(LevelManager.Singleton.AllCells.Where(z => z.CurrentChip.Type == targetChipType).ToArray());
                    break;
                case SpecialChipType.SpecialM18:
                    CurrentCell.Pulling(
                        CurrentCell.GetNeighbour(DirectionType.Top),
                        CurrentCell.GetNeighbour(DirectionType.Bot),
                        CurrentCell.GetNeighbour(DirectionType.Left),
                        CurrentCell.GetNeighbour(DirectionType.Right),
                        CurrentCell.GetNeighbour(DirectionType.Top)?.GetNeighbour(DirectionType.Left),
                        CurrentCell.GetNeighbour(DirectionType.Top)?.GetNeighbour(DirectionType.Right),
                        CurrentCell.GetNeighbour(DirectionType.Bot)?.GetNeighbour(DirectionType.Left),
                        CurrentCell.GetNeighbour(DirectionType.Bot)?.GetNeighbour(DirectionType.Right));
                    break;
                case SpecialChipType.SpecialBlasterH:
                    Cell left = CurrentCell.GetNeighbour(DirectionType.Left);
                    Cell right = CurrentCell.GetNeighbour(DirectionType.Right);
                    while (left.NotNull())
                    {
                        CurrentCell.Pulling(left);
                        left = left.GetNeighbour(DirectionType.Left);
                    }
                    while (right.NotNull())
                    {
                        CurrentCell.Pulling(right);
                        right = right.GetNeighbour(DirectionType.Right);
                    }
                    break;
                case SpecialChipType.SpecialBlasterV:
                    Cell top = CurrentCell.GetNeighbour(DirectionType.Top);
                    Cell bot = CurrentCell.GetNeighbour(DirectionType.Bot);
                    while (top.NotNull())
                    {
                        CurrentCell.Pulling(top);
                        top = top.GetNeighbour(DirectionType.Top);
                    }
                    while (bot.NotNull())
                    {
                        CurrentCell.Pulling(bot);
                        bot = bot.GetNeighbour(DirectionType.Bot);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void FadeOut(SpecialChip specialChip) { }
        public void SetExecutionState(bool state) => IsExecuted = state;
    }
}