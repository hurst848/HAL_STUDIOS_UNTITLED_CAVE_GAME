using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class levelGeneratorScript : MonoBehaviour
{
    /*
     *  INDEX REFERENCE:
     *      - this is the reference for the rooms and segments lists
     *      - index 0 is reserved for the starting room
     *      - index 1 is reserved for the end room
     */

    public List<GameObject> rooms;

    public string seed;

    public int magnitude = 100;

    private bool levelGenerated = false;

    private List<GameObject> generatedlevel = new List<GameObject>();

    public bool generateLevell = false;

    public NavMeshSurface surface;

    

    private void Start()
    {
        generatedlevel.Add(Instantiate(rooms[0]));
    }


    void Update()
    {
        if (generateLevell)
        {
            Debug.Log("WORKING");
            generateLevell = false;
            generateLevelLinear();
        }
        if (Input.GetKeyDown("space"))
        {
            generateLinearRoom();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(generateLevel());
        }
    }

    IEnumerator generateLevel()
    {
        int trueSeed = 0;
        for (int i = 0; i < seed.Length; i++)
        {
            trueSeed += seed[i];
        }
        Random.InitState(trueSeed);
        for (int i = 0; i < magnitude; i++)
        {
            generateLinearRoom();
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;
    }
    
    public void generateLevelLinear()
    {
        

        /*
            PROBABLE FIX:
                - for each loop of while, create a tmp list of rooms
                    remove room from the list once all valid nodes have 
                    been tested. Do something simmilar for rooms so it is
                    not repeated and every possibility is tested
                - if every combination is tested and still dosent work,
                    break the program

         */


        if (!levelGenerated)
        {
            generatedlevel.Add(Instantiate(rooms[0]));
            while (generatedlevel.Count < magnitude)
            {
                // choose which node on the newest room to generate from, switch up the room if nodes are avalible
                if (generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count <= 0)
                {
                    // switch the newest room with one that has availible rooms
                    GameObject tmpA = generatedlevel[generatedlevel.Count - 1];
                    int newRoom = findNewRoom();
                    generatedlevel[generatedlevel.Count - 1] = generatedlevel[newRoom];
                    generatedlevel[newRoom] = tmpA;
                }
                int startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1);
                nodeData _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
                List<int> usedIndicies = new List<int>();

                // calcualte all rooms compatible with this node
                List<int> validRooms = getCompatibleRooms(_a);

                bool roomGenerated = false;

                // loop through until room is generated
                while (!roomGenerated)
                {
                    // pick a room from the compatible list
                    int indexOfRoom = Random.Range(0, validRooms.Count - 1);
                    int roomToBeChecked = validRooms[indexOfRoom];

                    // make a list of all the nodes in that room that are compatible
                    List<int> validNodes = isRoomCompatible(_a, rooms[roomToBeChecked]);

                    // loop through all compatible nodes of this room
                    while (validNodes.Count > 0 && !roomGenerated)
                    {
                        // update compatible node list as the amount of nodes may have changed
                        //validNodes = isRoomCompatible(_a, rooms[roomToBeChecked]);

                        // instantiate the room
                        generatedlevel.Add(Instantiate(rooms[roomToBeChecked]));

                        // pick one of the valid nodes and assign it to gameobject _b
                        int chosenIndex = Random.Range(0, validNodes.Count - 1);
                        GameObject _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[chosenIndex]];

                        // rotate the new room to the correct orrientation
                        float rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                        generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                        // move the room to the correct position
                        generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                        (_a.gameObject.transform.position.x - _b.transform.position.x),
                        (_a.gameObject.transform.position.y - _b.transform.position.y),
                        (_a.gameObject.transform.position.z - _b.transform.position.z));

                        // check if the new room intersects with any nearby rooms
                        if (checkIntersect())
                        {
                            // if it is intersecting, destroy the room and remove this node from the valid list
                            GameObject tbr = generatedlevel[generatedlevel.Count - 1];
                            generatedlevel.RemoveAt(generatedlevel.Count - 1);
                            Destroy(tbr);
                            validNodes.RemoveAt(chosenIndex);
                        }
                        else
                        {
                            GameObject g12 = generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes[startIndex];
                            GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[chosenIndex]];
                            generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes.RemoveAt(0);
                            generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(validNodes[chosenIndex]);
                            Destroy(g12);
                            Destroy(g13);
                            roomGenerated = true;
                        }


                    }
                    // if all valid nodes are tested in a room, remove it from the valid rooms list
                    validRooms.RemoveAt(indexOfRoom);

                    // if all possible combinations fail on that start node, choose a new one
                    if (!(validRooms.Count > 0))
                    {
                        Debug.Log("no rooms found");
                        if (generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count != usedIndicies.Count)
                        {
                            // add this node to a list so it cant be used again
                            usedIndicies.Add(startIndex);
                            bool newStartIndex = false;

                            // generate a new index, excluding those from the usedIndicies list
                            while (!newStartIndex)
                            {
                                startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1);
                                if (!(usedIndicies.Contains(startIndex)))
                                {
                                    _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
                                    validRooms = getCompatibleRooms(_a);
                                    newStartIndex = true;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("INVALID ROOM, ERROR DETECTED");
                            Debug.Break();
                        }

                    }
                }

            }
        }
        // uncomment when level gen is done
        //surface.BuildNavMesh();
    }

    void generateLinearRoom()
    {
        
        // choose which node on the newest room to generate from, switch up the room if nodes are avalible
        if (generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count <= 0)
        {
            // switch the newest room with one that has availible rooms
            GameObject tmpA = generatedlevel[generatedlevel.Count - 1];
            int newRoom = findNewRoom();
            generatedlevel[generatedlevel.Count - 1] = generatedlevel[newRoom];
            generatedlevel[newRoom] = tmpA;
        }
        int startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1);
        nodeData _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
        List<int> usedIndicies = new List<int>();

        // calcualte all rooms compatible with this node
        List<int> validRooms = getCompatibleRooms(_a);

        bool roomGenerated = false;

        // loop through until room is generated
        while (!roomGenerated)
        {
            // pick a room from the compatible list
            int indexOfRoom = Random.Range(0, validRooms.Count - 1);
            int roomToBeChecked = validRooms[indexOfRoom];

            // make a list of all the nodes in that room that are compatible
            List<int> validNodes = isRoomCompatible(_a, rooms[roomToBeChecked]);

            // loop through all compatible nodes of this room
            while (validNodes.Count > 0 && !roomGenerated)
            {
                // instantiate the room
                generatedlevel.Add(Instantiate(rooms[roomToBeChecked]));

                // pick one of the valid nodes and assign it to gameobject _b
                int chosenIndex = Random.Range(0, validNodes.Count - 1);
                GameObject _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[chosenIndex]];

                // rotate the new room to the correct orrientation
                float rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                // move the room to the correct position
                generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                (_a.gameObject.transform.position.x - _b.transform.position.x),
                (_a.gameObject.transform.position.y - _b.transform.position.y),
                (_a.gameObject.transform.position.z - _b.transform.position.z));

                // check if the new room intersects with any nearby rooms
                if (checkIntersect())
                {
                    // if it is intersecting, destroy the room and remove this node from the valid list
                    GameObject tbr = generatedlevel[generatedlevel.Count - 1];
                    generatedlevel.RemoveAt(generatedlevel.Count - 1);
                    Destroy(tbr);
                    validNodes.RemoveAt(chosenIndex);
                }
                else
                {
                    GameObject g12 = generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes[startIndex];
                    GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[chosenIndex]];
                    generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes.RemoveAt(startIndex);
                    generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(validNodes[chosenIndex]);
                    Destroy(g12);
                    Destroy(g13);
                    roomGenerated = true;
                }


                /*
                 * the loop exits if there are no avalible nodes in that room, 
                 * to fix encapsulate the entire thing in another loop until the room 
                 * is genereated, this should fix it probably
                 */
            }
            // if all valid nodes are tested in a room, remove it from the valid rooms list
            validRooms.RemoveAt(indexOfRoom);

            // if all possible combinations fail on that start node, choose a new one
            if (!(validRooms.Count > 0))
            {
                Debug.Log("no rooms found");
                if (generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count != usedIndicies.Count)
                {
                    // add this node to a list so it cant be used again
                    usedIndicies.Add(startIndex);
                    bool newStartIndex = false;

                    // generate a new index, excluding those from the usedIndicies list
                    while (!newStartIndex)
                    {
                        if (generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count <= 0)
                        {
                            // switch the newest room with one that has availible rooms
                            GameObject tmpA = generatedlevel[generatedlevel.Count - 1];
                            int newRoom = findNewRoom();
                            generatedlevel[generatedlevel.Count - 1] = generatedlevel[newRoom];
                            generatedlevel[newRoom] = tmpA;
                        }
                        startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1);
                        if (!(usedIndicies.Contains(startIndex)))
                        {
                            _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
                            validRooms = getCompatibleRooms(_a);
                            newStartIndex = true;
                        }
                    }
                }
                else
                {
                    Debug.Log("INVALID ROOM, ERROR DETECTED");
                    Debug.Break();
                }

            }
        }
    }
    



    private List<int> isRoomCompatible(nodeData _a, GameObject _room)
    {
        List<int> validIndicies = new List<int>();

        for (int i = 0; i < _room.GetComponent<roomData>().listOfNodes.Count; i++)
        {
            if (isNodeCompatible(_a, _room.GetComponent<roomData>().listOfNodes[i].GetComponent<nodeData>()))
            {
                validIndicies.Add(i);
            }
        }

        return validIndicies;
    }


    private bool isNodeCompatible(nodeData _a, nodeData _b)
    {
        if (_a.canConnectToCap == _b.isCap)
        {
            if (_a.canConnectToSmall == _b.canConnectToSmall)
            {
                return true;
            }
            else if (_a.canConnectToMedium == _b.isMedium)
            {
                return true;
            }
            else if (_a.canConnectToLarge == _b.isLarge)
            {
                return true;
            }
        }
        else if (_a.canConnectToCorridor == _b.isCorridor)
        {
            if (_a.canConnectToSmall == _b.canConnectToSmall)
            {
                return true;
            }
            else if (_a.canConnectToMedium == _b.isMedium)
            {
                return true;
            }
            else if (_a.canConnectToLarge == _b.isLarge)
            {
                return true;
            }
        }
        else if (_a.canConnectToRoom == _b.isRoom)
        {
            if (_a.canConnectToSmall == _b.canConnectToSmall)
            {
                return true;
            }
            else if (_a.canConnectToMedium == _b.isMedium)
            {
                return true;
            }
            else if (_a.canConnectToLarge == _b.isLarge)
            {
                return true;
            }
        }
        return false;
    }

    private nodeData getValidNode()
    {
        nodeData _a = new nodeData();

        for (int i = 0; i < generatedlevel.Count; i++)
        {
            if (!(generatedlevel[i].GetComponent<roomData>().listOfNodes.Count <= 0))
            {
                _a = generatedlevel[i].GetComponent<roomData>().listOfNodes[0].GetComponent<nodeData>();
            }
        }

        return _a;
    }


    private bool checkIntersect()
    {
        for (int i = 0; i < generatedlevel.Count - 1; i++)
        {
            Bounds b = generatedlevel[i].transform.GetChild(0).GetComponent<BoxCollider>().bounds;
            b.size *= 0.95f;
            if (generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().bounds.Intersects(b))
            {
                return true;
            }
        }
        return false;
    }

    private List<int> getCompatibleRooms(nodeData _a)
    {
        List<int> rtrn = new List<int>();
        for (int i = 2; i < rooms.Count; i++)
        {
            if (isRoomCompatible(_a, rooms[i]).Count > 0)
            {
                rtrn.Add(i);
            }
        }
        return rtrn;
    }

    private int findNewRoom()
    {
        int rtrn = 0;

        for (int i = generatedlevel.Count - 1; i >= 0; i--)
        {
            if (generatedlevel[i].GetComponent<roomData>().listOfNodes.Count != 0)
            {
                rtrn = i;
                break;
            }
        }

        return rtrn;
    }
}
