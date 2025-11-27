using System;
using UnityEngine;

public class HeartSystem
{
    public static bool IsUnlimited = false;
    private static bool isRefreshing = false;
    public static void RefreshHeart()
    {
        if (isRefreshing) return; 
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        IsUnlimited = PlayerData.HeartUnlimitedTime > now;
        if (PlayerData.Heart >= PlayerData.MaxHeart) return;
        if (PlayerData.LastHeartUsedTime == 0) return;
        isRefreshing = true;

        long totalInterval = now - PlayerData.LastHeartUsedTime;

        const int interval = PlayerData.HeartInterval * 60; 
        int regenCount = (int)(totalInterval / interval);

        if (regenCount > 0)
        {
            int possibleRegen = Mathf.Min(regenCount, PlayerData.MaxHeart - PlayerData.Heart);
            PlayerData.Heart += possibleRegen;

            if (PlayerData.Heart >= PlayerData.MaxHeart)
            {
                PlayerData.LastHeartUsedTime = 0; 
            }
            else
            { 
                PlayerData.LastHeartUsedTime += possibleRegen * interval;
            }
        }
        
        isRefreshing = false;
    }
    public static bool UseHeart()
    {
        RefreshHeart();

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (IsUnlimited)
        {
            return true;
        }
        
        if (PlayerData.Heart > 0)
        {
            PlayerData.Heart--;
            if (PlayerData.Heart == PlayerData.MaxHeart - 1)
            {
                PlayerData.LastHeartUsedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            return true;
        }

        return false; 
    }
    
    public static void AddHeart()
    {
        if (PlayerData.Heart < PlayerData.MaxHeart)
        {
            PlayerData.Heart++;
            if (PlayerData.Heart == PlayerData.MaxHeart) PlayerData.LastHeartUsedTime = 0;
            RefreshHeart();
        }
    }
    
    public static string GetTimeNextHeartString()
    {
        RefreshHeart();
        
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (PlayerData.HeartUnlimitedTime > now)
        {
            long timeCheck = Math.Max(0, PlayerData.HeartUnlimitedTime - now);
            TimeSpan unlimit = TimeSpan.FromSeconds(timeCheck);

            if (unlimit.TotalHours >= 1)
                return $"{(int)unlimit.TotalHours}HR";
            return unlimit.ToString(@"mm\:ss");
        } 
        if (PlayerData.Heart >= PlayerData.MaxHeart) return "Max";

        long nextHeartTime = PlayerData.LastHeartUsedTime + PlayerData.HeartInterval * 60;
        long remaining = Math.Max(0, nextHeartTime - now);

        TimeSpan time = TimeSpan.FromSeconds(remaining);
        return time.ToString(@"mm\:ss"); 
    }
    
    public static void AddHeartUnlimitedTime(int minutes)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long addedSeconds = minutes * 60;
        if (PlayerData.HeartUnlimitedTime > now)
        {
            PlayerData.HeartUnlimitedTime += addedSeconds;
        }
        else
        {
            PlayerData.HeartUnlimitedTime = now + addedSeconds;
        }
    }
}
