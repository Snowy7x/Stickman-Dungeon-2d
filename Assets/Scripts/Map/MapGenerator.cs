using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Room[] roomPrefabs;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float roomMinSize = 5f;
    [SerializeField] private float roomMaxSize = 10f;
    [SerializeField] private int numRooms = 10;

    [SerializeField] private float corridorWidth;
    [SerializeField] private float wallHeight;
    
    private List<Room> rooms = new List<Room>();
    private List<Rect> corridors = new List<Rect>();

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Create the first room
        Room roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        float firstRoomSize = Random.Range(roomMinSize, roomMaxSize);
        Room firstRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
        firstRoom.transform.SetParent(transform);
        firstRoom.transform.localScale = new Vector3(firstRoomSize, firstRoomSize, 1f);
        
        rooms.Add(firstRoom);

        // Generate additional rooms
        for (int i = 0; i < numRooms; i++)
        {
            // Select a random existing room
            Room currentRoom = rooms[^1];

            // Choose a random wall to spawn the new room
            Transform randomWall = currentRoom.GetRandomWallPosition();

            // Calculate the direction and distance for the corridor
            float corridorDistance = 15;

            float size = Random.Range(roomMinSize, roomMaxSize);
            Vector3 offset = randomWall.localPosition;

            if (Mathf.Abs(offset.x) > 0 && offset.y == 0)
                offset.x = (offset.x > 0 ? 1 : -1) * (size / 2) + (offset.x > 0 ? 1 : -1) * corridorDistance;
            if (Mathf.Abs(offset.y) > 0 && offset.x == 0)
                offset.y = (offset.y > 0 ? 1 : -1) * (size / 2) + (offset.y > 0 ? 1 : -1) * corridorDistance;

            int safetyI = 15;
            while (IsPositionOverlapping(randomWall.position + offset, new Vector2(size, size)) && safetyI > 0)
            {
                safetyI--;
                randomWall = currentRoom.GetRandomWallPosition(randomWall);
                offset = randomWall.localPosition;
                if (Mathf.Abs(offset.x) > 0 && offset.y == 0)
                    offset.x = (offset.x > 0 ? 1 : -1) * (size / 2) + (offset.x > 0 ? 1 : -1) * corridorDistance;
                if (Mathf.Abs(offset.y) > 0 && offset.x == 0)
                    offset.y = (offset.y > 0 ? 1 : -1) * (size / 2) + (offset.y > 0 ? 1 : -1) * corridorDistance;
            }

            if (IsPositionOverlapping(randomWall.position + offset, new Vector2(size, size)) && safetyI > 0) break;

            // Spawn the new room and connect it with a corridor
            Room newRoom = Instantiate(roomPrefab, randomWall.position + offset, Quaternion.identity);
            newRoom.transform.SetParent(transform);
            newRoom.transform.localScale = new Vector3(size, size, 1f);
            ConnectRooms(currentRoom, newRoom, randomWall);

            // Add the new room to the list
            rooms.Add(newRoom);
        }

        // Get the center position of all rooms
        Rect mapRect = CalculateBounds();
        Vector2 centerPoint = mapRect.center;
        centerPoint -= new Vector2(roomMaxSize / 2, roomMaxSize / 2);
    
        GraphScanner.instance.ScanGraph(mapRect.width, mapRect.height, 1, centerPoint);
    }

    public Rect CalculateBounds() {
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float maxY = -Mathf.Infinity;

        foreach (Room room in rooms) {
            minX = Mathf.Min(minX, room.GetBounds().xMin);
            minY = Mathf.Min(minY, room.GetBounds().yMin);
            maxX = Mathf.Max(maxX, room.GetBounds().xMax);
            maxY = Mathf.Max(maxY, room.GetBounds().yMax);
        }

        foreach (Rect corridor in corridors) {
            minX = Mathf.Min(minX, corridor.xMin);
            minY = Mathf.Min(minY, corridor.yMin);
            maxX = Mathf.Max(maxX, corridor.xMax);
            maxY = Mathf.Max(maxY, corridor.yMax);
        }

        float width = maxX - minX;
        float height = maxY - minY;
        Vector2 center = new Vector2(minX + width/2, minY + height/2);
        return new Rect(center.x - width/2, center.y - height/2, width, height);
    }

    bool IsPositionOverlapping(Vector2 position, Vector2 size)
    {
        float left = position.x - size.x / 2f;
        float right = position.x + size.x / 2f;
        float bottom = position.y - size.y / 2f;
        float top = position.y + size.y / 2f;

        foreach (Room room in rooms)
        {
            Vector3 pos = room.transform.position;
            float roomLeft = pos.x - room.GetSize().x / 2f;
            float roomRight = pos.x + room.GetSize().x / 2f;
            float roomBottom = pos.y - room.GetSize().y / 2f;
            float roomTop = pos.y + room.GetSize().y / 2f;

            if (left < roomRight && right > roomLeft && bottom < roomTop && top > roomBottom)
            {
                return true;
            }
        }

        return false;
    }

    void ConnectRooms(Room room1, Room room2, Transform wall1)
    {
        // Get the positions and sizes of the rooms
        Vector2 room1Position = room1.transform.position;
        Vector2 room2Position = room2.transform.position;
        Transform wall2 = room2.GetClosestWallPosition(room1Position);

        // Calculate the direction and distance between the rooms
        Vector2 direction = (room2Position - room1Position).normalized;

        // Calculate the position and rotation of the corridor walls
        float distance = Vector2.Distance(wall1.position, wall2.position);

        Vector2 middlePoint = Vector2.Lerp(wall1.position, wall2.position, 0.5f);
        float wallLength = distance - wall1.localScale.x - wall2.localScale.x;
        Vector2 offset = wall1.rotation.eulerAngles.z > 45 ? new Vector2(wallHeight, 0) : new Vector2(0, wallHeight);
        
        Quaternion wallRotation = Quaternion.LookRotation(Vector3.forward, direction);

        room1.SetWallGap(wallHeight, wall1);
        room2.SetWallGap(wallHeight, wall2);
        
        // Generate the corridor walls
        GameObject wall1Object = Instantiate(wallPrefab, middlePoint + offset, wallRotation, transform);
        GameObject wall2Object = Instantiate(wallPrefab, middlePoint - offset, wallRotation, transform);
        float cw = Mathf.Min(distance, corridorWidth);
        wall1Object.transform.localScale = new Vector3(cw, wallLength, 1f);
        wall2Object.transform.localScale = new Vector3(cw, wallLength, 1f);
        
        Rect wall1Rect = new Rect(wall1.position, wall1.localScale);
        Rect wall2Rect = new Rect(wall2.position, wall2.localScale);
        corridors.Add(wall1Rect);
        corridors.Add(wall2Rect);

    }

    
    /*
    void ConnectRooms(GameObject room1, GameObject room2)
    {
        Vector2 room1Position = room1.transform.position;
        Vector2 room2Position = room2.transform.position;
        float room1Size = room1.transform.localScale.x;
        float room2Size = room2.transform.localScale.x;

        // Calculate the direction and distance between the rooms
        Vector2 direction = (room2Position - room1Position).normalized;
        float distance = Vector2.Distance(room1Position, room2Position);

        // Calculate the position and rotation of the corridor walls
        Vector2 wall1Position = room1Position + (direction * room1Size / 2f);
        Vector2 wall2Position = room2Position - (direction * room2Size / 2f);
        
        Vector2 middlePoint = Vector2.Lerp(wall1Position, wall2Position, 0.5f);
        
        Quaternion wallRotation = Quaternion.LookRotation(Vector3.forward, direction);

        // Generate the corridor walls
        GameObject wall1Object = Instantiate(wallPrefab, middlePoint, wallRotation, transform);
        GameObject wall2Object = Instantiate(wallPrefab, middlePoint, wallRotation, transform);
        float wallLength = distance + (room1Size / 2f) + (room2Size / 2f);
        wall1Object.transform.localScale = new Vector3(corridorWidth, wallLength, 1f);
        wall2Object.transform.localScale = new Vector3(corridorWidth, wallLength, 1f);
    }
    */

}