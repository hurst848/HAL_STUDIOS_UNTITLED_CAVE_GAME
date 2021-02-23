using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomMap
{
    public List<List<bool>> pathData = new List<List<bool>>();

    public roomMap()
    {
        throw new System.Exception("NOT A VALID INIT");
    }
    public roomMap(Vector2Int _size)
    {
        for (int i = 0; i < _size.y; i++)
        {
            pathData.Add(new List<bool>(_size.x));
        }
    }
}
