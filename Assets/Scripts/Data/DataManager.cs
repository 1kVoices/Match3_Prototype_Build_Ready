using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField]
        private bool _encrypt;
        private GameData _gameData;
        private DataFileHandler _dataHandler;
        private List<IData> _dataObjects;

        private void Start()
        {
            _dataHandler = new DataFileHandler(_encrypt);
            _dataObjects = FindAllData();
            LoadGame();
        }

        private static List<IData> FindAllData()
        {
            var dataObjects = FindObjectsOfType<MonoBehaviour>().OfType<IData>();
            return new List<IData>(dataObjects);
        }

        private void NewGame() => _gameData = new GameData();

        private void LoadGame()
        {
            _gameData = _dataHandler.Load();

            if (_gameData.IsNull())
                NewGame();

            foreach (IData data in _dataObjects)
                data.LoadData(_gameData);
        }

        private void SaveGame()
        {
            foreach (IData data in _dataObjects)
                data.SaveData(ref _gameData);

            _dataHandler.Save(_gameData);
        }

        private void OnApplicationQuit() => SaveGame();
    }
}