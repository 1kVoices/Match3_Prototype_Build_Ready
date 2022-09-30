using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    [RequireComponent(typeof(Image))]
    public class PlayerObjective : MonoBehaviour
    {
        private bool _isCompleted;
        private Image _objectiveChip;
        private TextMeshProUGUI _count;
        public int CurrentCount { get; private set; }
        [SerializeField]
        private Sprite _apple;
        [SerializeField]
        private Sprite _avocado;
        [SerializeField]
        private Sprite _kiwi;
        [SerializeField]
        private Sprite _banana;
        [SerializeField]
        private Sprite _orange;
        [SerializeField]
        private Sprite _peach;

        private void Start()
        {
            _objectiveChip = GetComponent<Image>();
            _count = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void UpdateCount()
        {
            if (_isCompleted) return;
            CurrentCount = int.Parse(_count.text);
            CurrentCount--;
            _count.text = CurrentCount.ToString();
            if (CurrentCount <= 0) _isCompleted = true;
        }

        public void SetCurrentObjective(ChipType type, int count)
        {
            CurrentCount = count;
            _count.text = CurrentCount.ToString();

            _objectiveChip.sprite = type switch
            {
                ChipType.Apple => _apple,
                ChipType.Avocado => _avocado,
                ChipType.Kiwi => _kiwi,
                ChipType.Banana => _banana,
                ChipType.Orange => _orange,
                ChipType.Peach => _peach,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
