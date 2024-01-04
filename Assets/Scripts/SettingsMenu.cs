using Settings;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class SettingsMenu : MonoBehaviour
{
    private List<string> resolutionStrings;

    [Header("Video Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown fullscreenModeDropdown;
    [SerializeField] private TMP_Dropdown languageDropdown;


    [Header("Control Settings")]
    [SerializeField] private TMP_Dropdown gameDropdown;
    [SerializeField] private TMP_InputField sensitivityInputField;

    [Header("Buttons")]
    [SerializeField] private Button saveButton;

    private GameSettings currentSettings;
    private GameSettings selectedSettings;

    private string currentResolutionString;
    private string resolutionStringFormat = "{0}x{1}";

    bool isVideoChanged;
    bool isControlChanged;




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
        LoadSettings();
        UpdateSaveButtonInteractivity(false);
        isVideoChanged = false;
        isControlChanged = false;
    }

    private void LoadSettings()
    {
        GetCurrentValues();
        SetMenuValues();
    }

    private void GetCurrentValues()
    {
        currentSettings = SettingsManager.GetSettings();

        selectedSettings = new GameSettings(currentSettings);

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
        gameDropdown.value = MouseSensitivity.GameTitles.IndexOf(currentSettings.Control.Sensitivity.SourceGame);
        sensitivityInputField.text = currentSettings.Control.Sensitivity.SourceGameSensitivity.ToString();
    }


    #region VideoChanged

    public void OnFullscreenModeDropdownChanged(int index)
    {
        selectedSettings.Video.FullscreenMode = VideoSettings.GetFullScreenMode(index);
        isVideoChanged = true;
        UpdateSaveButtonInteractivity(true);
    }

    public void OnResolutionDropdownChanged(int index)
    {
        selectedSettings.Video.Resolution = VideoSettings.Resolutions[index];
        isVideoChanged = true;
        UpdateSaveButtonInteractivity(true);
    }

    public void OnLanguageDropdownChanged(int index)
    {
        LanguageManager.Instance.SetLanguage(LanguageManager.LocalesCodes[index]);
    }

    #endregion

    #region ControlChanged

    public void OnGameDropdownChanged(int index)
    {
        string sourceGame = selectedSettings.Control.Sensitivity.SourceGame;
        string targetGame = MouseSensitivity.GameTitles[index];

        float sensitivity = MouseSensitivity.ConvertBetweenGames(sourceGame, targetGame, selectedSettings.Control.Sensitivity.SourceGameSensitivity);
        sensitivityInputField.text = Math.Round(sensitivity, 3).ToString();

        selectedSettings.Control.Sensitivity.SourceGame = targetGame;

        isControlChanged = true;
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

        selectedSettings.Control.Sensitivity.SourceGameSensitivity = sensitivityValue;

        isControlChanged = true;
        UpdateSaveButtonInteractivity(true);
    }

    public void OnSensitivityEndEdit(string value)
    {
        if (!float.TryParse(value, out float sensitivityValue))
        {
            sensitivityInputField.text = "1";
        }
        Debug.Log($"Sensitivity changed to: {sensitivityValue}");
    }

    #endregion

    private void UpdateSaveButtonInteractivity(bool interactable)
    {
        saveButton.interactable = interactable;
    }

    public void SaveChanges()
    {
        if (!isVideoChanged) selectedSettings.Video = null;
        if (!isControlChanged) selectedSettings.Control = null;

        GameSettings settings = new GameSettings()
        {
            Video = selectedSettings.Video,
            Control = selectedSettings.Control
        };

        SettingsManager.ApplySettings(settings);
        SettingsManager.SaveSettings(settings);

        PauseMenu.Hide();
    }
}

public static class ResolutionExtension
{
    public static string ToStringExtended(this Resolution resolution, string format)
    {
        return string.Format(format,
                             resolution.width,
                             resolution.height);
    }
}
