# Tarea Unity - Roll a Ball

## Qué es esto?

El código representa diferentes scripts para el control de GameObjects como Player, Camera o PickUps. Se emplean métodos para el manejo de ìnputs de usuario mediante el input system de Unity y se emplean para tener dos cámaras perfectamente funcionales manteniendo el movimiento WASD ligado al tipo de cámara empleado.

## `PlayerController`

Control del jugador. Encargado del *movimiento* en función del tradicional *WASD* y de la interacción con `colisionables` del mapa.

1. `Movimiento del Jugador`  

El movimiento surge gracias a fuerzas aplicadas al GameObject del jugador. Estas fuerzas actuan en un plano tridimensional mientras que el control del jugador con las teclas WASD envian fuerzas ***bidimensionales***, es decir, en X e Y.
   - En mi caso particular, implemento los métodos del archivo `.inputactions` como OnMove(), OnLook() y otros para recibir los inputs del usuario y trabajar con ellos.
   - Cuando se detecta un movimiento, se actualizan las variables `movementX` y `movementY` con los valores correspondientes.
   - En el método `FixedUpdate()`, se calcula el movimiento relativo a la cámara para que el jugador se mueva en la misma dirección en que la cámara está mirando.

2. `Interacción con Colisionables`  

   El jugador destruye objetos etiquetados como "PickUp" al tocarlos dando la ilusion de "coleccionables".

3. `Movimiento en relación a la cámara`

Para que el jugador se mueva de manera funcional hay que tener en cuenta si juega en 3ª o en 1ª persona.

```csharp
if (movementInput != Vector2.zero)
```
- Este bloque verifica si el jugador ha dado alguna entrada de movimiento (por ejemplo, con WASD, las flechas del teclado o un joystick).
- `movementInput` es un vector bidimensional que contiene la entrada en los ejes `X` (horizontal) e `Y` (vertical). Si es diferente de `(0, 0)`, significa que el jugador está intentando mover el objeto.

---

### **1. Calcular la dirección hacia adelante**
```csharp
Vector3 forward = playerCamera.transform.forward;
forward.y = 0f;
forward.Normalize();
```
- **`playerCamera.transform.forward`**: Este vector apunta hacia adelante según la orientación de la cámara del jugador.
  - Ejemplo: Si la cámara está mirando hacia el norte, `forward` apuntará en esa dirección.
- **`forward.y = 0f;`**: Esto asegura que el movimiento no tenga componente vertical. Por ejemplo, si la cámara está inclinada hacia arriba o hacia abajo, esta línea ignora esa inclinación para que el objeto no "salte" o "caiga" mientras se mueve.
- **`forward.Normalize();`**: Se asegura de que el vector tenga una magnitud de 1 (unidad) para que las velocidades no se vean afectadas por la escala del vector. Es útil cuando calculamos direcciones.

---

### **2. Calcular la dirección hacia la derecha**
```csharp
Vector3 right = playerCamera.transform.right;
right.y = 0f;
right.Normalize();
```
- **`playerCamera.transform.right`**: Este vector apunta hacia la derecha desde la perspectiva de la cámara.
  - Ejemplo: Si la cámara está mirando hacia el norte, `right` apuntará hacia el este.
- Similar al paso anterior, se establece `right.y = 0f` para evitar componentes verticales y se normaliza el vector para mantener su magnitud en 1.

---

### **3. Combinar direcciones y entrada**
```csharp
Vector3 movement = (forward * movementInput.y + right * movementInput.x) * speed;
```
- Aquí se calcula el vector final de movimiento combinando las direcciones hacia adelante (`forward`) y hacia la derecha (`right`), ponderadas por la entrada del jugador:
  - **`movementInput.y`**: Representa cuánto se mueve el jugador hacia adelante o hacia atrás.
    - Multiplicamos esto por el vector `forward` para mover el objeto en esa dirección.
  - **`movementInput.x`**: Representa cuánto se mueve el jugador hacia la derecha o izquierda.
    - Multiplicamos esto por el vector `right` para mover el objeto lateralmente.
- Finalmente, este vector combinado se multiplica por `speed` para ajustar la velocidad del movimiento.

---

### **4. Aplicar fuerza al Rigidbody**
```csharp
rb.AddForce(movement, ForceMode.Force);
```
- **`rb.AddForce`**: Aplica una fuerza al objeto que tiene un `Rigidbody` para moverlo.
- **`movement`**: Es el vector de fuerza calculado previamente.
- **`ForceMode.Force`**: Indica que esta fuerza es continua y proporcional al tiempo. Esto da como resultado un movimiento suave y controlado.

---

### **Resumen del flujo**
1. **Entrada del jugador**: Se verifica si hay movimiento (`movementInput`).
2. **Cálculo de direcciones**:
   - Se determina hacia dónde es "adelante" y "derecha" desde la perspectiva de la cámara.
   - Estas direcciones se ajustan para ignorar componentes verticales.
3. **Cálculo del vector de movimiento**:
   - La entrada del jugador se combina con estas direcciones y se ajusta por la velocidad.
4. **Aplicación de fuerza**:
   - Se aplica una fuerza al `Rigidbody` para mover el objeto físicamente en el mundo.

---

### **Ejemplo visual**
- Si la cámara mira al norte:
  - Presionar `W` mueve al objeto hacia el norte.
  - Presionar `D` mueve al objeto hacia el este.
  - Presionar `W` y `D` juntos mueve al objeto en diagonal (noreste).

Todo esto hace que el objeto se mueva de manera natural en relación con la cámara, incluso si el jugador rota la cámara.

## `CameraController`

Este código permite que una sola cámara cambie dinámicamente entre los modos de **primera persona** y **tercera persona**, según la selección del jugador. A continuación, se desglosan los componentes principales y su funcionamiento:

### **Variables Importantes**

### **Configuración General**
- `player`: Hace referencia al jugador (la bola) para posicionar la cámara con respecto a él.
- `controls`: Instancia de `PlayerControls`, usada para manejar entradas del jugador.
- `lookInput`: Almacena la entrada del movimiento del mouse.
- `isFirstPerson`: Booleano que determina si la cámara está en modo primera persona o tercera persona.

### **Configuración de Tercera Persona**
- `thirdPersonHeight`: Altura de la cámara en tercera persona.
- `thirdPersonDistance`: Distancia de la cámara al jugador.
- `thirdPersonOffset`: Vector calculado basado en la altura y la distancia.

### **Configuración de Primera Persona**
- `rotationSpeed`: Velocidad de rotación en primera persona.
- `firstPersonHeightOffset`: Altura de la cámara para que coincida con la posición del "punto de vista" del jugador.
- `rotationX` y `rotationY`: Acumuladores para las rotaciones vertical y horizontal respectivamente.

---

### **Métodos Principales**

### **Inicialización de la Cámara**
- **`Start()`**: Calcula el offset inicial de la cámara para la vista en tercera persona basándose en `thirdPersonHeight` y `thirdPersonDistance`.

```csharp
thirdPersonOffset = new Vector3(0, thirdPersonHeight, -thirdPersonDistance);
```

### **Entrada del Jugador**
- **`Update()`**:
  1. Escucha las teclas numéricas presionadas (1 para tercera persona y 2 para primera persona).
  2. Cambia el valor de `isFirstPerson` según la tecla presionada.

```csharp
if (controls.Player.NumberKeys.triggered)
{
    var control = controls.Player.NumberKeys.activeControl;
    if (control != null)
    {
        string keyPressed = control.displayName;
        switch (keyPressed)
        {
            case "1":
                isFirstPerson = false; // Cambiar a tercera persona.
                break;
            case "2":
                isFirstPerson = true; // Cambiar a primera persona.
                break;
        }
    }
}
```

### **Actualización de la Vista**
- **`LateUpdate()`**: Llama a la función de actualización adecuada según el modo seleccionado (`isFirstPerson`).

```csharp
if (isFirstPerson)
{
    UpdateFirstPersonView();
}
else
{
    UpdateThirdPersonView();
}
```

### **Modo Tercera Persona**
- **`UpdateThirdPersonView()`**:
  1. Calcula la posición de la cámara basándose en la posición del jugador más el offset configurado.
  2. Ajusta la cámara para que mire hacia el jugador.

```csharp
Vector3 desiredPosition = player.transform.position + thirdPersonOffset;
transform.position = desiredPosition;
transform.LookAt(player.transform.position);
```

### **Modo Primera Persona**
- **`UpdateFirstPersonView()`**:
  1. Posiciona la cámara justo sobre el jugador utilizando `firstPersonHeightOffset`.
  2. Calcula la rotación acumulada basándose en la entrada del mouse (`lookInput`).
  3. Aplica la rotación acumulada a la cámara.

```csharp
transform.position = player.transform.position + Vector3.up * firstPersonHeightOffset;
rotationX = Mathf.Clamp(rotationX - lookInput.y * rotationSpeed * Time.deltaTime, -90f, 90f);
rotationY += lookInput.x * rotationSpeed * Time.deltaTime;
transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
```

### **Entrada de Movimiento del Mouse**
- **`OnLook(InputAction.CallbackContext context)`**: Captura la entrada del movimiento del mouse para actualizar la rotación en primera persona.

```csharp
if (context.performed)
{
    lookInput = context.ReadValue<Vector2>();
}
else if (context.canceled)
{
    lookInput = Vector2.zero;
}
```

---

## **Resumen del Flujo del Código**
1. **Inicialización**: Se configura la cámara y los controles del jugador.
2. **Entrada del Jugador**: Se detecta si el jugador presiona una tecla numérica para alternar entre modos.
3. **Actualización de la Cámara**:
   - Si está en modo tercera persona, se coloca detrás y arriba del jugador mirando hacia él.
   - Si está en modo primera persona, se posiciona en la cabeza del jugador y sigue la rotación del mouse.
4. **Entrada del Mouse**: En modo primera persona, las rotaciones del mouse actualizan la orientación de la cámara.
