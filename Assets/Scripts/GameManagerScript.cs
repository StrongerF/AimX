using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject playerCameraObject;

    private PlayerCamera playerCameraScript;
    private bool isPaused = false;


    private void Awake()
    {
        playerCameraScript = playerCameraObject.GetComponent<PlayerCamera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
        pauseMenuUI.SetActive(true);
        playerCameraScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        playerCameraScript.enabled = true;

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
