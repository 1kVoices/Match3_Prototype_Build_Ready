namespace Match3
{
    public class SledgeHammer : Tool
    {
        protected override void PlayerClicked(Cell cell)
        {
            if (!IsInput || cell.IsNull() || !cell.HasChip()
                || !cell.CurrentChip.IsInteractable || cell.CurrentChip.IsAnimating) return;

            base.PlayerClicked(cell);
            LevelManager.Singleton.DestroyChips(null,
                cell, cell.Top, cell.Bot, cell.Left, cell.Right);
            if (HitAmount > 0) return;
            UseAmount--;
            UpdateState();
            HighlightCells(cell, false);
        }

        protected override void OnCellPointerEnter(Cell cell)
        {
            if (!IsInput) return;
            HighlightCells(cell, true);
        }

        protected override void OnCellPointerExit(Cell cell)
        {
            HighlightCells(cell, false);
        }

        private void HighlightCells(Cell cell, bool isOn)
        {
            cell.Redness(isOn);
            if(cell.Top.NotNull()) cell.Top.Redness(isOn);
            if (cell.Bot.NotNull()) cell.Bot.Redness(isOn);
            if (cell.Left.NotNull()) cell.Left.Redness(isOn);
            if (cell.Right.NotNull()) cell.Right.Redness(isOn);
        }
    }
}