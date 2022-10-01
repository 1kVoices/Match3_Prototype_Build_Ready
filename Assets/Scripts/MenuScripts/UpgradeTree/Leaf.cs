using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3
{
    public class Leaf : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private int _cost;
        private Image _image;
        private Button _button;
        private Hint _hint;
        [SerializeField]
        private string _hintText;
        [SerializeField]
        private GameObject _costGameObject;
        public int Cost => _cost;
        public bool IsUpgraded { get; private set; }

        private void Start()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
            _hint = GetComponentInChildren<Hint>();

            _hint.SetText(_hintText, _cost.ToString());
            _hint.gameObject.SetActive(false);
        }

        public void Upgrade()
        {
            IsUpgraded = true;
            DeactivateButton();
            _costGameObject.SetActive(false);
        }

        private void DeactivateButton()
        {
            _button.interactable = false;
            _image.color = Color.green;
        }

        public void OnPointerEnter(PointerEventData eventData) => _hint.gameObject.SetActive(true);
        public void OnPointerExit(PointerEventData eventData) => _hint.gameObject.SetActive(false);
    }
}