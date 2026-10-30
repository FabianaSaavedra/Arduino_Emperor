using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float maxSteeringAngle = 90f;
    [SerializeField] private float inputSmoothing = 5f;

    [Header("Calibration")]
    [SerializeField, Range(0f, 0.3f)] private float deadZone = 0.05f;
    [SerializeField] private bool invertSpeed = false;
    [SerializeField] private bool invertSteering = false;

    [Header("Keyboard Fallback")]
    [SerializeField] private bool keyboardFallback = true;

    [Header("Inicio de carrera")]
    [Tooltip("Segundos que el carro espera quieto mientras corre el conteo 3, 2, 1, GO")]
    [SerializeField] private float startDelay = 3f;
    private float currentSpeed;
    private float currentSteering;
    private Rigidbody rb;

    public float CurrentSpeed => currentSpeed;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        // Mientras corre el conteo, el carro no se puede mover
        if (Time.timeSinceLevelLoad < startDelay)
        {
            currentSpeed = 0f;
            currentSteering = 0f;
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.angularVelocity = Vector3.zero;
            return;
        }
        var reader = ArduinoSerialReader.Instance;
        float rawSpeed, rawSteering;

        if (reader != null && reader.IsConnected)
        {
            // Hay Arduino: se usan los potenciómetros (igual que antes)
            reader.GetNormalizedValues(out rawSpeed, out rawSteering);
        }
        else if (keyboardFallback && Keyboard.current != null)
        {
            // No hay Arduino: se usa el teclado
            var kb = Keyboard.current;
            rawSpeed = 0f;
            rawSteering = 0f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) rawSpeed += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) rawSpeed -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) rawSteering += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) rawSteering -= 1f;
        }
        else
        {
            return;
        }

        if (invertSpeed) rawSpeed = -rawSpeed;
        if (invertSteering) rawSteering = -rawSteering;

        float targetSpeed = ApplyDeadZone(rawSpeed) * maxSpeed;
        float targetSteering = ApplyDeadZone(rawSteering) * maxSteeringAngle;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * inputSmoothing);
        currentSteering = Mathf.Lerp(currentSteering, targetSteering, Time.deltaTime * inputSmoothing);

        // Steer only when moving
        float steeringFactor = Mathf.Sign(currentSpeed) * Mathf.Clamp01(Mathf.Abs(currentSpeed) / 2f);

        // Mover con física para que choque con las paredes y active los checkpoints
        Vector3 velocidad = transform.forward * currentSpeed;
        velocidad.y = rb.linearVelocity.y; // conservar la gravedad
        rb.linearVelocity = velocidad;
        rb.angularVelocity = new Vector3(0f, currentSteering * steeringFactor * Mathf.Deg2Rad, 0f);
    }

    private float ApplyDeadZone(float value)
    {
        if (Mathf.Abs(value) < deadZone) return 0f;
        float sign = Mathf.Sign(value);
        return sign * (Mathf.Abs(value) - deadZone) / (1f - deadZone);
    }
}