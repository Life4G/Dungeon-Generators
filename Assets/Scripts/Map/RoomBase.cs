using System.Collections.Generic;
using UnityEngine;
using static SetOperations;

//Возможные операции над комнатами (смтр теорию по множествам)
public static class SetOperations
{
    public enum Operations
    {
        Intersect,
        Union,
        Difference,
        SymmetricDifference,
        None
    }
    public static Operations GetRandomOperation()
    {
        int random = Random.Range(0, 4);
        Operations operation = Operations.None;
        switch (random)
        {
            case 0:
                operation = Operations.Intersect;
                break;
            case 1:
                operation = Operations.Union;
                break;
            case 2:
                operation = Operations.Difference;
                break;
            case 3:
                operation = Operations.SymmetricDifference;
                break;
        }
        return operation;
    }
    public static Operations GetRandomSubOperation()
    {
        int random = Random.Range(0, 2);
        Operations operation = Operations.None;
        switch (random)
        {
            case 0:
                operation = Operations.Union;
                break;
            case 1:
                operation = Operations.Difference;
                break;
        }
        return operation;
    }
    public static List<Operations> GetOperationsList = new List<Operations>
    {
        Operations.Intersect,
        Operations.Union,
        Operations.Difference,
        Operations.SymmetricDifference

    };
    public static List<Operations> GetSubOperationsList = new List<Operations>
    {
        Operations.Union,
        Operations.Difference,
    };

}

//Возможные стили (если сделаешь класс то у меня есть пару идей как это все подвязать а пока будет enum)
public enum Styles
{
    Style1,
}

public abstract class RoomBase
{
    //Позиции центра комнаты (в прямом смысле центра; правда если условно размер 2x2 то центр там будет смещен :D)
    protected Vector2Int positionCenter;
    //Позиции тайлов
    protected HashSet<Vector2Int> positionRoomTiles;
    protected Styles style;

    //Функции с получением изменением позиции
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
    //Функции с получением изменением стиля
    public Styles GetStyle()
    {
        return style;
    }
    public void SetStyle(Styles style)
    {
        this.style = style;
    }
    //Функции с получением изменением позиции тайлов
    public HashSet<Vector2Int> GetTilesPos()
    {
        return positionRoomTiles;
    }
    public void SetTilesPos(HashSet<Vector2Int> positions)
    {
        positionRoomTiles = positions;
    }
    //Функция проверки пересечения
    public bool CheckIntersection(RoomBase other)
    {
        //return positionRoomTiles.Intersect(other.GetTilesPos()).Any();
        return positionRoomTiles.Overlaps(other.GetTilesPos());
    }
    //Функции проверки что эта комната входит в другую
    public bool IsSubsetOf(RoomBase other)
    {
        return positionRoomTiles.IsSubsetOf(other.GetTilesPos());
    }
    public bool IsSupersetOf(RoomBase other)
    {
        return positionRoomTiles.IsSupersetOf(other.GetTilesPos());
    }
    //Функции проверки что в эту комнату входит другая
    public bool IsProperSubsetOf(RoomBase other)
    {
        return positionRoomTiles.IsProperSubsetOf(other.GetTilesPos());
    }
    public bool IsProperSupersetOf(RoomBase other)
    {
        return positionRoomTiles.IsProperSupersetOf(other.GetTilesPos());
    }
    //Операция пересечения с другой комнатой
    public void Intersect(RoomBase other)
    {
        positionRoomTiles.IntersectWith(other.GetTilesPos());
    }
    //Операция объединения с другой комнатой
    public void Union(RoomBase other)
    {
        positionRoomTiles.UnionWith(other.GetTilesPos());
    }
    //Операция разности с другой комнатой
    public void Difference(RoomBase other)
    {
        positionRoomTiles.ExceptWith(other.GetTilesPos());
    }
    //Операция симметричной разности с другой комнатой
    public void SymmetricDifference(RoomBase other)
    {
        positionRoomTiles.SymmetricExceptWith(other.GetTilesPos());
    }

    public void DoOperation(RoomBase other, SetOperations.Operations operation)
    {
        switch(operation)
        {
            case SetOperations.Operations.Intersect:
                Intersect(other);
                break;
            case SetOperations.Operations.Union:
                Union(other);
                break;
            case SetOperations.Operations.Difference:
                Difference(other);
                break;
            case SetOperations.Operations.SymmetricDifference:
                SymmetricDifference(other);
                break;
        }
    }
    public bool TryOperation(RoomBase other, SetOperations.Operations operation)
    {
        RoomBase roomTest = this;
        switch (operation)
        {
            case SetOperations.Operations.Intersect:
                roomTest.Intersect(other);
                break;
            case SetOperations.Operations.Union:
                roomTest.Union(other);
                break;
            case SetOperations.Operations.Difference:
                roomTest.Difference(other);
                break;
            case SetOperations.Operations.SymmetricDifference:
                roomTest.SymmetricDifference(other);
                break;
        }
        return roomTest.Validate();
    }

    public SetOperations.Operations TryAllOperations(RoomBase other)
    {
        foreach (SetOperations.Operations op in GetOperationsList)
        {
            if (TryOperation(other, op))
            {
                return op;
            }
        }
        return Operations.None;
    }

    public SetOperations.Operations TryAllSubOperations(RoomBase other)
    {
        foreach (SetOperations.Operations op in GetSubOperationsList)
        {
            if (TryOperation(other, op))
            {
                return op;
            }
        }
        return Operations.None;
    }
    public abstract bool Validate();
}