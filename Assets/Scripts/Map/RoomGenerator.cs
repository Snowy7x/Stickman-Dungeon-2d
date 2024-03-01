using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] wallPrefabs; // array of wall prefabs
    [SerializeField] private GameObject[] shapePrefabs; // array of shape prefabs
    [SerializeField] private int minShapesPerRoom = 1; // minimum number of shapes to generate per room
    [SerializeField] private int maxShapesPerRoom = 5; // maximum number of shapes to generate per room
    [SerializeField] private float minShapeSize = 1f; // minimum size of shapes to generate
    [SerializeField] private float maxShapeSize = 3f; // maximum size of shapes to generate
    [SerializeField] private List<Action<List<GameObject>>> overlapEffects;
    [SerializeField] private float overlapChance;
    private Transform roomContainer; // container to hold all generated rooms

    private void Awake()
    {
        roomContainer = new GameObject("Rooms").transform;
        // Define overlap effects
        overlapEffects = new List<Action<List<GameObject>>>
        {
            MergeOverlapEffect,
            ColorizeOverlapEffect,
            ExplodeOverlapEffect
        };
    }

    public void GenerateRoom(Vector2 position, Vector2 size)
    {
        // Create the walls
        GameObject leftWall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)],
            position - new Vector2(size.x / 2, 0f), Quaternion.identity);
        leftWall.transform.localScale = new Vector2(1f, size.y);
        leftWall.transform.parent = roomContainer;

        GameObject rightWall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)],
            position + new Vector2(size.x / 2, 0f), Quaternion.identity);
        rightWall.transform.localScale = new Vector2(1f, size.y);
        rightWall.transform.parent = roomContainer;

        GameObject topWall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)],
            position + new Vector2(0f, size.y / 2), Quaternion.identity);
        topWall.transform.localScale = new Vector2(size.x, 1f);
        topWall.transform.parent = roomContainer;

        GameObject bottomWall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)],
            position - new Vector2(0f, size.y / 2), Quaternion.identity);
        bottomWall.transform.localScale = new Vector2(size.x, 1f);
        bottomWall.transform.parent = roomContainer;

        // Generate the shapes
        int numShapes = Random.Range(minShapesPerRoom, maxShapesPerRoom + 1);
        List<GameObject> shapes = new List<GameObject>();
        for (int i = 0; i < numShapes; i++)
        {
            GameObject shape = Instantiate(shapePrefabs[Random.Range(0, shapePrefabs.Length)], roomContainer);

            float sizeMultiplier = Random.Range(minShapeSize, maxShapeSize);
            Vector2 shapePosition = new Vector2(Random.Range(position.x - size.x / 2, position.x + size.x / 2),
                Random.Range(position.y - size.y / 2, position.y + size.y / 2));

            shape.transform.localScale = new Vector2(sizeMultiplier, sizeMultiplier);
            shape.transform.position = shapePosition;
            shapes.Add(shape);
        }
        
        // Randomly choose an effect to apply
        int effectIndex = Random.Range(0, overlapEffects.Count);
        Action<List<GameObject>> overlapEffect = overlapEffects[effectIndex];
        // Apply overlap effect if chance is met
        if (overlapEffect != null && Random.value < overlapChance)
        {
            List<List<GameObject>> overlappingShapes = GetOverlappingShapes(shapes.ToArray());
            foreach (List<GameObject> overlappingList in overlappingShapes)
            {
                if (overlappingList.Count > 1)
                {
                    overlapEffect(overlappingList);
                }
            }
        }
    }
    
    public List<List<GameObject>> GetOverlappingShapes(GameObject[] shapes)
    {
        List<List<GameObject>> overlappingGroups = new List<List<GameObject>>();
        HashSet<GameObject> visitedShapes = new HashSet<GameObject>();

        foreach (GameObject shape in shapes)
        {
            if (!visitedShapes.Contains(shape))
            {
                List<GameObject> overlappingShapes = new List<GameObject>();
                overlappingShapes.Add(shape);

                for (int i = 0; i < overlappingShapes.Count; i++)
                {
                    GameObject currentShape = overlappingShapes[i];

                    foreach (GameObject otherShape in shapes)
                    {
                        if (currentShape != otherShape && !visitedShapes.Contains(otherShape) && ShapesOverlap(currentShape, otherShape))
                        {
                            overlappingShapes.Add(otherShape);
                            visitedShapes.Add(otherShape);
                        }
                    }
                }

                overlappingGroups.Add(overlappingShapes);
            }
        }

        return overlappingGroups;
    }
    
    public static bool ShapesOverlap(GameObject shape1, GameObject shape2)
    {
        Vector2 shape1Position = shape1.transform.position;
        Vector2 shape2Position = shape2.transform.position;
        Vector2 shape1Size = shape1.transform.lossyScale;
        Vector2 shape2Size = shape2.transform.lossyScale;

        float distance = Vector2.Distance(shape1Position, shape2Position);
        float sumOfSizes = Mathf.Max(shape1Size.x, shape1Size.y) + Mathf.Max(shape2Size.x, shape2Size.y);

        return distance < sumOfSizes;
    }


    private void SpawnShapes(Vector2 position, Vector2 size)
    {
        List<GameObject> spawnedShapes = new List<GameObject>();

        // Define the available shape types
        GameObject[] shapeTypes = shapePrefabs;

        // Calculate the area of the room
        float roomArea = size.x * size.y;

        // Loop through each shape type and spawn a random number of shapes
        foreach (GameObject shapeType in shapeTypes)
        {
            // Determine the size range for this shape type
            float minSize = Mathf.Min(minShapeSize, roomArea / 20f); // ensure the shape size is never too small
            float maxSize = Mathf.Min(maxShapeSize, roomArea / 4f); // ensure the shape size is never too big

            // Determine the number of shapes to spawn
            int numShapes = Mathf.RoundToInt(Random.Range(minShapesPerRoom, maxShapesPerRoom + 1) * (roomArea / 100f));

            for (int i = 0; i < numShapes; i++)
            {
                // Determine a random position and size for the shape
                float shapeSize = Random.Range(minSize, maxSize);
                Vector2 shapePos = new Vector2(
                    Random.Range(position.x - size.x / 2 + shapeSize, position.x + size.x / 2 - shapeSize),
                    Random.Range(position.y - size.y / 2 + shapeSize, position.y + size.y / 2 - shapeSize));

                // Spawn the shape
                GameObject newShape = Instantiate(shapeType, shapePos, Quaternion.identity, roomContainer);
                newShape.transform.localScale = new Vector2(shapeSize, shapeSize);

                // Ensure the new shape does not overlap with any previously spawned shapes
                bool overlaps = false;
                foreach (GameObject spawnedShape in spawnedShapes)
                {
                    if (Intersects(newShape.GetComponent<Renderer>(), spawnedShape.GetComponent<Renderer>()))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps)
                {
                    Destroy(newShape);
                }
                else
                {
                    spawnedShapes.Add(newShape);
                }
            }
        }
    }

    // Helper function to check if two renderers intersect
    private bool Intersects(Renderer r1, Renderer r2)
    {
        return r1.bounds.Intersects(r2.bounds);
    }
    
    private void MergeOverlapEffect(List<GameObject> shapes)
    {
        // Get the combined size of all shapes
        float totalSize = 0f;
        foreach (GameObject shape in shapes)
        {
            totalSize += shape.transform.localScale.x * shape.transform.localScale.y;
        }

        // Destroy all existing shapes
        foreach (GameObject shape in shapes)
        {
            Destroy(shape);
        }

        // Create a new shape with the combined size
        GameObject newShape = Instantiate(shapePrefabs[Random.Range(0, shapePrefabs.Length)], roomContainer);
        float newSize = Mathf.Sqrt(totalSize);
        newShape.transform.localScale = new Vector2(newSize, newSize);
        newShape.transform.position = shapes[0].transform.position;
    }
    
    private void ExplodeOverlapEffect(List<GameObject> shapes)
    {
        // Destroy all existing shapes
        foreach (GameObject shape in shapes)
        {
            Destroy(shape);
        }

        // Create new smaller shapes at the center of the original shapes
        for (int i = 0; i < shapes.Count; i++)
        {
            GameObject newShape = Instantiate(shapePrefabs[Random.Range(0, shapePrefabs.Length)], roomContainer);
            float newSize = shapes[i].transform.localScale.x / 2f;
            newShape.transform.localScale = new Vector2(newSize, newSize);
            newShape.transform.position = shapes[i].transform.position;
        }
    }
    private void ColorizeOverlapEffect(List<GameObject> shapes)
    {
        // Change the color of all overlapping shapes to a random color
        Color newColor = new Color(Random.value, Random.value, Random.value);
        foreach (GameObject shape in shapes)
        {
            shape.GetComponent<SpriteRenderer>().color = newColor;
        }
    }
    

}
