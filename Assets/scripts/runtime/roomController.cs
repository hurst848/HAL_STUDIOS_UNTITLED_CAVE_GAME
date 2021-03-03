using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomController : MonoBehaviour
{

    public List<GameObject> rooms;
    public int numRooms;


    private roomClassifier pathClassifier;
    private uint seed;
    // Start is called before the first frame update
    void Start()
    {
        
        seed = (uint)(Random.Range(0, int.MaxValue));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void generateMap()
    {

    }


    private int interopalateRoomFromNoise()
    {
        int roomIndex = 0;


        return roomIndex;
    }
}
