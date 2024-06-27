using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
public class MenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The name of the scene to be loaded")] private string sceneToBeLoad;

    [SerializeField] private AudioClip sfxClick;

    private AudioSource audioSource;

    public delegate void MenuEvent();
    public static event MenuEvent Resume;
    public static event MenuEvent Play;
    public static event MenuEvent Return;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    #region Menu Buttons

    public void PlayButton()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene(sceneToBeLoad);
#else
        SceneManager.LoadScene(1);
#endif
        Play?.Invoke();
        Pressed();
    }

    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Pressed();
    }

    public void ReturnButton()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene(sceneToBeLoad);
#else
        SceneManager.LoadScene(0);
#endif
        Return?.Invoke();
        Pressed();
    }

    public void ResumeButton()
    {
        Resume?.Invoke();
        Pressed();
    }

    public void Pressed()
    {
        if (audioSource && sfxClick) audioSource.PlayOneShot(sfxClick);
    }

    public void PlayAgain()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#else
        SceneManager.LoadScene(1);
#endif
        Pressed();
    }

    #endregion
}