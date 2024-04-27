using LlockhamIndustries.ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] roomPrefabsArray; // Serialized array visible in the Inspector
    private List<GameObject> roomPrefabs; // List that will be used at runtime

    public GameObject exitPrefab;

    public Transform playerTransform;

    public NavMeshSurface navSurface;

    private GameObject currentRoom;
    private List<GameObject> adjacentRooms = new List<GameObject>();

    private float lastNavmeshUpdate = 0f;
    private float navmeshUpdateDelay = 5;

    public GameObject ammoBox;
    public string ammoName = "AmmoPack";
    public GameObject healthBox;
    public string healthName = "HealthPack";

    // Start is called before the first frame update
    void Start()
    {
        roomPrefabs = new List<GameObject>(roomPrefabsArray);

        if (GameManager.Instance.spawnExit)
        {
            StartCoroutine(spawnExit(GameManager.Instance.exitSpawnDelay));
        }

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
                    GameObject randomRoomPrefab = roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Count)];
                        
                    // Instantiate the room at the empty's position and rotation
                    GameObject newRoom = Instantiate(randomRoomPrefab);



                    // Get a random empty child transform
                    List<int> eligibleIndexes = new List<int>();
                    for (int j = 0; j < newRoom.transform.childCount; j++)
                    {
                        if (newRoom.transform.GetChild(j).name.StartsWith("Passage"))
                        {
                            eligibleIndexes.Add(j);
                        }
                    }
                    int childIndex = eligibleIndexes[UnityEngine.Random.Range(0, eligibleIndexes.Count)];
                    Transform randomEmpty = newRoom.transform.GetChild(childIndex);

                    //Quaternion targetRotation = Quaternion.FromToRotation(randomEmpty.up, Vector3.back);
                    newRoom.transform.rotation = emptyTransform.rotation * Quaternion.Inverse(randomEmpty.rotation);
                    newRoom.transform.Rotate(0, 180, 0);

                    // Calculate the offset
                    Vector3 offset = randomEmpty.position - randomRoomPrefab.transform.position;

                    newRoom.transform.position = emptyTransform.position - offset;

                    //Save information on which passages are used and instantiate health and ammo refills
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
                        else if (newRoom.transform.GetChild(j).name.StartsWith(ammoName))
                        {
                            float randomValue = UnityEngine.Random.value;
                            if (randomValue < GameManager.Instance.ammoPackChance)
                            {
                                Instantiate(ammoBox, newRoom.transform.GetChild(j));
                            }
                        }
                        else if (newRoom.transform.GetChild(j).name.StartsWith(healthName))
                        {
                            float randomValue = UnityEngine.Random.value;
                            if (randomValue < GameManager.Instance.healthPackChance)
                            {
                                Instantiate(healthBox, newRoom.transform.GetChild(j));
                            }
                        }
                    }
                    currentRoom.transform.GetChild(i).GetComponent<PassageController>().connectPassage();

                    // Add the new room to the adjacentRooms array
                    adjacentRooms.Add(newRoom);

                    NavMesh.RemoveAllNavMeshData();
                    navSurface.BuildNavMesh();
                }
            }
        }
    }

    void MoveToRoom()
    {
        // Check if there are adjacent rooms
        if (adjacentRooms.Count > 0)
        {
            // Check all adjacent rooms until player is found
            foreach(GameObject room in adjacentRooms)
            {
                if (room.GetComponent<Collider>().bounds.Contains(playerTransform.position + new Vector3(0, 1, 0)))
                {

                    adjacentRooms.Remove(room);

                    ClearAdjacentRooms();
                    ClearPassages(currentRoom);
                    ConnectNearestPassage(currentRoom, playerTransform.position);

                    // Add the previous currentRoom to adjacentRooms
                    adjacentRooms.Add(currentRoom);

                    // Move the player to the new currentRoom
                    currentRoom = room;

                    // Instantiate new adjacent rooms for the new currentRoom
                    InstantiateAdjacentRooms();

                    break;
                }
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
            Bounds roomBounds = room.GetComponent<MeshCollider>().bounds;
            Collider[] collidersInRoom = Physics.OverlapBox(roomBounds.center, roomBounds.extents);
            
            foreach (Collider collider in collidersInRoom)
            {
                if (!collider.IsDestroyed() && collider.transform.root.CompareTag("Enemy"))
                {
                    Destroy(collider.transform.root.gameObject);
                }
            }
            Destroy(room);
        }

        adjacentRooms.Clear();
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
        adjacentRooms.RemoveAt(index);
    }

    public GameObject getCurrentRoom()
    {
        return currentRoom;
    }

    IEnumerator spawnExit(int timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        roomPrefabs.Add(exitPrefab);
    }
}
