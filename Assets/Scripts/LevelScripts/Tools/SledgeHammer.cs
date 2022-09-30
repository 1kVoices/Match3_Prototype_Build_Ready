namespace Match3
{
    public class SledgeHammer : Tool
    {
        protected override void PlayerClicked(Cell cell)
        {
            if (!IsInput || cell is null || !cell.HasChip()
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
            if(cell.Top is not null) cell.Top.Redness(isOn);
            if (cell.Bot is not null) cell.Bot.Redness(isOn);
            if (cell.Left is not null) cell.Left.Redness(isOn);
            if (cell.Right is not null) cell.Right.Redness(isOn);
        }
    }
}