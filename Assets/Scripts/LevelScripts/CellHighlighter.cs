using System.Collections;
using UnityEngine;

namespace Match3
{
    public class CellHighlighter : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Color _activeColor;
        private Color _inactiveColor;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _activeColor = new Color(255, 255, 255, 255);
            _inactiveColor = new Color(255, 255, 255, 0);
        }

        public void Activate()
        {
            _renderer.color = _activeColor;
        }

        public void Deactivate()
        {
            _renderer.color = _inactiveColor;
        }

        private IEnumerator Highlighting()
        {
            yield return null;
        }
    }
}