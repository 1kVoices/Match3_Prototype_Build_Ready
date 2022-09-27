using System.IO;
using UnityEngine;

namespace Match3
{
    public class DataFileHandler
    {
        private static readonly string path = Application.persistentDataPath + "/player.data";
        private const string _codeWord = "word";
        private readonly bool _encrypt;

        public DataFileHandler(bool useEncryption) => _encrypt = useEncryption;

        public GameData Load()
        {
            if (!File.Exists(path)) return null;

            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);

            string loadedString = reader.ReadToEnd();

            if (_encrypt)
                loadedString = EncryptDecrypt(loadedString);


            GameData loadedData = JsonUtility.FromJson<GameData>(loadedString);
            return loadedData;
        }

        public void Save(GameData data)
        {
            string storeData = JsonUtility.ToJson(data, true);

            if (_encrypt)
                storeData = EncryptDecrypt(storeData);


            FileStream stream = new FileStream(path, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);

            writer.Write(storeData);
            writer.Close();
            stream.Close();
        }

        private static string EncryptDecrypt(string data)
        {
            string modified = "";
            for (int i = 0; i < data.Length; i++)
                modified += (char)(data[i] ^ _codeWord[i % _codeWord.Length]);

            return modified;
        }
    }
}
