﻿using UnityEngine;

namespace C__scripts.Enemies
{
    public class EnemiesManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject[] enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Canvas canvasFight;
        [SerializeField] private Fight fight;
        
        [SerializeField] private float radius;
        [SerializeField] private float speed;
        
        
        private GameObject[] enemies;

        private int currentSpawnPoint;
        private int size;

        public void Start()
        {
            size = spawnPoints.Length;
            enemies = new GameObject[size];
        }

        public void Update()
        {
            if (Vector3.Distance(spawnPoints[currentSpawnPoint].position, player.transform.position) < 20f)
            {
                var e = Instantiate(enemyPrefab[currentSpawnPoint++]);
                e.GetComponent<Enemy>()
                    .Init(e, radius, speed, spawnPoints[currentSpawnPoint-1], player, canvasFight, fight);
                enemies[currentSpawnPoint-1] = e;
                
                if (currentSpawnPoint >= size) 
                    Destroy(gameObject);
            }
        }
    }
}