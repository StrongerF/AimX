using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Mouse Sensivity")]
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [Header("Shoot")]
    [SerializeField] private ObjectSpawner targetSpawner;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float rayDistance = 100f;

    [Space]
    [SerializeField] private Transform playerTransform;

    private float mouseX;
    private float mouseY;
    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        if (targetSpawner == null)
        {
            targetSpawner = GetComponent<ObjectSpawner>();
        }
    }

    private void Start()
    {
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
