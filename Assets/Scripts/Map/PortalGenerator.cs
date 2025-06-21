using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PortalGenerator : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    
    [SerializeField] private Tilemap blockTilemap, obstacleTilemap;
    [SerializeField] private GameObject portalPrefab;
    
    [SerializeField] private float maxPortalCountPerChunks;
    [SerializeField] private int portalGenerationDelay;

    [SerializeField] private int initialEnemyCount = 6;
    [SerializeField] private float enemyIncreaseRate = 0.05f;
    private float currentEnemyCount;

    private List<EnemySpawnPortal> portals;

    private void Awake()
    {
        portals = new List<EnemySpawnPortal>();
        currentEnemyCount = initialEnemyCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GeneratePortal());
    }
    
    IEnumerator GeneratePortal()
    {
        yield return new WaitUntil(()=>mapGenerator.CurrentConfig!=null);
        int width = mapGenerator.CurrentConfig.width * mapGenerator.CurrentConfig.chunkSize.x;
        int height = mapGenerator.CurrentConfig.height * mapGenerator.CurrentConfig.chunkSize.y;

        int chunkCount = mapGenerator.CurrentConfig.height * mapGenerator.CurrentConfig.width;
        
        while (true)
        {
            if (portals.Count < maxPortalCountPerChunks * chunkCount)
            {
                int x, y;
                
                while (true)
                {
                    x = Random.Range(0, width);
                    y = Random.Range(0, height);

                    if (blockTilemap.GetTile(new Vector3Int(x, y, 0)) == null &&
                        obstacleTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                        break;
                }
                
                EnemySpawnPortal portal = Instantiate(portalPrefab, new Vector3(x + 0.5f, y + 0.5f, 0),
                    Quaternion.identity).GetComponent<EnemySpawnPortal>();
                portal.MaxCount = Mathf.RoundToInt(currentEnemyCount);

                portals.Add(portal);
                portal.onDestroy.AddListener(() => portals.Remove(portal));
            }

            currentEnemyCount += Time.deltaTime * enemyIncreaseRate;
            yield return new WaitForSeconds(portalGenerationDelay);
        }
    }
}
