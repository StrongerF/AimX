using Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SettingsManager : MonoBehaviour 
{
    #region Instance
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
    #endregion

    // PlayerPrefs
    #region Registry Keys

    // Video settings
    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";
    private const string FullScreenKey = "FullscreenMode";

    // Control settings
    private const string GameSensitivityKey = "GameSensitivity";
    private const string MouseSensitivityKey = "MouseSensitivity";

    // Environment settings
    private const string WallSizeWidthKey = "WallSizeWidth";
    private const string WallSizeHeightKey = "WallSizeHeight";
    private const string WallDistanceKey = "WallDistance";

    private const string TargetSizeKey = "TargetSize";
    private const string TargetsCountKey = "TargetsCount";

    private const string TargetsMinDistanceKey = "TargetsMinDistance";
    private const string TargetMaxSpawnAttemptsKey = "TargetsMaxSpawnAttempts";
    
    #endregion


    private void Awake()
    {
        #region Instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private void Start()
    {
        LoadSettings();
    }

    public static void SaveSettings(GameSettings settings)
    {
        // Video
        if (settings.Video != null)
        {
            // I wanted to add a refresh rate feature, but I don't have a 144Hz monitor
        }

        // Control
        if (settings.Control != null)
        {
            PlayerPrefs.SetString(GameSensitivityKey, settings.Control.Sensitivity.SourceGame);
            PlayerPrefs.SetFloat(MouseSensitivityKey, settings.Control.Sensitivity.SourceGameSensitivity);
        }

        // Environment
        if (settings.Environment != null)
        {
            PlayerPrefs.SetFloat(WallSizeWidthKey, settings.Environment.WallWidth);
            PlayerPrefs.SetFloat(WallSizeHeightKey, settings.Environment.WallHeight);
            PlayerPrefs.SetInt(WallDistanceKey, settings.Environment.WallDistance);
            PlayerPrefs.SetFloat(TargetSizeKey, settings.Environment.TargetSize);
            PlayerPrefs.SetInt(TargetsCountKey, settings.Environment.TargetsCount);
            PlayerPrefs.SetFloat(TargetsMinDistanceKey, settings.Environment.TargetsMinDistance);
            PlayerPrefs.SetFloat(TargetMaxSpawnAttemptsKey, settings.Environment.MaxSpawnAttempts);
            
        }
        
        PlayerPrefs.Save();

        Debug.Log("SETTINGS SAVED!");
    }

    public static void ApplySettings(GameSettings settings)
    {

        if (settings.Video != null)
        {
            Debug.Log("APPLYING VIDEO SETTINGS...");
            Instance.ChangeResolution(settings.Video);
        }
        if (settings.Control != null)
        {
            Debug.Log("APPLYING CONTROL SETTINGS...");
            PlayerCamera.Sensitivity = Convert.ToSingle(Math.Round(settings.Control.Sensitivity.ModifiedSensitivity, 3));
        }
        if (settings.Environment != null)
        {
            Debug.Log("APPLYING ENVIRONMENT SETTINGS...");
            ObjectSpawner.WallSize = new Vector2(settings.Environment.WallWidth, settings.Environment.WallHeight);
            ObjectSpawner.WallDistance = settings.Environment.WallDistance;
            ObjectSpawner.TargetSize = settings.Environment.TargetSize;
            ObjectSpawner.TargetsCount = settings.Environment.TargetsCount;
            ObjectSpawner.MinTargetsDistance = settings.Environment.TargetsMinDistance;
            ObjectSpawner.MaxAttempts = settings.Environment.MaxSpawnAttempts;

            ObjectSpawner.Spawn();
        }

        Debug.Log("SETTINGS APPLIED!");
    }


    void ChangeResolution(VideoSettings videoSettings)
    {
        // When changing the RESOLUTION to a higher resolution
        // and at the same time changing the DISPLAY MODE
        // from FULLSCREEN to FULLSCREEN WINDOWED mode,
        // only the DISPLAY MODE is applied.

        // I use a coroutine to change the display mode in the previous frame
        // and change the resolution in the next frame
        StartCoroutine(ChangeResolutionCoroutine());

        IEnumerator ChangeResolutionCoroutine()
        {
            // Set display mode
            Screen.fullScreenMode = videoSettings.FullscreenMode;

            // Skip one frame
            yield return null;

            // Set resolution
            Screen.SetResolution(videoSettings.Resolution.width,
                                 videoSettings.Resolution.height,
                                 videoSettings.FullscreenMode);
        }
    }


    public static void LoadSettings()
    {
        GameSettings settings = GetSettings();
        ApplySettings(settings);
    }

    public static GameSettings GetSettings()
    {
        #region Video settings
        Resolution currentResolution = Screen.currentResolution;
        Resolution resolution = new Resolution()
        {
            width = currentResolution.width,
            height = currentResolution.height
        };
        FullScreenMode fullScreenMode = Screen.fullScreenMode;

        VideoSettings videoSettings = new VideoSettings(resolution, fullScreenMode);
        #endregion

        #region Control settings
        string selectedGame = PlayerPrefs.GetString(GameSensitivityKey, "Default");

        float sensMultiplier = 1f;
        // Check if the sensitivity multiplier for the selected game is available in the dictionary
        if (!MouseSensitivity.GameSensMultipliers.TryGetValue(selectedGame, out sensMultiplier))
        {
            selectedGame = MouseSensitivity.GameTitles.First();
        }

        float sensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, sensMultiplier);

        ControlSettings controlSettings = new ControlSettings(selectedGame, sensitivity);
        #endregion

        #region Environment settings
        EnvironmentSettings envSettings = new EnvironmentSettings();
        envSettings.WallWidth = PlayerPrefs.GetFloat(WallSizeWidthKey, envSettings.WallWidth);
        envSettings.WallHeight = PlayerPrefs.GetFloat(WallSizeHeightKey, envSettings.WallHeight);
        envSettings.WallDistance = PlayerPrefs.GetInt(WallDistanceKey, envSettings.WallDistance);
        envSettings.TargetSize = PlayerPrefs.GetFloat(TargetSizeKey, envSettings.TargetSize);
        envSettings.TargetsCount = PlayerPrefs.GetInt(TargetsCountKey, envSettings.TargetsCount);
        envSettings.TargetsMinDistance = PlayerPrefs.GetFloat(TargetsMinDistanceKey, envSettings.TargetsMinDistance);
        envSettings.MaxSpawnAttempts = PlayerPrefs.GetInt(TargetMaxSpawnAttemptsKey, envSettings.MaxSpawnAttempts);
        #endregion

        return new GameSettings(videoSettings, controlSettings, envSettings);
    }

}

