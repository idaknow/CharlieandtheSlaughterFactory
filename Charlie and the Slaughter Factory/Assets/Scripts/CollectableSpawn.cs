﻿using UnityEngine;
using System.Collections;

public class CollectableSpawn : MonoBehaviour
{

    public Transform[] collectibleSpawns;
    public GameObject collectible;

    // Call spawn method to spawn a coin
    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        // for every coin we want to instantiate
        for (int i = 0; i < collectibleSpawns.Length; i++)
        {
            //if the random number is greater than zero, instatiante the coin
            int collectibleFlip = Random.Range(0, 2);
            if (collectibleFlip > 0)
                Instantiate(collectible, collectibleSpawns[i].position, Quaternion.identity);
        }
    }
}