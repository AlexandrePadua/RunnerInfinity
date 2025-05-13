using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    [Header("Ground Settings")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private int initialGroundCount = 5;
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private float recycleDistance = 20f;

    [SerializeField] private float groundBottomOffset = 0.5f;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform obstacleSpawner;
    [SerializeField] private GameObject collectablePrefab;
    [SerializeField] private GameObject obstaclePrefab;

    private Vector2 yCameraRange;

    private List<GameObject> activeGroundSegments = new List<GameObject>();
    private float lastGroundX = 0f;
    private Queue<GameObject> groundPool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialGroundCount; i++)
        {
            SpawnGroundSegment();
        }

        yCameraRange = new(Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y + 0.4f, Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y - 0.4f);

        StartCoroutine(SpawnCollectables(2f));
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (playerTransform.position.x + spawnDistance > lastGroundX)
        {
            SpawnGroundSegment();
        }

        CheckRecycleGroundSegments();
    }
    void SpawnGroundSegment()
    {
        float groundLength = GetGroundLength();

        Camera mainCamera = Camera.main;
        float yPos = mainCamera.ViewportToWorldPoint(new(0, 0, 0)).y + groundBottomOffset;

        Vector3 spawnPosition = new Vector3(lastGroundX, yPos, 0);

        GameObject groundSegment;

        if (groundPool.Count > 0)
        {
            groundSegment = groundPool.Dequeue();
            groundSegment.transform.position = spawnPosition;
            groundSegment.SetActive(true);
        }
        else
        {
            groundSegment = Instantiate(groundPrefab, spawnPosition, Quaternion.identity);
            groundSegment.name = "GroundSegment_" + activeGroundSegments.Count;
        }

        activeGroundSegments.Add(groundSegment);

        lastGroundX += groundLength;
    }
    void CheckRecycleGroundSegments()
    {
        if (activeGroundSegments.Count == 0 || playerTransform == null) return;

        GameObject oldestSegment = activeGroundSegments[0];

        if (oldestSegment.transform.position.x < playerTransform.position.x - recycleDistance)
        {
            activeGroundSegments.RemoveAt(0);

            oldestSegment.SetActive(false);
            groundPool.Enqueue(oldestSegment);
        }
    }
    float GetGroundLength()
    {
        Renderer renderer = groundPrefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.x;
        }

        Collider2D collider = groundPrefab.GetComponent<Collider2D>();
        if (collider != null)
        {
            return collider.bounds.size.x;
        }

        // Valor padrão se não conseguir determinar
        Debug.LogWarning("Não foi possível determinar o comprimento do chão. Usando valor padrão de 10.");
        return 10f;
    }

    private IEnumerator SpawnCollectables(float timer)
    {
        yield return new WaitForSeconds(timer);

        Instantiate(collectablePrefab, new Vector3(Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).x + 0.2f, Random.Range(yCameraRange.x, yCameraRange.y), 0), Quaternion.identity);

        SpawnObstacles();

        StartCoroutine(SpawnCollectables(2f / GameController.Instance.difficultyMultiplier));
    }

    private void SpawnObstacles()
    {
        float chance = Random.Range(0f, 1f);

        if(chance < 0.5f)
        {
            Instantiate(obstaclePrefab, obstacleSpawner.position, Quaternion.identity);
        }
    }
}