using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public int npcCount;
    public float spawnRange;

    void Start()
    {
        SpawnNPCs();
    }

    //spawns the NPCs at random positions within the defined range
    void SpawnNPCs()
    {
        for(int i=0; i < npcCount; i++)
        {
            Vector3 randomSpawnPos = new Vector3(Random.Range(-spawnRange, spawnRange), 0.5f, Random.Range(-spawnRange, spawnRange));
            Instantiate(npcPrefab, randomSpawnPos, Quaternion.identity);  //creates the new NPC at the random position
        }

    }
}
