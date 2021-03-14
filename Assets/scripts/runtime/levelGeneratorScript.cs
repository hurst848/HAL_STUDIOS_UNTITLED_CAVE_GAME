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


    private class levelSegment
    {
        public int numberNodes = 0;
        public GameObject room;
        public levelSegment(GameObject _g)
        {
            room = _g;
            numberNodes = room.transform.childCount - 1;
        }
    }

    public List<GameObject> rooms;

    public string seed;
   
    public int magnitude = 100;

    private List<levelSegment> segments;
    private List<levelSegment> level;

    private bool levelGenerated = false;


    void Start()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            segments.Add(new levelSegment(rooms[i]));
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
            level.Add(new levelSegment(Instantiate(segments[2].room, transform)));
            bool levelFinished = false;
            while (!levelFinished)
            {
                // choose empty node -------------------------------------------------//
                int currentRoom = getValidNodeIndex();                                //
                GameObject currentNode = level[currentRoom].room.transform.GetChild(  //  
                    Random.Range(1,level[currentRoom].numberNodes)).gameObject;       //
                // ------------------------------------------------------------------ //

                // check if node is within magnitude ------------------------------------------------------------ //
                    // if it is, generate as normal                                                               //          
                if (Mathf.Abs(Vector3.Distance(currentNode.transform.position, transform.position)) < magnitude)  //
                {
                    int newRoomId = Random.Range(4, segments.Count - 1);
                    level.Add(new levelSegment(Instantiate(segments[newRoomId].room, transform)));
                    float requiredRot = currentNode.transform.eulerAngles.y  - 180;
                    float nodeRot  = level[level.Count - 1].room.transform.GetChild(1).transform.eulerAngles.y;
                    // rotate room to fit

                    // move room to loaction

                    // check validity
                }
                    // else use either the start, end or cap rooms
                else
                {
                    
                }
                // ---------------------------------------------------------------------------------------------- //
                
                
            }
        }
        else
        {
            throw new System.ArgumentException("LEVEL HAS ALREADY BEEN GENERATED");
        }

    }

    private int getValidNodeIndex()
    {
        for (int i = 0; i < level.Count; i++)
        {
            if (level[i].numberNodes != 0)
            {
                return i;
            }
        }
        return -1;
    }

    

}
