using TMPro;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(TMP_Text))]
public class ScoreCounter : MonoBehaviour, IUIElements
{
    private TMP_Text _score;
    private float _scoreValue;

    private void Awake()
    {
        _score = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        BuildManager.OnScore += ChangeValue;
        Health.OnScore += ChangeValue;
    }

    private void OnDisable()
    {
        BuildManager.OnScore -= ChangeValue;
        Health.OnScore -= ChangeValue;
    }

    public void ChangeValue(float value)
    {
        _scoreValue += value;
        _score.text = "" + _scoreValue;
    }
}
