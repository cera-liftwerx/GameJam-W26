using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "MapGen/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Header("prefabs")]
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;

    [Header("layout")]
    public int n = 4; // layer
    public int m = 3; // rooms per layer

    [Header("content multipliers")]
    [Range(0f, 1f)] public float powerupMultiplier = 0.4f;
    [Range(0f, 1f)] public float enemyRoomMultiplier = 0.5f;
    [Range(0f, 1f)] public float enemyCountMultiplier = 0.6f;
    [Range(0f, 0.2f)] public float coexistChance = 0.1f;

    [Header("difficulty")]
    [Range(1, 5)] public int difficulty = 2;
    public int maxEnemiesPerRoom = 9;
    public int minEnemiesPerRoom = 0;

    [Header("room spacing")]
    public float standardRoomWidth = 16f; // z spacing between adjacent standard rooms
    public float spawnToFirstLayer = 16.4f; // x spacing from spawn room to layer 1
    public float layerToControlRoom = 4.1f; // x spacing from last standard layer to control room
    public float standardLayerStep = 16f; // x spacing between middle standard layers
}