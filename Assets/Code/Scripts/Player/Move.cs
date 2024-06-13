using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Move : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] float speed = 5f;
    [SerializeField, Min(0)] float rotationSpeed = 5f;

    private InputHandler inputHandler;

    public static Move Instance { get; private set; }

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
        Rotation();
    }

    void Motion()
    {
        if (inputHandler.MoveInput != Vector2.zero)
        {
            Vector3 direction = new(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);
            transform.Translate(speed * Time.deltaTime * direction);

            OnMove?.Invoke();
        }
    }

    void Rotation()
    {
        if (inputHandler.RotateInput != Vector2.zero)
        {
            transform.Rotate(Vector3.up, inputHandler.RotateInput.x * rotationSpeed * Time.deltaTime * (MouseSensibility.sensibility * 2));
        }
    }
}
