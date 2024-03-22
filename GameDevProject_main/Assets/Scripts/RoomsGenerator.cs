using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class RoomsGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;

    public Transform playerTransform;

    public NavMeshSurface navSurface;

    private GameObject currentRoom;
    private GameObject[] adjacentRooms = new GameObject[0];

    private float lastNavmeshUpdate = 0f;
    private float navmeshUpdateDelay = 10;

    // Start is called before the first frame update
    void Start()
    {
        currentRoom = Instantiate(roomPrefabs[0]);

        for(int i = 0; i < currentRoom.transform.childCount; i++)
        {
            if (currentRoom.transform.GetChild(i).name.StartsWith("Passage"))
            {
                PassageController passageController = currentRoom.transform.GetChild(i).AddComponent<PassageController>();
            }
        }

        InstantiateAdjacentRooms();
    }

    // Update is called once per frame
    void Update()
    { 
        // Check if player has entered another room
        if(!currentRoom.GetComponent<Collider>().bounds.Contains(playerTransform.position + new Vector3(0,1,0)))
        {
            MoveToRoom();
        }

        if(lastNavmeshUpdate + navmeshUpdateDelay < Time.time) {
            NavMesh.RemoveAllNavMeshData();
            navSurface.BuildNavMesh();

            lastNavmeshUpdate = Time.time;
        }
    }

    void InstantiateAdjacentRooms()
    {
        //foreach (Transform emptyTransform in currentRoom.transform)
        for(int i = 0; i<currentRoom.transform.childCount; i++)
        {
            if (currentRoom.transform.GetChild(i).name.StartsWith("Passage"))
            {
                if (!currentRoom.transform.GetChild(i).GetComponent<PassageController>().getConnection())
                {
                    Transform emptyTransform = currentRoom.transform.GetChild(i);

                    // Get a random room prefab from the array
                    GameObject randomRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

                    // Instantiate the room at the empty's position and rotation
                    GameObject newRoom = Instantiate(randomRoomPrefab);

                    // Get a random empty child transform
                    int childIndex = Random.Range(0, newRoom.transform.childCount);
                    while (!newRoom.transform.GetChild(childIndex).name.StartsWith("Passage"))
                    {
                        childIndex = Random.Range(0, newRoom.transform.childCount);
                    }
                    Transform randomEmpty = newRoom.transform.GetChild(childIndex);

                    //Quaternion targetRotation = Quaternion.FromToRotation(randomEmpty.up, Vector3.back);
                    newRoom.transform.rotation = emptyTransform.rotation * Quaternion.Inverse(randomEmpty.rotation);
                    newRoom.transform.Rotate(0, 180, 0);

                    // Calculate the offset
                    Vector3 offset = randomEmpty.position - randomRoomPrefab.transform.position;

                    newRoom.transform.position = emptyTransform.position - offset;

                    //Save information on which passages are used
                    for (int j = 0; j < newRoom.transform.childCount; j++)
                    {
                        if (newRoom.transform.GetChild(j).name.StartsWith("Passage"))
                        {
                            PassageController passageController = newRoom.transform.GetChild(j).AddComponent<PassageController>();
                            if (j == childIndex)
                            {
                                passageController.connectPassage();
                            }
                        }
                    }
                    currentRoom.transform.GetChild(i).GetComponent<PassageController>().connectPassage();

                    // Add the new room to the adjacentRooms array
                    AddToAdjacentRooms(newRoom);

                    NavMesh.RemoveAllNavMeshData();
                    navSurface.BuildNavMesh();
                }
            }
        }
    }

    void AddToAdjacentRooms(GameObject newRoom)
    {
        // Resize the array and add the new room to the end
        System.Array.Resize(ref adjacentRooms, adjacentRooms.Length + 1);
        adjacentRooms[adjacentRooms.Length - 1] = newRoom;
    }

    void MoveToRoom()
    {
        // Check if there are adjacent rooms
        if (adjacentRooms.Length > 0)
        {
            // Check all adjacent rooms until player is found
            int i = 0;
            foreach(GameObject room in adjacentRooms)
            {
                if (room.GetComponent<Collider>().bounds.Contains(playerTransform.position + new Vector3(0, 1, 0)))
                {

                    RemoveFromAdjacentRooms(i);

                    ClearAdjacentRooms();
                    ClearPassages(currentRoom);
                    ConnectNearestPassage(currentRoom, playerTransform.position);

                    // Add the previous currentRoom to adjacentRooms
                    AddToAdjacentRooms(currentRoom);
                    
                    // Move the player to the new currentRoom
                    currentRoom = room;

                    // Instantiate new adjacent rooms for the new currentRoom
                    InstantiateAdjacentRooms();

                    break;
                }
                i++;
            }
        }
        else
        {
            Debug.Log("No adjacent rooms available.");
        }
    }

    void ClearAdjacentRooms()
    {
        foreach (GameObject room in adjacentRooms)
        {
            Destroy(room);
        }

        adjacentRooms = new GameObject[0];
    }

    void ClearPassages(GameObject room)
    {
        for (int i = 0; i < room.transform.childCount; i++)
        {
            if (room.transform.GetChild(i).name.StartsWith("Passage"))
            {
                room.transform.GetChild(i).GetComponent<PassageController>().cutPassage();
            }
        }
    }

    void ConnectNearestPassage(GameObject room, Vector3 position)
    {
        float minDistance = 100;
        int minIndex = 100;
        for (int i = 0; i < room.transform.childCount; i++)
        {
            if (room.transform.GetChild(i).name.StartsWith("Passage")) 
            { 
                float distance = Vector3.Distance(room.transform.GetChild(i).position, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }
        }
        try
        {
            room.transform.GetChild(minIndex).GetComponent<PassageController>().connectPassage();
        }
        catch (System.IndexOutOfRangeException)
        {
            // This block is executed if an IndexOutOfRangeException occurs
            Debug.LogError("Error: No child within the specified index range");
        }
    }

    void RemoveFromAdjacentRooms(int index)
    {
        if (index < 0 || index >= adjacentRooms.Length)
        {
            Debug.LogError("Index out of range for removing from adjacentRooms.");
            return;
        }

        // Shift elements to the left to remove the room at the specified index
        for (int i = index; i < adjacentRooms.Length - 1; i++)
        {
            adjacentRooms[i] = adjacentRooms[i + 1];
        }

        // Resize the array to remove the last element
        System.Array.Resize(ref adjacentRooms, adjacentRooms.Length - 1);
    }
}
