using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float damping = 0.5f;
    private Movement _movement;
    private float z;

    private Transform target;
    private Camera cam;
    private Rigidbody2D targetRb;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        z = transform.position.z;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        this.targetRb = target.GetComponent<Rigidbody2D>();
    }

    public void SetMovement(Movement movement)
    {
        this._movement = movement;
    }

    
    private void LateUpdate()
    {
        if (target != null)
        {
            // Smoothly follow the target's position
            Vector3 targetPosition = target.position;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, z);

            // Zoom in and out based on the target's velocity
            float targetVelocity = targetRb.velocity.magnitude;
            //float targetZoom = Mathf.Lerp(minZoom, maxZoom, targetVelocity / _movement.maxVelocity);
            //float currentZoom = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
            //cam.orthographicSize = currentZoom;

            // Damping for smooth zoom changes
            //cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, damping);

            // Dampen the rotation of the camera based on the target's rotation
            float targetAngularVelocity = targetRb.angularVelocity;
            float currentAngularVelocity = Mathf.Lerp(cam.transform.rotation.z, targetAngularVelocity, damping * Time.deltaTime);
            cam.transform.rotation = Quaternion.Euler(0, 0, currentAngularVelocity);
        }
    }
}