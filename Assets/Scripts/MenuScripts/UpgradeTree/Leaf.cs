using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class Leaf : MonoBehaviour
    {
        [SerializeField]
        private int _cost;
        public int Cost => _cost;
        public bool IsUpgraded { get; private set; }
        // public bool IsUpgradable { get; private set; } = true;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Button _button;

        public void Upgrade()
        {
            IsUpgraded = true;
            // IsUpgradable = !state;
            DeactivateButton();
        }

        private void DeactivateButton()
        {
            _button.interactable = false;
            _image.color = Color.green;
        }
    }
}