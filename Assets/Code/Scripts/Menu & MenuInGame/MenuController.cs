using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
public class MenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The name of the scene to be loaded")] private string sceneToBeLoad;

    [SerializeField] private AudioClip sfxClick;

    private AudioSource audioSource;

    public delegate void resumeDelegate();
    public static event resumeDelegate Resume;

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

    #endregion
}