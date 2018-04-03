using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {
    #region Inspector Variables
    [Header("Camera Settings:")]
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float cameraRotationLimit = 85f;
    #endregion

    #region Private Variables
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 thrusterForce = Vector3.zero;
    private Rigidbody rb;

    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Turn(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void CameraTurn(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

    public void Thrust(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformTurn();
        PerformCameraTurn();
    }

    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            //Like rb.translate but performs physics checks to avoid object collision
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
    
    private void PerformTurn()
    {
        if (rotation != Vector3.zero)
        {
            //Euler turns Vector3 into Quaternion
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        }
    }

    private void PerformCameraTurn()
    {
        if (cam != null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
}
