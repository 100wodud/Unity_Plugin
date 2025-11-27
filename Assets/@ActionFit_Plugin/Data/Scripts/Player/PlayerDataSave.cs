using ActionFit_Plugin.Data.Scripts;

[System.Serializable]
public class PlayerDataSave : ISaveObject
{
    public bool adsRemove = false;
    
    public int clearLevel = 0;
    
    
    //하트
    public int heart = 5;
    public long lastHeartUsedTime = 0;
    public long heartUnlimitedTime = 0;
    
    public void Flush() { }
}