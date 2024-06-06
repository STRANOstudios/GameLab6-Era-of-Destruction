using Cinemachine;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] float speed = 5f;
    [SerializeField, Min(0)] float rotationSpeed = 5f;

    private InputHandler inputHandler;

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
        if (inputHandler.moveInput != Vector2.zero)
        {
            Vector3 direction = new Vector3(inputHandler.moveInput.x, 0, inputHandler.moveInput.y);
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    void Rotation()
    {
        if (inputHandler.rotateInput != Vector2.zero)
        {
            transform.Rotate(Vector3.up, inputHandler.rotateInput.x * rotationSpeed * Time.deltaTime);
        }
    }

}
