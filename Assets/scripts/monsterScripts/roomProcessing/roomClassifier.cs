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
    public GameObject scanner;

    public int scannerResolution;
    public Vector3 centre;
    public Vector3 size;

    public bool showBounds = true;
    public bool runScan = false;
    public bool running = false;

    private roomMap mappedDataTouch;
    private roomMap mappedDataSight;
    private roomMap mappedDataSound;

    private float touchHeight = 1.75f;
    private float sightHeight = 1.75f;
    private float soundHeight = 1.75f;

    private scanColliosionCheck scanChecker;
    private Vector3 _startLoc = new Vector3();

    private List<Vector3> boundsLocations = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        // set the scanner resolution
        // RESOLUTION MUST BE THE SAME FOR ALL USES 
        scanner.transform.localScale = new Vector3(1.0f / scannerResolution, 1.0f / scannerResolution, 1.0f / scannerResolution);

        // GENERATE THE BOUNDS 
        boundsLocations.Add(centre + new Vector3(size.x / 2, size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, -size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, -size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, -size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, -size.y / 2, size.z / 2));

        scanner.transform.position = new Vector3(
            boundsLocations[0].x - (1.0f / scannerResolution) / 2, 
            boundsLocations[1].y + (1.0f / scannerResolution) / 2, 
            boundsLocations[0].z + (1.0f / scannerResolution) / 2);
        _startLoc = scanner.transform.position;
        // set the scanner in the start location

        // get the collider of the scanner 
        scanChecker = scanner.GetComponent<scanColliosionCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        if(showBounds)
        {
            Debug.DrawLine(boundsLocations[0], boundsLocations[1], Color.red);
            Debug.DrawLine(boundsLocations[0], boundsLocations[6], Color.red);
            Debug.DrawLine(boundsLocations[0], boundsLocations[3], Color.red);
            Debug.DrawLine(boundsLocations[1], boundsLocations[2], Color.red);
            Debug.DrawLine(boundsLocations[1], boundsLocations[7], Color.red);
            Debug.DrawLine(boundsLocations[2], boundsLocations[3], Color.red);
            Debug.DrawLine(boundsLocations[2], boundsLocations[5], Color.red);
            Debug.DrawLine(boundsLocations[3], boundsLocations[4], Color.red);
            Debug.DrawLine(boundsLocations[4], boundsLocations[5], Color.red);
            Debug.DrawLine(boundsLocations[4], boundsLocations[6], Color.red);
            Debug.DrawLine(boundsLocations[5], boundsLocations[7], Color.red);
            Debug.DrawLine(boundsLocations[6], boundsLocations[7], Color.red);
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
        Vector2Int relSize = new Vector2Int((int)(size.z * scannerResolution), (int)(size.x * scannerResolution));
        mappedDataSight = new roomMap(relSize);
        mappedDataTouch = new roomMap(relSize);
        mappedDataSound = new roomMap(relSize);
        for (int i = 0; i < relSize.x; i++)
        {

            for (int j = 0; j < relSize.y; j++)
            {

                float sightItr = 0.0f;
                float soundItr = 0.0f;
                float touchItr = 0.0f;

                for (int h = 0; h < size.y * scannerResolution; h++)
                {

                    sightItr += scannerResolution;
                    soundItr += scannerResolution;
                    touchItr += scannerResolution;

                    if (sightItr >= touchHeight)
                    {
                        mappedDataSight.pathData[i][j] = true;
                    }
                    if (touchItr >= touchHeight)
                    {
                        mappedDataSound.pathData[i][j] = true;
                    }
                    if (soundItr >= soundHeight)
                    {
                        mappedDataTouch.pathData[i][j] = true;
                    }
                    if (scanChecker.obstacleDetected == true)
                    {
                        sightItr = 0.0f; soundItr = 0.0f; touchItr = 0.0f;
                    }
                    scanner.transform.position = new Vector3(
                        scanner.transform.position.x,
                        scanner.transform.position.y + (1.0f / scannerResolution),
                        scanner.transform.position.z);

                }

                scanner.transform.position = new Vector3(
                    scanner.transform.position.x - (1.0f / scannerResolution),
                    _startLoc.y,
                    scanner.transform.position.z);

            }

            scanner.transform.position = new Vector3(
                    _startLoc.x,
                    _startLoc.y,
                    scanner.transform.position.z - (1.0f / scannerResolution));

        }


        string soundPath = "Assets/scripts/monsterScripts/roomProcessing/generatedPaths/" + roomID + "sound.txt";
        string sightPath = "Assets/scripts/monsterScripts/roomProcessing/generatedPaths/" + roomID + "sight.txt";
        string touchPath = "Assets/scripts/monsterScripts/roomProcessing/generatedPaths/" + roomID + "touch.txt";
        StreamWriter ioOutSound = new StreamWriter(soundPath, true);
        StreamWriter ioOutSight = new StreamWriter(sightPath, true);
        StreamWriter ioOutTouch = new StreamWriter(touchPath, true);

        for (int i = 0; i < relSize.x; i++)
        {
            for (int j = 0; i < relSize.y; j++)
            {
                ioOutSight.Write(mappedDataSight.pathData[i][j].ToString() + " ");
                ioOutSound.Write(mappedDataSound.pathData[i][j].ToString() + " ");
                ioOutTouch.Write(mappedDataTouch.pathData[i][j].ToString() + " ");
            }
        }
    }
}
