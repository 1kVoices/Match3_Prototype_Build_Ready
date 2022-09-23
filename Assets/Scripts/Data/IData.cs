namespace Match3.Data
{
    public interface IData
    {
        void LoadData(GameData data);

        void SaveData(ref GameData data);
    }
}