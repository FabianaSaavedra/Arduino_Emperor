using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);
    [SerializeField] private float positionSmoothing = 5f;
    [SerializeField] private float rotationSmoothing = 3f;
    void Start()
    {
        // Al iniciar, poner la cámara directamente detrás del carro (sin animación)
        if (target == null) return;
        transform.position = target.position + target.TransformDirection(offset);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position);
    }
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionSmoothing);

        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothing);
    }
}