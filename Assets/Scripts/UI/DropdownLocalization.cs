using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class DropdownLocalization : MonoBehaviour
{
    [SerializeField] private LocalizedStringTable localizedStringTable;
    [SerializeField] private TMPro.TMP_Dropdown dropdown;
    [SerializeField] private List<TMPro.TMP_Dropdown.OptionData> optionDataKeys;

    private void Start()
    {
        LocalizeDropdownItems();
    }

    public void LocalizeDropdownItems()
    {
        // Retrieve the localized string table from the localizedStringTable object
        StringTable localizedTable = localizedStringTable.GetTable();

        // Create a list to store localized dropdown items
        List<TMPro.TMP_Dropdown.OptionData> localizedOptions = new List<TMPro.TMP_Dropdown.OptionData>();

        foreach (TMPro.TMP_Dropdown.OptionData optionText in optionDataKeys)
        {
            // Retrieve the localized text for the current item from the string table
            string localizedText = localizedTable.GetEntry(optionText.text).LocalizedValue;
            localizedOptions.Add(new TMPro.TMP_Dropdown.OptionData(localizedText));
        }

        // Replace the original dropdown items with the localized ones.
        dropdown.options = localizedOptions;
    }
}
