using Assets.Scripts.Room;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Assets.Scripts.Map;
using TMPro;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private RoomStyleManager roomStyleManager;

    [SerializeField]
    public DungeonGeneratorBase generator;

    [SerializeField]
    public Tilemap tilemap;

    private RoomStyle currentRoomStyle;

    [SerializeField]
    private DungeonRoomManager roomManager;

    private DungeonMap map;

    /// <summary>
    /// Устанавливает текущий стиль комнаты, используя имя стиля.
    /// </summary>
    /// <param name="styleName">Имя стиля комнаты.</param>
    public void SetRoomStyle(string styleName)
    {
        currentRoomStyle = roomStyleManager.GetRoomStyle(styleName);
    }

    /// <summary>
    /// Устанавливает текущий стиль комнаты, используя индекс стиля.
    /// </summary>
    /// <param name="styleIndex">Индекс стиля комнаты.</param>
    public void SetRoomStyle(int styleIndex)
    {
        currentRoomStyle = roomStyleManager.GetRoomStyle(styleIndex);
    }

    /// <summary>
    /// Устанавливает случайный стиль комнаты из доступных в менеджере стилей.
    /// </summary>
    public void SetRandomRoomStyle()
    {
        SetRoomStyle(roomStyleManager.GetRandomStyleIndex());
    }

    /// <summary>
    /// Возвращает количество доступных стилей комнат.
    /// </summary>
    /// <returns>Количество стилей комнат.</returns>
    public int GetStylesCount()
    {
        return roomStyleManager.GetStylesCount();
    }

    /// <summary>
    /// Рисует тайлы на карте, используя заданный двумерный массив.
    /// </summary>
    /// <param name="array">Двумерный массив, представляющий карту.</param>
    /// <param name="isWall">Указывает, является ли массивом стен.</param>
    private void PaintFromArray(int[,] array, bool isWall)
    {
        System.Random rand = new System.Random();
        for (int y = 0; y < array.GetLength(1); y++)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                int roomIndex = array[x, y];
                if (roomIndex >= 0)
                {
                    currentRoomStyle = roomStyleManager.GetRoomStyle(roomManager.GetRoomStyleId(roomIndex));
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tileToUse = isWall ? DetermineWallTile(x, y, array, rand) : currentRoomStyle.floorTile[rand.Next(currentRoomStyle.floorTile.Count)];
                    tilemap.SetTile(tilePosition, tileToUse);
                }
            }
        }
    }

    /// <summary>
    /// Рисует тайлы на карте, используя карту подземелья.
    /// </summary>
    /// <param name="dungeonMap">Карта подземелья.</param>
    private void PaintFromDungeonMap(DungeonMap dungeonMap)
    {
        System.Random rand = new System.Random();
        for (int y = 0; y < dungeonMap.GetHeight(); y++)
        {
            for (int x = 0; x < dungeonMap.GetWidth(); x++)
            {
                int textureType = dungeonMap.tiles[x, y].textureType;
                if (dungeonMap.tiles[x, y].roomIndex >= 0 && generator.graph.IsRoom(dungeonMap.tiles[x, y].roomIndex))
                {
                    currentRoomStyle = roomStyleManager.GetRoomStyle(roomManager.GetRoomStyleId(dungeonMap.tiles[x, y].roomIndex));
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tileToUse = null;

                    switch (textureType)
                    {
                        case 1:
                            tileToUse = currentRoomStyle.topLeftCornerTile[rand.Next(currentRoomStyle.topLeftCornerTile.Count)];
                            break;
                        case 2:
                            tileToUse = currentRoomStyle.topRightCornerTile[rand.Next(currentRoomStyle.topRightCornerTile.Count)];
                            break;
                        case 3:
                            tileToUse = currentRoomStyle.bottomLeftCornerTile[rand.Next(currentRoomStyle.bottomLeftCornerTile.Count)];
                            break;
                        case 4:
                            tileToUse = currentRoomStyle.bottomRightCornerTile[rand.Next(currentRoomStyle.bottomRightCornerTile.Count)];
                            break;
                        case 5:
                            tileToUse = currentRoomStyle.leftWallTile[rand.Next(currentRoomStyle.leftWallTile.Count)];
                            break;
                        case 6:
                            tileToUse = currentRoomStyle.rightWallTile[rand.Next(currentRoomStyle.rightWallTile.Count)];
                            break;
                        case 7:
                            tileToUse = currentRoomStyle.topWallTile[rand.Next(currentRoomStyle.topWallTile.Count)];
                            break;
                        case 8:
                            tileToUse = currentRoomStyle.bottomWallTile[rand.Next(currentRoomStyle.bottomWallTile.Count)];
                            break;
                        default:
                            tileToUse = currentRoomStyle.floorTile[rand.Next(currentRoomStyle.floorTile.Count)];
                            break;
                    }
                    tilemap.SetTile(tilePosition, tileToUse);
                }
                else if (dungeonMap.tiles[x, y].roomIndex >= 0 && generator.graph.IsCorridor(dungeonMap.tiles[x, y].roomIndex))
                {
                    List<int> roomIndices = FindConnectedRoomsIndices(dungeonMap.tiles[x, y].roomIndex, generator.graph.graphMap);

                    int room1Index = roomIndices[0];
                    int room2Index = roomIndices[1];

                    DungeonRoom room1 = roomManager.GetRoomById(room1Index);
                    DungeonRoom room2 = roomManager.GetRoomById(room2Index);

                    RoomStyle style1 = roomStyleManager.GetRoomStyle(roomManager.GetRoomStyleId(room1Index));
                    RoomStyle style2 = roomStyleManager.GetRoomStyle(roomManager.GetRoomStyleId(room2Index));

                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tileToUse = null;

                    if (style1 == style2)
                    {
                        currentRoomStyle = style1;

                        switch (textureType)
                        {
                            case 1:
                                tileToUse = currentRoomStyle.topLeftCornerTile[rand.Next(currentRoomStyle.topLeftCornerTile.Count)];
                                break;
                            case 2:
                                tileToUse = currentRoomStyle.topRightCornerTile[rand.Next(currentRoomStyle.topRightCornerTile.Count)];
                                break;
                            case 3:
                                tileToUse = currentRoomStyle.bottomLeftCornerTile[rand.Next(currentRoomStyle.bottomLeftCornerTile.Count)];
                                break;
                            case 4:
                                tileToUse = currentRoomStyle.bottomRightCornerTile[rand.Next(currentRoomStyle.bottomRightCornerTile.Count)];
                                break;
                            case 5:
                                tileToUse = currentRoomStyle.leftWallTile[rand.Next(currentRoomStyle.leftWallTile.Count)];
                                break;
                            case 6:
                                tileToUse = currentRoomStyle.rightWallTile[rand.Next(currentRoomStyle.rightWallTile.Count)];
                                break;
                            case 7:
                                tileToUse = currentRoomStyle.topWallTile[rand.Next(currentRoomStyle.topWallTile.Count)];
                                break;
                            case 8:
                                tileToUse = currentRoomStyle.bottomWallTile[rand.Next(currentRoomStyle.bottomWallTile.Count)];
                                break;
                            default:
                                tileToUse = currentRoomStyle.floorTile[rand.Next(currentRoomStyle.floorTile.Count)];
                                break;
                        }
                        tilemap.SetTile(tilePosition, tileToUse);
                    }
                    else
                    {
                        TileBase tileToUse2 = null;

                        float distanceToRoom1Center = Vector2.Distance(new Vector2(x, y), new Vector2(room1.centerX, room1.centerY));
                        float distanceToRoom2Center = Vector2.Distance(new Vector2(x, y), new Vector2(room2.centerX, room2.centerY));
                        float totalDistance = distanceToRoom1Center + distanceToRoom2Center;
                        float blendFactor = 1.0f - (distanceToRoom1Center / totalDistance);

                        switch (textureType)
                        {
                            case 1:
                                tileToUse = style1.topLeftCornerTile[rand.Next(style1.topLeftCornerTile.Count)];
                                tileToUse2 = style2.topLeftCornerTile[rand.Next(style2.topLeftCornerTile.Count)];
                                break;
                            case 2:
                                tileToUse = style1.topRightCornerTile[rand.Next(style1.topRightCornerTile.Count)];
                                tileToUse2 = style2.topRightCornerTile[rand.Next(style2.topRightCornerTile.Count)];
                                break;
                            case 3:
                                tileToUse = style1.bottomLeftCornerTile[rand.Next(style1.bottomLeftCornerTile.Count)];
                                tileToUse2 = style2.bottomLeftCornerTile[rand.Next(style2.bottomLeftCornerTile.Count)];
                                break;
                            case 4:
                                tileToUse = style1.bottomRightCornerTile[rand.Next(style1.bottomRightCornerTile.Count)];
                                tileToUse2 = style2.bottomRightCornerTile[rand.Next(style2.bottomRightCornerTile.Count)];
                                break;
                            case 5:
                                tileToUse = style1.leftWallTile[rand.Next(style1.leftWallTile.Count)];
                                tileToUse2 = style2.leftWallTile[rand.Next(style2.leftWallTile.Count)];
                                break;
                            case 6:
                                tileToUse = style1.rightWallTile[rand.Next(style1.rightWallTile.Count)];
                                tileToUse2 = style2.rightWallTile[rand.Next(style2.rightWallTile.Count)];
                                break;
                            case 7:
                                tileToUse = style1.topWallTile[rand.Next(style1.topWallTile.Count)];
                                tileToUse2 = style2.topWallTile[rand.Next(style2.topWallTile.Count)];
                                break;
                            case 8:
                                tileToUse = style1.bottomWallTile[rand.Next(style1.bottomWallTile.Count)];
                                tileToUse2 = style2.bottomWallTile[rand.Next(style2.bottomWallTile.Count)];
                                break;
                            default:
                                tileToUse = style1.floorTile[rand.Next(style1.floorTile.Count)];
                                tileToUse2 = style2.floorTile[rand.Next(style2.floorTile.Count)];
                                break;
                        }

                        var t = CombineTiles(tileToUse2, tileToUse, blendFactor);
                        tilemap.SetTile(tilePosition, t);

                    }

                }
            }
        }
    }

    /// <summary>
    /// Поиск индексов комнат, связанных заданным коридором.
    /// </summary>
    /// <param name="corridorIndex"> Индекс коридора.</param>
    /// <param name="graphMap">Граф карты.</param>
    /// <returns>Список индесов комнат.</returns>
    private List<int> FindConnectedRoomsIndices(int corridorIndex, int[,] graphMap)
    {
        List<int> connectedRooms = new List<int>();
        for (int i = 0; i < graphMap.GetLength(0); i++)
        {
            for (int j = 0; j < graphMap.GetLength(1); j++)
            {
                if (graphMap[i, j] == corridorIndex)
                {
                    connectedRooms.Add(i);
                    connectedRooms.Add(j);
                    return connectedRooms;
                }
            }
        }
        return connectedRooms;
    }

    /// <summary>
    /// Наложение спрайтов друг на друга.
    /// </summary>
    /// <param name="firstTileBase"> Первый тайл.</param>
    /// <param name="secondTileBase"> Второй тайл для наложения.</param>
    /// <param name="alpha">Прозрачность второго тайла.</param>
    /// <returns>Новый спрайт.</returns>
    TileBase CombineTiles(TileBase firstTileBase, TileBase secondTileBase, float alpha)
    {
        Tile tile1 = firstTileBase as Tile;
        Tile tile2 = secondTileBase as Tile;

        if (tile1 == null || tile2 == null)
        {
            Debug.LogError("One of the tile bases is not a Tile or is null.");
            return null;
        }

        Sprite firstSprite = tile1.sprite;
        Sprite secondSprite = tile2.sprite;

        if (firstSprite == null || secondSprite == null)
        {
            Debug.LogError("One of the tiles does not have a sprite associated with it.");
            return null;
        }

        Texture2D firstTexture = new Texture2D((int)firstSprite.rect.width, (int)firstSprite.rect.height);
        Texture2D secondTexture = new Texture2D((int)secondSprite.rect.width, (int)secondSprite.rect.height);

        // копирование пикселей из атласа в новые текстуры
        firstTexture.SetPixels(firstSprite.texture.GetPixels((int)firstSprite.rect.x,
                                                             (int)firstSprite.rect.y,
                                                             (int)firstSprite.rect.width,
                                                             (int)firstSprite.rect.height));
        secondTexture.SetPixels(secondSprite.texture.GetPixels((int)secondSprite.rect.x,
                                                               (int)secondSprite.rect.y,
                                                               (int)secondSprite.rect.width,
                                                               (int)secondSprite.rect.height));
        firstTexture.Apply();
        secondTexture.Apply();

        // накладывание второй текстуры на первую
        for (int y = 0; y < firstTexture.height; y++)
        {
            for (int x = 0; x < firstTexture.width; x++)
            {
                Color firstColor = firstTexture.GetPixel(x, y);
                Color secondColor = secondTexture.GetPixel(x, y);
                firstTexture.SetPixel(x, y, Color.Lerp(firstColor, secondColor, alpha * secondColor.a));
            }
        }

        firstTexture.Apply();

        // создать новый спрайт
        Sprite combinedSprite = Sprite.Create(
            firstTexture,
            new Rect(0f, 0f, firstTexture.width, firstTexture.height),
            new Vector2(0.5f, 0.5f),
            16 // тут изначальный размер тайла !!!!!!!
        );
        // создать Tile и назначить ему полученный спрайт
        Tile combinedTile = ScriptableObject.CreateInstance<Tile>();
        combinedTile.sprite = combinedSprite;

        return combinedTile;
    }

    /// <summary>
    /// Определяет тайл стены, исходя из его позиции и окружения.
    /// </summary>
    /// <param name="x">X координата тайла.</param>
    /// <param name="y">Y координата тайла.</param>
    /// <param name="array">Двумерный массив, представляющий карту.</param>
    /// <param name="rand">Экземпляр класса Random.</param>
    /// <returns>Выбранный тайл стены.</returns>
    private TileBase DetermineWallTile(int x, int y, int[,] array, System.Random rand)
    {
        bool top = y + 1 < array.GetLength(1) && array[x, y + 1] != -1;
        bool bottom = y - 1 >= 0 && array[x, y - 1] != -1;
        bool left = x - 1 >= 0 && array[x - 1, y] != -1;
        bool right = x + 1 < array.GetLength(0) && array[x + 1, y] != -1;

        List<TileBase> possibleTiles = new List<TileBase>();

        if (!top && !left) possibleTiles = currentRoomStyle.topLeftCornerTile;
        else if (!top && !right) possibleTiles = currentRoomStyle.topRightCornerTile;
        else if (!bottom && !left) possibleTiles = currentRoomStyle.bottomLeftCornerTile;
        else if (!bottom && !right) possibleTiles = currentRoomStyle.bottomRightCornerTile;

        else if (top && bottom && !left) possibleTiles = currentRoomStyle.leftWallTile;
        else if (top && bottom && !right) possibleTiles = currentRoomStyle.rightWallTile;
        else if (left && right && !top) possibleTiles = currentRoomStyle.topWallTile;
        else if (left && right && !bottom) possibleTiles = currentRoomStyle.bottomWallTile;

        return possibleTiles.Any() ? possibleTiles[rand.Next(possibleTiles.Count)] : null;
    }

    /// <summary>
    /// Очищает все тайлы на карте.
    /// </summary>
    public void Clear()
    {
        tilemap.ClearAllTiles();
    }

    /// <summary>
    /// Перезагружает и генерирует карту подземелья.
    /// </summary>
    public void Reload()
    {
        Clear();

        map = new DungeonMap(generator.CreateDungeon());

        //roomManager = new DungeonRoomManager(map, generator.graph.graphMap);
        roomManager.Initialize(map, generator.graph.graphMap);
        roomManager.AssignRandomStylesToRooms(roomStyleManager);
        roomManager.AssignFactionsToRooms();

        roomManager.PrintRoomsInfo();
        roomManager.ClearRoomsInfoFromMap();
        roomManager.DisplayRoomsInfoOnMap();

        PaintFromDungeonMap(map);

        //----------------------------

    }

    /// <summary>
    /// Перезагружает и генерирует карту подземелья с использованием заданного сида генерации.
    /// </summary>
    /// <param name="seed">Сид генерации карты.</param>
    public void Reload(int seed)
    {
        Clear();

        map = new DungeonMap(generator.CreateDungeon(seed));

        //roomManager = new DungeonRoomManager(map, generator.graph.graphMap);
        roomManager.Initialize(map, generator.graph.graphMap);
        roomManager.AssignRandomStylesToRooms(roomStyleManager);
        roomManager.AssignFactionsToRooms();

        roomManager.PrintRoomsInfo();
        roomManager.ClearRoomsInfoFromMap();
        roomManager.DisplayRoomsInfoOnMap();

        PaintFromDungeonMap(map);
    }

    /// <summary>
    /// Инициализация и генерация начальной карты подземелья при старте.
    /// </summary>
    void Start()
    {
        Reload();
    }
}