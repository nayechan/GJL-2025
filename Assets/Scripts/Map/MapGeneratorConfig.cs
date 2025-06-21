using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(fileName = "MapGeneratorConfig", menuName = "Map Generator Config")]
public class MapGeneratorConfig : ScriptableObject
{
    [SerializeField] public int minFloor, maxFloor;
    
    [SerializeField] public GameObject[] chunkPrefabs;
    [SerializeField] public GameObject fallbackChunkPrefab, beginChunkPrefab, exitChunkPrefab, keyPrefab;

    [SerializeField] public TileBase blockTile, obstacleTile;
    [SerializeField] public TileBase targetBlockTile, targetObstacleTile;
    
    [SerializeField] public Vector2Int chunkSize = new Vector2Int(9, 9);
    [SerializeField] public int width = 4;
    [SerializeField] public int height = 12;
    
    [SerializeField] public int beginChunkX = 1;
    [SerializeField] public int endChunkX = 1;
    [SerializeField] public int beginChunkWidth = 2, endChunkWidth = 2;
    
    [SerializeField] public GameObject unlockedDoorPortalPrefab;
    [SerializeField] public Vector2Int endChunkDoorPos = new Vector2Int(8, 5);
}