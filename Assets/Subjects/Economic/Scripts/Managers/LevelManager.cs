using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public List<RoomController> allRooms = new List<RoomController>();
    public List<GameObject> allEnemies = new List<GameObject>();

    public int difficulty = 3;

    void Start()
    {
        if (LevelBridgeManager.instance != null)
        {
            difficulty = LevelBridgeManager.instance.economyGrade + 1;
        }

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int openRooms = 0;
        float itemsPercentage = 0f;
        int activeEnemies = 0;

        if (difficulty == 3)
        {
            openRooms = 18;
            itemsPercentage = 0.7f;
            activeEnemies = 3;
        }
        else if (difficulty == 4)
        {
            openRooms = 12;
            itemsPercentage = 0.5f;
            activeEnemies = 4;
        }
        else if (difficulty == 5)
        {
            openRooms = 6;
            itemsPercentage = 0.3f;
            activeEnemies = 5;
        }

        foreach (var room in allRooms)
        {
            room.SetupRoom(false);
        }

        List<RoomController> shuffledRooms = new List<RoomController>(allRooms);

        for (int i = 0; i < shuffledRooms.Count; i++)
        {
            RoomController temp = shuffledRooms[i];
            int randomIndex = Random.Range(i, shuffledRooms.Count);
            shuffledRooms[i] = shuffledRooms[randomIndex];
            shuffledRooms[randomIndex] = temp;
        }

        List<GameObject> accessibleFurniture = new List<GameObject>();

        for (int i = 0; i < openRooms; i++)
        {
            shuffledRooms[i].SetupRoom(true);

            Transform[] allChildren = shuffledRooms[i].GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.CompareTag("Destructible"))
                {
                    accessibleFurniture.Add(child.gameObject);
                }
            }
        }

        int finalCount = Mathf.RoundToInt(accessibleFurniture.Count * itemsPercentage);


        for (int i = 0; i < accessibleFurniture.Count; i++)
        {
            GameObject temp = accessibleFurniture[i];
            int randomIndex = Random.Range(i, accessibleFurniture.Count);
            accessibleFurniture[i] = accessibleFurniture[randomIndex];
            accessibleFurniture[randomIndex] = temp;
        }

        for (int i = 0; i < accessibleFurniture.Count; i++)
        {
            var itemScript = accessibleFurniture[i].GetComponent<FurnitureItem>();
            if (itemScript == null) continue;


            if (i < finalCount)
            {
                itemScript.SetAsTarget(true);
            }
            else
            {
                itemScript.SetAsTarget(false);
            }
        }

        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i] != null)
            {
                allEnemies[i].SetActive(i < activeEnemies);
            }
        }

        NavMeshSurfaceManagement.Instance.RebakeNavMeshSurface();
    }
}
