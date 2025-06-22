using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [field: SerializeField] public int CurrentFloor { get; private set; } = 1;
    [SerializeField] private List<MapGeneratorConfig> config;
    
    [SerializeField] private SpriteRenderer bgSpriteRenderer;
    [SerializeField] private Tilemap blockTilemap, propTilemap, obstacleTilemap;

    public MapGeneratorConfig CurrentConfig { get; private set; }
    
    private GameObject[,] mapGrid;
    private bool[,] visited;
    
    Dictionary<Vector2Int, HashSet<Vector2Int>> pathConnections;
    
    TileBase[,] GetTiles(GameObject chunkPrefab)
    {
        // chunkPrefab에서 Tilemap 컴포넌트 찾기
        Tilemap tilemap = chunkPrefab.GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap component not found on prefab");
            return null;
        }

        // 타일맵의 타일이 깔려있는 영역 가져오기
        BoundsInt bounds = tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        TileBase[,] tiles = new TileBase[width, height];

        // 영역 전체를 순회하며 타일을 배열에 저장
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int localPos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                tiles[x, y] = tilemap.GetTile(localPos);
            }
        }

        return tiles;
    }
    
    public void GenerateMap(int floor)
    {
        CurrentConfig = config.FirstOrDefault(_config => _config.minFloor <= floor && floor <= _config.maxFloor) 
                        ?? config[^1];
        
        bgSpriteRenderer.sprite = CurrentConfig.bgSprite;
        
        int width = CurrentConfig.width;
        int height = CurrentConfig.height;
        
        Vector2Int chunkSize = CurrentConfig.chunkSize;
        
        visited = new bool[width, height];
        pathConnections = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    
        DFS(Vector2Int.zero);

        Vector2Int beginChunkPos = new Vector2Int(CurrentConfig.beginChunkX, 0);
        Vector2Int endChunkPos = new Vector2Int(CurrentConfig.endChunkX, height - 1);
    
        foreach (var pos in pathConnections.Keys)
        {
            if (pos == beginChunkPos)
                SetChunkToTilemap(pos, GetTiles(CurrentConfig.beginChunkPrefab));

            if (pos == endChunkPos)
            {
                SetChunkToTilemap(pos, GetTiles(CurrentConfig.exitChunkPrefab));

                if (CurrentConfig.unlockedDoorPortalPrefab != null)
                {
                    Vector3 doorPos = new Vector3(
                        pos.x * chunkSize.x + CurrentConfig.exitChunkDoorPos.x,
                        pos.y * chunkSize.y + CurrentConfig.exitChunkDoorPos.y,
                        0);
                
                    GameObject unlockedDoorPortal = Instantiate(
                        CurrentConfig.unlockedDoorPortalPrefab, doorPos, quaternion.identity, transform);
                }

                if (CurrentConfig.bossEnemyPrefab != null)
                {
                    Vector3 bossPos = new Vector3(
                        pos.x * chunkSize.x + CurrentConfig.bossEnemyPos.x,
                        pos.y * chunkSize.y + CurrentConfig.bossEnemyPos.y,
                        0);
                
                    GameObject boss = Instantiate(
                        CurrentConfig.bossEnemyPrefab, bossPos, quaternion.identity);
                }
            }
            
            if (pos.y == beginChunkPos.y && beginChunkPos.x <= pos.x &&
                     pos.x <= beginChunkPos.x + CurrentConfig.beginChunkWidth - 1)
                continue;
            
            if (pos.y == endChunkPos.y && endChunkPos.x <= pos.x &&
                    pos.x <= endChunkPos.x + CurrentConfig.endChunkWidth - 1)
                continue;
            
            var directions = pathConnections[pos];
            GameObject prefab = SelectChunkMatchingDirections(directions);
            
            TileBase[,] tiles = GetTiles(prefab);
            SetChunkToTilemap(pos, tiles);
        }
        
        for (int x = 0; x < width * chunkSize.x; ++x)
        {
            for(int y = -10; y <= 0; ++y)
                blockTilemap.SetTile(new Vector3Int(x, y, 0), CurrentConfig.targetBlockTile);
            
            int maxHeightInBlock = height * chunkSize.y;
            
            for(int y = maxHeightInBlock; y <= maxHeightInBlock + 10; ++y)
                blockTilemap.SetTile(new Vector3Int(x, y, 0), CurrentConfig.targetBlockTile);
        }

        for (int y = 0; y < height * chunkSize.y; ++y)
        {
            blockTilemap.SetTile(new Vector3Int(0, y, 0), CurrentConfig.targetBlockTile);
            blockTilemap.SetTile(new Vector3Int(width * chunkSize.x - 1, y, 0), CurrentConfig.targetBlockTile);
        }
        
        if(CurrentConfig.bossEnemyPrefab == null)
            InstantiateKey(width, height, chunkSize);
    }

    void InstantiateKey(int width, int height, Vector2Int chunkSize)
    {
        while (true)
        {
            int x = Random.Range(0, width * chunkSize.x);
            int y = Random.Range((height * chunkSize.y) / 2, (height-1) * chunkSize.y);

            if (!blockTilemap.HasTile(new Vector3Int(x, y, 0)) &&
                !obstacleTilemap.HasTile(new Vector3Int(x, y, 0)))
            {
                Instantiate(CurrentConfig.keyPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                break;
            }
        }
    }

    void SetChunkToTilemap(Vector2Int pos, TileBase[,] tiles)
    {
        int xLen = tiles.GetLength(0);
        int yLen = tiles.GetLength(1);
        
        Vector2Int chunkSize = CurrentConfig.chunkSize;
        
        for (int x = 0; x < xLen; x++)
        {
            for (int y = 0; y < yLen; y++)
            {
                TileBase tile = tiles[x,y];
                    
                Vector2Int calculatedPos = new Vector2Int(
                    pos.x * chunkSize.x + x, pos.y * chunkSize.y + y);
                    
                if(tile == CurrentConfig.blockTile) 
                    blockTilemap.SetTile(
                        new Vector3Int(calculatedPos.x, calculatedPos.y, 0), CurrentConfig.targetBlockTile);
                
                else if(tile ==  CurrentConfig.obstacleTile)
                    obstacleTilemap.SetTile(
                        new Vector3Int(calculatedPos.x, calculatedPos.y, 0), CurrentConfig.targetObstacleTile);

                else
                    propTilemap.SetTile(new Vector3Int(calculatedPos.x, calculatedPos.y, 0), tile);
            }
        }
    }
    
    void DFS(Vector2Int current)
    {
        visited[current.x, current.y] = true;
        List<Vector2Int> dirs = shuffledDirList();

        foreach (var dir in dirs)
        {
            Vector2Int next = current + dir;
            if (IsInBounds(next) && !visited[next.x, next.y])
            {
                if (!pathConnections.ContainsKey(current))
                    pathConnections[current] = new HashSet<Vector2Int>();
                if (!pathConnections.ContainsKey(next))
                    pathConnections[next] = new HashSet<Vector2Int>();

                pathConnections[current].Add(dir);
                pathConnections[next].Add(-dir);

                DFS(next);
            }
        }
    }
    
    GameObject SelectChunkMatchingDirections(HashSet<Vector2Int> dirs)
    {
        List<GameObject> candidates = new();

        foreach (var prefab in CurrentConfig.chunkPrefabs)
        {
            var meta = prefab.GetComponent<ChunkMetadata>();
            if (MatchesDirections(meta, dirs))
            {
                candidates.Add(prefab);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"No chunk matches directions {string.Join(",", dirs)}");
            return CurrentConfig.fallbackChunkPrefab; // optional
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    bool MatchesDirections(ChunkMetadata meta, HashSet<Vector2Int> dirs)
    {
        foreach (var dir in dirs)
        {
            if (dir == Vector2Int.up    && !meta.u) return false;
            if (dir == Vector2Int.down  && !meta.d) return false;
            if (dir == Vector2Int.left  && !meta.l) return false;
            if (dir == Vector2Int.right && !meta.r) return false;
        }
        return true;
    }


    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < CurrentConfig.width && pos.y >= 0 && pos.y < CurrentConfig.height;
    }
    
    List<Vector2Int> shuffledDirList()
    {
        List<Vector2Int> dirs = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        for (int i = 0; i < dirs.Count; i++)
        {
            int j = Random.Range(i, dirs.Count);
            (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
        }

        return dirs;
    }
}
