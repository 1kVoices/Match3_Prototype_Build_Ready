namespace Match3
{
    public class NTimesPlayerCombo : ExtraObjective
    {
        protected override void Init()
        {
            LevelManager.Singleton.OnSpawnSpecial += SpecialSpawned;
        }

        private void SpecialSpawned()
        {
            ConditionMet();
        }

        protected override void Completed()
        {
            LevelManager.Singleton.OnSpawnSpecial -= SpecialSpawned;
        }
    }
}