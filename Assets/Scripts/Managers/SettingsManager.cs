using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static Settings.ControlSettings;

public class SettingsManager : MonoBehaviour 
{
    private static SettingsManager instance;
    public static SettingsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SettingsManager>();
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("SettingsManager");
                    instance = gameObject.AddComponent<SettingsManager>();
                }
            }
            return instance;
        }
    }

    // PlayerPrefs
    #region Registry Keys

    // Video settings
    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";
    private const string FullScreenKey = "FullscreenMode";
    private const string RefreshRateNumeratorKey = "RefreshRateNumerator";
    private const string RefreshRateDenominatorKey = "RefreshRateDenominator";

    // Control settings
    private const string GameSensitivityKey = "GameSensitivity";
    private const string MouseSensitivityKey = "MouseSensitivity";

    #endregion

    // Change mouse sensitivity
    [SerializeField] private PlayerCamera playerCamera;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadSettings();
    }

    public static void SaveSettings(GameSettings settings)
    {
        // Video
        if (settings.Video.IsChanged)
        {
            PlayerPrefs.SetInt(ResolutionWidthKey, settings.Video.Resolution.width);
            PlayerPrefs.SetInt(ResolutionHeightKey, settings.Video.Resolution.height);
            PlayerPrefs.SetInt(FullScreenKey, VideoSettings.FullScreenModes.IndexOf(settings.Video.FullscreenMode));
            PlayerPrefs.SetInt(RefreshRateNumeratorKey, (int)settings.Video.Resolution.refreshRateRatio.numerator);
            PlayerPrefs.SetInt(RefreshRateDenominatorKey, (int)settings.Video.Resolution.refreshRateRatio.denominator);
            
        }

        // Control
        if (settings.Control.IsChanged)
        {
            PlayerPrefs.SetString(GameSensitivityKey, settings.Control.Sensitivity.SourceGame.Title);
            PlayerPrefs.SetFloat(MouseSensitivityKey, settings.Control.Sensitivity.SourceGameSensitivity);
        }
       

        PlayerPrefs.Save();
    }

    public static void ApplySettings(GameSettings settings)
    {
        if (settings.Video.IsChanged)
        {
            Debug.Log("APPLYING VIDEO SETTINGS");
            Screen.fullScreenMode = settings.Video.FullscreenMode;
            Screen.SetResolution(settings.Video.Resolution.width,
                                 settings.Video.Resolution.height,
                                 settings.Video.FullscreenMode,
                                 settings.Video.Resolution.refreshRateRatio);
            // TODO: Fix resolution changing
            // When changing the RESOLUTION to a higher resolution
            // and at the same time changing the DISPLAY MODE
            // from FULLSCREEN to FULLSCREEN WINDOWED mode,
            // only the DISPLAY MODE is applied
        }
        if (settings.Control.IsChanged)
        {
            Debug.Log("APPLYING CONTROL SETTINGS");
            Instance.playerCamera.sensX = settings.Control.Sensitivity.ModifiedSensitivity;
            Instance.playerCamera.sensY = settings.Control.Sensitivity.ModifiedSensitivity;
        }
        
        
        
        Debug.Log("CHANGES APPLIED");
    }

    public static void LoadSettings()
    {
        GameSettings settings = GetSettingsFromPlayerPrefs();
        ApplySettings(settings);
    }

    public static GameSettings GetSettingsFromPlayerPrefs()
    {
        // Video settings
        Resolution resolution = new Resolution()
        {
            width = PlayerPrefs.GetInt(ResolutionWidthKey, Screen.width),
            height = PlayerPrefs.GetInt(ResolutionHeightKey, Screen.height),
            refreshRateRatio = new RefreshRate()
            {
                numerator = (uint)PlayerPrefs.GetInt(RefreshRateNumeratorKey, 60),
                denominator = (uint)PlayerPrefs.GetInt(RefreshRateDenominatorKey, 1)
            }
        };
        FullScreenMode fullScreenMode = VideoSettings.GetFullScreenMode(PlayerPrefs.GetInt(FullScreenKey, 0));

        VideoSettings videoSettings = new VideoSettings(resolution, fullScreenMode) { IsChanged = true };


        // Control settings
        string selectedGameString = PlayerPrefs.GetString(GameSensitivityKey, "Default");
        MouseSensitivity.Game selectedGame = MouseSensitivity.Games.First(g => g.Title == selectedGameString);

        float sensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, selectedGame.SensitivityMultiplier);

        MouseSensitivity mouseSensitivity = new MouseSensitivity(selectedGame, sensitivity);

        ControlSettings controlSettings = new ControlSettings(mouseSensitivity) { IsChanged = true };

        return new GameSettings(videoSettings, controlSettings);
    }

}

