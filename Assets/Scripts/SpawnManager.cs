using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    Transform[] spawnPoints;
    private void Awake()
    {
        Instance = this;
        spawnPoints = transform.Find("PlayerSpawns").GetComponentsInChildren<Transform>();
        foreach (Transform sp in spawnPoints)
        {
            sp.gameObject.SetActive(false);
        }
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}