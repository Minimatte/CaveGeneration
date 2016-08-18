using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkMap : MonoBehaviour {

    public GameObject cube;
    [Range(0, 500)]
    public int size;
    [Range(0f, 30)]
    public float scale = 7f, scaleMod = 5f, offSetHeight = 1.5f;
    public bool enableHeight = true, move = false;

    public GameObject player;

    bool[,,] visited;

    public int chunksize = 1024;
    void Start() {
        // player.transform.position = new Vector3(chunksize / 2, 10, chunksize / 2);
        visited = new bool[chunksize, chunksize, chunksize];
        player.transform.position = new Vector3(0, 10, 0);

        foreach (Vector3 n in GetNeighbours(new Vector3(player.transform.position.x, 0, player.transform.position.z))) {
            SpawnWorld(n, size);

        }

    }

    void SpawnWorld(Vector3 location, int size) {

        visited[(int)location.x, 0, (int)location.z] = true;

        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {

                var go = Instantiate(cube, new Vector3(x + location.x * size, 0, z + location.z * size), Quaternion.identity) as GameObject;

                go.transform.parent = transform;
                var height = Mathf.PerlinNoise(go.transform.position.x / scale, go.transform.position.z / scale);
                setHeight(go.transform, height);
                setColor(go.transform, height);
            }
        }
    }

    void setColor(Transform child, float height) {
        child.GetComponent<Renderer>().material.color = new Color(height, height, height, height);
    }

    void setHeight(Transform child, float height) {
        var yValue = Mathf.RoundToInt(height * scaleMod);
        var v = new Vector3(child.transform.position.x, yValue, child.transform.position.z);
        child.transform.position = v;
    }

    void Update() {

       // print((int)player.transform.position.x / size);
        if (!visited[(int)player.transform.position.x / size, 0, (int)player.transform.position.z / size]) {
            foreach (Vector3 n in GetNeighbours(new Vector3(player.transform.position.x, 0, player.transform.position.z))) {
                SpawnWorld(n, size);
            }
        }

        if (move) {
            foreach (Transform child in transform) {
                var height = Mathf.PerlinNoise(child.transform.position.x / scale, child.transform.position.z / scale);
                setHeight(child.transform, height);
            }
        }
    }

    public List<Vector3> GetNeighbours(Vector3 location) {
        List<Vector3> neighbours = new List<Vector3>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {

                int checkX = (int)location.x + x;
                int checkY = (int)location.y + y;

                if (checkX >= 0 && checkX < size && checkY >= 0 && checkY < size) {
                    neighbours.Add(new Vector3(checkX, 0, checkY));
                }
            }
        }
        return neighbours;
    }
}

