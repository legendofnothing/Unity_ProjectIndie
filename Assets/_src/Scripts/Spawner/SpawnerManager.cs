using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace _src.Scripts.Spawner {
    public class SpawnerManager : MonoBehaviour {
        private List<GameObject> _spawnerList;

        private void Awake(){
            _spawnerList = new List<GameObject>();
        }
    
        private void Start(){
            StartCoroutine(SpawnEnemy(3));
        }

        public void AddSpawner(GameObject spawner){
            _spawnerList.Add(spawner);
        }

        IEnumerator SpawnEnemy(int amount){
            yield return new WaitForSeconds(1.2f);

            var rnd = new Random();
            var randomSpawners = _spawnerList.OrderBy(x => rnd.Next()).Take(amount).ToList();

            foreach (var spawner in randomSpawners)
            {
                spawner.GetComponent<SpawnerBase>().Spawn();
            }
        }
    }
}
