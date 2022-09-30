using UnityEngine;

namespace Match3
{
    public class ToolsManager : MonoBehaviour, IData
    {
        private Tool _hammer;
        private Tool _sledgeHammer;
        private GameData _data;

        private void Start()
        {
            _hammer = GetComponentInChildren<Hammer>();
            _sledgeHammer = GetComponentInChildren<SledgeHammer>();

            _hammer.SetStats(_data.HammerAmount, _data.HammerHitAmount, _data.HammerCd);
            _sledgeHammer.SetStats(_data.SledgeHammerAmount, _data.SledgeHammerHitAmount, _data.SledgeHammerCd);
        }

        public void ActivateButtons()
        {
            if (_hammer.IsInteractable)
                _hammer.EnableButton();
            if (_sledgeHammer.IsInteractable)
                _sledgeHammer.EnableButton();
        }

        public void DisableButtons()
        {
            _hammer.DisableButton();
            _sledgeHammer.DisableButton();
        }

        public void LoadData(GameData data) => _data = data;
        public void SaveData(ref GameData data) { }
    }
}