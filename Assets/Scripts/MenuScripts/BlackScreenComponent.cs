using UnityEngine;

namespace Match3
{
    public class BlackScreenComponent : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            MenuEvents.Singleton.OnBlackScreenBleached();
        }
    }
}