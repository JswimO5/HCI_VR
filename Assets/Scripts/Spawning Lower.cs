using UnityEngine;

public class ModelSpawner : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    public GameObject[] modelPrefabs; // Assign these in Inspector

    [Header("Spawn Settings")]
    public Vector3 spawnArea = new Vector3(10f, 0f, 10f); // area range
    public int numberToSpawn = 5; // how many to spawn total

    void Start()
    {
        SpawnModels();
    }

    void SpawnModels()
    {
        if (modelPrefabs.Length == 0)
        {
            Debug.LogWarning("No model prefabs assigned to ModelSpawner.");
            return;
        }

        for (int i = 0; i < numberToSpawn; i++)
        {
            // pick a random model
            GameObject prefab = modelPrefabs[Random.Range(0, modelPrefabs.Length)];

            // pick a random position within spawn area
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                spawnArea.y,
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
            );

            // spawn it
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            Instantiate(prefab, randomPos, randomRot);
        }
    }
}