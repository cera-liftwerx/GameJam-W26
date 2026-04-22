using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;


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
    public Transform playerSpawnPoint;

    [Header("teleportation")]
    public GameObject[] floorObjects; // assign all floor tile gameobjects in inspector

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

    public void SetupTeleportation()
    {
        TeleportationArea teleportArea = FindObjectOfType<TeleportationArea>();
        if (teleportArea == null)
        {
            Debug.LogWarning("no teleportation area found in scene");
            return;
        }

        foreach (var floor in floorObjects)
        {
            if (floor == null) continue;
            Collider col = floor.GetComponent<Collider>();
            if (col == null) continue;

            if (!teleportArea.colliders.Contains(col))
                teleportArea.colliders.Add(col);
        }

        // force re registration by toggling the component
        teleportArea.enabled = false;
        teleportArea.enabled = true;
    }
}