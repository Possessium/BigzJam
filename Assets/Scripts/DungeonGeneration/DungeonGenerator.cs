using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonGenerationData;

    private List<Vector2Int> dungeonRooms;

    private void Start()
    {
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData);
        SpawnRooms(dungeonRooms);
    }

    private int[] Random(int length)
    {
        int[] randomList = new int[length];

        for (int i = 0; i < length; i++)
        {
            randomList[i] = i + 1;
           
        }

        randomList = randomList.OrderBy(i => Guid.NewGuid()).ToArray();

        return randomList;   
    }


    private void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {

        RoomController.instance.LoadRoom("Start", 0, 0);
        
        foreach(Vector2Int roomLocation in rooms)
        {
            int[] randomList = Random(5);
            int number = randomList[0];
            RoomController.instance.LoadRoom("Room" + number, roomLocation.x, roomLocation.y);


        }
    }
}
