namespace Match3
{
    public class Quest_ActivateSuperChip : ExtraObjective
    {
        private SpecialChipType _targetSpecial;
        private string _special;

        protected override void Init()
        {
            var random = UnityEngine.Random.Range(0, 4);
            LevelManager.Singleton.OnSpecialActivateEvent += SpecialActivateEvent;
            switch (random)
            {
                case 0:
                    _targetSpecial = SpecialChipType.Sun;
                    _special = "Sun";
                    break;
                case 1:
                    _targetSpecial = SpecialChipType.M18;
                    _special = "M18";
                    break;
                case 2:
                    _targetSpecial = SpecialChipType.BlasterH;
                    _special = "Horizontal Blaster";
                    break;
                case 3:
                    _targetSpecial = SpecialChipType.BlasterV;
                    _special = "Vertical Blaster";
                    break;
            }
            UpdateCount();
        }

        protected override void UpdateCount()
        {
            base.UpdateCount();
            if (IsCompleted) return;
            _questText.text = $"Activate {_special} <color=#FF0000>X{TargetCount - CurrentCount}</color>";
        }

        private void SpecialActivateEvent(SpecialChipType special)
        {
            if (special == _targetSpecial) ConditionMet();
        }

        protected override void Completed() => LevelManager.Singleton.OnSpecialActivateEvent -= SpecialActivateEvent;
    }
}