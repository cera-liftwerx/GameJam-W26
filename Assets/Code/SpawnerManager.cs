using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnerManager : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int totalItemsInGame = 3;
    private List<SpawnPoint> allPoints = new List<SpawnPoint>();

    void Start()
    {
        // Find all your SpawnPoint scripts in the scene
        allPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).ToList();

        for (int i = 0; i < totalItemsInGame; i++) 
        {
            SpawnInitialItem();
        }
    }

    void SpawnInitialItem()
    {
        SpawnPoint bestPoint = GetRandomAvailablePoint();
        if (bestPoint != null)
        {
            GameObject newObj = Instantiate(prefabToSpawn, bestPoint.transform.position, bestPoint.transform.rotation);
            
            GrabAndTeleport itemScript = newObj.GetComponent<GrabAndTeleport>();
            itemScript.manager = this;
            itemScript.currentPoint = bestPoint;
            
            bestPoint.isOccupied = true;
        }
    }

    public void MoveToRandomPoint(GameObject objToMove, SpawnPoint oldPoint)
    {
        // Free up the room the item just left
        if (oldPoint != null) oldPoint.isOccupied = false;

        // Find a new empty room/point
        SpawnPoint newPoint = GetRandomAvailablePoint();

        if (newPoint != null)
        {
            // Snap the object to the new location instantly
            objToMove.transform.position = newPoint.transform.position;
            objToMove.transform.rotation = newPoint.transform.rotation;
            
            // Update the references
            newPoint.isOccupied = true;
            objToMove.GetComponent<GrabAndTeleport>().currentPoint = newPoint;
        }
    }

    SpawnPoint GetRandomAvailablePoint()
    {
        List<SpawnPoint> available = allPoints.Where(p => !p.isOccupied).ToList();
        if (available.Count == 0) return null;
        return available[Random.Range(0, available.Count)];
    }
}
