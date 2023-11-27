using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageChangeHandler : MonoBehaviour
{
    [SerializeField] private DropdownLocalization dropdownLocalization;

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(Locale locale)
    {
        dropdownLocalization.LocalizeDropdownItems();
    }
}
