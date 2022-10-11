namespace Match3
{
    public class Quest_PlayerCombo : ExtraObjective
    {
        protected override void Init()
        {
            LevelManager.Singleton.OnSpawnSpecial += ConditionMet;
            UpdateCount();
        }

        protected override void Completed() => LevelManager.Singleton.OnSpawnSpecial -= ConditionMet;
        protected override void UpdateCount()
        {
            base.UpdateCount();
            if (IsCompleted) return;
            _questText.text = $"Create any Super Chip <color=#FF0000>X{TargetCount - CurrentCount}</color>";
        }
    }
}