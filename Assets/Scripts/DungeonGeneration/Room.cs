using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public int Width;

    public int Height;

    public int X;

    public int Z;

    private bool updatedDoors = false;
    
   

    public Room(int x, int z)
    {
        X = x;
        Z = z;
    }

    public Door leftDoor1;

    public Door leftDoor2;

    public Door rightDoor1;

    public Door rightDoor2;

    public Door topDoor1;

    public Door topDoor2;

    public Door bottomDoor1;

    public Door bottomDoor2;

    public List<Door> doors = new List<Door>();

    // Start is called before the first frame update
    void Start()
    {
        if(RoomController.instance == null)
        {
            Debug.Log("You pressed play in the wrong scene!");
            return;
        }

        Door[] ds = GetComponentsInChildren<Door>();

        
        foreach(Door d in ds)
        {
            doors.Add(d);
            switch(d.doorType)
            {
                case Door.DoorType.right1:
                    rightDoor1 = d;
                    break;

                case Door.DoorType.right2:
                    rightDoor2 = d;
                    break;

                case Door.DoorType.left1:
                    leftDoor1 = d;
                    break;

                case Door.DoorType.left2:
                    leftDoor2 = d;
                    break;

                case Door.DoorType.top1:
                    topDoor1 = d;
                    break;

                case Door.DoorType.top2:
                    topDoor2 = d;
                    break;

                case Door.DoorType.bottom1:
                    bottomDoor1 = d;
                    break;

                case Door.DoorType.bottom2:
                    bottomDoor2 = d;
                    break;
            }
        }
        RoomController.instance.RegisterRoom(this);
    }

    void Update()
    {
        if(name.Contains("End") && !updatedDoors)
        {
            RemoveUnconnectedDoors();
            updatedDoors = true;
        }
        
    }

    public void RemoveUnconnectedDoors()
    {
        foreach(Door door in doors)
        {
            switch(door.doorType)
            {
                case Door.DoorType.right1:
                    if (GetRight() == null)
                        door.gameObject.SetActive(false);
                    break;

                case Door.DoorType.right2:
                    if (GetRight() == null)
                        door.gameObject.SetActive(false);
                    break;

                case Door.DoorType.left1:
                    if (GetLeft() == null)
                        door.gameObject.SetActive(false);
                    break;

                case Door.DoorType.left2:
                    if (GetLeft() == null)
                        door.gameObject.SetActive(false);
                    break;

                case Door.DoorType.top1:
                    if (GetTop() == null)
                        door.gameObject.SetActive(false);
                    break;

                case Door.DoorType.top2:
                    if (GetTop() == null)
                        door.gameObject.SetActive(false);
                    break;

                case Door.DoorType.bottom1:
                    if (GetBottom() == null)
                        door.gameObject.SetActive(false);
                    break;

                case Door.DoorType.bottom2:
                    if (GetBottom() == null)
                        door.gameObject.SetActive(false);
                    break;
            }
        }
    }

    public Room GetRight()
    {
        if(RoomController.instance.DoesRoomExist(X + 1, Z))
        {
            return RoomController.instance.FindRoom(X + 1, Z);
        }

        return null;
    }

    public Room GetLeft()
    {
        if (RoomController.instance.DoesRoomExist(X - 1, Z))
        {
            return RoomController.instance.FindRoom(X - 1, Z);
        }
        
        return null;


    }

    public Room GetTop()
    {
        if (RoomController.instance.DoesRoomExist(X, Z + 1))
        {
            return RoomController.instance.FindRoom(X, Z + 1);
        }

        return null;

    }

    public Room GetBottom()
    {
        if (RoomController.instance.DoesRoomExist(X, Z - 1))
        {
            return RoomController.instance.FindRoom(X, Z - 1);
        }

        return null;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, 0, Height));

    }
    
    public Vector3 GetRoomCenter()
    {
        return new Vector3(X * Width, 0,  Z * Height);
    }
}
