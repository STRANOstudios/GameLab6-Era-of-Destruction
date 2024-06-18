using System;
using UnityEngine;

[DisallowMultipleComponent]
public class LoseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Canvas canvas;

    public static Action GameOver;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!canvas) Debug.LogWarning("canvas not assigned");
    }

#endif

    private void OnEnable()
    {
        HealthManager.playerDeath += Lose;
        Countdown.TimeIsFinish += Lose;
    }

    private void OnDisable()
    {
        HealthManager.playerDeath -= Lose;    
        Countdown.TimeIsFinish -= Lose;
    }

    void Lose()
    {
        GameOver?.Invoke();

        canvas.gameObject.SetActive(true);
    }
}
