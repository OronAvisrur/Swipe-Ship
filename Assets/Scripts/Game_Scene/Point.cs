using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int X {get; set;}
    public int Y {get; set;}

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
