
namespace antoinegleisberg.SaveSystem
{
    public interface ISaveable
    {
        public void LoadData(object data);

        public object GetSaveData();
    }
}