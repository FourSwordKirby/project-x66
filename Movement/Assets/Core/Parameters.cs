using UnityEngine;
using System.Collections;

public class Parameters : MonoBehaviour {

    public enum Directions
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
        Stop
    };

    public static bool oppositeDirection(Directions dir_1, Directions dir_2)
    {
        switch (dir_1)
        {
            case Directions.North: 
                return (dir_2 == Directions.SouthEast || dir_2 == Directions.South || dir_2 == Directions.SouthWest);
            case Directions.NorthEast: 
                return (dir_2 == Directions.West || dir_2 == Directions.South || dir_2 == Directions.SouthWest);
            case Directions.East: 
                return (dir_2 == Directions.NorthWest || dir_2 == Directions.West || dir_2 == Directions.SouthWest);
            case Directions.SouthEast: 
                return (dir_2 == Directions.NorthWest || dir_2 == Directions.West || dir_2 == Directions.North);
            case Directions.South: 
                return (dir_2 == Directions.NorthEast || dir_2 == Directions.North || dir_2 == Directions.NorthWest);
            case Directions.SouthWest: 
                return (dir_2 == Directions.East || dir_2 == Directions.North || dir_2 == Directions.NorthEast);
            case Directions.West: 
                return (dir_2 == Directions.SouthEast || dir_2 == Directions.East || dir_2 == Directions.NorthEast);
            case Directions.NorthWest: 
                return (dir_2 == Directions.SouthEast || dir_2 == Directions.South || dir_2 == Directions.East);
        }
        return false;
    }

    public static Directions getOppositeDirection(Directions dir)
    {
        switch (dir)
        {
            case Directions.North:
                return Directions.South;
            case Directions.NorthEast:
                return Directions.SouthWest;
            case Directions.East:
                return Directions.West;
            case Directions.SouthEast:
                return Directions.NorthWest;
            case Directions.South:
                return Directions.North;
            case Directions.SouthWest:
                return Directions.NorthEast;
            case Directions.West:
                return Directions.East;
            case Directions.NorthWest:
                return Directions.SouthEast;
        }
        return Directions.Stop;
    }

    //Do we need this?
    public enum PlayerStatus
    {
        Default, //Normal everyday state
        Immovable, //Not affected by forces
        Invulnerable, //Doesn't take damage, can be moved around (reduced knockback?)
        Invincible, //No damageno knockback
        Counter //Can initiate a counter attack
    }
}
