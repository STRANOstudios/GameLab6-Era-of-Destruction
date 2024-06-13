using UnityEngine;

[DisallowMultipleComponent]
public class PauseManager : MonoBehaviour
{
    public static bool isGameInPaused = false;

    public delegate void Pause(bool value);
    public static event Pause IsPaused;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    private void OnEnable()
    {
        MenuController.Resume += PauseGame;
    }

    private void OnDisable()
    {
        MenuController.Resume -= PauseGame;
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        isGameInPaused = !isGameInPaused;
        Time.timeScale = isGameInPaused ? 0 : 1;

        Cursor.lockState = isGameInPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isGameInPaused;

        IsPaused?.Invoke(isGameInPaused);
    }

}
