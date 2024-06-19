using System;
using UnityEngine;

public class MoveNightmare : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] float speed = 5f;
    [SerializeField, Min(0)] float rotationSpeed = 5f;

    private InputHandler inputHandler;

    public static MoveNightmare Instance { get; private set; }

    public static Action OnMove;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        inputHandler = InputHandler.Instance;
    }

    void Update()
    {
        Motion();
    }

    void Motion()
    {
        if (inputHandler.MoveInput != Vector2.zero)
        {
            Vector3 targetDirection = new Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.position += speed * Time.deltaTime * transform.forward;

            OnMove?.Invoke();
        }
    }
}
