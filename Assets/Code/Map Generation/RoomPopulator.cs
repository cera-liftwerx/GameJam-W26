using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class RoomPopulator
{
    private LayerGraph graph;
    private MapConfig config;


    public RoomPopulator(LayerGraph graph, MapConfig config)
    {
        this.graph = graph;
        this.config = config;
    }

    public void Populate()
    {
        // Debug.Log($"[forts] in populate");
        PopulateControlRoom();
        SpawnSpawnRoomPowerup();

        var eligible = graph.rooms
            .SelectMany(l => l)
            .Where(r => r.roomType == RoomType.Standard)
            .ToList();

        int total = eligible.Count;
        int powerupRoomCount = Mathf.FloorToInt(total * config.powerupRoomMultiplier);
        int enemyRoomCount = Mathf.FloorToInt(total * config.enemyRoomMultiplier);
        // Debug.Log($"[forts] total: {total}, powerupRoomCount: {powerupRoomCount}, enemyRoomCount: {enemyRoomCount}, config.powerupRoomMultiplier: {config.powerupRoomMultiplier}, config.enemyRoomMultiplier: {config.enemyRoomMultiplier}");

        Shuffle(eligible);

        for (int i = 0; i < eligible.Count; i++)
        {
            RoomNode room = eligible[i];
            bool wantsPowerup = i < powerupRoomCount;
            bool wantsEnemy = i < enemyRoomCount;
            bool coexist = Random.value < config.coexistChance;
            // Debug.Log($"[forts] populating room: {room.roomType}, i: {i }, powerupRoomCount: {powerupRoomCount}, enemyRoomCount: {enemyRoomCount}, wantsPowerup: {wantsPowerup}, wantsEnemy: {wantsEnemy}, coexist: {coexist}, config.coexistChance: {config.coexistChance}");

            RoomRole role;
            if (wantsPowerup && wantsEnemy && coexist) 
            {
                role = RoomRole.Both;
            }
            else if (wantsEnemy) 
            {
                role = RoomRole.Enemy;
            }
            else if (wantsPowerup) 
            {
                role = RoomRole.Powerup;
            }
            else 
            {
                role = RoomRole.Empty;
            }
            // Debug.Log($"[forts] populating room: {room.layerIndex}, {room.roomIndex}, room type: {room.roomType}, room role: {role}");

            room.SetRole(role);

            if (role == RoomRole.Enemy || role == RoomRole.Both)
            {
                // Debug.Log($"[forts] calling SpawnEnemies");
                SpawnEnemies(room);
            }
            if (role == RoomRole.Powerup || role == RoomRole.Both)
            {
                // Debug.Log($"[forts] calling spawnPowerups");
                SpawnPowerups(room);
            }
        }
    }

    void SpawnSpawnRoomPowerup()
    {
        var spawnRoom = graph.rooms[0][0];
        if (spawnRoom == null || config.powerupPrefab == null) return;

        var spawnPoints = spawnRoom.powerupSpawnPoints
            .Where(sp => sp != null)
            .ToArray();

        if (spawnPoints.Length == 0)
        {
            // Debug.LogWarning("spawn room has no powerup spawn points assigned");
            return;
        }

        Shuffle(spawnPoints);
        // Debug.Log($"[powerup] spawning 1 powerup in spawn room at {spawnPoints[0].position}");

        GameObject powerup = GameObject.Instantiate(
            config.powerupPrefab,
            spawnPoints[0].position,
            spawnPoints[0].rotation,
            spawnRoom.transform
        );
        spawnPoints[0].GetComponent<SpawnPoint>().isOccupied = true;
    }

    void SpawnEnemies(RoomNode room)
    {
        if (room.roomType != RoomType.Standard)
        {
            return;
        }
        if (config.enemyPrefab == null)
        {
            // Debug.LogWarning("no enemy prefab assigned in MapConfig");
            return;
        }

        // enemy count scales with layer depth and difficulty
        int count = Mathf.FloorToInt(config.difficulty * room.layerIndex * config.enemyCountMultiplier);
        count = Mathf.Clamp(count, config.minEnemiesPerRoom, config.maxEnemiesPerRoom);

        var spawnPoints = room.enemySpawnPoints
            .Where(sp => sp != null)
            .ToArray();

        Shuffle(spawnPoints); // pick random spawn points rather than always using the first n

        for (int i = 0; i < count && i < spawnPoints.Length; i++)
        {
            GameObject enemy = GameObject.Instantiate(
                config.enemyPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation,
                room.transform
            );
            enemy.transform.localScale = Vector3.one * 0.4f;
        }
    }

    void SpawnPowerups(RoomNode room)
    {
        if (room.roomType != RoomType.Standard) 
        {
            return;
        }
        if (config.powerupPrefab == null)
        {
            // Debug.LogWarning("no powerup prefab assigned in MapConfig");
            return;
        }

        int count = Mathf.FloorToInt(room.layerIndex * config.powerupCountMultiplier / config.difficulty);
        count = Mathf.Clamp(count, 1, room.powerupSpawnPoints.Length);
        // Debug.Log($"[powerup] count: {count}, layer: {room.layerIndex}, room: {room.roomIndex}");
        var spawnPoints = room.powerupSpawnPoints
            .Where(sp => sp != null)
            .ToArray();

        Shuffle(spawnPoints);


        for (int i = 0; i < count && i < spawnPoints.Length; i++)
        {
            GameObject powerup = GameObject.Instantiate(
                config.powerupPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation,
                room.transform
            );
            spawnPoints[i].GetComponent<SpawnPoint>().isOccupied = true;
        }
    }

    void PopulateControlRoom()
    {
        var controlRoom = graph.rooms
            .SelectMany(l => l)
            .FirstOrDefault(r => r.roomType == RoomType.Control);

        if (controlRoom == null || config.enemyPrefab == null) return;
        if (controlRoom.bossSpawnPoint == null)
        {
            // Debug.LogWarning("control room has no boss spawn point assigned");
            return;
        }

        GameObject boss = GameObject.Instantiate(
            config.enemyPrefab,
            controlRoom.bossSpawnPoint.position,
            controlRoom.bossSpawnPoint.rotation,
            controlRoom.transform
        );

        EnemyCharacter enemyCharacter = boss.GetComponent<EnemyCharacter>();
        enemyCharacter.setBossstatus();
        boss.transform.localScale = Vector3.one * 0.4f;
    }

    void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
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