using UnityEngine;

namespace Match3
{
    public class DataManagerInvocator : MonoBehaviour
    {
        private void Start() => DataManager.Singleton.FindData();
    }
}