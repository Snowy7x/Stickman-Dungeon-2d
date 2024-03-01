using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding;

public class GraphScanner : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    public static GraphScanner instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    public void ScanGraph(float mapWidth, float mapHeight, float nodeSize, Vector3 center)
    {
        Debug.Log("Scanning graph...");
        StartCoroutine(Scan((int)mapWidth, (int)mapHeight, nodeSize, center));
    }

    IEnumerator Scan(int mapWidth, int mapHeight, float nodeSize, Vector3 center)
    {
        // Get a reference to the AstarPath object
        AstarPath astar = AstarPath.active;

        // Resize the grid graph to match the map size
        GridGraph graph = astar.data.gridGraph;
        graph.SetDimensions(mapWidth, mapHeight, nodeSize);
        graph.center = center;
        
        foreach (Progress progress in AstarPath.active.ScanAsync()) {
            //Debug.Log("Scanning... " + progress.description + " - " + (progress.progress*100).ToString("0") + "%");
            progressBar.value = progress.progress;
            yield return null;
        }
        
        // Wait for the scan to complete then disable the loading screen
        GameManager.Instance.StartGame();
        Debug.Log("Graph scanned!");
        StopAllCoroutines();
    }
}