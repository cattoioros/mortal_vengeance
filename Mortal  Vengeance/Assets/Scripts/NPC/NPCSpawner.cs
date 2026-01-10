using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public int npcCount = 5;
    public float spawnRange = 15f;
    public float minSpawnDistance = 5f;

    void Start()
    {
        SpawnNPCs();
    }

    //spawns NPCs in a circular area around the spawner, ensuring they are not too close
    void SpawnNPCs()
    {
        for (int i = 0; i < npcCount; i++)
        {
            Vector2 circle;
            do
            {
                circle = Random.insideUnitCircle * spawnRange;
            }
            while (circle.magnitude < minSpawnDistance);

            Vector3 spawnPos = transform.position + new Vector3(circle.x, 0f, circle.y);
            Instantiate(npcPrefab, spawnPos, Quaternion.identity); // Spawn the NPC at the calculated position
        }
    }
}
