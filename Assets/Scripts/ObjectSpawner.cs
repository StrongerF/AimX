using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    #region Instance
    private static ObjectSpawner instance;
    public static ObjectSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ObjectSpawner>();
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("ObjectSpawner");
                    instance = gameObject.AddComponent<ObjectSpawner>();
                }
            }
            return instance;
        }
    }
    #endregion

    #region Unity Editor Properties

    [Header("Wall Properties")]
    [SerializeField] private GameObject wallObject;

    [SerializeField] private bool applyOverridedSettings;

    
    [Space]

    [SerializeField][Min(0)] private Vector2 wallSize = new Vector2(20, 15);
    [SerializeField][Min(0)] private int2 minWallSize = new int2(3, 3);
    [SerializeField][Min(0)] private int2 maxWallSize = new int2(40, 30);

    [Space]

    [SerializeField][Min(0)] private int wallDistance = 20;
    [SerializeField][Min(0)] private int minWallDistance = 15;
    [SerializeField][Min(0)] private int maxWallDistance = 50;

    


    [Header("Prefab")]
    [SerializeField] private GameObject objectPrefab;

    [Space]

    [SerializeField][Min(0.1f)] private float objectSize = 1;
    [SerializeField][Min(0.1f)] private float minObjectSize = 0.1f;
    [SerializeField][Min(0.1f)] private float maxObjectSize = 5f;

    [Space]

    [SerializeField][Min(1)] private int numberOfObjects = 10;
    [SerializeField][Min(1)] private int minObjectsCount = 1;
    [SerializeField][Min(1)] private int maxObjectsCount = 30;


    [Header("Object Creation Limitations")]
    [SerializeField][Min(0)] private float minDistance = 2f;
    [SerializeField][Range(0, 30)] private int maxAttempts = 10;

    #endregion

    #region Static Properties

    #region Wall
    [DoNotSerialize] public static Vector2 WallSize
    {
        get => Instance.wallSize;
        set
        {
            if (Instance.applyOverridedSettings) return;
            Instance.wallSize = value;
        }
    }
    [DoNotSerialize] public static int WallDistance
    {
        get => Instance.wallDistance;
        set
        {
            if (Instance.applyOverridedSettings) return;
            Instance.wallDistance = value;
        }
    }
    #endregion

    #region Target
    [DoNotSerialize] public static float TargetSize
    {
        get => Instance.objectSize;
        set
        {
            if (Instance.applyOverridedSettings) return;
            Instance.objectSize = value;
        }
    }
    [DoNotSerialize] public static int TargetsCount
    {
        get => Instance.numberOfObjects;
        set
        {
            if (Instance.applyOverridedSettings) return;
            Instance.numberOfObjects = value;
        }
    }
    #endregion

    #region Advanced
    [DoNotSerialize] public static float MinTargetsDistance
    {
        get => Instance.minDistance;
        set
        {
            if (Instance.applyOverridedSettings) return;
            Instance.minDistance = value;
        }
    }
    [DoNotSerialize] public static int MaxAttempts
    {
        get => Instance.maxAttempts;
        set
        {
            if (Instance.applyOverridedSettings) return;
            Instance.maxAttempts = value;
        }
    }
    #endregion

    #region Value Borders
    [DoNotSerialize] public static int2 MinWallSize => Instance.minWallSize;
    [DoNotSerialize] public static int2 MaxWallSize => Instance.maxWallSize;

    [DoNotSerialize] public static int MinWallDistance => Instance.minWallDistance;
    [DoNotSerialize] public static int MaxWallDistance => Instance.maxWallDistance;

    [DoNotSerialize] public static float MinTargetSize => Instance.minObjectSize;
    [DoNotSerialize] public static float MaxTargetSize => Instance.maxObjectSize;

    [DoNotSerialize] public static float MinTargetsCount => Instance.minObjectsCount;
    [DoNotSerialize] public static float MaxTargetsCount => Instance.maxObjectsCount;

    #endregion

    #endregion


    private float verticalInterval;
    private float horizontalInterval;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    
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

    private void SetWallSize(Vector2 size)
    {
        wallObject.transform.localScale = size;

        // Set spawner interval range
        verticalInterval = size.y / 2 - objectSize;
        horizontalInterval = size.x / 2 - objectSize;
    }

    private void SetWallDistance(float distance)
    {
        // Get eye height to centralize wall vertically
        float playerEyeHeight = PlayerCamera.Instance.transform.position.y;
        wallObject.transform.position = new Vector3(0, playerEyeHeight, distance);

        // Set spawner position
        transform.position = wallObject.transform.position;
    }

    private void SetObjectSize(float size)
    {
        // Set prefab size
        objectPrefab.transform.localScale = Vector3.one * size;
    }

    private void GenerateObjects(int numbersOfObjects)
    {
        // Generate {numberOfObjects} gameObjects prefabs
        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject newObject = Instantiate(objectPrefab, GetRandomPosition(), Quaternion.identity);
            newObject.transform.SetParent(transform, false);

            spawnedObjects.Add(newObject);
        }
    }

    private void DestroyChildObjects()
    {
        spawnedObjects.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }



    public static void Spawn()
    {
        Instance.SpawnObjects();
    }

    private void SpawnObjects()
    {
        SetWallSize(wallSize);
        SetWallDistance(wallDistance);
        SetObjectSize(objectSize);

        DestroyChildObjects();
        GenerateObjects(numberOfObjects);

        

        applyOverridedSettings = false;
    }

    // Generate a random safe position for an object
    public Vector3 GetRandomPosition(GameObject gameObject = null)
    {
        int attemptsCount = 0;
        Vector3 position = Vector3.zero;
        bool isSafePosition = false;

        while (!isSafePosition)
        {
            attemptsCount++;

            // Generate a random position within specified boundaries
            position = new Vector3(Random.Range(-horizontalInterval, horizontalInterval),
                                   Random.Range(-verticalInterval, verticalInterval),
                                   0);

            // Prevents infinite loops if a safe location cannot be found
            if (attemptsCount > maxAttempts)
            {
                Debug.LogWarning("No safe location");
                return position;
            }

            // Check if the position is safe
            isSafePosition = IsSafePosition(position, gameObject);
        }

        return position;
    }

    // Check if the target position is safe and doesn't collide with other objects
    private bool IsSafePosition(Vector3 target, GameObject gameObject = null)
    {
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            // Skip checking the current object itself
            if (spawnedObject == gameObject) continue;

            // Calculate the distance between the target position and existing objects
            float distance = Vector3.Distance(target, spawnedObject.transform.localPosition);

            // If the distance is less than the minimum allowed, the position is not safe
            if (distance < minDistance)
            {
                return false;
            }
        }

        // Else the position is safe
        return true;
    }

    // Move the specified object to a new safe position
    internal void ChangePosition(GameObject gameObject)
    {
        int index = spawnedObjects.IndexOf(gameObject);
        // If the object is in the spawnedObjects list, update its position
        if (index != -1)
        {
            // Get a new safe position for the object
            Vector3 newPosition = GetRandomPosition(gameObject);

            spawnedObjects[index].transform.localPosition = newPosition;
        }
    }

}
