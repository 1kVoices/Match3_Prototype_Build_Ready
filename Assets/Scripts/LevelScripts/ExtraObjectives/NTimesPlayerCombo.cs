namespace Match3
{
    public class NTimesPlayerCombo : ExtraObjective
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
            _questText.text = $"Create any Super Chip X{TargetCount - CurrentCount}";
        }
    }
}