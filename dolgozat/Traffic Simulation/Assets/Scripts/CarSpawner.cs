using System;
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
        public int maxCars = 20;
        public List<GameObject> Cars = new List<GameObject>();
        public GameObject CarPrefab;

        void Update()
        {
            if (currentCars < maxCars)
            {
                Invoke("SpawnCar", 2);
                currentCars++;
            }
            for(int i = 0;i<Cars.Count;i++)
            {
                if (Cars[i] == null)
                {
                    Cars.Remove(Cars[i]);
                    currentCars--;
                }
            }
        }
        void SpawnCar()
        {
            Cars.Add(Instantiate(CarPrefab));
            Debug.Log("car created");
            Cars.Last().GetComponent<CarEngine>().path = GameObject.FindGameObjectWithTag("GameController").GetComponent<RoadGenerator>().graph.GenerateRandomPath(5);
        }
    }
}
