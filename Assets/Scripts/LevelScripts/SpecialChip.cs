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
        private bool _isExecuted;

        public void Action()
        {
            if (_isExecuted) return;
            _isExecuted = true;
            SetAnimationState(true);
            ChipAnimator.SetTrigger(Extensions.ActionTrigger);
            MarkNeighbours();
        }

        private void MarkNeighbours() //намеренно вызывается несколько раз для отслеживания фишек на поле
        {
            switch (_specialType)
            {
                case SpecialChipType.None:
                    break;
                case SpecialChipType.Sun:
                    StandardChip targetChip = CurrentCell.PreviousChip;
                    StandardChip randomChip = LevelManager.Singleton
                        .ChipPrefabs[
                            UnityEngine.Random.Range(0,
                                LevelManager.Singleton.ChipPrefabs.Length - LevelManager.Singleton.REMOVE)];

                    if (targetChip.IsNull()) targetChip = randomChip;

                    var affectedCells =
                        LevelManager.Singleton.AllCells()
                            .Where(z => z.CurrentChip.NotNull() && z.CurrentChip.Type == targetChip.Type).ToArray();

                    if (targetChip.Type == ChipType.None)
                        affectedCells = LevelManager.Singleton.AllCells().ToArray();

                    LevelManager.Singleton.DestroyChips(CurrentCell, affectedCells);
                    break;
                case SpecialChipType.M18:
                    LevelManager.Singleton.DestroyChips(CurrentCell,
                        CurrentCell, CurrentCell.GetNeighbour(DirectionType.Top),
                        CurrentCell.GetNeighbour(DirectionType.Bot),
                        CurrentCell.GetNeighbour(DirectionType.Left),
                        CurrentCell.GetNeighbour(DirectionType.Right),
                        CurrentCell.GetNeighbour(DirectionType.Top)?.GetNeighbour(DirectionType.Left),
                        CurrentCell.GetNeighbour(DirectionType.Top)?.GetNeighbour(DirectionType.Right),
                        CurrentCell.GetNeighbour(DirectionType.Bot)?.GetNeighbour(DirectionType.Left),
                        CurrentCell.GetNeighbour(DirectionType.Bot)?.GetNeighbour(DirectionType.Right));
                    break;
                case SpecialChipType.BlasterH:
                    Cell left = CurrentCell.GetNeighbour(DirectionType.Left);
                    Cell right = CurrentCell.GetNeighbour(DirectionType.Right);
                    while (left.NotNull())
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, left);
                        left = left.GetNeighbour(DirectionType.Left);
                    }
                    while (right.NotNull())
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, right);
                        right = right.GetNeighbour(DirectionType.Right);
                    }
                    break;
                case SpecialChipType.BlasterV:
                    Cell top = CurrentCell.GetNeighbour(DirectionType.Top);
                    Cell bot = CurrentCell.GetNeighbour(DirectionType.Bot);
                    while (top.NotNull())
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, top);
                        top = top.GetNeighbour(DirectionType.Top);
                    }
                    while (bot.NotNull())
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, bot);
                        bot = bot.GetNeighbour(DirectionType.Bot);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void FadeOut(SpecialChip specialChip) { }
    }
}