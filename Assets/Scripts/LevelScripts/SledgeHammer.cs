namespace Match3
{
    public class SledgeHammer : Tool
    {
        protected override void PlayerClicked(Cell cell)
        {
            if (!IsInput) return;
            base.PlayerClicked(cell);
            LevelManager.Singleton.DestroyChips(null,
                cell, cell.Top, cell.Bot, cell.Left, cell.Right);
            UpdateState();
        }
    }
}