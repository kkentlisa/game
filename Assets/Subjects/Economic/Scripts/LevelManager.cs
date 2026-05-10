using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;

public class LevelManager : MonoBehaviour
{
    public List<RoomController> allRooms = new List<RoomController>();

    public int difficulty = 3;

    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int openRooms = 0;

        if (difficulty == 3) openRooms = 18;
        else if (difficulty == 4) openRooms = 12;
        else if (difficulty == 5) openRooms = 6;

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

        for (int i = 0; i < openRooms; i++)
        {
            shuffledRooms[i].SetupRoom(true);
        }
    }
}
