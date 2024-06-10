using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

[DisallowMultipleComponent]
public class MissileLauncher : MonoBehaviour
{
    [Header("Settings VFX")]
    [SerializeField, Min(0f)] float speed = 10f;

    [Header("References")]
    [SerializeField] Transform missile;
    [SerializeField] ParticleSystem exhaustFlames;

    Vector3 position;

    private void Awake()
    {
        position = missile.position;
    }

    public void Shoot(float time)
    {
        StartCoroutine(LauncherCycle(time - (time / 4)));
    }

    IEnumerator LauncherCycle(float time)
    {
        // Targeting
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, -90f);
        float rotationSpeed = 5f;

        while (Quaternion.Angle(missile.rotation, targetRotation) > 0.1f)
        {
            missile.rotation = Quaternion.RotateTowards(missile.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Launch
        exhaustFlames.Play();

        Vector3 launchPosition = new (missile.position.x, 20f, missile.position.z);

        float elapsedTime = 0f;
        while (elapsedTime < time / 3)
        {
            missile.transform.position = Vector3.Lerp(missile.position, launchPosition, speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(time / 6);

        // Reload
        exhaustFlames.Stop();
        missile.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, 0f));
    }

    public Transform Missile => missile;
    public ParticleSystem ExhaustFlames => exhaustFlames;
}
