using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_spawn_fish : MonoBehaviour
{
    [SerializeField] List<GameObject> prefab_fish;
    [SerializeField] List<GameObject> obj_spawned_fish;
    [SerializeField] List<Transform> obj_spawn_area;
    
    [SerializeField] GameObject prefab_waypoint;
    [SerializeField] List<GameObject> obj_waypoints;
    [SerializeField] float f_repeat_rate = 0.5f;
    [SerializeField] int i_fish_limit = 25;
    [SerializeField] int i_fish_min_scale = 50;
    [SerializeField] int i_fish_max_scale = 75;
    void Start()
    {
        DOTween.Init();
        InvokeRepeating("spawnNPC", 0.5f, f_repeat_rate);
    }

    void spawnNPC()
    {
        if(obj_spawned_fish.Count < i_fish_limit)
        {
            bool isJellyFish;
            int spawn_number = Random.Range(0, prefab_fish.Count);
            int direction = Random.Range(0, 2);
            Transform tempSpawnArea = obj_spawn_area[direction];
            Transform targetSpawnArea;//set move-to target
            if (direction == 0)
            {
                //if spawn on left Side
                targetSpawnArea = obj_spawn_area[1];
            } else
            {
                //if spawn on right Side
                targetSpawnArea = obj_spawn_area[0];
            }

            if (prefab_fish[spawn_number].name == "prefab_fish_g")
            {
                isJellyFish = true;
            }
            else
            {
                isJellyFish = false;
            }
            int i_fish_scale = Random.Range(i_fish_min_scale, i_fish_max_scale);
            float f_fish_scale = ((float)i_fish_scale) / 100;
            Quaternion defaultRotation = Quaternion.Euler(0,0,0);
            GameObject tempSpawn = Instantiate(prefab_fish[spawn_number], RandomPosition(tempSpawnArea), defaultRotation);
            GameObject tempWaypoint = Instantiate(prefab_waypoint, SetRandomTargetPoint(targetSpawnArea), defaultRotation);
            tempSpawn.transform.localScale = new Vector3(f_fish_scale, f_fish_scale, f_fish_scale);
            tempSpawn.transform.SetParent(tempSpawnArea);
            tempWaypoint.transform.SetParent(targetSpawnArea);
            tempSpawn.GetComponent<script_fish_ai>().setTargetArea(tempWaypoint.transform, isJellyFish);
            obj_spawned_fish.Add(tempSpawn);
            obj_waypoints.Add(tempWaypoint);
        }
    }

    public Vector3 RandomPosition(Transform tempSpawnArea)
    {
        Vector3 randomPosition = new Vector3(
                tempSpawnArea.localPosition.x,
                Random.Range(-tempSpawnArea.position.y, tempSpawnArea.position.y),
                0
            );
        randomPosition = transform.TransformPoint(randomPosition.x, randomPosition.y, randomPosition.z);
        return randomPosition;
    }

    public Vector3 SetRandomTargetPoint(Transform targetArea)
    {
        Vector3 randomPosition = new Vector3(
                targetArea.localPosition.x,
                Random.Range(-targetArea.position.y, targetArea.position.y),
                0
            );
        randomPosition = transform.TransformPoint(randomPosition.x, randomPosition.y, randomPosition.z);

        return randomPosition;
    }

    public void destroyFirstItemInList()
    {
        obj_spawned_fish.Remove(obj_spawned_fish[0]);
        obj_waypoints.Remove(obj_waypoints[0]);
    }
}
