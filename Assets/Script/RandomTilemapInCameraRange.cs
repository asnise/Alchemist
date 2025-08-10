using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTilemapInCameraRange : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] randomTiles;
    public float[] tileProbabilities; // Probability weights for each tile
    public Camera mainCamera;
    public int tileBuffer = 2;
    public int seed = 42; // Seed for random tile generation

    private void Start()
    {
        // Set the random seed for consistent tile generation
        Random.InitState(seed);

        // Check that the probability array matches the tile array
        if (tileProbabilities.Length != randomTiles.Length)
        {
            Debug.LogError("The length of tileProbabilities must match the length of randomTiles.");
        }
    }

    void Update()
    {
        // Get the camera bounds
        Vector3 minCameraBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 maxCameraBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        // Convert world position to Tilemap grid coordinates
        Vector3Int minCellPos = tilemap.WorldToCell(minCameraBounds) - new Vector3Int(tileBuffer, tileBuffer, 0);
        Vector3Int maxCellPos = tilemap.WorldToCell(maxCameraBounds) + new Vector3Int(tileBuffer, tileBuffer, 0);

        // Clear any tiles outside the camera range
        ClearOutOfBoundsTiles(minCellPos, maxCellPos);

        // Set random tiles within the camera's view
        for (int x = minCellPos.x; x <= maxCellPos.x; x++)
        {
            for (int y = minCellPos.y; y <= maxCellPos.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                // If there is no tile at the current position, place a random one based on probability
                if (!tilemap.HasTile(cellPosition))
                {
                    TileBase randomTile = GetRandomTileByProbability();
                    tilemap.SetTile(cellPosition, randomTile);
                }
            }
        }
    }

    // Method to clear tiles that are outside of the camera bounds
    private void ClearOutOfBoundsTiles(Vector3Int minCellPos, Vector3Int maxCellPos)
    {
        // Iterate through all tile positions in the tilemap
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                // Check if the tile is out of the current camera's bounds
                if (cellPosition.x < minCellPos.x || cellPosition.x > maxCellPos.x ||
                    cellPosition.y < minCellPos.y || cellPosition.y > maxCellPos.y)
                {
                    // Remove the tile if it's out of bounds
                    if (tilemap.HasTile(cellPosition))
                    {
                        tilemap.SetTile(cellPosition, null);
                    }
                }
            }
        }
    }

    // Method to get a random tile based on probability
    private TileBase GetRandomTileByProbability()
    {
        float totalWeight = 0f;
        foreach (float weight in tileProbabilities)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < randomTiles.Length; i++)
        {
            cumulativeWeight += tileProbabilities[i];
            if (randomValue < cumulativeWeight)
            {
                return randomTiles[i];
            }
        }

        return randomTiles[0]; // Fallback in case no tile is selected
    }
}
