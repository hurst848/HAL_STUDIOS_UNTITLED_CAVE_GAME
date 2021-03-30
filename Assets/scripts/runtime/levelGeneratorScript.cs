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
     *      - index 2 is reserved for the hub room
     *      - index 3 is reserved for a cap room
     */

    public List<GameObject> rooms;

    public string seed;

    public int magnitude = 100;

    private bool levelGenerated = false;

    private List<GameObject> generatedlevel = new List<GameObject>();

    public bool generateLevell = false;

    public NavMeshSurface surface;


    private List<int> roomIndicies = new List<int>();

    void Update()
    {
        if (generateLevell)
        {
            Debug.Log("WORKING");
            generateLevell = false;
            generateLevelLinear();
        }
    }
    /*
        public void generateLevel() // may need to make async
        {
            // Convert seed string to int, then seed the random number generator
            int trueSeed = 0;
            for (int i =0; i<seed.Length; i++)
            {
                trueSeed += seed[i]; 
            }
            Random.InitState(trueSeed);

            // Check to see if level if generated, if it has then throw and execption
            if (!levelGenerated)
            {
                generatedlevel.Add(Instantiate(rooms[2]));
                while (areThereValidNodesRemaining())
                {
                    nodeData _a = getValidNode();
                    bool validRoomSelected = false;
                    while (!validRoomSelected)
                    {
                        int roomToBeChecked = Random.Range(3, rooms.Count);
                        List<int> validNodes = isRoomCompatible(_a, rooms[roomToBeChecked]);
                        if (validNodes.Count > 0)
                        {
                            // generate the correct location and rotation
                            Vector3 pos = new Vector3();
                            Vector3 rot = new Vector3();
                            // instatiate the gameObject
                            generatedlevel.Add(Instantiate(rooms[roomToBeChecked],pos,Quaternion.Euler(rot), generatedlevel[0].transform));

                            //check collisions with other pieces and update other nodes if they share the same location

                            validRoomSelected = true;
                        }
                    }
                }

            }
            else
            {
                throw new System.ArgumentException("LEVEL HAS ALREADY BEEN GENERATED");
            }

        }
    */
    public void generateLevelLinear()
    {
        int trueSeed = 0;
        for (int i = 0; i < seed.Length; i++)
        {
            trueSeed += seed[i];
        }
        Random.InitState(trueSeed);

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
                resetRoomIndicies();
                int startIndex = Random.Range(0, generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1);
                nodeData _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[startIndex].GetComponent<nodeData>();
                bool roomGenerated = false;
                while (!roomGenerated)
                {
                    Debug.Log(roomIndicies.Count - 1);
                    int roomToBeChecked = Random.Range(roomIndicies[0], roomIndicies[roomIndicies.Count - 1]);
                    List<int> validNodes = isRoomCompatible(_a, rooms[roomToBeChecked]);
                    while (validNodes.Count > 0 && !roomGenerated)
                    {
                        validNodes = isRoomCompatible(_a, rooms[roomToBeChecked]);
                        if (validNodes.Count > 0)
                        {
                            generatedlevel.Add(Instantiate(rooms[roomToBeChecked]));
                            int chosenIndex = Random.Range(0, validNodes.Count - 1);
                            GameObject _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[chosenIndex]];
                            // rotate the new room
                            float rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360);
                            generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                            // -----------------------move the room to the right place-------------------//
                            generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(  //   
                                (_a.gameObject.transform.position.x - _b.transform.position.x),          //
                                (_a.gameObject.transform.position.y - _b.transform.position.y),          //
                                (_a.gameObject.transform.position.z - _b.transform.position.z));         //
                            // ------------------------------------------------------------------------- //

                            // ---------check if the new room intersects with anything--------- //
                            if (checkIntersect())                                               //
                            {                                                                   //
                                Debug.Log("node was intersecting");                             //
                                GameObject tbr = generatedlevel[generatedlevel.Count - 1];      //
                                generatedlevel.RemoveAt(generatedlevel.Count - 1);              //
                                Destroy(tbr);                                                   //
                                validNodes.RemoveAt(chosenIndex);                               //
                            }                                                                   //
                            // ---------------------------------------------------------------- //
                            else
                            {
                                // delete used nodes
                                GameObject g12 = generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes[startIndex];
                                GameObject g13 = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[chosenIndex]];
                                generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes.RemoveAt(0);
                                generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(validNodes[chosenIndex]);
                                Destroy(g12);
                                Destroy(g13);
                                roomGenerated = true;
                            }
                        }
                        else
                        {
                            Debug.Log("failed to find a valid node");
                        }

                    }
                    roomIndicies.Remove(roomToBeChecked);
                }
                
            }
        }
        // uncomment when level gen is done
        //surface.BuildNavMesh();
    }
    private void OnDrawGizmos()
    {
        foreach (GameObject g in generatedlevel)
        {
            //Gizmos.DrawCube(g.transform.position, g.transform.GetChild(0).GetComponent<BoxCollider>().bounds.size);
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
            if (generatedlevel[generatedlevel.Count - 1].transform.GetChild(0).GetComponent<BoxCollider>().bounds.Intersects(generatedlevel[i].transform.GetChild(0).GetComponent<BoxCollider>().bounds))
            {
                return true;
            }
        }
        return false;
    }

    private void resetRoomIndicies()
    {
        roomIndicies = new List<int>();
        for (int i = 2; i < rooms.Count -1; i++)
        {
            roomIndicies.Add(i);
        }

    }
}
