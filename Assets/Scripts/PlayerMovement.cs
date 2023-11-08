using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float moveSmoothTime = 0.25f;

    [Header("Jumping")]
    [SerializeField] private float gravityStrength = 9.81f;
    [SerializeField] private float jumpStrength = 3f;

    [Header("Crouching")]
    [SerializeField] private float crouchSmoothTime = 0.1f;
    [SerializeField] private float crouchHeight = 0.7f;




    private CharacterController characterController;

    private Vector3 currentMoveVelocity;
    private Vector3 moveDampVelocity;

    private Vector3 crouchVector;
    private Vector3 standVector;
    private Vector3 currentTransformScale;
    private Vector3 crouchDampVelocity;

    private Vector3 currentForceVelocity;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        standVector = transform.localScale;
        crouchVector = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);

        currentTransformScale = standVector;
    }


    private void Update()
    {
        // Get player's input
        Vector3 playerInput = new Vector3()
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        };
        playerInput.Normalize();

        // Convert the player's movement direction to character controller's direction
        Vector3 moveVector = transform.TransformDirection(playerInput);


        // Set the player's speed based on input (crouch, walk, or run)
        float currentSpeed;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = walkSpeed;
        }
        else currentSpeed = moveSpeed;

        // Set the player's height based on input
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Crouch();
        }
        else
        {
            Stand();
        }

        // Update the current movement velocity smoothly
        currentMoveVelocity = Vector3.SmoothDamp(currentMoveVelocity,
                                                 moveVector * currentSpeed,
                                                 ref moveDampVelocity,
                                                 moveSmoothTime);

        characterController.Move(currentMoveVelocity * Time.deltaTime);


        // Perform a downward raycast to check if user is on ground
        if (Physics.Raycast(transform.position, Vector3.down, transform.localScale.y + 0.1f))
        {
            currentForceVelocity.y = -2f;

            if (Input.GetButton("Jump"))
            {
                Jump();
            }
        }
        else
        {
            // Acceleration of gravity
            currentForceVelocity.y -= gravityStrength * Time.deltaTime;
        }

        characterController.Move(currentForceVelocity * Time.deltaTime);


    }

    private void Crouch()
    {
        currentTransformScale = Vector3.SmoothDamp(currentTransformScale, crouchVector, ref crouchDampVelocity, crouchSmoothTime);
        transform.localScale = currentTransformScale;
    }

    private void Stand()
    {
        currentTransformScale = Vector3.SmoothDamp(currentTransformScale, standVector, ref crouchDampVelocity, crouchSmoothTime);
        transform.localScale = currentTransformScale;
    }

    private void Jump()
    {
        currentForceVelocity.y = jumpStrength;
        Debug.Log("Jumped");
    }


}
