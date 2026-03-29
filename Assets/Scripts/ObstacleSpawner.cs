using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("障碍物预制体")]
    public List<GameObject> obstaclePrefabs;
    public List<GameObject> platformPrefabs;
    
    [Header("生成设置")]
    public float spawnDistance = 50f;
    public float spawnInterval = 10f;
    public float platformLength = 10f;
    public int maxObstacles = 20;
    
    [Header("障碍物设置")]
    public float minObstacleHeight = 1f;
    public float maxObstacleHeight = 3f;
    public float minGap = 2f;
    public float maxGap = 5f;
    
    [Header("玩家引用")]
    public Transform player;
    
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private float nextSpawnZ = 0f;
    private float lastPlatformEnd = 0f;
    
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // 生成初始平台
        SpawnInitialPlatforms();
    }
    
    void Update()
    {
        if (player == null) return;
        
        // 当玩家接近时生成新的障碍物
        if (player.position.z + spawnDistance > nextSpawnZ)
        {
            SpawnPlatformWithObstacles();
        }
        
        // 清理远离玩家的障碍物
        CleanupFarObjects();
    }
    
    void SpawnInitialPlatforms()
    {
        // 生成起始平台
        for (int i = 0; i < 5; i++)
        {
            SpawnPlatformWithObstacles();
        }
    }
    
    void SpawnPlatformWithObstacles()
    {
        if (platformPrefabs.Count == 0) return;
        
        // 随机选择平台
        GameObject platformPrefab = platformPrefabs[Random.Range(0, platformPrefabs.Count)];
        
        // 计算平台位置
        Vector3 platformPosition = new Vector3(0f, 0f, lastPlatformEnd);
        
        // 生成平台
        GameObject platform = Instantiate(platformPrefab, platformPosition, Quaternion.identity);
        platform.transform.SetParent(transform);
        spawnedObjects.Add(platform);
        
        // 获取平台长度（假设平台在Z轴方向）
        Renderer platformRenderer = platform.GetComponentInChildren<Renderer>();
        float currentPlatformLength = platformLength;
        if (platformRenderer != null)
        {
            currentPlatformLength = platformRenderer.bounds.size.z;
        }
        
        // 在平台上生成障碍物
        SpawnObstaclesOnPlatform(platform, currentPlatformLength);
        
        // 更新下一个生成位置
        lastPlatformEnd += currentPlatformLength + Random.Range(minGap, maxGap);
        nextSpawnZ = lastPlatformEnd;
    }
    
    void SpawnObstaclesOnPlatform(GameObject platform, float platformLength)
    {
        if (obstaclePrefabs.Count == 0) return;
        
        // 随机决定生成多少个障碍物
        int obstacleCount = Random.Range(0, 4);
        
        for (int i = 0; i < obstacleCount; i++)
        {
            // 随机选择障碍物
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
            
            // 计算障碍物位置（在平台上随机位置）
            float randomZ = platform.transform.position.z + Random.Range(2f, platformLength - 2f);
            float randomX = Random.Range(-3f, 3f);
            float randomY = Random.Range(minObstacleHeight, maxObstacleHeight);
            
            Vector3 obstaclePosition = new Vector3(randomX, randomY / 2f, randomZ);
            
            // 生成障碍物
            GameObject obstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity);
            obstacle.transform.SetParent(platform.transform);
            
            // 随机缩放
            float scale = Random.Range(0.8f, 1.5f);
            obstacle.transform.localScale = new Vector3(scale, randomY, scale);
        }
    }
    
    void CleanupFarObjects()
    {
        if (player == null) return;
        
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
                continue;
            }
            
            // 如果物体远离玩家后方，销毁它
            if (spawnedObjects[i].transform.position.z < player.position.z - 50f)
            {
                Destroy(spawnedObjects[i]);
                spawnedObjects.RemoveAt(i);
            }
        }
    }
    
    // 重置生成器
    public void ResetSpawner()
    {
        // 销毁所有生成的物体
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
        
        // 重置位置
        nextSpawnZ = 0f;
        lastPlatformEnd = 0f;
        
        // 重新生成初始平台
        SpawnInitialPlatforms();
    }
}
