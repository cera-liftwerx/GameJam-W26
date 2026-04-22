using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum RoomRole { Powerup, Enemy, Both, Empty }
public enum RoomType { Spawn, Standard, Control }

public class RoomNode : MonoBehaviour
{
    [Header("room type")]
    public RoomType roomType; // set this in each prefab in the inspector

    [Header("doors")]
    public List<GameObject> doorTop; // spawn room has 3, standard room has 1, control room leave empty

    [Header("spawn points")]
    public Transform[] powerupSpawnPoints;
    public Transform[] enemySpawnPoints;
    public Transform bossSpawnPoint;

    [HideInInspector] public RoomRole role;
    [HideInInspector] public int layerIndex;
    [HideInInspector] public int roomIndex;

    public void SetRole(RoomRole r)
    {
        // spawn and control rooms never get content, ignore any role assignment
        if (roomType != RoomType.Standard)
        {
            return;
        }

        role = r;

        foreach (var sp in powerupSpawnPoints)
        {
            if (sp != null)
            {
                sp.gameObject.SetActive(false);
            }
        }

        if (r == RoomRole.Powerup || r == RoomRole.Both)
        {
            var valid = powerupSpawnPoints.Where(sp => sp != null).ToArray();
            if (valid.Length > 0)
            {
                valid[Random.Range(0, valid.Length)].gameObject.SetActive(true);
            }
        }

        bool hasEnemies = r == RoomRole.Enemy || r == RoomRole.Both;
        foreach (var sp in enemySpawnPoints)
        {
            if (sp != null) sp.gameObject.SetActive(hasEnemies);
        }
    }
}