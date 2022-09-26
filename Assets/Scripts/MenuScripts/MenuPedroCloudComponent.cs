using System.Collections;
using System.Linq;
using Match3.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class MenuPedroCloudComponent : MonoBehaviour, IData
    {
        [SerializeField, Range(0, 0.1f)]
        private float _textDelay;
        [SerializeField, Range(3, 7)]
        private int _phrasesDelay;
        [SerializeField]
        private TextMeshProUGUI _cloudText;
        [SerializeField]
        private Sprite[] _pedroEmotions;
        [SerializeField]
        private Image _pedroImage;
        private string[] _pedroQuestPhrases;
        private string[] _pedroHelloAgain;
        private GameData _data;

        private void Start()
        {
            _pedroQuestPhrases = new[]
            {
                "Hello, Traveller!\nI'm Pedro The Fruit Seller",
                "I'd offer you\nbuy my fruits\nbut...",
                "Those bandits stole them all!!!",
                "Oh! I've got an idea...",
                "May be you will help me?\nThen i give you some fruits\nfor free!"
            };
            _pedroHelloAgain = new[]
            {
                "Hello My Friend! Choose a level!",
                "Oh it's you again!\nWanna bring me some fruits?",
                "Hello, Traveller, fruits are waiting!"
            };
        }

        public IEnumerator PedroQuestLine()
        {
            if (_data.WasFirstStart == false)
            {
                for (int i = 0; i < _pedroQuestPhrases.Length; i++)
                {
                    StartCoroutine(TextFiller(_pedroQuestPhrases[i]));
                    _pedroImage.sprite = _pedroEmotions[i];
                    yield return new WaitForSeconds(_phrasesDelay);
                }
                _data.WasFirstStart = true;
            }
            else
            {
                StartCoroutine(TextFiller(_pedroHelloAgain[Random.Range(0, _pedroHelloAgain.Length)]));
                _pedroImage.sprite = _pedroEmotions.First();
            }
            MenuEvents.OnPedroAskedHelp();
        }

        // private IEnumerator Delayed

        private IEnumerator TextFiller(string entireText)
        {
            int index = 0;

            while (index <= entireText.Length)
            {
                _cloudText.text = entireText.Substring(0, index);
                index++;
                yield return new WaitForSeconds(_textDelay);
            }
        }

        public void LoadData(GameData data)
        {
            _data = data;
        }

        public void SaveData(ref GameData data)
        {
            data.WasFirstStart = _data.WasFirstStart;
        }
    }
}