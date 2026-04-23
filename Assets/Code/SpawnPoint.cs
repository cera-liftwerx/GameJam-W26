using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool isOccupied = false;
    public Transform spawnPointTf;

    public SpawnPoint(Transform tf)
    {
        isOccupied = true;
        spawnPointTf = tf;
    }
}
