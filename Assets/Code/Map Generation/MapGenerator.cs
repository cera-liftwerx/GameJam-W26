using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public MapConfig config;

    [Header("prefabs")]
    public GameObject controlRoomPrefab;
    public GameObject standardRoomPrefab;
    public GameObject spawnRoomPrefab;
    public GameObject tutorialNote;
    private LayerGraph graph = new();


    void Awake()
    {
        // GenerateMap();
        // microphoneDetector.SetActive(true);
    }

    public void GenerateMap()
    {
        RandomizeConfig();
        graph = new LayerGraph();
        PlaceAllRooms();
        RandomizeTopDoors();
        new RoomPopulator(graph, config).Populate();
        PlacePlayer();
        tutorialNote.SetActive(true);
    }

    void PlacePlayer()
    {
        // Debug.Log("why fort PlacePlayer");
        var spawnRoom = graph.rooms[0][0];
        if (spawnRoom == null || spawnRoom.playerSpawnPoint == null)
        {
            Debug.LogWarning("no player spawn point assigned on spawn room");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("no gameobject tagged player found in scene");
            return;
        }
        // Debug.Log($"why fort {spawnRoom.playerSpawnPoint.position}, player name {player.name}");
        // spawn a temporary floor collider under the player spawn point
        GameObject tempFloor = new GameObject("temp floor collider");
        tempFloor.transform.position = spawnRoom.playerSpawnPoint.position - new Vector3(0, 0.1f, 0);
        BoxCollider col = tempFloor.AddComponent<BoxCollider>();
        col.size = new Vector3(3f, 0.2f, 3f);
        player.transform.position = spawnRoom.playerSpawnPoint.position;
    }

    void RandomizeConfig()
    {
        config.n = Random.Range(3, 6); // inclusive
        config.m = Random.Range(2, 3) * 2 - 1; // random odd number between 3 and 5 inclusive
    }

    void RandomizeTopDoors()
    {
        for (int layer = 0; layer < graph.LayerCount - 1; layer++)
        {
            var layerRooms = graph.rooms[layer];

            // collect all top doors across all rooms in this layer
            var allTopDoors = layerRooms
                .SelectMany(r => r.doorTop)
                .Where(d => d != null)
                .ToList();

            if (allTopDoors.Count == 0) continue;

            Shuffle(allTopDoors);

            for (int i = 0; i < allTopDoors.Count; i++)
            {
                // first shuffled door always stays enabled to guarantee a path exists
                DoorOpener opener = allTopDoors[i].GetComponentInChildren<DoorOpener>();
                if (opener != null)
                {
                    opener.isDisabled = i != 0 && Random.value > 0.5f;
                    // Debug.Log($"[door] layer {layer} door {allTopDoors[i].name} disabled: {opener.isDisabled}");
                }
            }
        }
    }

    void PlaceAllRooms()
    {
        int totalLayers = config.n + 1; // layer 0 (spawn) through layer n (control)

        for (int layer = 0; layer < totalLayers; layer++)
        {
            int count = RoomCountForLayer(layer, totalLayers);
            graph.rooms.Add(new List<RoomNode>());

            GameObject prefab = layer == 0 ? spawnRoomPrefab
                : layer == totalLayers - 1 ? controlRoomPrefab
                : standardRoomPrefab;

            for (int i = 0; i < count; i++)
            {
                SpawnRoom(layer, i, count, prefab);   
                graph.rooms[layer][i].SetupTeleportation();
            }
        }
    }

    int RoomCountForLayer(int layer, int totalLayers)
    {
        if (layer == 0 || layer == totalLayers - 1)
        {
            return 1;
        }
        if (layer == 1 || layer == totalLayers - 2)
        {
            return 3;
        }
        return config.m;
    }

    void SpawnRoom(int layer, int roomIndex, int roomsInLayer, GameObject prefab)
    {
        float totalWidth = (roomsInLayer - 1) * config.standardRoomWidth;
        float z = -totalWidth / 2f + roomIndex * config.standardRoomWidth;

        float x;
        int totalLayers = config.n + 1;
        if (layer == 0)
        {
            x = (totalLayers - 1) * config.spawnToFirstLayer; // spawn room starts furthest out
        }
        else if (layer == totalLayers - 1)
        {
            x = 0f; // control room at origin
        }
        else
        {
            x = (totalLayers - 1 - layer) * config.standardLayerStep; // standard layers step inward
        }

        Vector3 pos = new Vector3(x, 0f, z);

        RoomType type = prefab.GetComponent<RoomNode>().roomType;
        if (type == RoomType.Spawn)
        {
            pos += new Vector3(-0.556f, 0f, 8.221f);
        }
        else if (type == RoomType.Control)
        {
            pos += new Vector3(11.964f, 0f, 0.381f);
        }

        Quaternion rot = type == RoomType.Spawn
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.identity;

        GameObject go = Instantiate(prefab, pos, rot, transform);
        go.tag = "MapGeometry";
        go.name = $"Room_L{layer}_R{roomIndex}";

        RoomNode node = go.GetComponent<RoomNode>();
        node.layerIndex = layer;
        node.roomIndex = roomIndex;
        graph.rooms[layer].Add(node);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}