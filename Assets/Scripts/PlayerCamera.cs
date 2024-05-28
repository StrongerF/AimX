using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region Instance
    private static PlayerCamera instance;
    public static PlayerCamera Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerCamera>();
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("PlayerCamera");
                    instance = gameObject.AddComponent<PlayerCamera>();
                }
            }
            return instance;
        }
    }
    #endregion

    public static float Sensitivity
    {
        get => Instance.sensX;
        set
        {
            Instance.sensX = value;
            Instance.sensY = value;
        }
    }

    [Header("Mouse Sensivity")]
    public float sensX;
    public float sensY;

    [Header("Shoot")]
    [SerializeField] private ObjectSpawner targetSpawner;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float rayDistance = 100f;

    [Space]
    [SerializeField] private Transform playerTransform;

    private float mouseX;
    private float mouseY;
    private float xRotation;

    private void Awake()
    {
        #region Instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private void Start()
    {
        targetSpawner = ObjectSpawner.Instance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get mouse input
        mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        xRotation -= mouseY;

        // Limit up and down rotation to -90 and 90 degrees
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerTransform.Rotate(Vector3.up, mouseX);

        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }

    void Fire()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance);
        Debug.Log("Shot");

        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, rayDistance, targetMask))
        {
            targetSpawner.ChangePosition(raycastHit.collider.gameObject);

            Debug.Log("Hit");
        }
    }
}
