using TMPro;
using UnityEngine;

namespace Match3
{
    public class MoneyManager : MonoBehaviour, IData
    {
        private TextMeshProUGUI _text;
        private int _money;

        private void Start()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _text.text = _money.ToString();
        }

        public void AddMoney(int money)
        {
            var current = int.Parse(_text.text);
            current += money;
            _text.text = current.ToString();
        }

        public void LoadData(GameData data) => _money = data.Money;
        public void SaveData(ref GameData data) => data.Money = int.Parse(_text.text);
    }
}