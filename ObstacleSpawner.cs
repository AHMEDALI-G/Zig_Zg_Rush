using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public PathGenerator pathGenerator;

    [Header("Obstacle Prefabs")]
    public GameObject obstacleTall;
    public GameObject obstacleLow;
    public GameObject obstacleWide;
    public GameObject obstacleCave;
    public GameObject obstacleHurdle;
    public GameObject obstacleDrum;
    public GameObject obstacleFence;

    [Header("Coins")]
    public GameObject coinPrefab;

    [Header("Spawn Settings")]
    public float spawnDistanceAhead = 60f;

    // --- CHANGED: Much faster spawning ---
    public float minSpawnInterval = 0.2f;
    public float maxSpawnInterval = 0.8f;

    public float laneDistance = 3f;

    private float spawnTimer = 0f;
    private float nextSpawnTime = 0.8f;
    private int spawnsSinceLastCave = 0;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= nextSpawnTime)
        {
            SpawnRandomObstacle();
            spawnTimer = 0f;

            // --- CHANGED: Reaches max difficulty in 20 seconds instead of 60 ---
            float difficulty = Mathf.Clamp01(Time.timeSinceLevelLoad / 20f);
            nextSpawnTime = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, difficulty);
        }
    }

    void SpawnRandomObstacle()
    {
        float spawnZ = player.position.z + spawnDistanceAhead;
        float pathX = pathGenerator.GetPathXAtZ(spawnZ);

        spawnsSinceLastCave++;

        // 1. 25% Chance to spawn a Coin Bundle
        if (Random.Range(0, 100) < 25)
        {
            SpawnCoinBundle(pathX, spawnZ);
            return;
        }

        // 2. Check for Cave
        bool canSpawnCave = (spawnsSinceLastCave >= 3) && (Time.timeSinceLevelLoad > 5f);
        if (canSpawnCave && Random.Range(0, 100) < 20)
        {
            SpawnCave(pathX, spawnZ);
            spawnsSinceLastCave = 0;
            return;
        }

        // 3. Check for Rolling Drum (Only after 15 seconds now, so it appears faster)
        if (Time.timeSinceLevelLoad > 15f && Random.Range(0, 100) < 15)
        {
            SpawnDrum(pathX, spawnZ);
            return;
        }

        // 4. Standard Obstacles
        int roll = Random.Range(0, 100);
        if (roll < 20) SpawnZigzagTrap(pathX, spawnZ);
        else if (roll < 40) SpawnHurdle(pathX, spawnZ);
        else if (roll < 60) SpawnDouble(pathX, spawnZ);
        else if (roll < 80) SpawnSingle(pathX, spawnZ);
        else SpawnWide(pathX, spawnZ);
    }

    void SpawnCoinBundle(float pathX, float z)
    {
        if (coinPrefab == null) return;

        int lane = Random.Range(0, 3);
        float x = pathX + (lane - 1) * laneDistance;

        // 1. A line of coins on the ground leading up
        for (int i = 0; i < 3; i++)
        {
            Instantiate(coinPrefab, new Vector3(x, 1.2f, z + (i * 2f)), Quaternion.identity);
        }

        // 2. A U-Shape arc of coins in the air
        float startZ = z + 8f;
        Instantiate(coinPrefab, new Vector3(x - 1.5f, 2f, startZ), Quaternion.identity);
        Instantiate(coinPrefab, new Vector3(x - 1.5f, 2.5f, startZ + 2f), Quaternion.identity);
        Instantiate(coinPrefab, new Vector3(x, 3f, startZ + 4f), Quaternion.identity);
        Instantiate(coinPrefab, new Vector3(x + 1.5f, 2.5f, startZ + 6f), Quaternion.identity);
        Instantiate(coinPrefab, new Vector3(x + 1.5f, 2f, startZ + 8f), Quaternion.identity);
    }

    void SpawnDrum(float pathX, float z)
    {
        if (obstacleDrum == null) return;
        int lane = Random.Range(0, 3);
        float x = pathX + (lane - 1) * laneDistance;
        GameObject ob = Instantiate(obstacleDrum, new Vector3(x, 1.5f, z + 20f), Quaternion.identity);
        AttachCleanup(ob);
    }

    void SpawnCave(float pathX, float z)
    {
        if (obstacleCave == null) return;
        GameObject ob = Instantiate(obstacleCave, new Vector3(pathX, 0f, z), Quaternion.identity);
        AttachCleanup(ob);
    }

    void SpawnZigzagTrap(float pathX, float z)
    {
        int freeLane = 1;
        for (int lane = 0; lane < 3; lane++)
        {
            if (lane == freeLane) continue;
            float x = pathX + (lane - 1) * laneDistance;
            GameObject ob = Instantiate(obstacleTall, new Vector3(x, 1f, z), Quaternion.identity);
            AttachCleanup(ob);
        }
    }

    void SpawnHurdle(float pathX, float z)
    {
        if (obstacleHurdle == null) return;
        int lane = Random.Range(0, 3);
        float x = pathX + (lane - 1) * laneDistance;
        GameObject ob = Instantiate(obstacleHurdle, new Vector3(x, 0.4f, z - 10f), Quaternion.identity);
        AttachCleanup(ob);
    }

    void SpawnSingle(float pathX, float z)
    {
        int lane = Random.Range(0, 3);
        float x = pathX + (lane - 1) * laneDistance;
        GameObject prefab = (Random.Range(0, 100) < 50) ? obstacleTall : obstacleLow;
        float y = (prefab == obstacleTall) ? 1f : 0.4f;
        if (prefab == null) return;
        GameObject ob = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        AttachCleanup(ob);
    }

    void SpawnDouble(float pathX, float z)
    {
        if (obstacleTall == null) return;
        int freeLane = Random.Range(0, 3);
        for (int lane = 0; lane < 3; lane++)
        {
            if (lane == freeLane) continue;
            float x = pathX + (lane - 1) * laneDistance;
            GameObject ob = Instantiate(obstacleTall, new Vector3(x, 1f, z), Quaternion.identity);
            AttachCleanup(ob);
        }
    }

    void SpawnWide(float pathX, float z)
    {
        if (obstacleWide == null) return;
        GameObject ob = Instantiate(obstacleWide, new Vector3(pathX, 1f, z), Quaternion.identity);
        AttachCleanup(ob);
    }

    void AttachCleanup(GameObject ob)
    {
        ObstacleCleanup cleanup = ob.GetComponent<ObstacleCleanup>();
        if (cleanup != null) cleanup.player = player;
    }
}