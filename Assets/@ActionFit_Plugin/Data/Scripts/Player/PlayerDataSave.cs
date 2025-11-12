using ActionFit_Plugin.Data.Scripts;

[System.Serializable]
public class PlayerDataSave : ISaveObject
{
    public bool adsRemove = false;
    
    public int clearLevel = 0;
    
    public void Flush() { }
}