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
            switch (i)
            {
                case 0:
                    if (MoneyManager.Singleton.Money < _leaves[0].Cost) return false;
                    MoneyManager.Singleton.RemoveMoney(_leaves[0].Cost);
                    return true;
                case 1:
                    if (MoneyManager.Singleton.Money < _leaves[1].Cost || !_leaves[0].IsUpgraded) return false;
                    MoneyManager.Singleton.RemoveMoney(_leaves[1].Cost);
                    return true;
                case 2:
                    if (MoneyManager.Singleton.Money < _leaves[2].Cost || !_leaves[0].IsUpgraded || !_leaves[1].IsUpgraded) return false;
                    MoneyManager.Singleton.RemoveMoney(_leaves[2].Cost);
                    return true;
                default:
                    return false;
            }
        }
    }
}