using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] FishSpecies fishSpecies;
    [SerializeField] GameObject fishPrefab;
    [SerializeField] int numberOfFish = 10;
    [SerializeField] Vector2 spawnAreaSize = new Vector2(10f, 10f);
    [SerializeField] float spawnCooldown;
    int currentFishCount = 0;
    float timer;
    void SpawnFish()
    {
        if (fishSpecies == null)
        {
            Debug.LogError("Fish species is not assigned.");
            return;
        }
        else
        {
            
        }
    }
    private void Update()
    {
        Debug.Log(currentFishCount);
        if (currentFishCount < numberOfFish)
        {
            timer += Time.deltaTime;
            if(timer >= spawnCooldown)
            {
                timer = 0f;
                Vector3 spawnPosition = new Vector3(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                    0) + transform.position;
                GameObject fishObject = Instantiate(fishPrefab, spawnPosition, Quaternion.LookRotation(Vector3.right), transform);
                Fish fish = fishObject.GetComponent<Fish>();
                fish.species = fishSpecies;
                fish.Spawn();
                currentFishCount++;
                fishObject.GetComponent<Fish>().onDeath.AddListener(OnFishDeath);
            }
        }
    }
    private void OnFishDeath()
    {
        currentFishCount--;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
