using System.Collections.Generic; using UnityEngine; 
//Возможные операции над комнатами (смтр теорию по множествам) public enum Operations {     Intersect,     Union,     Difference,     SymmetricDifference, } 
//Возможные стили (если сделаешь класс то у меня есть пару идей как это все подвязать а пока будет enum) public enum Styles {     Style1, }  public abstract class RoomBase {
    //Позиции центра комнаты (в прямом смысле центра; правда если условно размер 2x2 то центр там будет смещен :D)     protected Vector2Int positionCenter;
    //Позиции тайлов     protected HashSet<Vector2Int> positionRoomTiles;     protected Styles style; 
    //Функции с получением изменением позиции     public Vector2Int GetPos()     {         return positionCenter;     }     public void SetPos(int x, int y)     {         positionCenter = new Vector2Int(x, y);     }     public void SetPos(Vector2Int pos)     {         positionCenter = pos;     }     //Функции с получением изменением стиля
    public Styles GetStyle()     {         return style;     }     public void SetStyle(Styles style)     {         this.style = style;     }
    //Функции с получением изменением позиции тайлов
    public HashSet<Vector2Int> GetTilesPos()     {         return positionRoomTiles;     }     public void SetTilesPos(HashSet<Vector2Int> positions)     {         positionRoomTiles = positions;     }
    //Функция проверки пересечения
    public bool CheckIntersection(RoomBase other)     {         //return positionRoomTiles.Intersect(other.GetTilesPos()).Any();         return positionRoomTiles.Overlaps(other.GetTilesPos());     }     //Функции проверки что эта комната входит в другую
    public bool IsSubsetOf(RoomBase other)     {         return positionRoomTiles.IsSubsetOf(other.GetTilesPos());     }     public bool IsSupersetOf(RoomBase other)     {         return positionRoomTiles.IsSupersetOf(other.GetTilesPos());     }
    //Функции проверки что в эту комнату входит другая
    public bool IsProperSubsetOf(RoomBase other)     {         return positionRoomTiles.IsProperSubsetOf(other.GetTilesPos());     }     public bool IsProperSupersetOf(RoomBase other)     {         return positionRoomTiles.IsProperSupersetOf(other.GetTilesPos());     }
    //Операция пересечения с другой комнатой     public void Intersect(RoomBase other)     {         positionRoomTiles.IntersectWith(other.GetTilesPos());     }     //Операция объединения с другой комнатой
    public void Union(RoomBase other)     {         positionRoomTiles.UnionWith(other.GetTilesPos());     }
    //Операция разности с другой комнатой
    public void Difference(RoomBase other)     {         positionRoomTiles.ExceptWith(other.GetTilesPos());     }
    //Операция симметричной разности с другой комнатой
    public void SymmetricDifference(RoomBase other)     {         positionRoomTiles.SymmetricExceptWith(other.GetTilesPos());     } 
    //Функция валидации для реализации наследниками обязательна     public abstract bool Validate(); }