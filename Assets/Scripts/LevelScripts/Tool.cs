using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public abstract class Tool : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Button _button;
        protected bool IsInput;
        public int Count { get; private set; } = 1;

        private void Start()
        {
            LevelManager.Singleton.OnPlayerClick += PlayerClicked;
            if (Count < 1) _button.interactable = false;
            _text.text = Count.ToString();
        }

        public void OnClick()
        {
            LevelManager.Singleton.SetToolState(true);
            IsInput = true;
        }

        protected virtual void PlayerClicked(Cell cell)
        {
            if (!cell.CurrentChip.IsInteractable || cell.CurrentChip.IsAnimating) return;
            Count--;
            _text.text = Count.ToString();
        }

        protected void UpdateState()
        {
            LevelManager.Singleton.SetToolState(false);
            if (Count < 1) _button.interactable = false;
            IsInput = false;
        }

        private void OnDestroy()
        {
            LevelManager.Singleton.OnPlayerClick -= PlayerClicked;
        }
    }
}