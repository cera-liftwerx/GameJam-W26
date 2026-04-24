using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "MapGen/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Header("prefabs")]
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public GameObject powerupPrefab;

    [Header("layout")]
    [SerializeField] public int n = 4; // layer
    [SerializeField] public int m = 3; // rooms per layer

    [Header("content multipliers")]
    [SerializeField] public float powerupRoomMultiplier = 1f;
    [SerializeField] public float powerupCountMultiplier = 1f;
    [SerializeField] public float enemyRoomMultiplier = 0.5f;
    [SerializeField] public float enemyCountMultiplier = 0.5f;
    [SerializeField] public float coexistChance = 1f;

    [Header("difficulty")]
    [SerializeField] public int difficulty = 1;
    [SerializeField] public int maxEnemiesPerRoom = 6;
    [SerializeField] public int minEnemiesPerRoom = 0;

    [Header("room spacing")]
    [SerializeField] public float standardRoomWidth = 16f; // z spacing between adjacent standard rooms
    [SerializeField] public float spawnToFirstLayer = 16f; // x spacing from spawn room to layer 1
    [SerializeField] public float layerToControlRoom = 4.1f; // x spacing from last standard layer to control room
    [SerializeField] public float standardLayerStep = 16f; // x spacing between middle standard layers
}