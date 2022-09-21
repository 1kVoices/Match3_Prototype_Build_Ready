using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public abstract class ExtraObjective : MonoBehaviour
    {
        [SerializeField]
        private int _targetCount;
        [SerializeField]
        private Sprite[] _sprites;
        private Image _questImage;
        public int TargetCount => _targetCount;
        public int CurrentCount { get; private set; }
        protected int RandomInt;
        private QuestManager _manager;
        private TextMeshProUGUI _completedText;

        public void Start()
        {
            _manager = FindObjectOfType<QuestManager>();
            _questImage = GetComponent<Image>();
            _completedText = GetComponentInChildren<TextMeshProUGUI>();

            RandomInt = UnityEngine.Random.Range(0, _sprites.Length);
            _questImage.sprite = _sprites[RandomInt];
            _completedText.enabled = false;
            Init();
        }

        protected abstract void Init();
        protected abstract void Completed();

        protected void ConditionMet()
        {
            CurrentCount++;
            CheckTarget();
            _manager.UpdateCount();
        }

        private void CheckTarget()
        {
            if (CurrentCount >= _targetCount) QuestCompleted();
        }

        private void QuestCompleted()
        {
            _questImage.enabled = false;
            _completedText.enabled = true;
            Completed();
        }
    }
}