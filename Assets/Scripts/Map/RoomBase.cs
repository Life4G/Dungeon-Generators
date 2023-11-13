using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Operations
{
    Intersect,
    Union,
    Difference,
    SymmetricDifference,
}

public enum Styles
{
    Style1,
}

public abstract class RoomBase
{
    protected Vector2Int positionCenter;
    protected HashSet<Vector2Int> positionRoomTiles;
    protected Styles style;

    public Vector2Int GetPos()
    {
        return positionCenter;
    }
    public void SetPos(int x, int y)
    {
        positionCenter = new Vector2Int(x, y);
    }
    public void SetPos(Vector2Int pos)
    {
        positionCenter = pos;
    }
    public Styles GetStyle()
    {
        return style;
    }
    public void SetStyle(Styles style)
    {
        this.style = style;
    }
    public HashSet<Vector2Int> GetTilesPos()
    {
        return positionRoomTiles;
    }
    public void SetTilesPos(HashSet<Vector2Int> positions)
    {
        positionRoomTiles = positions;
    }
    public bool CheckIntersection(RoomBase other)
    {
        //return positionRoomTiles.Intersect(other.GetTilesPos()).Any();
        return positionRoomTiles.Overlaps(other.GetTilesPos());
    }
    public bool IsSubsetOf(RoomBase other)
    {
        return positionRoomTiles.IsSubsetOf(other.GetTilesPos());
    }
    public bool IsSupersetOf(RoomBase other)
    {
        return positionRoomTiles.IsSupersetOf(other.GetTilesPos());
    }
    public bool IsProperSubsetOf(RoomBase other)
    {
        return positionRoomTiles.IsProperSubsetOf(other.GetTilesPos());
    }
    public bool IsProperSupersetOf(RoomBase other)
    {
        return positionRoomTiles.IsProperSupersetOf(other.GetTilesPos());
    }
    public void Intersect(RoomBase other)
    {
        positionRoomTiles.IntersectWith(other.GetTilesPos());
    }
    public void Union(RoomBase other)
    {
        positionRoomTiles.UnionWith(other.GetTilesPos());
    }
    public void Difference(RoomBase other)
    {
        positionRoomTiles.ExceptWith(other.GetTilesPos());
    }
    public void SymmetricDifference(RoomBase other)
    {
        positionRoomTiles.SymmetricExceptWith(other.GetTilesPos());
    }

    public abstract bool Validate();
}