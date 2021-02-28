using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomMap
{
    public List<List<int>> soundPathData = new List<List<int>>();
    public List<List<int>> sightPathData = new List<List<int>>();
    public List<List<int>> touchPathData = new List<List<int>>();
    /*
     * 
     *  true    = open space
     * false    = closed space
     * 
     */
    public roomMap()
    {
        throw new System.Exception("NOT A VALID INIT");
    }
    public roomMap(Vector2Int _size)
    {
        for (int i = 0; i < _size.x; i++)
        {
            soundPathData.Add(new List<int>());
            sightPathData.Add(new List<int>());
            touchPathData.Add(new List<int>());
            for (int j = 0; j < _size.y; j++)
            {
                soundPathData[i].Add(0);
                sightPathData[i].Add(0);
                touchPathData[i].Add(0);
            }
        }
    }
}
