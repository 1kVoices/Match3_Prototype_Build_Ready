using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Match3
{
    public static class SaveData
    {
        private static readonly string path = Application.persistentDataPath + "/player.data";

        public static void SavePlayerData(PlayerProgressComponent player)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            PlayerData data = new PlayerData(player);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static PlayerData LoadData()
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();

                return data;
            }
            return null;
        }
    }
}