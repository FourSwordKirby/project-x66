using UnityEngine;
using System.Collections.Generic;

public interface INavMesh
{
    INavCell getClosestNavCell(Vector3 point);

    List<INavCell> getAllNavCells();
}

public interface INavCell
{
    /// <summary>
    /// Returns the center point for this NavCell
    /// </summary>
    Vector3 center { get; }

    /// <summary>
    /// Returns the reachable neighbors of this NavCell
    /// </summary>
    List<INavCell> neighbors { get; }

    /// <summary>
    ///  Determines if this NavCell is adjacent to the other
    /// </summary>
    bool isAdjacent(INavCell other);

    /// <summary>
    /// Special properties that affect this particular INavCell.
    /// It is up to agents that use path finding to determine if they
    /// want to interact with a particular property in their heuristic.
    /// </summary>
    Dictionary<string, object> properties { get; }
}