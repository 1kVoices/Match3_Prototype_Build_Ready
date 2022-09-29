namespace Match3
{
    public class Hammer : Tool
    {
        protected override void PlayerClicked(Cell cell)
        {
            if (!IsInput || cell.IsNull() || !cell.HasChip() ||
                !cell.CurrentChip.IsInteractable || cell.CurrentChip.IsAnimating) return;

            base.PlayerClicked(cell);
            LevelManager.Singleton.DestroyChips(null, cell);

            if (HitAmount > 0) return;
            UseAmount--;
            UpdateState();
            cell.Redness(false);
        }

        protected override void OnCellPointerEnter(Cell cell)
        {
            if (!IsInput) return;
            cell.Redness(true);
        }

        protected override void OnCellPointerExit(Cell cell)
        {
            cell.Redness(false);
        }
    }
}