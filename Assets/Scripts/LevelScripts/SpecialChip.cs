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

        private void MarkNeighbours()
        {
            switch (_specialType)
            {
                case SpecialChipType.None:
                    break;
                case SpecialChipType.Sun:
                    StandardChip targetChip = CurrentCell.PreviousChip;

                    var activeChips = LevelManager.Singleton.AllCells
                        .Where(z => z.CurrentChip.NotNull())
                        .Where(x => x.CurrentChip.Type != ChipType.None)
                        .Select(c => c.CurrentChip)
                        .ToArray();

                    StandardChip randomChip = activeChips[UnityEngine.Random.Range(0, activeChips.Length)];

                    if (targetChip.IsNull()) targetChip = randomChip;

                    var affectedCells =
                        LevelManager.Singleton.AllCells
                            .Where(z => z.CurrentChip.NotNull() && z.CurrentChip.Type == targetChip.Type).ToArray();

                    if (targetChip.Type == ChipType.None)
                        affectedCells = LevelManager.Singleton.AllCells.ToArray();

                    LevelManager.Singleton.DestroyChips(CurrentCell, affectedCells);
                    break;
                case SpecialChipType.M18:
                    LevelManager.Singleton.DestroyChips(
                        CurrentCell,
                        CurrentCell.Top, CurrentCell.Bot,
                        CurrentCell.Left, CurrentCell.Right,
                        CurrentCell.Top? CurrentCell.Top.Left? CurrentCell.Top.Left : null : null,
                        CurrentCell.Top? CurrentCell.Top.Right? CurrentCell.Top.Right : null : null,
                        CurrentCell.Bot? CurrentCell.Bot.Left? CurrentCell.Bot.Left : null : null,
                        CurrentCell.Bot? CurrentCell.Bot.Right? CurrentCell.Bot.Right : null : null);
                    break;
                case SpecialChipType.BlasterH:
                    Cell left = CurrentCell.Left;
                    Cell right = CurrentCell.Right;
                    while (left.NotNull())
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, left);
                        left = left.Left;
                    }
                    while (right.NotNull())
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, right);
                        right = right.Right;
                    }
                    break;
                case SpecialChipType.BlasterV:
                    Cell top = CurrentCell.Top;
                    Cell bot = CurrentCell.Bot;
                    while (top.NotNull())
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, top);
                        top = top.Top;
                    }
                    while (bot.NotNull())
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, bot);
                        bot = bot.Bot;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void FadeOut(SpecialChip specialChip) { }
    }
}