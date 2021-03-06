using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


public class RoomInfo
{
    public string name;

    public int x;

    public int z;
}

public class RoomController : MonoBehaviour
{
    [SerializeField] BJ_NavmeshGenerator Navmesh;

    public List<GameObject> floor = new List<GameObject>();

    public static RoomController instance;

    string currentWorldName = "Basement";

    RoomInfo currentLoadRoomData;

    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();

    public List<Room> loadedRooms = new List<Room>();

    bool isLoadingRoom = false;
    bool spawnedBossRoom = false;
    [SerializeField] bool updatedRooms = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
      

    }

    void Update()
    {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        if(isLoadingRoom)
        {
            return;
        }

        if(loadRoomQueue.Count == 0)
        {
            if (!spawnedBossRoom)
            {
                StartCoroutine(SpawnBossRoom());
            }         
            else if(spawnedBossRoom && !updatedRooms)
            {
                foreach(Room room in loadedRooms)
                {
                    room.RemoveUnconnectedDoors();
                    DungeonGenerator.I.DungeonGenerated();
                }
                updatedRooms = true;
            }
            return;
        }

        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;

        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));
    }
    
    IEnumerator SpawnBossRoom()
    {
        spawnedBossRoom = true;
        yield return new WaitForSeconds(0.5f);
        if(loadRoomQueue.Count == 0)
        {
            Room bossRoom = loadedRooms[loadedRooms.Count - 1];
            Room tempRoom = new Room(bossRoom.X, bossRoom.Z);
            Destroy(bossRoom.gameObject);
            var roomToRemove = loadedRooms.Single(r => r.X == tempRoom.X && r.Z == tempRoom.Z);
            loadedRooms.Remove(roomToRemove);
            LoadRoom("End", tempRoom.X, tempRoom.Z);
        }
    }
       
    public void LoadRoom(string name, int x, int z)
    {
        if(DoesRoomExist(x, z) == true)
        {
            DungeonGenerator.I.DecreaseRoomCount();
            return;
        }
        
        RoomInfo newRoomData = new RoomInfo();
        newRoomData.name = name;
        newRoomData.x = x;
        newRoomData.z = z;

        
        loadRoomQueue.Enqueue(newRoomData);
    }

    IEnumerator LoadRoomRoutine(RoomInfo info) 
    {
        string roomName = currentWorldName + info.name;
        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);

        while(loadRoom.isDone == false)
        {
            yield return null;
        }
    }

    public void RegisterRoom(Room room)
    {
        if (currentLoadRoomData == null)
        {
            DungeonGenerator.I.DecreaseRoomCount();
            return;
        }

        if (!DoesRoomExist(currentLoadRoomData.x, currentLoadRoomData.z))
        {

            
            room.transform.position = new Vector3(
                currentLoadRoomData.x * room.Width,
                0,
                currentLoadRoomData.z * room.Height
               
            );

            room.X = currentLoadRoomData.x;
            room.Z = currentLoadRoomData.z;
            room.name = currentWorldName + "-" + currentLoadRoomData.name + " " + room.X + ", " + room.Z;
            room.transform.parent = transform;
            

            isLoadingRoom = false;

            loadedRooms.Add(room);

            
            floor.Clear();
            floor.Add(room.Floor.gameObject);
            Navmesh.SetNavMeshElements(floor);
            DungeonGenerator.I.AddGeneratedRoom();

            /////
        }
        else
        {
            Destroy(room.gameObject);
            isLoadingRoom = false;
        }
    }

   public bool DoesRoomExist(int x, int z)
    {
        return loadedRooms.Find(item => item.X == x && item.Z == z) != null;
    }

    public Room FindRoom(int x, int z)
    {
        return loadedRooms.Find(item => item.X == x && item.Z == z);
    }

    public void ClearRooms()
    {
        loadedRooms.Clear();
        updatedRooms = spawnedBossRoom = false;
    }

}
