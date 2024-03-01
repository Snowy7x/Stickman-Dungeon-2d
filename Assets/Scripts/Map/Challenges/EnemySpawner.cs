using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Enemy;

namespace Map.Challenges
{
    [Serializable]
    // add to menu
    [CreateAssetMenu(fileName = "New Enemy Spawner", menuName = "Map/Challenges/Enemy Spawner")]
    public class EnemySpawner : RoomChallenge
    {
        
        [SerializeField] private GameObject[] flyingEnemies;
        [SerializeField] private GameObject[] groundEnemies;
        private int waveCount = 0;
        [SerializeField] int waveCountMax = 3;
        private float waveTime = 0f;
        [SerializeField] float waveTimeMax = 5f;
        
        private List<Actor> aliveEnemies = new List<Actor>();


        public override void OnPlayerEnter()
        {
            base.OnPlayerEnter();
            SpawnWave();
            // TODO: Spawn enemies.
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (isComplete) return;
            // update wave timer
            waveTime += Time.deltaTime;
            if (waveTime >= waveTimeMax && aliveEnemies.Count == 0)
            {
                waveTime = 0f;
                waveCount++;
                if (waveCount >= waveCountMax)
                {
                    OnEnd();
                }
                else
                {
                    SpawnWave();
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            // TODO: Destroy enemies.
        }

        public override void OnPlayerExit()
        {
            // TODO: Destroy enemies.
        }

        private void SpawnWave()
        {
            switch (challengeDifficulty)
            {
                // Easy
                case 0:
                default:
                    SpawnEnemies(5, 0);
                    break;
                
                // Medium
                case 1:
                    SpawnEnemies(10, 0);
                    break;
                // Hard
                case 2:
                    SpawnEnemies(15, 0);
                    break;
                // Extreme
                case 3:
                    SpawnEnemies(20, 0);
                    break;
            }
        }

        private void SpawnEnemies(int flyCount, int groundCount)
        {
            for (int i = 0; i < flyCount; i++)
            {
                var enemy = GameObject.Instantiate(flyingEnemies[0], room.GetRandomPosition(), Quaternion.identity);
                enemy.transform.SetParent(room.transform);
            }
            
            for (int i = 0; i < groundCount; i++)
            {
                var enemy = GameObject.Instantiate(groundEnemies[0], room.GetRandomPosition(), Quaternion.identity);
                enemy.transform.SetParent(room.transform);
                Actor enemyScript =  enemy.GetComponent<Actor>();
                if (enemyScript != null)
                {
                    enemyScript.OnDie += OnEnemyDie;
                    aliveEnemies.Add(enemyScript);
                }
            }
        }

        // On Asset first created assign default values
        private void OnEnable()
        {
            if (challengeName == null)
            {
                challengeName = "EnemySpawner";
                challengeDescription = "Enemy waves will spawn every amount of time, so be ready!";
                challengeDifficulty = 1;
                challengeReward = 100;
                challengeTime = 60;
                maxScore = 100;
                
                // Reset wave count
                waveCount = 0;
                waveCountMax = 3;
                waveTime = 0f;
                waveTimeMax = 5f;
            }
        }

        public override void Initialize(Room nRoom, int nDifficulty)
        {
            base.Initialize(nRoom, nDifficulty);
            // Reset wave count
            waveCount = 0;
            waveCountMax = 3;
            waveTime = 0f;
            waveTimeMax = 5f;
        }

        public void OnEnemyDie(Actor enemy)
        {
            if (aliveEnemies.Contains(enemy))
            {
                aliveEnemies.Remove(enemy);
                AddScore(10);
            }
        }
    }
}