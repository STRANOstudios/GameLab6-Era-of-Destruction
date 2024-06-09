using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private InputHandler inputHandler;

    public static bool isGameInPaused = false;
    private bool isPressable = true;

    public delegate bool Pause(bool value);
    public static event Pause isPaused;

    private void Awake()
    {
        #region Singleton

        if (Instance != null)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        #endregion

        // change value for menu
        Cursor.lockState = isGameInPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isGameInPaused;
    }

    private void Start()
    {
        inputHandler = InputHandler.Instance;
    }

    private void FixedUpdate()
    {
        if (inputHandler.escapeTrigger && isPressable)
        {
            StartCoroutine(Escape());
            PauseGame();
        }
    }

    IEnumerator Escape()
    {
        isPressable = false;
        yield return new WaitForSeconds(0.3f);
        isPressable = true;
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

        isPaused?.Invoke(isGameInPaused);
    }
}
