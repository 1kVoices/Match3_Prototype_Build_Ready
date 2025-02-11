﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField]
        private bool _encrypt;
        private GameData _gameData;
        private DataFileHandler _dataHandler;
        private List<IData> _dataObjects;
        public static DataManager Singleton;

        private void Awake()
        {
            _dataHandler = new DataFileHandler(_encrypt);
            if (!Singleton)
            {
                Singleton = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);

            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            Start();
        }

        private void Start()
        {
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

            if (_gameData is null)
                NewGame();

            foreach (IData data in _dataObjects)
                data.LoadData(_gameData);
        }

        public void SaveGame()
        {
            foreach (IData data in _dataObjects.Where(data => data is not null))
                data.SaveData(ref _gameData);

            _dataHandler.Save(_gameData);
        }

        private void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            SaveGame();
        }
    }
}