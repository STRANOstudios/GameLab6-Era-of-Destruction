using System.Collections;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(GameManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private InputHandler inputHandler;

    public static bool isGameInPaused = true;
    private bool isPressable = true;

    public delegate bool Pause(bool value);
    public static event Pause isPaused;

    private void Awake()
    {
        #region Singleton

        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        #endregion

        // change value for menu
        Cursor.lockState = isGameInPaused ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = isGameInPaused;
    }

    private void Start()
    {
        inputHandler = InputHandler.Instance;
    }

    private void FixedUpdate()
    {
        if (inputHandler.EscapeTrigger && isPressable)
        {
            StartCoroutine(Escape());
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

        Cursor.lockState = isGameInPaused ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = isGameInPaused;

        isPaused?.Invoke(isGameInPaused);
    }
}
