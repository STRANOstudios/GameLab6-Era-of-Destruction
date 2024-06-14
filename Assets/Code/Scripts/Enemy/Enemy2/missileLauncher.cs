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

    IEnumerator LauncherCycle(float delay) // rifare il metodo in modo corretto
    {
        // Reload 20%
        missile.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, 0f));

        yield return new WaitForSecondsRealtime(delay * 0.2f);

        // Targeting 50%
        Quaternion initialRotation = missile.rotation; // Rotazione iniziale del missile
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, -90f); // Rotazione di -90 gradi rispetto all'asse z

        float targetingTime = delay * 0.5f; // Time
        float elapsedTime = 0f;

        while (elapsedTime < targetingTime)
        {
            float rotationProgress = elapsedTime / targetingTime; // Calcolo del progresso della rotazione
            missile.rotation = Quaternion.Lerp(initialRotation, targetRotation, rotationProgress); // Interpolazione lineare tra la rotazione iniziale e quella target
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        missile.rotation = targetRotation;

        // Launch 30%
        exhaustFlames.Play();
        float launchTime = delay * 0.3f; // Tempo di lancio
        elapsedTime = 0f;

        while (elapsedTime < launchTime)
        {
            missile.position += speed * Time.deltaTime * Vector3.up; // Spostamento del missile verso l'alto a velocità costante
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        exhaustFlames.Stop();
    }


    public Transform Missile => missile;
    public ParticleSystem ExhaustFlames => exhaustFlames;
}
