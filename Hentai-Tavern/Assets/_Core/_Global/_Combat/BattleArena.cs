using UnityEngine;

namespace _Core.GameEvents.Battle
{
    public class BattleArena : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawn;
        [SerializeField] private Transform[] enemySpawns;

        public Transform PlayerSpawn => playerSpawn;
        public Transform[] EnemySpawns => enemySpawns;

        private void Awake()
        {
            if (!playerSpawn) playerSpawn = transform.Find("PlayerPoint");
            if (enemySpawns == null || enemySpawns.Length == 0)
            {
                enemySpawns = new Transform[3];
                enemySpawns[0] = transform.Find("Enemy 1 Point");
                enemySpawns[1] = transform.Find("Enemy 2 Point");
                enemySpawns[2] = transform.Find("Enemy 3 Point");
            }
        }
    }
}