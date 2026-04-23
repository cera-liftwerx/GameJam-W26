using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SpawnerManager : MonoBehaviour
{
    // public GameObject prefabToSpawn;
    // public int totalItemsInGame = 3;
    private List<SpawnPoint> allPoints = new List<SpawnPoint>();
    public Slider fartBar;
    private float fartsRemaining = 0f;
    private float maxFarts = 2f;

    void Start()
    {
        // Find all your SpawnPoint scripts in the scene
        allPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).ToList();
        // Debug.Log($"[fart] allpoints has {allPoints.Count} items");

        // for (int i = 0; i < totalItemsInGame; i++) 
        // {
        //     SpawnInitialItem();
        // }
    }

    // void SpawnInitialItem()
    // {
    //     SpawnPoint bestPoint = GetRandomAvailablePoint();
    //     if (bestPoint != null)
    //     {
    //         Quaternion uprightRotation = Quaternion.Euler(90, 0, 0);
    //         GameObject newObj = Instantiate(prefabToSpawn, bestPoint.transform.position, uprightRotation);
            
    //         GrabAndTeleport itemScript = newObj.GetComponent<GrabAndTeleport>();
    //         itemScript.manager = this;
    //         itemScript.currentPoint = bestPoint;
            
    //         bestPoint.isOccupied = true;
    //     }
    // }

    public void MoveToRandomPoint(GameObject objToMove, SpawnPoint oldPoint)
    {
        // Debug.Log("[fart] inside MoveToRandomPoint");

        // Find a new empty room/point
        SpawnPoint newPoint = GetRandomAvailablePoint();

        // Free up the room the item just left
        if (oldPoint != null) oldPoint.isOccupied = false;

        if (newPoint != null)
        {
            // Snap the object to the new location instantly
            objToMove.transform.position = newPoint.transform.position;
            // objToMove.transform.rotation = newPoint.transform.rotation;
            
            // Update the references
            newPoint.isOccupied = true;
            objToMove.GetComponent<GrabAndTeleport>().currentPoint = newPoint;
            // Debug.Log("[fart] inside MoveToRandomPoint and moved point");
        } else
        {
            objToMove.SetActive(false);
            // Debug.Log("[fart] inside MoveToRandomPoint and hid bc no availaible spawnpoints");
        }
    }

    SpawnPoint GetRandomAvailablePoint()
    {
        // Debug.Log("[fart] inside GetRandomAvailablePoint");
        List<SpawnPoint> available = allPoints.Where(p => !p.isOccupied).ToList();
        if (available.Count == 0) return null;
        return available[Random.Range(0, available.Count)];
    }

    public bool ReleaseFart()
    {
        if (fartsRemaining <= 0) return false;
        
        fartsRemaining -= 1f;
        fartBar.value = (float)fartsRemaining / (float)maxFarts; // Slider value is 0 to 1

        return true;
    }

    public void replenishFart()
    {
        // Debug.Log("[fart] inside replenishFart");
        if (fartsRemaining >= maxFarts) return;
        
        fartsRemaining += 1f;
        fartBar.value = (float)fartsRemaining / (float)maxFarts; // Slider value is 0 to 1

        // Debug.Log($"[fart] changed fartbar val to {fartBar.value}");
    }
}