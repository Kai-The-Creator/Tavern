using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public List<GameObject> chunkPrefab;
    public int maxChunks = 10;
    public float chunkSize = 3f;

    private Dictionary<Vector3, GameObject> chunks = new Dictionary<Vector3, GameObject>();

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        ClearDungeon();

        Vector3 startPosition = Vector3.zero;
        CreateChunk(startPosition);

        for (int i = 1; i < maxChunks; i++)
        {
            GameObject randomChunk = GetRandomChunk();
            Vector3 direction = GetRandomDirection();
            Vector3 newPosition = randomChunk.transform.position + direction * chunkSize;

            if (!chunks.ContainsKey(newPosition))
            {
                GameObject newChunk = CreateChunk(newPosition);
                ConnectChunks(randomChunk, newChunk);
            }
        }
    }

    GameObject CreateChunk(Vector3 position)
    {
        GameObject chunk = Instantiate(chunkPrefab[Random.Range(0,chunkPrefab.Count)], position, Quaternion.identity);
        chunk.transform.SetParent(transform);
        chunks.Add(position, chunk);
        return chunk;
    }

    void ConnectChunks(GameObject chunkA, GameObject chunkB)
    {
        Vector3 offset = chunkB.transform.position - chunkA.transform.position;
        Vector3 direction = Vector3.zero;

        if (offset.x > 0) direction = Vector3.right;
        else if (offset.x < 0) direction = Vector3.left;
        else if (offset.z > 0) direction = Vector3.forward;
        else if (offset.z < 0) direction = Vector3.back;

        switch (direction)
        {
            case Vector3 right when direction == Vector3.right:
                chunkA.transform.Find("RightWall").gameObject.SetActive(false);
                chunkB.transform.Find("LeftWall").gameObject.SetActive(false);
                break;
            case Vector3 left when direction == Vector3.left:
                chunkA.transform.Find("LeftWall").gameObject.SetActive(false);
                chunkB.transform.Find("RightWall").gameObject.SetActive(false);
                break;
            case Vector3 forward when direction == Vector3.forward:
                chunkA.transform.Find("FrontWall").gameObject.SetActive(false);
                chunkB.transform.Find("BackWall").gameObject.SetActive(false);
                break;
            case Vector3 back when direction == Vector3.back:
                chunkA.transform.Find("BackWall").gameObject.SetActive(false);
                chunkB.transform.Find("FrontWall").gameObject.SetActive(false);
                break;
        }
    }

    GameObject GetRandomChunk()
    {
        List<GameObject> chunkList = new List<GameObject>(chunks.Values);
        return chunkList[Random.Range(0, chunkList.Count)];
    }

    Vector3 GetRandomDirection()
    {
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        return directions[Random.Range(0, directions.Length)];
    }

    public void ClearDungeon()
    {
        foreach (var chunk in chunks.Values)
        {
            if (Application.isPlaying)
                Destroy(chunk);
            else
                DestroyImmediate(chunk);
        }
        chunks.Clear();
    }
}