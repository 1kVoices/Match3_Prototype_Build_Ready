using UnityEngine;

namespace Match3
{
    public class BlackScreenComponent : StateMachineBehaviour
    {
        [SerializeField]
        private bool _isWhitening;
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_isWhitening) MenuEvents.OnBlackScreenBleached();
            else MenuEvents.OnBlackScreenDarken();
        }
    }
}