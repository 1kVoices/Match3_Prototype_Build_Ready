using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3
{
    public class DifficultySwitcher : MonoBehaviour, IData
    {
        [SerializeField]
        private LevelButton[] _allLevels;
        [SerializeField]
        private GameObject[] _switchers;
        private GameData _data;
        private LinkedList<LevelButton> _easyLevels;
        private LinkedList<LevelButton> _mediumLevels;
        private LinkedList<LevelButton> _hardLevels;

        private void Start()
        {
            _easyLevels = new LinkedList<LevelButton>();
            _mediumLevels = new LinkedList<LevelButton>();
            _hardLevels = new LinkedList<LevelButton>();

            var levelsCount = _data.LevelsCompleted.Count(kvp => kvp.Value);

            for (int i = 0; i <= levelsCount; i++)
            {
                _allLevels[i].SetInteractionState(true);
                if (i == levelsCount) continue;
                _allLevels[i].SetRewardState(true);
            }

            for (int i = 0; i < _allLevels.Length; i++)
            {
                _allLevels[i].gameObject.SetActive(false);

                if (i <= 3)
                    _easyLevels.AddLast(_allLevels[i]);
                else if (i <= 7)
                    _mediumLevels.AddLast(_allLevels[i]);
                else
                    _hardLevels.AddLast(_allLevels[i]);
            }
        }

        public void ActivateSwitchers()
        {
            foreach (GameObject switcher in _switchers)
                switcher.gameObject.SetActive(true);
        }

        public void EnableLevel_EditorEvent(int difficulty)
        {
            switch (difficulty)
            {
                case 0: //easy
                    EnableLevelGroup(_easyLevels, true);
                    EnableLevelGroup(_mediumLevels, false);
                    EnableLevelGroup(_hardLevels, false);
                    break;
                case 1: //medium
                    EnableLevelGroup(_easyLevels, false);
                    EnableLevelGroup(_mediumLevels, true);
                    EnableLevelGroup(_hardLevels, false);
                    break;
                case 2: //hard
                    EnableLevelGroup(_easyLevels, false);
                    EnableLevelGroup(_mediumLevels, false);
                    EnableLevelGroup(_hardLevels, true);
                    break;
            }
        }

        private static void EnableLevelGroup(LinkedList<LevelButton> _levels, bool isEnable)
        {
            foreach (LevelButton levelButton in _levels)
            {
                switch (isEnable)
                {
                    case true:
                        levelButton.gameObject.SetActive(true);
                        if (levelButton.IsShowedUp == false)
                            levelButton.ShowUp();
                        if (levelButton.IsRewardShowed)
                            levelButton.ShowReward();
                        break;
                    case false:
                        levelButton.gameObject.SetActive(false);
                        levelButton.HideReward();
                        break;
                }
            }
        }

        public void LoadData(GameData data) => _data = data;
        public void SaveData(ref GameData data) { }
    }
}