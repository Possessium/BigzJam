using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator I { get; private set; }

    public DungeonGenerationData dungeonGenerationData;

    private List<Vector2Int> dungeonRooms = new List<Vector2Int>();

    [SerializeField] BJ_NavmeshGenerator Navmesh;

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        // Supprimer ancien donjon
        // Reset tout

        dungeonRooms.Clear();
        RoomController.instance.ClearRooms();

        Navmesh.ResetList();

        List<GameObject> _go = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            _go.Add(transform.GetChild(i).gameObject);
        }

        foreach (GameObject _g in _go)
        {
            Destroy(_g);
        }

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

    int totalRooms = 0;
    int roomCount = 0;

    public void SetRoomCount(int _i) => totalRooms = _i; 

    public void AddGeneratedRoom()
    {
        roomCount++;

        if(roomCount >= totalRooms)
        {
            Navmesh.BuildNavMesh();

            BJ_Enemy[] _e = FindObjectsOfType<BJ_Enemy>();
            foreach (BJ_Enemy _en in _e)
            {
                _en.Init();
            }

            BJ_GameManager.I.RoomReady();
        }
    }
}
