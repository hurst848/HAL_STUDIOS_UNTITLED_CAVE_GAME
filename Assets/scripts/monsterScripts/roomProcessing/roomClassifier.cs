using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomClassifier : MonoBehaviour
{
    /*  TODO:
     *      - add movement intervals to the scanner
     *      - export to file once generated
     */






    public GameObject scanner;

    public float scannerResolution;
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

    private List<Vector3> boundsLocations = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        // set the scanner resolution
        // RESOLUTION MUST BE THE SAME FOR ALL USES 
        scanner.transform.localScale = new Vector3(scannerResolution, scannerResolution, scannerResolution);

        // GENERATE THE BOUNDS 
        boundsLocations.Add(centre + new Vector3(size.x / 2, size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, -size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, -size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, size.y / 2, -size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(-size.x / 2, -size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, size.y / 2, size.z / 2));
        boundsLocations.Add(centre + new Vector3(size.x / 2, -size.y / 2, size.z / 2));

        scanner.transform.position = new Vector3(boundsLocations[0].x - scannerResolution/2, boundsLocations[1].y + scannerResolution / 2, boundsLocations[0].z + scannerResolution / 2);
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
            // execute scan
        }
      
    }

    private void scanRoom()
    {
        Vector2Int relSize = new Vector2Int((int)(size.z / scannerResolution), (int)(size.x / scannerResolution));
        mappedDataSight = new roomMap(relSize);
        mappedDataTouch = new roomMap(relSize);
        mappedDataSound = new roomMap(relSize);
        for (int i = 0; i < size.z / scannerResolution; i++)
        {
            for (int j = 0; j < size.x / scannerResolution; j++)
            {
                float sightItr = 0.0f;
                float soundItr = 0.0f;
                float touchItr = 0.0f;
                for (int h = 0; h < size.y / scannerResolution; h++)
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
                    
                }
            }
        }
    }
}
