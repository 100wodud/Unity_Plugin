using System;

public static class PlayerDataEvent
{
    public static event Action LevelValueChange;

    public static void InvokeLevel() => LevelValueChange?.Invoke();

}