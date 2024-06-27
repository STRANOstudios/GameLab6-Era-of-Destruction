using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent, RequireComponent(typeof(Slider))]
public class Attack1 : MonoBehaviour, IUIElements
{
    private Slider _slider;
    private bool isSetted = false;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        ShootingManager.Fire += ChangeValue;
    }

    private void OnDisable()
    {
        ShootingManager.Fire -= ChangeValue;
    }

    public void ChangeValue(float value)
    {
        if (!isSetted)
        {
            _slider.maxValue = value;
            isSetted = true;
        }

        _slider.value = value;
    }
}
