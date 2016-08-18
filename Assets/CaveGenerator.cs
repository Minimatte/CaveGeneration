using UnityEngine;
using System.Collections;

public class CaveGenerator : MonoBehaviour {

    public GameObject wall, floor, player;
    [Range(4, 6)]
    public int wallMax = 4;

    int[,] map;

    public float iterations = 1;

    public string seed = "";
    public bool useRandomSeed = false;

    public Vector2 mapSize;
    [Range(0f, 100f)]
    public float threashold = 50;

    bool spawnedMap = false;


    void Start() {
        map = new int[(int)mapSize.x, (int)mapSize.y];
        NewMap();
    }


    void SpawnPlayer() {

        int spawnX;
        int spawnY;

        do {
            spawnX = Random.Range(0, (int)mapSize.x);
            spawnY = Random.Range(0, (int)mapSize.y);
        } while (map[spawnX, spawnY] != 0);

        Instantiate(player, new Vector3(-(int)mapSize.x / 2 + spawnX + .5f, 2, -(int)mapSize.y / 2 + spawnY + .5f), Quaternion.identity);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F1))
            NewMap();
        if (Input.GetKeyDown(KeyCode.F2))
            SpawnMap();
    }


    void NewMap() {
        if (useRandomSeed)
            seed = Random.Range(int.MinValue, int.MaxValue).ToString();
        PopulateMap();
    }

    void SpawnMap() {
        spawnedMap = true;
        for (int x = 0; x < (int)mapSize.x; x++)
            for (int y = 0; y < (int)mapSize.y; y++) {
                if (map[x, y] == 1)
                    Instantiate(wall, new Vector3(-(int)mapSize.x / 2 + x + .5f, 0, -(int)mapSize.y / 2 + y + .5f), Quaternion.identity);
                else {
                    Instantiate(floor, new Vector3(-(int)mapSize.x / 2 + x + .5f, 0, -(int)mapSize.y / 2 + y + .5f), Quaternion.identity);
                    Instantiate(floor, new Vector3(-(int)mapSize.x / 2 + x + .5f, 8, -(int)mapSize.y / 2 + y + .5f), Quaternion.identity);
                }
            }

        SpawnPlayer();
    }


    void PopulateMap() {
        System.Random random = new System.Random(seed.GetHashCode());


        for (int x = 0; x < (int)mapSize.x; x++)
            for (int y = 0; y < (int)mapSize.y; y++) {
                if (x == 0 || y == 0 || x == (int)mapSize.x - 1 || y == (int)mapSize.y - 1)
                    map[x, y] = 1;
                else
                    map[x, y] = (random.Next(0, 100) < threashold) ? 1 : 0;
            }

        for (int i = 0; i < iterations; i++) {
            SmoothMap();
        }
    }

    void SmoothMap() {
        for (int x = 0; x < (int)mapSize.x; x++) {
            for (int y = 0; y < (int)mapSize.y; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > wallMax)
                    map[x, y] = 1;
                else if (neighbourWallTiles < wallMax)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
                if (neighbourX >= 0 && neighbourX < (int)mapSize.x && neighbourY >= 0 && neighbourY < (int)mapSize.y) {
                    if (neighbourX != gridX || neighbourY != gridY) {
                        wallCount += map[neighbourX, neighbourY];
                    }
                } else {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }


    void OnDrawGizmos() {
        if (!spawnedMap) {
            Gizmos.DrawWireCube(transform.position, new Vector3((int)mapSize.x, 1, (int)mapSize.y));
            if (map != null) {
                for (int x = 0; x < (int)mapSize.x; x++) {
                    for (int y = 0; y < (int)mapSize.y; y++) {
                        Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                        Vector3 pos = new Vector3(-(int)mapSize.x / 2 + x + .5f, 0, -(int)mapSize.y / 2 + y + .5f);
                        Gizmos.DrawCube(pos, Vector3.one);
                    }
                }
            }
        }
    }
}
