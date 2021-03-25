using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        List<int> test = new List<int>();
        test.Add(1);
        test.Add(1);
        test.Add(1);
        test.Add(1);

        Debug.Log(test.Count);
    }

    void Update()
    {
        if (generateLevell)
        {
            Debug.Log("WORKING");
            generateLevell = false;
            generateLevelLinear();
        }
    }

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

    public void generateLevelLinear()
    {
        int trueSeed = 0;
        for (int i = 0; i < seed.Length; i++)
        {
            trueSeed += seed[i];
        }
        Random.InitState(trueSeed);

        if (!levelGenerated)
        {
            generatedlevel.Add(Instantiate(rooms[0]));
            for (int i = 0; i < magnitude - 1; i++)
            {
                nodeData _a = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[0].GetComponent<nodeData>();
                int roomToBeChecked = Random.Range(4, rooms.Count - 1);
                List<int> validNodes = isRoomCompatible(_a, rooms[roomToBeChecked]);
                if (validNodes.Count > 0)
                {
                    generatedlevel.Add(Instantiate(rooms[roomToBeChecked]));
                    GameObject _b = generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes[validNodes[Random.Range(0, validNodes.Count - 1)]];
                    //Random.Range(0,generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.Count - 1)
                    // rotate the new room
                    float rotDiff = (((_a.gameObject.transform.rotation.eulerAngles.y - 180) + 360) % 360) - (((_b.gameObject.transform.rotation.eulerAngles.y) + 360) % 360); 
                    generatedlevel[generatedlevel.Count - 1].transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y + rotDiff, transform.eulerAngles.z);

                    // move the room to the right place
                    generatedlevel[generatedlevel.Count - 1].transform.position += new Vector3(
                        (_a.gameObject.transform.position.x - _b.transform.position.x),
                        (_a.gameObject.transform.position.y - _b.transform.position.y),
                        (_a.gameObject.transform.position.z - _b.transform.position.z));

                    // delete used nodes
                    generatedlevel[generatedlevel.Count - 2].GetComponent<roomData>().listOfNodes.RemoveAt(0);
                    generatedlevel[generatedlevel.Count - 1].GetComponent<roomData>().listOfNodes.RemoveAt(validNodes[0]);
                }
                else
                {
                    i--;
                }
            }
        }
    }



    private bool areThereValidNodesRemaining()
    {
        // checks the generatedLevel list for nodes that do not have rooms on
        for (int i = 0; i < generatedlevel.Count; i++)
        {
            for (int j = 0; j < generatedlevel[i].GetComponent<roomData>().listOfNodes.Count; j++)
            {
                if (!generatedlevel[i].GetComponent<roomData>().listOfNodes[j].GetComponent<nodeData>().connectedWithAnotherNode)
                {
                    return true;
                }
            }
        }
        return false;
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

        for(int i = 0; i < generatedlevel.Count; i++)
        {
            if (!(generatedlevel[i].GetComponent<roomData>().listOfNodes.Count <= 0))
            {
                _a = generatedlevel[i].GetComponent<roomData>().listOfNodes[0].GetComponent<nodeData>();
            }
        }

        return _a;
    }

   

}
