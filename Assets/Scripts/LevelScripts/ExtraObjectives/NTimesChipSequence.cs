using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class NTimesChipSequence : ExtraObjective
    {
        private LinkedList<ChipType> _sequence;
        private int _currentElement;
        private string _baseColor;
        private string _greenColor;
        private string _colorElement0;
        private string _colorElement1;
        private string _colorElement2;
        private WaitForSecondsRealtime _delay;

        protected override void Init()
        {
            LevelManager.Singleton.OnChipSequenceDestroy += Destroyed;
            LevelManager.Singleton.OnSpecialActivateEvent += SpecialActivate;
            _delay = new WaitForSecondsRealtime(1f);
            _questText.fontSize = 25;
            _baseColor = "2C3E50";
            _greenColor = "27AE60";
            _colorElement0 = _baseColor;
            _colorElement1 = _baseColor;
            _colorElement2 = _baseColor;

            _sequence = new LinkedList<ChipType>();

            var activeChipTypes = LevelManager.Singleton.ChipChances.Select(chip => chip.Key.Type).ToArray();

            while (_sequence.Count != 3)
            {
                ChipType chipType = activeChipTypes[UnityEngine.Random.Range(0, activeChipTypes.Length)];

                while (_sequence.Contains(chipType))
                    chipType = activeChipTypes[UnityEngine.Random.Range(0, activeChipTypes.Length)];

                _sequence.AddLast(chipType);
            }
            UpdateCount();
        }

        private void SpecialActivate(SpecialChipType obj)
        {
            DarkenAll();
            _currentElement = 0;
            UpdateCount();
        }

        private void Destroyed((ChipType chip1, ChipType chip2) chipPair)
        {
            if (_currentElement >= 3) return;
            if (chipPair.chip1 == _sequence.ElementAt(_currentElement))
            {
                Highlight(_currentElement);
                _currentElement++;
            }
            else if (chipPair.chip1 != _sequence.ElementAt(_currentElement))
            {
                if (chipPair.chip2 == _sequence.ElementAt(_currentElement))
                {
                    Highlight(_currentElement);
                    _currentElement++;
                    return;
                }
                DarkenAll();
                _currentElement = 0;
            }
            if (_currentElement == 3)
            {
                StartCoroutine(DelayedDarken());
                _currentElement = 0;
                ConditionMet();
            }
            UpdateCount();
        }

        private IEnumerator DelayedDarken()
        {
            yield return _delay;
            DarkenAll();
            UpdateCount();
        }

        private void DarkenAll()
        {
            _colorElement0 = _baseColor;
            _colorElement1 = _baseColor;
            _colorElement2 = _baseColor;
        }

        private void Highlight(int currentElement)
        {
            switch (currentElement)
            {
                case 0:
                    _colorElement0 = _greenColor;
                    break;
                case 1:
                    _colorElement1 = _greenColor;
                    break;
                case 2:
                    _colorElement2 = _greenColor;
                    break;
            }
        }

        protected override void Completed()
        {
            LevelManager.Singleton.OnChipSequenceDestroy -= Destroyed;
            LevelManager.Singleton.OnSpecialActivateEvent -= SpecialActivate;
        }

        protected override void UpdateCount()
        {
            base.UpdateCount();
            if (IsCompleted) return;
            _questText.text =
                $"<color=#FF0000>X{TargetCount - CurrentCount}</color> Destroy in sequence of\n " +
                $"<color=#{_colorElement0}>{_sequence.ElementAt(0)}</color>, " +
                $"<color=#{_colorElement1}>{_sequence.ElementAt(1)}</color>, " +
                $"<color=#{_colorElement2}>{_sequence.ElementAt(2)}</color>";
        }
    }
}