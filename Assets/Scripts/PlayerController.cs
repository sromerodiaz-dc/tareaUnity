using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
{
    private PlayerControls controls;
    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector2 lookInput;

    public float speed = 10f;  // Velocidad de movimiento.
    public Camera playerCamera;  // Referencia a la cámara para el cálculo de la dirección de movimiento.

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);  // Registra los métodos de entrada.
        rb = GetComponent<Rigidbody>();  // Obtén el Rigidbody de la bola.
    }

    private void OnEnable()
    {
        controls.Player.Enable();  // Habilita las acciones del jugador.
    }

    private void OnDisable()
    {
        controls.Player.Disable();  // Deshabilita las acciones del jugador.
    }

    public void OnNumberKeys(InputAction.CallbackContext context)
    { }

    // Método llamado cuando se recibe la entrada de movimiento
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            movementInput = context.ReadValue<Vector2>();  // Lee la entrada de movimiento.
        }
        else if (context.canceled)
        {
            movementInput = Vector2.zero;  // Resetea la entrada de movimiento cuando se cancela.
        }
    }

    // Método llamado cuando se recibe la entrada del mouse
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lookInput = context.ReadValue<Vector2>();  // Lee el movimiento del mouse.
        }
        else if (context.canceled)
        {
            lookInput = Vector2.zero;  // Resetea la entrada del mouse cuando se cancela.
        }
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            // Calcula la dirección hacia adelante y hacia la derecha basándonos en la cámara.
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0f;  // Evita que se mueva hacia arriba/abajo.
            forward.Normalize();  // Normaliza la dirección para mantener una velocidad constante.

            Vector3 right = playerCamera.transform.right;
            right.y = 0f;  // Evita que se mueva hacia arriba/abajo.
            right.Normalize();

            // Calcula el vector de movimiento en función de la entrada.
            Vector3 movement = (forward * movementInput.y + right * movementInput.x) * speed;

            // Aplica la fuerza al Rigidbody.
            rb.AddForce(movement, ForceMode.Force);  // Utiliza ForceMode.Force para aplicar una fuerza constante.
        }
    }

    // Detectar las colisiones con los pick-ups
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))  // Si el objeto tiene la etiqueta "PickUp"
        {
            Destroy(other.gameObject);  // Destruir el pick-up al ser recogido.
        }
    }
}
