using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Wall")]
    [SerializeField] private Transform wallObject;
    [SerializeField][Range(5, 30)] private int verticalWallSize = 15;
    [SerializeField][Range(5, 50)] private int horizontalWallSize = 20;

    [Header("Prefab")]
    [SerializeField] private GameObject objectPrefab;
    [SerializeField][Range(0.1f, 3f)] private float targetSize = 1;
    [SerializeField][Range(1, 30)] private int numberOfObjects = 10;

    [Header("Object Creation Limitations")]
    [SerializeField][Min(0)] private float minDistance = 2f;
    [SerializeField][Range(0, 30)] private int maxAttempts = 10;



    private float verticalInterval;
    private float horizontalInterval;


    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Awake()
    {
        // Set wall size
        wallObject.transform.localScale = new Vector3(horizontalWallSize, verticalWallSize, 1);

        // Set prefab size
        objectPrefab.transform.localScale = Vector3.one * targetSize;

        // Get half the lengths of the sides of the wall
        // to generate a random position (for example, from -length to +length)
        // and subtract the prefab radius.
        verticalInterval = verticalWallSize / 2 - objectPrefab.transform.localScale.y;
        horizontalInterval = horizontalWallSize / 2 - objectPrefab.transform.localScale.x;
    }

    private void Start()
    {
        // Generate {numberOfObjects} gameObjects prefabs
        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject newObject = Instantiate(objectPrefab, GetRandomPosition(), Quaternion.identity);
            newObject.transform.SetParent(transform, false);

            spawnedObjects.Add(newObject);
        }
    }

    // Generate a random safe position for an object
    public Vector3 GetRandomPosition(GameObject gameObject = null)
    {
        int attemptsCount = 0;
        Vector3 position = Vector3.zero;
        bool safePosition = false;

        while (!safePosition)
        {
            attemptsCount++;

            // Generate a random position within specified boundaries
            position = new Vector3(Random.Range(-horizontalInterval, horizontalInterval),
                                   Random.Range(-verticalInterval, verticalInterval),
                                   0);

            // Prevents infinite loops if a safe location cannot be found
            if (attemptsCount > maxAttempts)
            {
                Debug.Log("No safe location");
                return position;
            }

            // Check if the position is safe
            safePosition = IsSafePosition(position, gameObject);
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
