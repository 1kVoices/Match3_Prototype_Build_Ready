using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    [RequireComponent(typeof(Animator))]
    public class ExtraObjective : MonoBehaviour
    {
        [SerializeField]
        private int _targetCount;
        [SerializeField]
        private Image _completedImage;
        [SerializeField]
        private Image _questImage;
        [SerializeField]
        private Sprite[] _sprites;
        private Animator _animator;
        protected int RandomInt;
        private int _count;
        public int TargetCount => _targetCount;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            RandomInt = UnityEngine.Random.Range(0, _sprites.Length);
            _questImage.sprite = _sprites[RandomInt];
        }

        protected void ConditionMatch()
        {
            _count++;
            CheckTarget();
        }

        private void CheckTarget()
        {
            if (_count >= _targetCount) QuestCompleted();
        }

        private void QuestCompleted()
        {
            // _animator
        }
    }
}