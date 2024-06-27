using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Fireratio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Slider sliderAttack1;
    [SerializeField] Slider sliderAttack2;
    [Space]
    [SerializeField] Image imageAttack1;
    [SerializeField] Image imageAttack2;

    [SerializeField] Sprite iconAttack1;
    [SerializeField] Sprite iconAttack2;

    private bool isSetted1 = false;
    private bool isSetted2 = false;

    private Sprite defaultIcon1;
    private Sprite defaultIcon2;

    private void Awake()
    {
        defaultIcon1 = imageAttack1.sprite;
        defaultIcon2 = imageAttack2.sprite;
    }

    private void OnEnable()
    {
        ShootingManager.Load1 += ChangeValue1;
        ShootingManager.Load2 += ChangeValue2;
    }

    private void OnDisable()
    {
        ShootingManager.Load1 -= ChangeValue1;
        ShootingManager.Load2 -= ChangeValue2;
    }

    public void ChangeValue1(float value)
    {
        if (!isSetted1)
        {
            sliderAttack1.maxValue = value;
            isSetted1 = true;
        }

        sliderAttack1.value = value;

        imageAttack1.sprite = sliderAttack1.value == sliderAttack1.maxValue ? iconAttack1 : defaultIcon1;
    }

    public void ChangeValue2(float value)
    {
        if (!isSetted2)
        {
            sliderAttack2.maxValue = value;
            isSetted2 = true;
        }

        sliderAttack2.value = value;

        imageAttack2.sprite = sliderAttack2.value == sliderAttack2.maxValue ? iconAttack2 : defaultIcon2;
    }
}
