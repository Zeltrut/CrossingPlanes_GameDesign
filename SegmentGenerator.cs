using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SegmentGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [Tooltip("The different segment prefabs that can be spawned.")]
    public GameObject[] segment;
    [SerializeField] private float zPos = 50;
    [SerializeField] private bool creatingSegment = false;

    [Header("Dynamic Generation Speed")]
    [Tooltip("The time to wait between segments when the player is below the speed threshold.")]
    [SerializeField] private float baseGenerationDelay = 2.3f;
    [Tooltip("The time to wait between segments when the player is above the speed threshold.")]
    [SerializeField] private float fastGenerationDelay = 1.0f;
    [Tooltip("The player speed required to trigger the faster generation delay.")]
    [SerializeField] private float speedThreshold = 20f;
    
    [Header("References")]
    [Tooltip("A reference to the player's FirstPersonController script to read its speed.")]
    [SerializeField] private FirstPersonController firstPersonController;

    [Header("Optimization")]
    [Tooltip("A reference to the player's transform for distance checks.")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("The distance behind the player at which segments will be destroyed.")]
    [SerializeField] private float destroyDistance = 100f;

    private List<GameObject> spawnedSegments = new List<GameObject>();
    public List<SegmentData> generatedSegmentData = new List<SegmentData>();

    void Start()
    {
        if (playerTransform == null || firstPersonController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                firstPersonController = player.GetComponent<FirstPersonController>();
            }
            else
            {
                Debug.LogError("SegmentGenerator Error: Could not find GameObject with tag 'Player'. Dynamic speed and segment destruction will not work.");
            }
        }
    }

    void Update()
    {
        if (creatingSegment == false)
        {
            creatingSegment = true;
            StartCoroutine(GenerateSegmentCoroutine());
        }

        CheckAndDestroySegments();
    }

    private void CheckAndDestroySegments()
    {
        if (playerTransform == null) return;

        // Loop backwards to safely remove items from the list while iterating.
        for (int i = spawnedSegments.Count - 1; i >= 0; i--)
        {
            if (playerTransform.position.z > spawnedSegments[i].transform.position.z + destroyDistance)
            {
                Destroy(spawnedSegments[i]);
                spawnedSegments.RemoveAt(i);
                generatedSegmentData.RemoveAt(i);
            }
        }
    }

    IEnumerator GenerateSegmentCoroutine()
    {
        int segmentNum = Random.Range(0, segment.Length);
        Vector3 spawnPosition = new Vector3(0, 0, zPos);
        
        GameObject newSegment = Instantiate(segment[segmentNum], spawnPosition, Quaternion.identity);
        spawnedSegments.Add(newSegment);
        generatedSegmentData.Add(new SegmentData { prefabIndex = segmentNum, position = spawnPosition });

        zPos += 50;


        float delay = baseGenerationDelay;
        // If the controller is linked and the player's speed is over the threshold, use the faster delay.
        if (firstPersonController != null && firstPersonController.CurrentSpeed > speedThreshold)
        {
            delay = fastGenerationDelay;
        }

        yield return new WaitForSeconds(delay);
        creatingSegment = false;
    }

    public void LoadSegments(List<SegmentData> segmentsToLoad)
    {
        StopAllCoroutines();
        creatingSegment = false;

        foreach (GameObject spawnedSegment in spawnedSegments)
        {
            Destroy(spawnedSegment);
        }
        spawnedSegments.Clear();
        generatedSegmentData.Clear();

        if (segmentsToLoad == null || segmentsToLoad.Count == 0) return;

        foreach (SegmentData data in segmentsToLoad)
        {
            GameObject loadedSegment = Instantiate(segment[data.prefabIndex], data.position, Quaternion.identity);
            spawnedSegments.Add(loadedSegment);
            generatedSegmentData.Add(data);
        }

        if (spawnedSegments.Count > 0)
        {
            zPos = spawnedSegments[spawnedSegments.Count - 1].transform.position.z + 50;
        }
    }
}

