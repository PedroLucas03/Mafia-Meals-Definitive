using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset;
    public bool useOffsetValues = true;

    [Header("Rotation Settings")]
    public float rotateSpeed = 5f;
    public float maxViewAngle = 60f;
    public float minViewAngle = -45f;
    public bool invertY = false;

    [Header("Smoothing")]
    public float smoothSpeed = 10f;
    public float rotationSmoothTime = 0.12f;

    private Transform pivot;
    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;

    void Start()
    {
        if (!useOffsetValues)
        {
            offset = target.position - transform.position;
        }

        pivot = new GameObject("CameraPivot").transform;
        pivot.position = target.position;
        pivot.parent = null;

        currentRotation = pivot.eulerAngles;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        pivot.position = target.position;

        HandleRotation();
        ApplyCameraPosition();
    }

    void HandleRotation()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed * (invertY ? 1 : -1);

        currentRotation.y += horizontal;
        currentRotation.x += vertical;

        // Limita o ângulo vertical
        currentRotation.x = Mathf.Clamp(currentRotation.x, minViewAngle, maxViewAngle);

        // Suavização da rotação
        pivot.rotation = Quaternion.Euler(
            Mathf.SmoothDampAngle(pivot.eulerAngles.x, currentRotation.x, ref rotationSmoothVelocity.x, rotationSmoothTime),
            Mathf.SmoothDampAngle(pivot.eulerAngles.y, currentRotation.y, ref rotationSmoothVelocity.y, rotationSmoothTime),
            0
        );
    }

    void ApplyCameraPosition()
    {
        Vector3 desiredPosition = target.position - (pivot.rotation * offset);

        // Suavização do movimento
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Garante que a câmera não fique abaixo do alvo
        if (transform.position.y < target.position.y - 0.5f)
        {
            transform.position = new Vector3(transform.position.x, target.position.y - 0.5f, transform.position.z);
        }

        transform.LookAt(target);
    }
}