using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject playerCameraObject;

    private PlayerCamera playerCameraScript;
    private bool isPaused = false;


    private void Awake()
    {
        playerCameraScript = playerCameraObject.GetComponent<PlayerCamera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !settingsPanel.active)
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
        playerCameraScript.enabled = false;

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
        playerCameraScript.enabled = true;

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
