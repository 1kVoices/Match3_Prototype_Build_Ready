using UnityEngine;

namespace Match3
{
    public class MenuAnimatorEventReceiver : MonoBehaviour
    {
        [SerializeField]
        private LevelButton _parent;

        public void ShowUpEnded() => _parent.ShowedUp();
    }
}