using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class MenuPedroCloudComponent : MonoBehaviour
    {
        [SerializeField, Range(50, 300), Tooltip("Time in miliseconds")]
        private int _textDelay;

        [SerializeField, Range(3, 7)]
        private int _phrasesDelay;

        [SerializeField]
        private TextMeshProUGUI _textCloud;

        [SerializeField]
        private PlayerProgressComponent _player;

        [SerializeField]
        private Sprite[] _pedroEmotions;

        [SerializeField]
        private Image _pedroImage;

        private string[] _pedroQuestPhrases;

        private string[] _pedroHelloAgain;
        private void Start()
        {
            _pedroQuestPhrases = new[]
            {
                "Hello, Traveller!\nI'm Pedro The Fruit Seller",
                "I'd offer you buy my fruits but...",
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

        public async void PedroQuestLine()
        {
            bool isFirstStartUp = !_player.IsVeryFirstStart;

            if (isFirstStartUp)
            {
                for (int i = 0; i < _pedroQuestPhrases.Length; i++)
                {
                    TextFiller(_pedroQuestPhrases[i]);
                    if(_pedroImage.sprite != null) {_pedroImage.sprite = _pedroEmotions[i];}
                    await Task.Delay(_phrasesDelay * 1000);
                }
                _player.IsVeryFirstStart = true;
            }
            else
            {
                TextFiller(_pedroHelloAgain[Random.Range(0,_pedroHelloAgain.Length)]);
                _pedroImage.sprite = _pedroEmotions.First();
            }

            MenuEvents.Singleton.OnPedroAskedHelp();
        }
        private async void TextFiller(string entireText)
        {
            int index = 0;

            while (index <= entireText.Length)
            {
                _textCloud.text = entireText.Substring(0, index);
                index++;
                await Task.Delay(_textDelay);
            }
        }
    }
}