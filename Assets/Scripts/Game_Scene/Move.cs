using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private int x;
    private int y;
    private int direction;

    public int X {get {return x;} set {x = value;} }
    public int Y {get {return y;} set {y = value;} }
    public int Direction {get {return direction;} set {direction = value;} }

    public Move(int x, int y, int direction)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
    }
}
