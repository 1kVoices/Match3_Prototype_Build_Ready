namespace Match3
{
    public class NTimesSuperChip : ExtraObjective
    {
        private SpecialChipType _targetSpecial;

        protected override void Init()
        {
            LevelManager.Singleton.OnSpecialActivateEvent += SpecialActivateEvent;
            switch (RandomInt)
            {
                case 0:
                    _targetSpecial = SpecialChipType.Sun;
                    break;
                case 1:
                    _targetSpecial = SpecialChipType.M18;
                    break;
                case 2:
                    _targetSpecial = SpecialChipType.BlasterH;
                    break;
                case 3:
                    _targetSpecial = SpecialChipType.BlasterV;
                    break;
            }
        }

        private void SpecialActivateEvent(SpecialChipType special)
        {
            if (special == _targetSpecial) ConditionMet();
        }

        protected override void Completed()
        {
            LevelManager.Singleton.OnSpecialActivateEvent -= SpecialActivateEvent;
        }
    }
}