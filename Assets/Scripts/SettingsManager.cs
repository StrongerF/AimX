using System.Collections.Generic;
using UnityEngine;

public static class SettingsManager
{
    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";
    private const string FullScreenKey = "FullscreenMode";
    private const string RefreshRateKey = "RefreshRate";

    public static void SaveChanges(GameSettings settings)
    {
        PlayerPrefs.SetInt(ResolutionWidthKey, settings.ResolutionWidth);
        PlayerPrefs.SetInt(ResolutionHeightKey, settings.ResolutionHeight);
        PlayerPrefs.SetInt(FullScreenKey, GameSettings.FullScreenModes.IndexOf(settings.FullscreenMode));
        PlayerPrefs.SetInt(RefreshRateKey, settings.RefreshRate);

        if (settings.IsSafe)
        {
            Debug.Log("SAFE");
        }

        PlayerPrefs.Save();
    }

    public static void ApplySettings(GameSettings settings)
    {
        Screen.SetResolution(settings.ResolutionWidth,
                             settings.ResolutionHeight,
                             settings.FullscreenMode,
                             settings.RefreshRate);

        Debug.Log("CHANGES APPLIED");
    }

    public static void ApplySettingsOnStartup()
    {
        int resWidth = PlayerPrefs.GetInt(ResolutionWidthKey, Screen.width);
        int resHeight = PlayerPrefs.GetInt(ResolutionHeightKey, Screen.height);
        FullScreenMode fullScreenMode = GameSettings.GetFullScreenMode(PlayerPrefs.GetInt(FullScreenKey, 0));
        int refreshRate = PlayerPrefs.GetInt(RefreshRateKey, 60);
        GameSettings settings = new GameSettings(resWidth, resHeight, fullScreenMode, refreshRate);
        ApplySettings(settings);
    }

}

public class GameSettings
{
    public static readonly List<FullScreenMode> FullScreenModes = new List<FullScreenMode>
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };

    public enum Game
    {
        CSGO,
        Valorant
    }

    public static readonly Dictionary<Game, decimal> GameSensitivities = new Dictionary<Game, decimal>
    {
        { Game.CSGO, 0.44m },
        { Game.Valorant, 1.3992m }
    };

    public bool IsSafe { get; }
    public int ResolutionWidth { get; set; }
    public int ResolutionHeight { get; set; }
    public FullScreenMode FullscreenMode { get; set; }
    public int RefreshRate { get; set; }


    public GameSettings(int width, int height, FullScreenMode fullscreenMode, int refreshRate)
    {
        IsSafe = false;

        ResolutionWidth = width;
        ResolutionHeight = height;
        FullscreenMode = fullscreenMode;
        RefreshRate = refreshRate;
    }

    public static FullScreenMode GetFullScreenMode(int index)
    {
        switch (index)
        {
            case 0:
                return FullScreenMode.ExclusiveFullScreen;
            case 1:
                return FullScreenMode.FullScreenWindow;
            case 2:
                return FullScreenMode.Windowed;
            default:
                return FullScreenMode.ExclusiveFullScreen;
        }
    }

    public static decimal ConvertSensivityFromGame(Game firstGame, Game secondGame, decimal currentSensitivity)
    {
        return GameSensitivities[firstGame] * GameSensitivities[secondGame];
    }
}