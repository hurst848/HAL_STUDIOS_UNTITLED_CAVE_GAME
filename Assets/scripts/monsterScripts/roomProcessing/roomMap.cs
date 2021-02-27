using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomMap
{
    public List<List<bool>> pathData = new List<List<bool>>();
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
            pathData.Add(new List<bool>());
            for (int j = 0; j < _size.y; j++)
            {
                pathData[i].Add(false);
            }
        }
    }
}
