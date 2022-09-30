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
            if (!IsUpgradable(i)) return;
            _parent.Upgrade(this, i);
            _leaves[i].Upgrade();
        }

        private bool IsUpgradable(int i)
        {
            return i switch
            {
                0 => true, //cost
                1 => _leaves[0].IsUpgraded,
                2 => _leaves[0].IsUpgraded && _leaves[1].IsUpgraded,
                _ => false
            };
        }
    }
}