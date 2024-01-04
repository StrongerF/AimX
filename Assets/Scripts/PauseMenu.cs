using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu instance;
    public static PauseMenu Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PauseMenu>();
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("PauseMenu");
                    instance = gameObject.AddComponent<PauseMenu>();
                }
            }
            return instance;
        }
    }


    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowSettingsMenu()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void HideSettingsMenu()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

}
