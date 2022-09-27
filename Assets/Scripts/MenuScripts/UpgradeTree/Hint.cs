using TMPro;
using UnityEngine;

namespace Match3
{
    public class Hint : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private TextMeshProUGUI _cost;

        public void SetText(string text, string cost)
        {
            _text.text = text;
            _cost.text = cost;
        }
    }
}