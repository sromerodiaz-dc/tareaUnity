using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, PlayerControls.IPlayerActions
{
    public GameObject player; // Referencia al jugador (la bola).

    private PlayerControls controls;
    private Vector2 lookInput; // Entrada de rotación para la cámara.
    private bool isFirstPerson = false; // Indica si está en modo primera persona.

    // Configuración para la vista en tercera persona.
    public float thirdPersonHeight = 10f; // Altura de la cámara en tercera persona.
    public float thirdPersonDistance = 10f; // Distancia de la cámara al jugador.
    private Vector3 thirdPersonOffset;

    // Configuración para la vista en primera persona.
    public float rotationSpeed = 100f; // Velocidad de rotación para la cámara en primera persona.
    public float firstPersonHeightOffset = 0.5f; // Altura de la cámara en primera persona.

    private float rotationX = 0f; // Rotación acumulada en el eje vertical (primera persona).
    private float rotationY = 0f; // Rotación acumulada en el eje horizontal (primera persona).

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Start()
    {
        // Configurar el offset inicial para la vista en tercera persona.
        thirdPersonOffset = new Vector3(0, thirdPersonHeight, -thirdPersonDistance);
    }

    private void Update()
    {
        // Alternar entre cámaras basado en las teclas numéricas del 1 al 9.
        if (controls.Player.NumberKeys.triggered)
        {
            // Verificar cuál tecla numérica fue presionada.
            var control = controls.Player.NumberKeys.activeControl;
            if (control != null)
            {
                string keyPressed = control.displayName;
                Debug.Log($"Tecla presionada: {keyPressed}");

                // Alternar entre vistas según la tecla.
                switch (keyPressed)
                {
                    case "1Pq=U":
                        isFirstPerson = false; // Cambiar a tercera persona.
                        break;
                    case "2Pq=U":
                        isFirstPerson = true; // Cambiar a primera persona.
                        break;
                        // Puedes agregar más casos aquí si lo necesitas.
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (isFirstPerson)
        {
            UpdateFirstPersonView();
        }
        else
        {
            UpdateThirdPersonView();
        }
    }

    private void UpdateThirdPersonView()
    {
        // Posición deseada para la cámara en tercera persona.
        Vector3 desiredPosition = player.transform.position + thirdPersonOffset;
        transform.position = desiredPosition;

        // Hacer que la cámara mire al jugador.
        transform.LookAt(player.transform.position);
    }

    private void UpdateFirstPersonView()
    {
        // Actualizar posición de la cámara para la primera persona.
        transform.position = player.transform.position + Vector3.up * firstPersonHeightOffset;

        // Aplicar rotación acumulada basada en la entrada.
        rotationX = Mathf.Clamp(rotationX - lookInput.y * rotationSpeed * Time.deltaTime, -90f, 90f);
        rotationY += lookInput.x * rotationSpeed * Time.deltaTime;

        // Aplicar rotación final.
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    // Método llamado automáticamente cuando hay entrada en el mouse.
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lookInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            lookInput = Vector2.zero;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Implementación vacía ya que no se utiliza movimiento aquí.
    }

    public void OnNumberKeys(InputAction.CallbackContext context)
    {
        //
    }
}
