using UnityEngine;

namespace Match3
{
    public class Branch : MonoBehaviour
    {
        [SerializeField]
        private Leaf[] _leaves;
        public Leaf[] Leaves => _leaves;
        private Tree _parent;

        private void Start()
        {
            _parent = GetComponentInParent<Tree>();
        }

        public void Upgrade(int i)
        {
            if (IsUpgradable(i))
            {
                _parent.Upgrade(this, i);
                _leaves[i].Upgrade();
            }
        }

        private bool IsUpgradable(int i)
        {
            switch (i)
            {
                case 0:
                    return true;
                case 1:
                    return _leaves[0].IsUpgraded;
                case 2:
                    return _leaves[0].IsUpgraded && _leaves[1].IsUpgraded;
            }
            return false;
        }
    }
}