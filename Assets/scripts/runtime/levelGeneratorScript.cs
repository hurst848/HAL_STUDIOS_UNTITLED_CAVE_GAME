using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class levelGeneratorScript : MonoBehaviour
{
    /*
     *  ROOMS INDEX REFERENCE:
     *      - this is the reference for the rooms and segments lists
     *      - index 0 is reserved for the starting room
     *      - index 1 is reserved for the end room
     *      - index 2 is a cap room
     *      
     *  OFFSHOOT ROOM INDEX:
     *      - TBD
     */


    /*
     * TODO:
     *      - add extranouse paths after main generataion
     *      - add the end to newest generated room (dosent have to be "compatible")
     *      - optimise when needed (get wait times as low as possible), list of optimizeations bellow:
     *          - check in the is room compatible function if there would be an intersection with the cap
     *            room to give an idea if something is going to fit there
     * 
     */

    public List<GameObject> rooms;

    public List<GameObject> offShootRooms;

    public GameObject doorBlock;

    public string seed;

    public int magnitude = 100;

    public int offshootMaximumMagnitude = 10;

    private bool levelGenerated = false;

    private List<GameObject> generatedlevel = new List<GameObject>();
    [HideInInspector]
    public bool generateLevell = false;

    public NavMeshSurface surface;
    [HideInInspector]
    public GameObject hostObject;

    private bool pollIntersection = false;
    private int numIntersections = 0;
    private LayerMask mask;

    private GameObject nodeChecker;
    private bool useNodeChecker = false;

    private bool waitForRoom = false;

    private void Start()
    {
        mask = LayerMask.GetMask("roomGenDetection");
        hostObject = new GameObject();
        generatedlevel.Add(Instantiate(rooms[0], hostObject.transform));
    }


    void Update()
    {
        if (generateLevell)
        {
            Debug.Log("WORKING");
            generateLevell = false;
        }
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(generateLinearRoom());
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(generateLevel());
        }
    }

    private void FixedUpdate()
    {
        if (pollIntersection)
        {
            // if pool intersection is true, check to see if the new room will intersect until made false
            Vector3 boxCentre = generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().bounds.center;// + new Vector3(0, 0.15f, 0);
            if (Physics.CheckBox(boxCentre, 
                generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size * 0.25f, 
                generatedlevel[generatedlevel.Count - 1].transform.rotation,
                mask, QueryTriggerInteraction.Collide))
            {
                numIntersections++;
            }
        }
        else if (useNodeChecker)
        {
            // if pool intersection is true, check to see if the new room will intersect until made false
            Vector3 boxCentre = nodeChecker.transform.GetChild(0).GetComponent<BoxCollider>().bounds.center;// + new Vector3(0, 0.15f, 0);
            if (Physics.CheckBox(boxCentre,
                nodeChecker.transform.GetChild(0).GetComponent<BoxCollider>().size * 0.1f,
                nodeChecker.transform.rotation,
                mask, QueryTriggerInteraction.Collide))
            {
                numIntersections++;
            }
        }
        else
        {
            numIntersections = 0;
        }


    }

    IEnumerator generateLevel()
    {
        // init seed
        int trueSeed = 0;
        for (int i = 0; i < seed.Length; i++)
        {
            trueSeed += seed[i];
        }
        Random.InitState(trueSeed);
        // generate the initial path of level with the number of rooms = to the magnitude
        for (int i = 0; i < magnitude; i++)
        {
            StartCoroutine(generateLinearRoom());
            // wait for the room to generate so that the intersection checks can take place
            while (waitForRoom)
            {
                yield return new WaitForSeconds(0.01f); 
            }
        }
        StartCoroutine(generateEndingRoom());
        Debug.Log("main level generated");
        Debug.Log("LEVEL GENERATED Y'ALL");
        yield return null;
    }

    IEnumerator generateLinearRoom()
    {
        waitForRoom = true;
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
        //int startIndex = validStartIndicies[Random.Range(0, validStartIndicies.Count - 1)];
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
                generatedlevel.Add(Instantiate(rooms[roomToBeChecked], hostObject.transform));
                

                // pick one of the valid nodes and assign it to gameobject _b
                if (validNodes.Count > 0)
                {
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
                   
                    //poll intersection
                    generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");
                    generatedlevel[generatedlevel.Count - 2].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");
                    pollIntersection = true;
                    yield return new WaitForSeconds(0.1f / 2);
                    // check if the new room intersects with any nearby rooms
                    if (numIntersections > 0)
                    {
                        // if it is intersecting, destroy the room and remove this node from the valid list
                        GameObject tbr = generatedlevel[generatedlevel.Count - 1];
                        generatedlevel.RemoveAt(generatedlevel.Count - 1);
                        Destroy(tbr);
                        validNodes.RemoveAt(chosenIndex);
                    }
                    else
                    {
                        
                        //Debug.Break();
                        GameObject g12 = generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes[startIndex];
                        GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[chosenIndex]];
                        generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes.RemoveAt(startIndex);
                        generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(validNodes[chosenIndex]);
                        Destroy(g12);
                        Destroy(g13);
                        roomGenerated = true;
                    }
                    pollIntersection = false;
                    generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
                    generatedlevel[generatedlevel.Count - 2].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
                    yield return new WaitForSeconds(0.1f / 2);
                }
            }

            // if all valid nodes are tested in a room, remove it from the valid rooms list
            validRooms.RemoveAt(indexOfRoom);

            // if all possible combinations fail on that start node, choose a new one
            if (!(validRooms.Count > 0))
            {
                usedIndicies.Add(startIndex);
                if (generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count != usedIndicies.Count)
                {
                    // add this node to a list so it cant be used again
                    bool newStartIndex = false;
                    // generate a new index, excluding those from the usedIndicies list
                    int newIndex = 0;
                    while (!newStartIndex && newIndex < generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count)
                    {
                        if (!(usedIndicies.Contains(newIndex)))
                        {
                            startIndex = newIndex;
                            _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
                            validRooms = getCompatibleRooms(_a);
                            newStartIndex = true;
                        }
                        else
                        {
                            newIndex++;
                        }
                    }
                    if (!newStartIndex)
                    {
                        usedIndicies = new List<int>();
                        // swap
                        GameObject tmpA = generatedlevel[generatedlevel.Count - 1];
                        int newRoom = findNewRoom();
                        generatedlevel[generatedlevel.Count - 1] = generatedlevel[newRoom];
                        generatedlevel[newRoom] = tmpA;
                        // reset and update out of loop values
                        _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
                        validRooms = getCompatibleRooms(_a);
                        startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1);
                    }
                }
                else
                {
                    usedIndicies = new List<int>();
                    // swap
                    GameObject tmpA = generatedlevel[generatedlevel.Count - 1];
                    int newRoom = findNewRoom();
                    generatedlevel[generatedlevel.Count - 1] = generatedlevel[newRoom];
                    generatedlevel[newRoom] = tmpA;
                    // reset and update out of loop values
                    _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
                    validRooms = getCompatibleRooms(_a);
                    startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1);
                }

            }
            
            
        }
        waitForRoom = false;
        yield return null;
    }

    IEnumerator generateEndingRoom()
    {
        bool done = false;
        generatedlevel.Add(Instantiate(rooms[1], hostObject.transform));
        GameObject _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0];
        for (int i = generatedlevel.Count - 2; i >= 0; i--)
        {
            for (int j = 0; j < generatedlevel[i].GetComponent<roomData>().listOfNodes.Count; j++)
            {
                nodeData _a = generatedlevel[i].GetComponent<roomData>().listOfNodes[j].GetComponent<nodeData>();

                float rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                // move the room to the correct position
                generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                (_a.gameObject.transform.position.x - _b.transform.position.x),
                (_a.gameObject.transform.position.y - _b.transform.position.y),
                (_a.gameObject.transform.position.z - _b.transform.position.z));

                //poll intersection
                generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");
                generatedlevel[i].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");
                pollIntersection = true;
                yield return new WaitForSeconds(0.1f / 2);
                // check if the new room intersects with any nearby rooms
                if (!(numIntersections > 0))
                {
                    done = true;
                }
                pollIntersection = false;
                generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
                generatedlevel[i].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
                yield return new WaitForSeconds(0.1f / 2);
                if (done) { break; }
            }
            if (done) { break; }
        }

        yield return null;
    }

    IEnumerator generateOffshootPaths()
    {
        List<GameObject> offshootOriginRooms = getRemainingRooms();

        for (int i =0; i < offshootOriginRooms.Count; i++)
        {
            for (int nd = 0; nd < offshootOriginRooms[i].GetComponent<roomData>().listOfNodes.Count; nd++)
            {
                // initial pass to remove / block off any nodes that cant have expanding paths
                GameObject testBox = Instantiate(rooms[2]);
                float rotDiff = (((offshootOriginRooms[i].GetComponent<roomData>().listOfNodes[nd].gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((testBox.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                testBox.transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                testBox.transform.position += new Vector3(
                    (offshootOriginRooms[i].GetComponent<roomData>().listOfNodes[nd].gameObject.transform.position.x - testBox.transform.position.x),
                    (offshootOriginRooms[i].GetComponent<roomData>().listOfNodes[nd].gameObject.transform.position.y - testBox.transform.position.y),
                    (offshootOriginRooms[i].GetComponent<roomData>().listOfNodes[nd].gameObject.transform.position.z - testBox.transform.position.z));
                
                offshootOriginRooms[i].layer = LayerMask.NameToLayer("Ground");
                testBox.layer = LayerMask.NameToLayer("Ground");
                nodeChecker = testBox;
                useNodeChecker = true;
                yield return new WaitForSeconds(0.1f / 2);
                if (numIntersections > 0)
                {
                    generatedlevel.Add(Instantiate(doorBlock, hostObject.transform));
                    
                    GameObject _b = generatedlevel[generatedlevel.Count - 1];
                    GameObject _a = offshootOriginRooms[i].GetComponent<roomData>().listOfNodes[nd];

                    float rotDiffb = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                    generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiffb, transform.eulerAngles.z);

                    // move the room to the correct position
                    generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                    (_a.gameObject.transform.position.x - _b.transform.position.x),
                    (_a.gameObject.transform.position.y - _b.transform.position.y),
                    (_a.gameObject.transform.position.z - _b.transform.position.z));

                    offshootOriginRooms[i].GetComponent<roomData>().listOfNodes.RemoveAt(nd);
                    Destroy(_a);
                }
                useNodeChecker = false;
                offshootOriginRooms[i].layer = LayerMask.NameToLayer("roomGenDetection");
                Destroy(testBox);
                yield return new WaitForSeconds(0.1f / 2);

            }
        }

        // update the list of remaining rooms with the plugged ones removed 
        offshootOriginRooms = getRemainingRooms();
        for (int i = 0; i < offshootOriginRooms.Count; i++)
        {
            for (int node = 0; node < offshootOriginRooms[i].GetComponent<roomData>().listOfNodes.Count; node++)
            {
                
            }
        }


        yield return null;
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

    private List<int> getCompatibleRooms(nodeData _a)
    {
        List<int> rtrn = new List<int>();
        for (int i = 3; i < rooms.Count; i++)
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

        for (int i = generatedlevel.Count - 2; i >= 0; i--)
        {
            if (generatedlevel[i].GetComponent<roomData>().listOfNodes.Count != 0)
            {
                rtrn = i;
                break;
            }
        }

        return rtrn;
    }

    private List<GameObject> getRemainingRooms()
    {
        List<GameObject> rtrn = new List<GameObject>();

        for (int i = 0; i < generatedlevel.Count; i++)
        {
            rtrn.Add(generatedlevel[i]);
        }

        return rtrn;
    }

    private List<GameObject> shuffleOffshootRooms()
    {
        List<GameObject> rtrn = offShootRooms;
        for (int i =0; i < rtrn.Count; i++)
        {
            int swapIndex = -1;
            while (swapIndex == i || swapIndex == -1)
            {
                swapIndex = Random.Range(0, rtrn.Count - 1);
            }

            GameObject tmp = rtrn[i];
            rtrn[i] = rtrn[swapIndex];
            rtrn[swapIndex] = tmp;
    
        }
        return rtrn;
    }
   
    private void purgeNodes()
    {
        
    }
}
