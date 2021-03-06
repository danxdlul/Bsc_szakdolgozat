﻿using System;
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
        public int carPathMaxLength = 5;
        public int spawndelay = 3;
        public int maxCars = 20;
        public int maxBuses = 2;
        public List<GameObject> Cars = new List<GameObject>();
        public List<GameObject> Buses = new List<GameObject>();
        public GameObject CarPrefab;
        public GameObject BusPrefab;
        private List<Edge> test = new List<Edge>();
        public Path buspath;
        public Path reversepath;

        void Start()
        {
            startCoroutines();
        }
        public void startCoroutines()
        {
            test.Clear();
            buspath = gameObject.GetComponent<RoadGenerator>().graph.BusPath;
            
            reversepath = gameObject.GetComponent<RoadGenerator>().graph.ReverseBusPath;
            for (int i = reversepath.Edges.Count - 1; i >= 0; i--)
            {
                test.Add(reversepath.Edges[i]);
            }
            this.reversepath.Edges = test;
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
                    Cars.Last().GetComponent<CarEngine>().path = gameObject.GetComponent<RoadGenerator>().graph.GenerateRandomPath(carPathMaxLength);
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
                    Buses.Last().GetComponent<BusEngine>().setPaths(buspath,reversepath);
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
        public void setMaxCars(string n)
        {
            if (int.Parse(n) > 0)
            {
                maxCars = int.Parse(n);
            }            
        }
        public void setMaxBuses(string n)
        {
            if (int.Parse(n) > 0)
            {
                maxBuses = int.Parse(n);
            }
        }
        public void setSpawnDelay(string t)
        {
            if (int.Parse(t) > 0)
            {
                spawndelay = int.Parse(t);
            }
        }
        public void setPathLength(string n)
        {
            if(int.Parse(n) > 1)
            {
                carPathMaxLength = int.Parse(n);
            }
        }
    }
}
