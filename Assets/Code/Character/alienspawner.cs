using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alienspawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPool = new List<GameObject>();
    public static List<GameObject> allZombies = new List<GameObject>();
    [SerializeField] private float baseSpawnPeriod = 10f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    { 
        while (true)
        {
            int index = 0;//Random.Range(0, spawnPool.Count);
            GameObject zomb = Instantiate(spawnPool[index], transform.position, transform.rotation);
            allZombies.Add(zomb);
            yield return new WaitForSeconds(baseSpawnPeriod);
        }
    }
}
