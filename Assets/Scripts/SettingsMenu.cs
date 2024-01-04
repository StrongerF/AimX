using Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static Settings.ControlSettings;

public class SettingsMenu : MonoBehaviour
{
    private List<string> resolutionStrings;

    [Header("Video Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private string resolutionStringFormat = "{0}x{1}@{2}Hz";
    [SerializeField] private TMP_Dropdown fullscreenModeDropdown;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("Control Settings")]
    [SerializeField] private TMP_Dropdown gameDropdown;
    [SerializeField] private TMP_InputField sensitivityInputField;

    [Header("Buttons")]
    [SerializeField] private Button saveButton;

    private GameSettings currentSettings;

    private string currentResolutionString;

    #region Video Settings
    private VideoSettings selectedVideoSettings;
    #endregion
    #region Control Settings
    private ControlSettings selectedControlSettings;
    #endregion


    

    private void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        #region Video Settings

        // Add values in resolution dropdown
        resolutionDropdown.ClearOptions();
        resolutionStrings = new List<string>(VideoSettings.Resolutions.Length);

        foreach (var resolution in VideoSettings.Resolutions)
        {
            string resolutionString = resolution.ToStringExtended(resolutionStringFormat);
            resolutionStrings.Add(resolutionString);
        }

        resolutionDropdown.AddOptions(resolutionStrings);

        // Add values in language dropdown
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(LanguageManager.LocalesNames);

        #endregion
        #region Control Settings

        gameDropdown.ClearOptions();
        gameDropdown.AddOptions(MouseSensitivity.GameTitles);

        #endregion
    }
    private void OnEnable()
    {
        LoadCurrentSettings();
        selectedVideoSettings.IsChanged = false;
        selectedControlSettings.IsChanged = false;
    }

    private void LoadCurrentSettings()
    {
        SetCurrentValues();
        SetMenuValues();
    }

    private void SetCurrentValues()
    {
        currentSettings = SettingsManager.GetSettingsFromPlayerPrefs();

        selectedVideoSettings = currentSettings.Video;
        selectedControlSettings = currentSettings.Control;

        currentResolutionString = currentSettings.Video.Resolution.ToStringExtended(resolutionStringFormat);
        Debug.Log(currentResolutionString);
        
    }

    private void SetMenuValues()
    {
        // Video
        languageDropdown.value = LanguageManager.LocalesNames.IndexOf(LanguageManager.CurrentLanguage);
        resolutionDropdown.value = resolutionStrings.IndexOf(currentResolutionString);
        fullscreenModeDropdown.value = VideoSettings.FullScreenModes.IndexOf(currentSettings.Video.FullscreenMode);

        // Control
        gameDropdown.value = MouseSensitivity.Games.IndexOf(currentSettings.Control.Sensitivity.SourceGame);
        sensitivityInputField.text = currentSettings.Control.Sensitivity.SourceGameSensitivity.ToString();

        UpdateSaveButtonInteractivity(false);
    }



    public void OnFullscreenModeDropdownChanged(int index)
    {
        selectedVideoSettings.FullscreenMode = VideoSettings.GetFullScreenMode(index);
        selectedVideoSettings.IsChanged = true;
        UpdateSaveButtonInteractivity(true);
    }

    public void OnResolutionDropdownChanged(int index)
    {
        selectedVideoSettings.Resolution = VideoSettings.Resolutions[index];
        selectedVideoSettings.IsChanged = true;
        UpdateSaveButtonInteractivity(true);
    }

    public void OnLanguageDropdownChanged(int index)
    {
        LanguageManager.Instance.SetLanguage(LanguageManager.LocalesCodes[index]);
    }

    public void OnGameDropdownChanged(int index)
    {
        MouseSensitivity.Game sourceGame = selectedControlSettings.Sensitivity.SourceGame;
        MouseSensitivity.Game targetGame = MouseSensitivity.Games[index];

        float sensitivity = MouseSensitivity.ConvertBetweenGames(sourceGame, targetGame, selectedControlSettings.Sensitivity.SourceGameSensitivity);
        sensitivityInputField.text = Math.Round(sensitivity, 3).ToString();

        selectedControlSettings.Sensitivity.SourceGame = targetGame;

        selectedControlSettings.IsChanged = true;
        UpdateSaveButtonInteractivity(true);
    }

    public void OnSensitivityValueChanged(string value)
    {
        float sensitivityValue;

        if (!float.TryParse(value, out sensitivityValue))
        {
            Debug.LogError("Invalid sensitivity value entered.");
            return;
        }

        if (sensitivityValue < 0) sensitivityValue = 0;

        selectedControlSettings.Sensitivity.SourceGameSensitivity = sensitivityValue;
        Debug.Log($"Sensitivity changed to: {sensitivityValue}");

        selectedControlSettings.IsChanged = true;
        UpdateSaveButtonInteractivity(true);
    }

    public void OnSensitivityEndEdit(string value)
    {
        if (!float.TryParse(value, out float sensitivityValue))
        {
            sensitivityInputField.text = "1";
        }
    }

    private void UpdateSaveButtonInteractivity(bool interactable)
    {
        saveButton.interactable = interactable;
    }

    public void SaveChanges()
    {
        GameSettings settings = new GameSettings(selectedVideoSettings, selectedControlSettings);

        SettingsManager.ApplySettings(settings);
        SettingsManager.SaveSettings(settings);

        Debug.Log("CHANGES SAVED");

        PauseMenu.Instance.HideSettingsMenu();
    }
}

public static class ResolutionExtension
{
    public static string ToStringExtended(this Resolution resolution, string format)
    {
        return string.Format(format,
                             resolution.width,
                             resolution.height,
                             resolution.refreshRateRatio.value.ToString("0"));
    }
}
