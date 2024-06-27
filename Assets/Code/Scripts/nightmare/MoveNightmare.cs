using System;
using UnityEngine;

public class MoveNightmare : MonoBehaviour
{
    public static MoveNightmare Instance { get; private set; }

    [Header("Settings")]
    [SerializeField, Min(0)] float speed = 5f;
    [SerializeField, Min(0)] float rotationSpeed = 5f;

    [Header("References")]
    [SerializeField] Transform rotator;

    private InputHandler inputHandler;

    private Vector3 mousePosition = Vector3.zero;

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
        Rotetion();
    }

    void Motion()
    {
        if (inputHandler.MoveInput != Vector2.zero)
        {
            Vector3 targetDirection = new Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y).normalized;

            transform.position += targetDirection * speed * Time.deltaTime;

            OnMove?.Invoke();
        }
    }

    private void Rotetion()
    {
        // Ottieni la posizione del mouse
        Vector3 mousePosition = Input.mousePosition;

        // Converti la posizione del mouse dallo schermo alle coordinate del mondo di gioco
        mousePosition.z = Camera.main.transform.position.y; // Usa la distanza della camera sull'asse Y
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Calcola la direzione dalla posizione del giocatore alla posizione del mouse
        Vector3 direction = worldMousePosition - rotator.position;

        // Ignora l'altezza (asse Y) per la rotazione orizzontale
        direction.y = 0;

        // Calcola la rotazione necessaria per puntare verso la direzione
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Applica la rotazione al transform in modo fluido
        rotator.rotation = Quaternion.Slerp(rotator.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }
}
