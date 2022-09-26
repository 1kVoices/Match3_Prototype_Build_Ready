using UnityEngine;

namespace Match3
{
    public abstract class Tree : MonoBehaviour
    {
        [SerializeField]
        protected Branch LeftBranch;
        [SerializeField]
        protected Branch MidBranch;
        [SerializeField]
        protected Branch RightBranch;

        public abstract void Upgrade(Branch branch, int level);
    }
}