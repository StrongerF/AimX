using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("GameManager");
                    instance = gameObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private PlayerCamera playerCamera;

    private bool isPaused = false;


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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !settingsPanel.activeSelf)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        // Show pause menu and disable camera control
        pauseMenu.SetActive(true);
        playerCamera.enabled = false;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void Resume()
    {
        // Hide pause menu and enable camera control
        pauseMenu.SetActive(false);
        playerCamera.enabled = true;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void Quit()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
