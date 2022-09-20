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
            _current = _objectives[UnityEngine.Random.Range(0, _objectives.Length)];
            Instantiate(_current, transform);
            _text.text = $"{_current.TargetCount}X";
        }
    }
}