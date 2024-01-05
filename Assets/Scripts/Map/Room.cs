using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SetOperations;

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
    None,Style1,
}

public class Room
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
    public bool CheckIntersection(Room other)
    {
        return positionRoomTiles.Overlaps(other.GetTilesPos());
    }
    //Функции проверки что эта комната входит в другую
    public bool IsSubsetOf(Room other)
    {
        return positionRoomTiles.IsSubsetOf(other.GetTilesPos());
    }
    public bool IsSupersetOf(Room other)
    {
        return positionRoomTiles.IsSupersetOf(other.GetTilesPos());
    }
    //Функции проверки что в эту комнату входит другая
    public bool IsProperSubsetOf(Room other)
    {
        return positionRoomTiles.IsProperSubsetOf(other.GetTilesPos());
    }
    public bool IsProperSupersetOf(Room other)
    {
        return positionRoomTiles.IsProperSupersetOf(other.GetTilesPos());
    }
    //Операция пересечения с другой комнатой
    public void Intersect(Room other)
    {
        positionRoomTiles.IntersectWith(other.GetTilesPos());
    }
    //Операция объединения с другой комнатой
    public void Union(Room other)
    {
        positionRoomTiles.UnionWith(other.GetTilesPos());
    }
    //Операция разности с другой комнатой
    public void Difference(Room other)
    {
        positionRoomTiles.ExceptWith(other.GetTilesPos());
    }
    //Операция симметричной разности с другой комнатой
    public void SymmetricDifference(Room other)
    {
        positionRoomTiles.SymmetricExceptWith(other.GetTilesPos());
    }

    public bool CheckConnection(Room other)
    {
        return Validate(other.GetTilesPos());
    }

    public void DoOperation(Room other, SetOperations.Operations operation)
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
    public bool TryOperation(Room other, SetOperations.Operations operation)
    {
        Room roomTest = this;
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

    public SetOperations.Operations TryAllOperations(Room other)
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

    public SetOperations.Operations TryAllSubOperations(Room other)
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
    public bool Validate()
    {

        if (positionRoomTiles.Count < 8)
            return false;

        List<Vector2Int> OpenStates = new List<Vector2Int>();
        List<Vector2Int> ClosedStates = new List<Vector2Int>();
        if (GetTilesPos().Count == 0)
            return false;
        OpenStates.Add(GetTilesPos().First());
        while (OpenStates.Count > 0)
        {
            Vector2Int currentPos = OpenStates.First();
            OpenStates.Remove(OpenStates.First());
            ClosedStates.Add(currentPos);
            List<Vector2Int> newPoses = Direction2D.GetNewCardinalPosesFromPos(currentPos);

            if (newPoses.Count == 0 && OpenStates.Count != 0)
                return false;

            foreach (var pos in newPoses)
            {
                if (!(OpenStates.Contains(pos) || ClosedStates.Contains(pos)) && GetTilesPos().Contains(pos))
                {
                    OpenStates.Add(pos);
                }
            }
        }
        if (ClosedStates.Count == GetTilesPos().Count)
            return true;

        return false;
    }
    public bool Validate(HashSet<Vector2Int> tilesPos)
    {
        HashSet<Vector2Int> tilesPosTest = new HashSet<Vector2Int>(this.GetTilesPos());
        tilesPosTest.UnionWith(tilesPos);
        if(tilesPosTest.Count<16)
            return false;

        List<Vector2Int> OpenStates = new List<Vector2Int>();
        List<Vector2Int> ClosedStates = new List<Vector2Int>();
        if (tilesPosTest.Count == 0)
            return false;
        OpenStates.Add(tilesPosTest.First());
        while (OpenStates.Count > 0)
        {
            Vector2Int currentPos = OpenStates.First();
            OpenStates.Remove(OpenStates.First());
            ClosedStates.Add(currentPos);
            List<Vector2Int> newPoses = Direction2D.GetNewCardinalPosesFromPos(currentPos);

            if (newPoses.Count == 0 && OpenStates.Count != 0)
                return false;

            foreach (var pos in newPoses)
            {
                if (!(OpenStates.Contains(pos) || ClosedStates.Contains(pos)) && tilesPosTest.Contains(pos))
                {
                    OpenStates.Add(pos);
                }
            }
        }
        if(ClosedStates.Count == tilesPosTest.Count)
        return true;

        return false;
    }
}