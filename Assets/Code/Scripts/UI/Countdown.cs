using System;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour, IUIElements
{
    private TMP_Text countdownText;

    public float totalTime = 60f;
    private float timeRemaining;

    private Action TimeIsFinish;

    private void Awake()
    {
        countdownText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        timeRemaining = totalTime;
    }

    private void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining < 0f)
        {
            timeRemaining = 0f;

            TimeIsFinish?.Invoke();
        }

        UpdateCountdownText();
    }

    private void OnEnable()
    {
        BuildManager.OnSec += ChangeValue;
    }

    private void OnDisable()
    {
        BuildManager.OnSec -= ChangeValue;
    }

    private void UpdateCountdownText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ChangeValue(float value)
    {
        timeRemaining += value;
    }
}
