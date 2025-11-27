using System;

public static class PlayerDataEvent
{
    public static event Action LevelValueChange;
    public static event Action HeartValueChange;
    public static event Action UnlimitedHeartChange;

    public static void InvokeLevel() => LevelValueChange?.Invoke();
    public static void InvokeHeart() => HeartValueChange?.Invoke();
    public static void InvokeUnlimitedHeart() => UnlimitedHeartChange?.Invoke();

}