using TMPro;
using UnityEngine;

public class UILoseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ScoreCounter scoreCounter;
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text time;

    private float timer = 0f;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!scoreCounter) Debug.LogWarning("scoreCounter not assigned");
        if (!time) Debug.LogWarning("time TMP_text not assigned");
        if (!score) Debug.LogWarning("score TMP_text not assigned");
    }

#endif

    private void Awake()
    {
        timer = Time.realtimeSinceStartup;
    }

    private void OnEnable()
    {
        LoseController.GameOver += GameOver;
    }

    private void OnDisable()
    {
        LoseController.GameOver -= GameOver;        
    }

    private void GameOver()
    {
        time.text = (Time.realtimeSinceStartup - timer).ToString("F2");
        score.text = scoreCounter.GetScore.ToString();
    }
}
