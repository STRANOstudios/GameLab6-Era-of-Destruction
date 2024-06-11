using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent, RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour, IUIElements
{
    private Slider _slider;
    private bool _isSetted = false;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        HealthManager.HealthValue += ChangeValue;
    }

    private void OnDisable()
    {
        HealthManager.HealthValue -= ChangeValue;
    }

    public void ChangeValue(float value)
    {
        if (!_isSetted)
        {
            _slider.maxValue = value;
            _isSetted = true;
        }

        _slider.value = value;
    }
}
