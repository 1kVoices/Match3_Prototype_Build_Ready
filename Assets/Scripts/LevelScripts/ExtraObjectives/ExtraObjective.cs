using TMPro;
using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class ExtraObjective : MonoBehaviour
    {
        [SerializeField]
        private int _targetCount;
        protected TextMeshProUGUI _questText;
        protected int TargetCount => _targetCount;
        protected int CurrentCount { get; set; }
        protected bool IsCompleted { get; private set; }
        private QuestManager _manager;

        public void Start()
        {
            _manager = FindObjectOfType<QuestManager>();
            _questText = GetComponent<TextMeshProUGUI>();

            Init();
        }

        protected abstract void Init();
        protected abstract void Completed();

        protected virtual void UpdateCount()
        {
            if (CurrentCount < TargetCount) return;

            _questText.text = "Completed!";
            IsCompleted = true;
        }

        protected void ConditionMet()
        {
            CurrentCount++;
            CheckTarget();
            UpdateCount();
        }

        private void CheckTarget()
        {
            if (CurrentCount >= _targetCount) QuestCompleted();
        }

        private void QuestCompleted()
        {
            Completed();
            _manager.QuestCompleted();
        }
    }
}