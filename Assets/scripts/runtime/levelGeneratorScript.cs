using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class levelGeneratorScript : MonoBehaviour
{
    /*
     *  ROOMS INDEX REFERENCE:
     *      - this is the reference for the rooms and segments lists
     *      - index 0 is reserved for the starting room
     *      - index 1 is reserved for the end room
     *      - index 2 is a cap room
     *      - lastIndex will always be the vertical corridor
     *      
     *  OFFSHOOT ROOM INDEX:
     *      - TBD
     */




    public List<GameObject> rooms;

    public List<GameObject> spawnableItems;

    public GameObject monster;

    public GameObject doorBlock;

    public string seed;

    public int magnitude = 100;

    private bool levelGenerated = false;

    [HideInInspector]
    public List<GameObject> generatedlevel = new List<GameObject>();
    [HideInInspector]
    public bool generateLevell = false;

    public NavMeshSurface surface;
    [HideInInspector]
    public GameObject hostObject;

    public menuAgent menuController; 

    private bool pollIntersection = false;
    private int numIntersections = 0;
    public LayerMask mask;

    private GameObject nodeChecker;
    private bool useNodeChecker = false;

    private bool waitForRoom = false;
    private float intersectionMagnitudeFactor = 0.1f;

    // SPAWN RATES FOR PICKUP ITEMS //
    float cobbleSpawnRate = 0.50f;
    float inhalerSpawnRate = 0.25f;
    float medkitSpawnRate = 0.20f;

    // Stuff for faster/more reliable room generation
    int numberOfFails = 0;
    float intersectionMultipier = 1.0f;

    

    private void shuffleRoomList()
    {
        for (int i = 3; i < rooms.Count; i++)
        {
            int indexToSwitch = Random.Range(3, rooms.Count - 1);
            GameObject tmp = rooms[i];
            rooms[i] = rooms[indexToSwitch];
            rooms[indexToSwitch] = tmp;
        }
    }

    private void Start()
    {
        //mask = LayerMask.GetMask("roomGenDetection");
        hostObject = new GameObject();
        generatedlevel.Add(Instantiate(rooms[0], hostObject.transform));
        spawnInitialRooms();
        seed = gameHandler.gameSeed;
        magnitude = gameHandler.gameMagnitude;
       // gameHandler.numMonsters = 2;
        GameObject.FindGameObjectWithTag("Player").transform.position = generatedlevel[0].transform.position;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().useGravity = false;
        //GameObject.FindGameObjectWithTag("Player").GetComponent<cameraController>().canMove = false;
        
    }


    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(newRoomGeneration());
        }
    }

    void spawnInitialRooms()
    {
        int numOfInitRooms = 3;
        for (int i = 0; i < numOfInitRooms; i++)
        {
            nodeData _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0].GetComponent<nodeData>();

            if (i == numOfInitRooms - 1) { generatedlevel.Add(Instantiate(rooms[rooms.Count -1], hostObject.transform)); }
            else { generatedlevel.Add(Instantiate(rooms[3], hostObject.transform)); }

            GameObject _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[1];

            float rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
            generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

            generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                            (_a.gameObject.transform.position.x - _b.transform.position.x),
                            (_a.gameObject.transform.position.y - _b.transform.position.y),
                            (_a.gameObject.transform.position.z - _b.transform.position.z));

            GameObject g12 = generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes[0];
            GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[1];
            generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes.RemoveAt(0);
            generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(1);
            Destroy(g12);
            Destroy(g13);
        }
    }


    private void FixedUpdate()
    {
        if (pollIntersection)
        {
            // if pool intersection is true, check to see if the new room will intersect until made false
            Vector3 boxCentre = generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().bounds.center;// + new Vector3(0, 0.15f, 0);
            if (Physics.CheckBox(boxCentre,
                generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size * intersectionMagnitudeFactor,
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
                nodeChecker.transform.GetChild(0).GetComponent<BoxCollider>().size,
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

    public void generateLevel()
    {
        // init seed
        int trueSeed = 0;
        for (int i = 0; i < seed.Length; i++)
        {
            trueSeed += seed[i];
        }
        Random.InitState(trueSeed);


        StartCoroutine(newRoomGeneration());

        Debug.Log("main level generated");
        Debug.Log("LEVEL GENERATED Y'ALL");

    }

   /* IEnumerator generateLinearRoom()
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

        // SHUFFLE //
        shuffleRoomList();
        // ------- //

        // calcualte all rooms compatible with this node
        List<int> validRooms = getCompatibleRooms(_a);
        Debug.Log(validRooms.Count);
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


    IEnumerator realgenerateOffshootPaths()
    {
        intersectionMagnitudeFactor = 0.75f;
        List<GameObject> roomsToBeChecked = getRemainingRooms();
        for (int i = 0; i < roomsToBeChecked.Count; i++)
        {
            for (int j = 0; j < generatedlevel[i].GetComponent<roomData>().listOfNodes.Count; j++)
            {
                generatedlevel.Add(Instantiate(rooms[2], hostObject.transform));
                GameObject _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0];
                nodeData _a = generatedlevel[i].GetComponent<roomData>().listOfNodes[j].GetComponent<nodeData>();

                float rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                    (_a.gameObject.transform.position.x - _b.transform.position.x),
                    (_a.gameObject.transform.position.y - _b.transform.position.y),
                    (_a.gameObject.transform.position.z - _b.transform.position.z));

                generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");
                generatedlevel[i].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");

                pollIntersection = true;
                yield return new WaitForSeconds(0.1f / 2);
                if (numIntersections > 0)
                {
                    GameObject tbr = generatedlevel[generatedlevel.Count - 1];
                    generatedlevel.RemoveAt(generatedlevel.Count - 1);
                    Destroy(tbr);

                    generatedlevel.Add(Instantiate(doorBlock, hostObject.transform));
                    _b = generatedlevel[generatedlevel.Count - 1];

                    rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                    generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                    generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                    (_a.gameObject.transform.position.x - _b.transform.position.x),
                    (_a.gameObject.transform.position.y - _b.transform.position.y),
                    (_a.gameObject.transform.position.z - _b.transform.position.z));

                }

                pollIntersection = false;
                yield return new WaitForSeconds(0.1f / 2);

            }
        }
        intersectionMagnitudeFactor = 0.25f;
        surface.BuildNavMesh();
        StartCoroutine(spawnPickups());
        spawnMonsters();

        yield return null;

    }*/


    private List<int> isRoomCompatible(nodeData _a, GameObject _room)
    {
        List<int> validIndicies = new List<int>();

        for (int i = 0; i < _room.GetComponent<roomData>().listOfNodes.Count; i++)
        {
            if (isNodeCompatible(_a, _room.GetComponent<roomData>().listOfNodes[i].GetComponent<nodeData>()) && isNodeCompatible(_room.GetComponent<roomData>().listOfNodes[i].GetComponent<nodeData>(), _a))
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

    private List<int> getCompatibleRooms(nodeData _a, bool includeLarge)
    {
        List<int> rtrn = new List<int>();
        if (includeLarge)
        {
            for (int i = 3; i < rooms.Count; i++)
            {
                if (isRoomCompatible(_a, rooms[i]).Count > 0)
                {
                    rtrn.Add(i);
                }
            }
        }
        else
        {
            for (int i = 3; i < 9; i++)
            {
                if (isRoomCompatible(_a, rooms[i]).Count > 0)
                {
                    rtrn.Add(i);
                }
            }
        }
        
        return rtrn;
    }

    private int findNewRoom()
    {
        int rtrn = 0;
        List<int> possibleRooms = new List<int>();
        for (int i = generatedlevel.Count - 1; i >= 0; i--)
        {
            if (generatedlevel[i].GetComponent<roomData>().listOfNodes.Count >= 1)
            {
                possibleRooms.Add(i);
            }
        }
        rtrn = possibleRooms[Random.Range(0, possibleRooms.Count - 1)];
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

    IEnumerator spawnPickups()
    {
        float test = 1.0f;
        for (int i = 0; i < generatedlevel.Count; i++)
        {
            // test to see if cobble spawns
            test = Random.Range(0.0f, 1.0f);
            if (test <= cobbleSpawnRate)
            {
                // instatiate cobble
                float spawnLocX = Random.Range(
                    generatedlevel[i].transform.position.x - generatedlevel[i].transform.lossyScale.x,
                    generatedlevel[i].transform.position.x + generatedlevel[i].transform.lossyScale.x);
                float spawnLocZ = Random.Range(
                    generatedlevel[i].transform.position.z - generatedlevel[i].transform.lossyScale.z,
                    generatedlevel[i].transform.position.z + generatedlevel[i].transform.lossyScale.z);
                Vector3 spawnPos = new Vector3(
                    spawnLocX,
                    generatedlevel[i].transform.position.y + 0.5f,
                    spawnLocZ);
                Instantiate(spawnableItems[0], spawnPos, Quaternion.identity);
            }

            // test to see if inhaler spawns
            test = Random.Range(0.0f, 1.0f);
            if (test <= inhalerSpawnRate)
            {
                // instatiate inhaler
                float spawnLocX = Random.Range(
                    generatedlevel[i].transform.position.x - generatedlevel[i].transform.lossyScale.x,
                    generatedlevel[i].transform.position.x + generatedlevel[i].transform.lossyScale.x);
                float spawnLocZ = Random.Range(
                    generatedlevel[i].transform.position.z - generatedlevel[i].transform.lossyScale.z,
                    generatedlevel[i].transform.position.z + generatedlevel[i].transform.lossyScale.z);
                Vector3 spawnPos = new Vector3(
                    spawnLocX,
                    generatedlevel[i].transform.position.y + 0.5f,
                    spawnLocZ);
                Instantiate(spawnableItems[1], spawnPos, Quaternion.identity);
            }

            // test to see if medkit spawns
            test = Random.Range(0.0f, 1.0f);
            if (test <= medkitSpawnRate)
            {
                // instatiate medkit
                float spawnLocX = Random.Range(
                    generatedlevel[i].transform.position.x - generatedlevel[i].transform.lossyScale.x,
                    generatedlevel[i].transform.position.x + generatedlevel[i].transform.lossyScale.x);
                float spawnLocZ = Random.Range(
                    generatedlevel[i].transform.position.z - generatedlevel[i].transform.lossyScale.z,
                    generatedlevel[i].transform.position.z + generatedlevel[i].transform.lossyScale.z);
                Vector3 spawnPos = new Vector3(
                    spawnLocX,
                    generatedlevel[i].transform.position.y + 0.5f,
                    spawnLocZ);
                Instantiate(spawnableItems[2], spawnPos, Quaternion.identity);
            }
        }
        yield return new WaitForSeconds(0.5f);
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("pickup");
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i].transform.position.y < -100)
            {
                Destroy(pickups[i]);
                pickups = GameObject.FindGameObjectsWithTag("pickup");
                i = 0;
            }
        }


        yield return null;
    }


    void spawnMonsters()
    {
        int numberOfRooms = generatedlevel.Count - 1;
        int spacing = numberOfRooms / gameHandler.numMonsters;

        for (int i = 0; i < gameHandler.numMonsters; i++)
        {
            Instantiate(monster, generatedlevel[numberOfRooms - (spacing * i)].transform.position, Quaternion.identity);
        }
    }

    private void purgeNodes()
    {
        List<List<int>> indiciesToBeRemoved = new List<List<int>>();
        for (int i = 0; i < generatedlevel.Count; i++)
        {
            indiciesToBeRemoved.Add(new List<int>());
        }


        for (int roomA = 0; roomA < generatedlevel.Count; roomA++)
        {
            GameObject _a = generatedlevel[roomA];
            for (int roomB = 0; roomB < generatedlevel.Count; roomB++)
            {
                GameObject _b = generatedlevel[roomB];
                if (_a.GetComponent<roomData>().listOfNodes.Count <= 0 || _b.GetComponent<roomData>().listOfNodes.Count <= 0)
                {
                    roomB = generatedlevel.Count;
                }
                else
                {
                    for (int aNode = 0; aNode < _a.GetComponent<roomData>().listOfNodes.Count; aNode++)
                    {
                        GameObject nodeA = _a.GetComponent<roomData>().listOfNodes[aNode];
                        for (int bNode = 0; bNode < _b.GetComponent<roomData>().listOfNodes.Count; bNode++)
                        {
                            GameObject nodeB = _b.GetComponent<roomData>().listOfNodes[bNode];
                            if (Vector3.Distance(nodeA.transform.position, nodeB.transform.position) < 0.1f)
                            {
                                if (!(indiciesToBeRemoved[roomA].Contains(aNode)))
                                {
                                    indiciesToBeRemoved[roomA].Add(aNode);
                                }
                                if (!(indiciesToBeRemoved[roomB].Contains(bNode)))
                                {
                                    indiciesToBeRemoved[roomB].Add(bNode);
                                }
                            }
                        }

                    }
                }
            }
        }
        Debug.Log("ROOMS WITH NODES TO BE PURGED: " + indiciesToBeRemoved.Count);
        for (int i = 0; i < indiciesToBeRemoved.Count; i++)
        {
            if (indiciesToBeRemoved[i].Count > 1)
            {
                List<GameObject> ltbr = new List<GameObject>();
                for (int j = 0; j < indiciesToBeRemoved[i].Count; j++)
                {
                    ltbr.Add(generatedlevel[i].GetComponent<roomData>().listOfNodes[indiciesToBeRemoved[i][j]]);
                }
                for (int j = 0; j < indiciesToBeRemoved[i].Count; j++)
                {
                    generatedlevel[i].GetComponent<roomData>().listOfNodes.RemoveAt(indiciesToBeRemoved[i][j]);
                }
                while (ltbr.Count != 0)
                {
                    Debug.Log("NODE PURGED");
                    GameObject tbr = ltbr[0];
                    ltbr.RemoveAt(0);
                    Destroy(tbr);
                }
            }
            else if (indiciesToBeRemoved[i].Count == 1)
            {
                Debug.Log("NODE PURGED");
                GameObject tbr = generatedlevel[i].GetComponent<roomData>().listOfNodes[0];
                generatedlevel[i].GetComponent<roomData>().listOfNodes.RemoveAt(0);
                Destroy(tbr);
            }
        }
    }


    IEnumerator newRoomGeneration()
    {
        menuController.loading();
        int trueSeed = 0;
        for (int i = 0; i < seed.Length; i++)
        {
            trueSeed += seed[i];
        }
        Random.InitState(trueSeed);

        while (generatedlevel.Count < magnitude)
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
            int startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count);
            //int startIndex = validStartIndicies[Random.Range(0, validStartIndicies.Count - 1)];
            nodeData _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
            List<int> usedIndicies = new List<int>();

            // SHUFFLE //
            //shuffleRoomList();
            // ------- //

            // calcualte all rooms compatible with this node
            float roomType = Random.Range(0.0f, 100.0f);
            List<int> validRooms;

            validRooms = getCompatibleRooms(_a, false);
            if (roomType >= 70)
            {
                // generate large room
                validRooms = getCompatibleRooms(_a, true);
            }
            else
            {
                // generate corridor / small room 
                validRooms = getCompatibleRooms(_a, false);
            }

            Debug.Log(validRooms.Count);
            bool roomGenerated = false;


            // loop through until room is generated
            while (!roomGenerated)
            {
                // pick a room from the compatible list
                int indexOfRoom = Random.Range(0, validRooms.Count);
                int roomToBeChecked = validRooms[indexOfRoom];

                // make a list of all the nodes in that room that are compatible
                List<int> validNodes = isRoomCompatible(_a, rooms[roomToBeChecked]);

                // loop through all compatible nodes of this room
                while (validNodes.Count > 0 && !roomGenerated)
                {
                    // fix any wierd layers
                    for (int i = 0; i < generatedlevel.Count; i++)
                    {
                        generatedlevel[i].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
                    }

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
                        generatedlevel[generatedlevel.Count - 2].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");
                        generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");
                        if (generatedlevel.Count >= 3) { generatedlevel[generatedlevel.Count - 3].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground"); }

                        //Physics.SyncTransforms();
                        Vector3 multi = new Vector3(generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size.x  *intersectionMultipier, 1.0f, generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size.y * intersectionMultipier);
                        yield return new WaitForFixedUpdate();
                        Collider[] roomsIntersecting = Physics.OverlapBox(generatedlevel[generatedlevel.Count - 1].transform.position, multi, Quaternion.identity, mask);
                        yield return new WaitForFixedUpdate();


                        // check if the new room intersects with any nearby rooms
                        if (roomsIntersecting.Length > 0)
                        {
                            Debug.Log("intersection");
                            // if it is intersecting, destroy the room and remove this node from the valid list
                            GameObject tbr = generatedlevel[generatedlevel.Count - 1];
                            generatedlevel.RemoveAt(generatedlevel.Count - 1);
                            Destroy(tbr);
                            validNodes.RemoveAt(chosenIndex);
                        }
                        else
                        {
                            intersectionMultipier = 1.0f;
                            numberOfFails = 0;
                            /*int chanceOfVerticalCorridor = Random.Range(0, 100);
                            if (chanceOfVerticalCorridor <= 1)
                            {
                                Debug.Log("vertCorridorSpawned");
                                GameObject tbr = generatedlevel[generatedlevel.Count - 1];
                                generatedlevel.RemoveAt(generatedlevel.Count - 1);
                                Destroy(tbr);

                                generatedlevel.Add(Instantiate(rooms[rooms.Count - 1], hostObject.transform));

                                validNodes = new List<int> { 0, 1 };
                                chosenIndex = Random.Range(0, validNodes.Count - 1);
                                _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[chosenIndex];


                                rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                                generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);
                                generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                                    (_a.gameObject.transform.position.x - _b.transform.position.x),
                                    (_a.gameObject.transform.position.y - _b.transform.position.y),
                                    (_a.gameObject.transform.position.z - _b.transform.position.z));

                            }
*/
                            GameObject g12 = generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes[startIndex];
                            GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[chosenIndex]];
                            generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes.RemoveAt(startIndex);
                            generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(validNodes[chosenIndex]);
                            Destroy(g12);
                            Destroy(g13);
                            roomGenerated = true;
 
                        }
                        generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
                        generatedlevel[generatedlevel.Count - 2].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
                        if (generatedlevel.Count >= 3) { generatedlevel[generatedlevel.Count - 3].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection"); }

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
                                validRooms = getCompatibleRooms(_a, false);
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
                            _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0].GetComponent<nodeData>();
                            validRooms = getCompatibleRooms(_a, false);
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
                        _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0].GetComponent<nodeData>();
                        validRooms = getCompatibleRooms(_a, false);
                        startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1);
                    }
                    numberOfFails++;
                }
                if (numberOfFails >= 100)
                {
                    Debug.Log("multiplier enabled");
                    intersectionMultipier = 0.9f;
                }

            }
            waitForRoom = false;
            yield return null;
        }

        // generate ending room
        generatedlevel.Add(Instantiate(rooms[1], hostObject.transform));
        GameObject endNode = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0];
        for (int i = generatedlevel.Count - 2; i >= 0; i--)
        {
            if (generatedlevel[i].GetComponent<roomData>().listOfNodes.Count > 0)
            {
                for (int j = 0; j < generatedlevel[i].GetComponent<roomData>().listOfNodes.Count; j++)
                {
                    float rotDiff = (((generatedlevel[i].GetComponent<roomData>().listOfNodes[j].gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((endNode.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                    generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);
                    generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                        (generatedlevel[i].GetComponent<roomData>().listOfNodes[j].gameObject.transform.position.x - endNode.transform.position.x),
                        (generatedlevel[i].GetComponent<roomData>().listOfNodes[j].gameObject.transform.position.y - endNode.transform.position.y),
                        (generatedlevel[i].GetComponent<roomData>().listOfNodes[j].gameObject.transform.position.z - endNode.transform.position.z));


                    generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");
                    generatedlevel[i].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ground");



                    Vector3 multi = new Vector3(generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size.x * 0.75f, 1.0f, generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size.y * 0.75f);
                    yield return new WaitForFixedUpdate();
                    Collider[] roomsIntersecting = Physics.OverlapBox(generatedlevel[generatedlevel.Count - 1].transform.position, multi, Quaternion.identity, mask);
                    // check if the new room intersects with any nearby rooms
                    if (roomsIntersecting.Length == 0)
                    {
                        // succses
                        
                        GameObject g12 = generatedlevel[i].GetComponent<roomData>().listOfNodes[j];
                        GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0];
                        generatedlevel[i].GetComponent<roomData>().listOfNodes.RemoveAt(j);
                        generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(0);
                        Destroy(g12);
                        Destroy(g13);
                        j = generatedlevel[i].GetComponent<roomData>().listOfNodes.Count - 1;
                        i = generatedlevel[i].GetComponent<roomData>().listOfNodes.Count;
                        Debug.Log("ending room in position");
                    }
                    else
                    {
                        // continue
                    }
                }
            }
        }


        // generate offshootRooms

        List<GameObject> remainingRooms = getRemainingRooms();
        for (int i = 0; i < remainingRooms.Count; i++)
        {
            if (remainingRooms[i].GetComponent<roomData>().listOfNodes.Count > 0)
            {
                for (int j = 0; j < remainingRooms[i].GetComponent<roomData>().listOfNodes.Count; j++)
                {
                    generatedlevel.Add(Instantiate(rooms[2], hostObject.transform));
                    GameObject _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0];
                    nodeData _a = generatedlevel[i].GetComponent<roomData>().listOfNodes[j].GetComponent<nodeData>();

                    float rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                    generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                    generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                        (_a.gameObject.transform.position.x - _b.transform.position.x),
                        (_a.gameObject.transform.position.y - _b.transform.position.y),
                        (_a.gameObject.transform.position.z - _b.transform.position.z));

                    intersectionMultipier = 1.0f;
                    Vector3 multi = new Vector3(generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size.x * intersectionMultipier, 1.0f, generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size.y * intersectionMultipier);
                    yield return new WaitForFixedUpdate();
                    Collider[] roomsIntersecting = Physics.OverlapBox(generatedlevel[generatedlevel.Count - 1].transform.position, multi, Quaternion.identity, mask);
                    if (roomsIntersecting.Length > 0)
                    {
                        generatedlevel[generatedlevel.Count - 1].transform.localScale = new Vector3(1, 1, 0.001f);

                        GameObject g12 = generatedlevel[i].GetComponent<roomData>().listOfNodes[j];
                        GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0];
                        generatedlevel[i].GetComponent<roomData>().listOfNodes.RemoveAt(j);
                        generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(0);
                        Destroy(g12);
                        Destroy(g13);
                    }
                    else
                    {
                        GameObject g12 = generatedlevel[i].GetComponent<roomData>().listOfNodes[j];
                        GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0];
                        generatedlevel[i].GetComponent<roomData>().listOfNodes.RemoveAt(j);
                        generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(0);
                        Destroy(g12);
                        Destroy(g13);
                    }
                    j--;
                }
            }
        }

        // fix any broken layers that may exist because of the intersection testing
        for (int i = 0; i < generatedlevel.Count; i++)
        {
            generatedlevel[i].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
        }




        // build nav mesh
        surface.BuildNavMesh();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().useGravity = true;
        GameObject.FindGameObjectWithTag("Player").transform.position = generatedlevel[0].transform.position;
        GameObject.FindGameObjectWithTag("Player").GetComponent<cameraController>().canMove = true;
        menuController.doneLoading();

        // generate the random pickups
        float test = 1.0f;
        for (int i = 0; i < generatedlevel.Count; i++)
        {
            // test to see if cobble spawns
            test = Random.Range(0.0f, 1.0f);
            if (test <= cobbleSpawnRate)
            {
                // instatiate cobble
                float spawnLocX = Random.Range(
                    generatedlevel[i].transform.position.x - generatedlevel[i].transform.lossyScale.x,
                    generatedlevel[i].transform.position.x + generatedlevel[i].transform.lossyScale.x);
                float spawnLocZ = Random.Range(
                    generatedlevel[i].transform.position.z - generatedlevel[i].transform.lossyScale.z,
                    generatedlevel[i].transform.position.z + generatedlevel[i].transform.lossyScale.z);
                Vector3 spawnPos = new Vector3(
                    spawnLocX,
                    generatedlevel[i].transform.position.y + 0.5f,
                    spawnLocZ);
                Instantiate(spawnableItems[0], spawnPos, Quaternion.identity);
            }

            // test to see if inhaler spawns
            test = Random.Range(0.0f, 1.0f);
            if (test <= inhalerSpawnRate)
            {
                // instatiate inhaler
                float spawnLocX = Random.Range(
                    generatedlevel[i].transform.position.x - generatedlevel[i].transform.lossyScale.x,
                    generatedlevel[i].transform.position.x + generatedlevel[i].transform.lossyScale.x);
                float spawnLocZ = Random.Range(
                    generatedlevel[i].transform.position.z - generatedlevel[i].transform.lossyScale.z,
                    generatedlevel[i].transform.position.z + generatedlevel[i].transform.lossyScale.z);
                Vector3 spawnPos = new Vector3(
                    spawnLocX,
                    generatedlevel[i].transform.position.y + 0.5f,
                    spawnLocZ);
                Instantiate(spawnableItems[1], spawnPos, Quaternion.identity);
            }

            // test to see if medkit spawns
            test = Random.Range(0.0f, 1.0f);
            if (test <= medkitSpawnRate)
            {
                // instatiate medkit
                float spawnLocX = Random.Range(
                    generatedlevel[i].transform.position.x - generatedlevel[i].transform.lossyScale.x,
                    generatedlevel[i].transform.position.x + generatedlevel[i].transform.lossyScale.x);
                float spawnLocZ = Random.Range(
                    generatedlevel[i].transform.position.z - generatedlevel[i].transform.lossyScale.z,
                    generatedlevel[i].transform.position.z + generatedlevel[i].transform.lossyScale.z);
                Vector3 spawnPos = new Vector3(
                    spawnLocX,
                    generatedlevel[i].transform.position.y + 0.5f,
                    spawnLocZ);
                Instantiate(spawnableItems[2], spawnPos, Quaternion.identity);
            }
        }

        // spawn monsters
        int numberOfRooms = generatedlevel.Count - 1;
        int spacing = numberOfRooms / gameHandler.numMonsters;

        for (int i = 0; i < gameHandler.numMonsters; i++)
        {
            Instantiate(monster, generatedlevel[numberOfRooms - (spacing * i)].transform.position, Quaternion.identity);
        }

        Debug.Log("ROOMGENERATED");
    }
}


//Collider[] roomsIntersecting = Physics.OverlapBox(generatedlevel[generatedlevel.Count - 1].transform.position, generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().size, Quaternion.identity, mask);