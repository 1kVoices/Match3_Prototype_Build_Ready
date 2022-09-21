using TMPro;
using UnityEngine;

namespace Match3
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField]
        private ExtraObjective[] _objectives;
        private ExtraObjective _current;
        private TextMeshProUGUI _text;

        private void Start()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _current = Instantiate(_objectives[UnityEngine.Random.Range(0, _objectives.Length)], transform);
            _text.text = $"{_current.TargetCount}X";
        }

        public void UpdateCount()
        {
            var count = _current.TargetCount - _current.CurrentCount;
            if (count <= 0)
            {
                _text.enabled = false;
                return;
            }
            _text.text = $"{count}X";
        }
    }
}