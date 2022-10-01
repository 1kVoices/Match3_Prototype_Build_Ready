using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class ToolsManager : MonoBehaviour, IData
    {
        private Button _breakButton;
        private Tool _hammer;
        private Tool _sledgeHammer;
        private GameData _data;
        private Tool _currentTool;

        private void Start()
        {
            _breakButton = GetComponentInChildren<Button>();
            _hammer = GetComponentInChildren<Hammer>();
            _sledgeHammer = GetComponentInChildren<SledgeHammer>();

            _breakButton.gameObject.SetActive(false);
            _hammer.SetStats(_data.HammerAmount, _data.HammerHitAmount, _data.HammerCd);
            _sledgeHammer.SetStats(_data.SledgeHammerAmount, _data.SledgeHammerHitAmount, _data.SledgeHammerCd);

            _hammer.ToolActive += HammerOnToolActive;
            _hammer.ToolUsed += ToolUsed;
            _sledgeHammer.ToolActive += HammerOnToolActive;
            _sledgeHammer.ToolUsed += ToolUsed;
        }

        private void ToolUsed()
        {
            _breakButton.gameObject.SetActive(false);
            SoundManager.Singleton.ToolHit();
        }

        private void HammerOnToolActive(Tool tool)
        {
            _currentTool = tool;
            _breakButton.gameObject.SetActive(true);
        }

        public void BreakTool()
        {
            _currentTool.BreakTool();
            _breakButton.gameObject.SetActive(false);
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

        private void OnDestroy()
        {
            _hammer.ToolActive -= HammerOnToolActive;
            _hammer.ToolUsed -= ToolUsed;
            _sledgeHammer.ToolActive -= HammerOnToolActive;
            _sledgeHammer.ToolUsed -= ToolUsed;
        }

        public void LoadData(GameData data) => _data = data;
        public void SaveData(ref GameData data) { }
    }
}