using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(GameManager))]
public class GameManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool inGame = false;

    public static GameManager Instance { get; private set; }

    [Flags]
    enum State
    {
        MENU,
        INGAME,
        MENUINGAME
    }

    private State currentState = State.MENU;

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

        Cursor.lockState = !inGame ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !inGame;
    }

    private void Start()
    {
        inputHandler = InputHandler.Instance;
    }

    private void FixedUpdate()
    {
        if (inputHandler.EscapeTrigger && isPressable && currentState != State.MENU)
        {
            StartCoroutine(Escape());
            PauseGame();
        }
    }

    private void OnEnable()
    {
        MenuController.Resume += PauseGame;
        MenuController.Play += StartGame;
        MenuController.Return += ReturnMenu;
    }

    private void OnDisable()
    {
        MenuController.Resume -= PauseGame;
        MenuController.Play -= StartGame;
        MenuController.Return -= ReturnMenu;
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

        currentState = isGameInPaused ? State.MENUINGAME : State.INGAME;

        isPaused?.Invoke(isGameInPaused);
    }

    private void StartGame()
    {
        currentState = State.INGAME;
        inGame = true;
        isGameInPaused = false;

        PauseGame();
    }

    private void ReturnMenu()
    {
        currentState = State.MENU;

        inGame = false;
        isGameInPaused = false;

        PauseGame();
    }
}
