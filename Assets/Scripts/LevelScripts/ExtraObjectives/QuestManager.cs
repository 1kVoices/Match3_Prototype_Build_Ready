using UnityEngine;

namespace Match3
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField]
        private ExtraObjective[] _objectives;
        private ExtraObjective _current;

        private void Start()
        {
            _current = Instantiate(_objectives[UnityEngine.Random.Range(0, _objectives.Length)], transform);
        }

        public void QuestCompleted()
        {
            //todo reward
        }
    }
}