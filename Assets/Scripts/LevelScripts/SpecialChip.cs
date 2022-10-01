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
            LevelManager.Singleton.OnSpecialActivate(_specialType);
            SetAnimationState(true);
            if (SpecialType == SpecialChipType.M18)
            {
                switch (LevelManager.Singleton.M18Level)
                {
                    case < 2:
                        ChipAnimator.SetTrigger(Extensions.ActionTrigger);
                        break;
                    case >= 2:
                        ChipAnimator.SetTrigger(Extensions.ExtraActionTrigger);
                        break;
                }
            }
            else
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

                    var activeChips = Extensions.ChipsOnMap();

                    StandardChip randomChip = activeChips[UnityEngine.Random.Range(0, activeChips.Length)];

                    targetChip ??= randomChip;

                    var affectedCells =
                        LevelManager.Singleton.AllCells
                            .Where(cell => cell.HasChip() && cell.CurrentChip.Type == targetChip.Type).ToArray();

                    if (targetChip.Type == ChipType.None)
                        affectedCells = LevelManager.Singleton.AllCells.ToArray();

                    LevelManager.Singleton.DestroyChips(CurrentCell, affectedCells);
                    SoundManager.Singleton.SunActive();
                    break;
                case SpecialChipType.M18:
                    switch (LevelManager.Singleton.M18Level)
                    {
                        case 0:
                            LevelManager.Singleton.DestroyChips(
                                CurrentCell,
                                CurrentCell.Top, CurrentCell.Bot,
                                CurrentCell.Left, CurrentCell.Right);
                            break;
                        case 1:
                            LevelManager.Singleton.DestroyChips(
                                CurrentCell,
                                CurrentCell.Top, CurrentCell.Bot,
                                CurrentCell.Left, CurrentCell.Right,
                                CurrentCell.Top? CurrentCell.Top.Left? CurrentCell.Top.Left : null : null,
                                CurrentCell.Top? CurrentCell.Top.Right? CurrentCell.Top.Right : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Left? CurrentCell.Bot.Left : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Right? CurrentCell.Bot.Right : null : null);
                            break;
                        case 2:
                            LevelManager.Singleton.DestroyChips(
                                CurrentCell,
                                CurrentCell.Top, CurrentCell.Bot,
                                CurrentCell.Left, CurrentCell.Right,
                                CurrentCell.Top? CurrentCell.Top.Left? CurrentCell.Top.Left : null : null,
                                CurrentCell.Top? CurrentCell.Top.Right? CurrentCell.Top.Right : null : null,
                                CurrentCell.Top? CurrentCell.Top.Top? CurrentCell.Top.Top : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Left? CurrentCell.Bot.Left : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Right? CurrentCell.Bot.Right : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Bot? CurrentCell.Bot.Bot : null : null,
                                CurrentCell.Left? CurrentCell.Left.Left? CurrentCell.Left.Left : null : null,
                                CurrentCell.Right? CurrentCell.Right.Right? CurrentCell.Right.Right : null : null);
                            break;
                        case 3://все написанно в одну строчку, чтобы избежать путанницы
                            LevelManager.Singleton.DestroyChips(
                                CurrentCell,
                                CurrentCell.Top, CurrentCell.Bot,
                                CurrentCell.Left, CurrentCell.Right,
                                CurrentCell.Top? CurrentCell.Top.Left? CurrentCell.Top.Left : null : null,
                                CurrentCell.Top? CurrentCell.Top.Right? CurrentCell.Top.Right : null : null,
                                CurrentCell.Top? CurrentCell.Top.Top? CurrentCell.Top.Top : null : null,
                                CurrentCell.Top? CurrentCell.Top.Top? CurrentCell.Top.Top.Left? CurrentCell.Top.Top.Left : null : null : null,
                                CurrentCell.Top? CurrentCell.Top.Top? CurrentCell.Top.Top.Right? CurrentCell.Top.Top.Right : null : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Left? CurrentCell.Bot.Left : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Right? CurrentCell.Bot.Right : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Bot? CurrentCell.Bot.Bot : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Bot? CurrentCell.Bot.Bot.Left? CurrentCell.Bot.Bot.Left : null : null : null,
                                CurrentCell.Bot? CurrentCell.Bot.Bot? CurrentCell.Bot.Bot.Right? CurrentCell.Bot.Bot.Right : null : null : null,
                                CurrentCell.Left? CurrentCell.Left.Left? CurrentCell.Left.Left : null : null,
                                CurrentCell.Left? CurrentCell.Left.Left? CurrentCell.Left.Left.Top? CurrentCell.Left.Left.Top : null : null : null,
                                CurrentCell.Left? CurrentCell.Left.Left? CurrentCell.Left.Left.Top? CurrentCell.Left.Left.Top.Top? CurrentCell.Left.Left.Top.Top : null : null : null : null,
                                CurrentCell.Left? CurrentCell.Left.Left? CurrentCell.Left.Left.Bot? CurrentCell.Left.Left.Bot : null : null : null,
                                CurrentCell.Left? CurrentCell.Left.Left? CurrentCell.Left.Left.Bot? CurrentCell.Left.Left.Bot.Bot? CurrentCell.Left.Left.Bot.Bot : null : null : null : null,
                                CurrentCell.Right? CurrentCell.Right.Right? CurrentCell.Right.Right : null : null,
                                CurrentCell.Right? CurrentCell.Right.Right? CurrentCell.Right.Right.Top? CurrentCell.Right.Right.Top : null : null : null,
                                CurrentCell.Right? CurrentCell.Right.Right? CurrentCell.Right.Right.Top? CurrentCell.Right.Right.Top.Top? CurrentCell.Right.Right.Top.Top : null : null : null : null,
                                CurrentCell.Right? CurrentCell.Right.Right? CurrentCell.Right.Right.Bot? CurrentCell.Right.Right.Bot : null : null : null,
                                CurrentCell.Right? CurrentCell.Right.Right? CurrentCell.Right.Right.Bot? CurrentCell.Right.Right.Bot.Bot? CurrentCell.Right.Right.Bot.Bot : null : null : null : null);
                            break;
                    }
                    SoundManager.Singleton.M18Active();
                    break;
                case SpecialChipType.BlasterH:
                    Cell left = CurrentCell.Left;
                    Cell right = CurrentCell.Right;
                    while (left is not null)
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, left);
                        left = left.Left;
                    }
                    while (right is not null)
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, right);
                        right = right.Right;
                    }
                    SoundManager.Singleton.BlasterActive();
                    break;
                case SpecialChipType.BlasterV:
                    Cell top = CurrentCell.Top;
                    Cell bot = CurrentCell.Bot;
                    while (top is not null)
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, top);
                        top = top.Top;
                    }
                    while (bot is not null)
                    {
                        LevelManager.Singleton.DestroyChips(CurrentCell, bot);
                        bot = bot.Bot;
                    }
                    SoundManager.Singleton.BlasterActive();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ExecutionEnded() => _isExecuted = false;
        public override void FadeOut(SpecialChip specialChip) { }
    }
}