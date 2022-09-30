using UnityEngine;

namespace Match3
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField]
        private ExtraObjective[] _objectives;
        [SerializeField, Range(10, 50)]
        private int _rewardExp;
        public int RewardExp => _rewardExp;

        private void Start() => Instantiate(_objectives[UnityEngine.Random.Range(0, _objectives.Length)], transform);

        public void QuestCompleted()
        {
            LevelManager.Singleton.RewardPlayer(this);
        }
    }
}