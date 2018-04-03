using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    //Makes items appear in editor despite being private access
    #region Inspector Variables
    [Header("Movement Settings:")]
    [SerializeField]
    private float moveSpeed= 5f;
    [SerializeField]
    private float lookSensitivity = 1f;
    [SerializeField]
    private float thrusterForce = 1000f;

    [Header("Spring Settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointDamper = 3f;
    [SerializeField]
    private float jointMaxForce = 40f;
    #endregion

    #region Private Variables
    private PlayerMotor motor;
    private ConfigurableJoint cJoint;
    private Animator animator;
    #endregion

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        cJoint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring, jointDamper);
    }

    void Update()
    {
        //Movement
        //Calculate movement velocity as 3D vector
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        //transform.right generates vector of (1, 0, 0) multiply by input (between -1 and 1) to get movement direction
        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 movevertical = transform.forward * zMove;

        //Normalize will combine the vectors into one instead of adding them to the end of one another, multiply by speed to provide constant level of acceleration
        Vector3 velocity = (moveHorizontal + movevertical) * moveSpeed;

        //Animate Movement
        animator.SetFloat("ForwardVelocity", zMove);

        motor.Move(velocity);

        //Rotation (y movement only)
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

        motor.Turn(rotation);

        //Camera rotation (up/down movement)
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * lookSensitivity;

        motor.CameraTurn(cameraRotationX);

        Vector3 _thrusterForce = Vector3.zero;
        if(Input.GetButton("Jump"))
        {
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f, 0f);
        }
        else
        {
            SetJointSettings(jointSpring, jointDamper);
        }

        motor.Thrust(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring, float _jointDamper)
    {
        cJoint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            positionDamper = _jointDamper,
            maximumForce = jointMaxForce
        };
    }
}
