using UnityEngine;

public class ModelSpawner : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    public GameObject[] coralPrefabs; // Assign these in Inspector
    public GameObject[] fishPrefabs; // Assign these in Inspector

    [Header("Spawn Settings")]
    public Vector3 spawnArea = new Vector3(10f, 0f, 10f); // area range
    public Vector3 coralProximity = new Vector3(1f, 0.5f, 1f); // area range
    public int numberToSpawn = 15; // how many to spawn total

    void Start()
    {
        SpawnModels();
    }

    void SpawnModels()
    {
        if (coralPrefabs.Length == 0)
        {
            Debug.LogWarning("No model prefabs assigned to ModelSpawner.");
            return;
        }

        for (int i = 0; i < numberToSpawn; i++)
        {
            // pick a random model
            GameObject prefab = coralPrefabs[Random.Range(0, coralPrefabs.Length)];
            GameObject fish = fishPrefabs[Random.Range(0, fishPrefabs.Length)];

            // pick a random position within spawn area
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                spawnArea.y,
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
            );

            Vector3 randomOff = new Vector3(
                Random.Range(-coralProximity.x / 2, coralProximity.x / 2),
                coralProximity.y,
                Random.Range(-coralProximity.z / 2, coralProximity.z / 2)
            );

            Vector3 fishPos = randomPos + randomOff;

            // spawn it
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            Instantiate(prefab, randomPos, randomRot);
            Instantiate(fish, fishPos, randomRot);
        }
    }
}