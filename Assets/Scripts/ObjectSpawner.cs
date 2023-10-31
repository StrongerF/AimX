using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Transform wallObject;
    [SerializeField] private GameObject objectPrefab;

    [SerializeField][Range(1, 30)] private int numberOfObjects = 10;
    [SerializeField][Min(0)] private float minDistance = 2f;
    [SerializeField][Range(0, 30)] private int maxAttempts = 10;
    [SerializeField] private Vector2 wallSize;

    
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Awake()
    {
        // Get half the lengths of the sides of the wall
        // to generate a random position (for example, from -length to +length)
        // and subtract the prefab radius.
        wallSize.x = wallSize.x / 2 - objectPrefab.transform.localScale.x;
        wallSize.y = wallSize.y / 2 - objectPrefab.transform.localScale.y;
    }

    private void Start()
    {
        // Generate {numberOfObjects} gameObjects prefabs
        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject newObject = Instantiate(objectPrefab, GetRandomPosition(), Quaternion.identity);
            newObject.transform.SetParent(wallObject.transform, false);

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
            position = new Vector3(Random.Range(-wallSize.x, wallSize.x),
                                   Random.Range(-wallSize.y, wallSize.y),
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
