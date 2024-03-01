using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    public Transform[] walls;
    [SerializeField] Challenges myChallenge;
    private RoomChallenge currentChallenge;
    private Rect bounds = Rect.zero;
    
    bool isStarted = false;
    bool canStart = false;

    private void Start()
    {
        myChallenge = (Challenges)Random.Range(0, Enum.GetValues(typeof(Challenges)).Length);
        canStart = true;
    }

    private void Update()
    {
        if (currentChallenge == null) return;
        currentChallenge.OnUpdate();
    }

    public Vector2 GetSize()
    {
        return transform.localScale;
    }
    
    public Rect GetBounds()
    {
        if (bounds == Rect.zero)
        {
            bounds = new Rect(transform.position, transform.localScale);
        }
        return bounds;
    }
    
    public Transform GetClosestWallPosition(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        Transform closestWall = transform;

        foreach (Transform child in walls)
        {
            Vector2 wallPosition = child.position;
            float distance = Vector2.Distance(position, wallPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWall = child;
            }
        }

        return closestWall;
    }

    public Vector2 GetOffsetFromWall(Vector2 wallPosition)
    {
        Vector2[] wallPositions = new Vector2[] { 
            transform.position + new Vector3(GetSize().x / 2f, 0f), 
            transform.position + new Vector3(-GetSize().x / 2f, 0f), 
            transform.position + new Vector3(0f, GetSize().y / 2f), 
            transform.position + new Vector3(0f, -GetSize().y / 2f) 
        };

        float closestDistance = float.MaxValue;
        Vector2 closestPosition = Vector2.zero;
        for (int i = 0; i < wallPositions.Length; i++)
        {
            float distance = Vector2.Distance(wallPositions[i], wallPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = wallPositions[i];
            }
        }

        Vector2 offset = wallPosition - closestPosition;
        return offset;
    }

    public void SetWallGap(float gap, Transform wall)
    {
        float length = wall.localScale.y > wall.localScale.x ? wall.localScale.y : wall.localScale.x;
        float width = wall.localScale.y > wall.localScale.x ? wall.localScale.x : wall.localScale.y;
        float lengthGap = gap / transform.localScale.x;
        float wallLength = (length / 2);

        Transform wall1  = Instantiate(wall, transform);
        Vector3 pos1 = wall.localPosition;
        if (wall.rotation.eulerAngles.z > 45) pos1.x += (pos1.x < 0 ? 1 : -1) * (wallLength / 2 + lengthGap / 2);
        else pos1.y += (pos1.y > 0 ? 1 : -1) * (wallLength / 2 + lengthGap / 2);
        wall1.SetLocalPositionAndRotation(pos1, wall.localRotation);
        wall1.localScale = new Vector3(width, wallLength - lengthGap, 1);
        
        Transform wall2  = Instantiate(wall, transform);
        Vector3 pos2 = wall.localPosition;
        if (wall.rotation.eulerAngles.z > 45) pos2.x += (pos2.x < 0 ? -1 : 1) * (wallLength / 2 + lengthGap / 2);
        else pos2.y += (pos2.y > 0 ? -1 : 1) * (wallLength / 2 + lengthGap / 2);
        wall2.SetLocalPositionAndRotation(pos2, wall.localRotation);
        wall2.localScale = new Vector3(width, wallLength - lengthGap, 1);

        List<Transform> nWalls = walls.ToList();
        nWalls.Remove(wall);
        nWalls.AddRange(new []{
            wall1, wall2
        });
        walls = nWalls.ToArray();
        wall.gameObject.SetActive(false);
    }

    public Transform GetRandomWallPosition(Transform blacked = null)
    {
        List<Transform> newWalls = walls.Where(x => x != blacked).ToList();
        return newWalls[Random.Range(0, newWalls.Count)];
    }
    
    // Random position in room
    public Vector2 GetRandomPosition()
    {
        Vector2 size = GetSize();
        return transform.position + new Vector3(Random.Range(-size.x / 2f, size.x / 2f), Random.Range(-size.y / 2f, size.y / 2f));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !isStarted && canStart)
        {
            Debug.Log("Player entered room 1");
            StartChallenge();
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player") && isStarted) currentChallenge.OnPlayerExit();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isStarted && canStart)
        {
            Debug.Log("Player entered room 2");
            StartChallenge();
        }
    }

    void StartChallenge()
    {
        currentChallenge = GameManager.Instance.StartChallenge(this, myChallenge);
        // try at least 3 times to start the challenge then give up
        if (currentChallenge == null) currentChallenge = GameManager.Instance.StartChallenge(this, myChallenge);
        if (currentChallenge == null) currentChallenge = GameManager.Instance.StartChallenge(this, myChallenge);
        if (currentChallenge == null) return;
        
        currentChallenge.Initialize(this, 0);
        isStarted = true;
        currentChallenge.OnPlayerEnter();
    }
}
