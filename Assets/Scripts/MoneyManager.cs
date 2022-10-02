using TMPro;
using UnityEngine;

namespace Match3
{
    public class MoneyManager : MonoBehaviour, IData
    {
        private TextMeshProUGUI _text;
        public int Money { get; private set; }

        private void Start()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _text.text = Money.ToString();
        }

        public void AddMoney(int money)
        {
            var current = int.Parse(_text.text);
            current += money;
            _text.text = current.ToString();
        }

        public void RemoveMoney(int money)
        {
            Money -= money;
            _text.text = Money.ToString();
        }

        public void LoadData(GameData data) => Money = data.Money;
        public void SaveData(ref GameData data) => data.Money = int.Parse(_text.text);
    }
}