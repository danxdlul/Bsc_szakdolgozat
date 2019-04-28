using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class CarSpawner : MonoBehaviour
    {
        private int currentCars = 0;
        private int currentBuses = 0;
        public int spawndelay = 3;
        public int maxCars = 20;
        public int maxBuses = 2;
        public List<GameObject> Cars = new List<GameObject>();
        public List<GameObject> Buses = new List<GameObject>();
        public GameObject CarPrefab;
        public GameObject BusPrefab;

        void Start()
        {
            StartCoroutine(CarSpawn());
            StartCoroutine(BusSpawn());
        }
        IEnumerator CarSpawn()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawndelay);
                if(currentCars < maxCars)
                {
                    Cars.Add(Instantiate(CarPrefab));
                    Debug.Log("car created");
                    Cars.Last().GetComponent<CarEngine>().path = gameObject.GetComponent<RoadGenerator>().graph.GenerateRandomPath(5);
                    currentCars++;
                }
                for (int i = 0; i < Cars.Count; i++)
                {
                    if (Cars[i] == null)
                    {
                        Cars.Remove(Cars[i]);
                        currentCars--;
                    }
                }
            }
            
        }
        IEnumerator BusSpawn()
        {
            while (true)
            {
                yield return new WaitForSeconds(10);
                if (currentBuses < maxBuses)
                {
                    Buses.Add(Instantiate(BusPrefab));
                    Buses.Last().GetComponent<BusEngine>().path = gameObject.GetComponent<RoadGenerator>().graph.BusPath;
                    Buses.Last().GetComponent<BusEngine>().reversePath = gameObject.GetComponent<RoadGenerator>().graph.ReverseBusPath;
                    currentBuses++;
                }
                for (int i = 0; i < Buses.Count; i++)
                {
                    if (Buses[i] == null)
                    {
                        Buses.Remove(Buses[i]);
                        currentBuses--;
                    }
                }
            }

        }
    }
}
