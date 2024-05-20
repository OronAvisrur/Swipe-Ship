using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    private List<Point> currentPathBlocksLocations;
    private List<Point> originalPathBlocksLocations;
    private int shipYPosition;
    private int moves;

    public List<Point> CurrentPathBlocksLocations {get {return currentPathBlocksLocations;} set {currentPathBlocksLocations = value;}}
    public List<Point> OriginalPathBlocksLocations {get {return originalPathBlocksLocations;} set {originalPathBlocksLocations = value;}}
    public int ShipYPosition{get{return shipYPosition;} set{shipYPosition = value;}}
    public int Moves{get{return moves;} set{moves = value;}}

    public GameData(List<Point> currentPathBlocksLocations, List<Point> originalPathBlocksLocations, int shipYPosition, int moves) 
    {
        this.currentPathBlocksLocations = currentPathBlocksLocations;
        this.originalPathBlocksLocations = originalPathBlocksLocations;
        this.shipYPosition = shipYPosition;
        this.moves = moves;
    }
}
