using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class roomClassifier : MonoBehaviour
{
    /*  TODO:
     *      - add movement intervals to the scanner
     *      - export to file once generated
     */




    public string roomID;
    public GameObject room;

    public int scannerResolution;
    public Vector3 centre;
    public Vector3 size;

    public bool showBounds = true;
    public bool runScan = false;
    public bool running = false;

    private roomMap mappedData;

    private float touchHeight = 1.75f;
    private float sightHeight = 1.75f;
    private float soundHeight = 1.75f;

  

    private List<Vector3> boundsLocations = new List<Vector3>();
    private float relScale;
    // Start is called before the first frame update
    void Start()
    {

        // set the scanner resolution
        // RESOLUTION MUST BE THE SAME FOR ALL USES 
        relScale = 1.0f / scannerResolution;

        // GENERATE THE BOUNDS 
        boundsLocations.Add(centre + new Vector3(-size.x / 2, -size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, -size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, -size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, -size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, size.y / 2, size.z / 2));

        
        // set the scanner in the start location

        // get the collider of the scanner 
        
    }

    // Update is called once per frame
    void Update()
    {
        if(showBounds)
        {
            Debug.DrawLine(boundsLocations[0], boundsLocations[1], Color.red);
            Debug.DrawLine(boundsLocations[1], boundsLocations[2], Color.red);
            Debug.DrawLine(boundsLocations[2], boundsLocations[3], Color.red);
            Debug.DrawLine(boundsLocations[3], boundsLocations[0], Color.red);
            Debug.DrawLine(boundsLocations[4], boundsLocations[5], Color.red);
            Debug.DrawLine(boundsLocations[5], boundsLocations[6], Color.red);
            Debug.DrawLine(boundsLocations[6], boundsLocations[7], Color.red);
            Debug.DrawLine(boundsLocations[7], boundsLocations[4], Color.red);
            Debug.DrawLine(boundsLocations[0], boundsLocations[4], Color.red);
            Debug.DrawLine(boundsLocations[1], boundsLocations[5], Color.red);
            Debug.DrawLine(boundsLocations[2], boundsLocations[6], Color.red);
            Debug.DrawLine(boundsLocations[3], boundsLocations[7], Color.red);
        }
        if (!running && runScan)
        {
            runScan = false;
            running = true;
            scanRoom();
            // execute scan
        }
      
    }

    private void scanRoom()
    {
        Vector3 checkLoc = new Vector3(boundsLocations[0].x + relScale, boundsLocations[0].y + relScale, boundsLocations[0].z + relScale);
        Vector3 startLoc = checkLoc;

        Vector2Int gridSize = new Vector2Int(Mathf.RoundToInt(size.x / relScale), Mathf.RoundToInt(size.z / relScale));
        mappedData = new roomMap(gridSize);

        for (int x = 0; x < gridSize.x; x++)
        {

            for (int y = 0; y < gridSize.y; y++)
            {

                float sightItr = 0.0f;
                float soundItr = 0.0f;
                float touchItr = 0.0f;

                for (int h = 0; h < Mathf.RoundToInt(size.y / relScale); h++)
                {

                    if (sightItr >= sightHeight) { mappedData.sightPathData[x][y] = 1; }
                    if (touchItr >= touchHeight) { mappedData.touchPathData[x][y] = 1; }
                    if (soundItr >= soundHeight) { mappedData.soundPathData[x][y] = 1; }
                    if (Physics.CheckBox(checkLoc, new Vector3(relScale/2,relScale/2,relScale/2)))
                    {
                        sightItr = 0.0f; 
                        soundItr = 0.0f; 
                        touchItr = 0.0f;
                    }
                    else
                    {
                        sightItr += relScale; 
                        soundItr += relScale; 
                        touchItr += relScale;
                    }

                    checkLoc = new Vector3(
                        checkLoc.x,
                        checkLoc.y + relScale,
                        checkLoc.z);

                }

                checkLoc = new Vector3(
                    checkLoc.x,
                    startLoc.y,
                    checkLoc.z + relScale);

            }

            checkLoc = new Vector3(
                checkLoc.x + relScale,
                startLoc.y,
                startLoc.z);
        }
        Debug.Log("SCAN SUCCSEFULLY COMPLETED");
        running = false;

        string path = "Assets/prefabs/rooms/" + roomID + ".txt";
        StreamWriter writer = new StreamWriter(path, true);
        for(int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0; j < gridSize.y; j++)
            {
                writer.Write(mappedData.sightPathData[i][j].ToString());
            }
        }
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                writer.Write(mappedData.soundPathData[i][j].ToString());
            }
        }
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                writer.Write(mappedData.touchPathData[i][j].ToString());
            }
        }

        writer.Close();

    }
}
