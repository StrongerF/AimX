using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    public void OpenSettingsMenu()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

}
