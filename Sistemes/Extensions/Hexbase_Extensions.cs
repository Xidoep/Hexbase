using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridExtensions
{
    //Direccions en relació al Grid
    public static Vector2Int Up(this Vector2Int coordenades) => coordenades + new Vector2Int(0, -1);

    public static Vector2Int UpRight(this Vector2Int coordenades) => coordenades + new Vector2Int(1, coordenades.IsParell() ? -1 : 0);
    public static Vector2Int DownRight(this Vector2Int coordenades) => coordenades + new Vector2Int(1, coordenades.IsParell() ? 0 : 1);

    public static Vector2Int Down(this Vector2Int coordenades) => coordenades + new Vector2Int(0, 1);
    public static Vector2Int DownLeft(this Vector2Int coordenades) => coordenades + new Vector2Int(-1, coordenades.IsParell() ? 0 : 1);
    public static Vector2Int UpLeft(this Vector2Int coordenades) => coordenades + new Vector2Int(-1, coordenades.IsParell() ? -1 : 0);



    static bool IsParell(this Vector2Int coordenades) => (coordenades.x % 2) == 0;

    /// <summary>
    /// Conté el numero donat entre 0 i 6
    /// </summary>
    public static int ClampVeins(this int vei) => vei % 6;

    /// <summary>
    /// Comprova si la coordenada està fora del grid.
    /// </summary>
    public static bool IsOutOfRange(this Vector2Int coordenades, int size) => coordenades.x != Mathf.Clamp(coordenades.x, 0, size - 1) || coordenades.y != Mathf.Clamp(coordenades.y, 0, size - 1);

    public static Hexagon Get(this Hexagon[,] grid, int x, int y) => grid[x, y];
    public static Hexagon Get(this Hexagon[,] grid, Vector2Int coordenada) => grid.Get(coordenada.x, coordenada.y);

    public static bool EstaBuida(this Hexagon[,] grid, Vector2Int coordenada) => grid.Get(coordenada) == null;

    public static void Set(this Hexagon[,] grid, Hexagon hexagon, int x, int y) => grid[x, y] = hexagon;
    public static void Set(this Hexagon[,] grid, Hexagon hexagon, Vector2Int coordenada) => grid[coordenada.x, coordenada.y] = hexagon;
    public static void Set(this Hexagon[,] grid, Hexagon hexagon) => grid.Set(hexagon, hexagon.Coordenades.x, hexagon.Coordenades.y);
    
    /// <summary>
    /// Retorna la posició de la coordenada donada.
    /// </summary>
    public static Vector3 GetWorldPosition(int column, int row)
    {
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = 1;

        shouldOffset = (column % 2) == 0;
        width = 2f * size;
        height = Mathf.Sqrt(3) * size;

        horizontalDistance = width * (3f / 4f);
        verticalDistance = height;

        offset = shouldOffset ? height / 2 : 0;

        xPosition = column * horizontalDistance;
        yPosition = row * verticalDistance - offset;

        return new Vector3(xPosition, 0, -yPosition);
    }


    static List<Hexagon> _tmpVeins;
    static List<Vector2Int> _tmpCoordenades;
    static List<Peça> _tmpPeces;
    static Hexagon _tmpPeça;
    public static List<Hexagon> Veins(this Hexagon[,] grid, Vector2Int coordenades)
    {
        if (_tmpVeins == null) _tmpVeins = new List<Hexagon>();
        else _tmpVeins.Clear();

        _tmpVeins.Add(grid.Get(coordenades.Up()));
        _tmpVeins.Add(grid.Get(coordenades.UpRight()));
        _tmpVeins.Add(grid.Get(coordenades.DownRight()));
        _tmpVeins.Add(grid.Get(coordenades.Down()));
        _tmpVeins.Add(grid.Get(coordenades.DownLeft()));
        _tmpVeins.Add(grid.Get(coordenades.UpLeft()));
        return _tmpVeins;
    }

    public static List<Vector2Int> VeinsCoordenades(this Hexagon[,] grid, Vector2Int coordenades)
    {
        if (_tmpCoordenades == null) _tmpCoordenades = new List<Vector2Int>();
        else _tmpCoordenades.Clear();

        _tmpCoordenades.Add(coordenades.Up());
        _tmpCoordenades.Add(coordenades.UpRight());
        _tmpCoordenades.Add(coordenades.DownRight());
        _tmpCoordenades.Add(coordenades.Down());
        _tmpCoordenades.Add(coordenades.DownLeft());
        _tmpCoordenades.Add(coordenades.UpLeft());
        return _tmpCoordenades;
    }

    public static List<Peça> VeinsPeça(this Hexagon[,] grid, Vector2Int coordenades)
    {
        if (_tmpPeces == null) _tmpPeces = new List<Peça>();
        else _tmpPeces.Clear();

        _tmpPeça = grid.Get(coordenades.Up());
        if (_tmpPeça != null && _tmpPeça.EsPeça) _tmpPeces.Add((Peça)_tmpPeça);

        _tmpPeça = grid.Get(coordenades.UpRight());
        if (_tmpPeça != null && _tmpPeça.EsPeça) _tmpPeces.Add((Peça)_tmpPeça);

        _tmpPeça = grid.Get(coordenades.DownRight());
        if (_tmpPeça != null && _tmpPeça.EsPeça) _tmpPeces.Add((Peça)_tmpPeça);

        _tmpPeça = grid.Get(coordenades.Down());
        if (_tmpPeça != null && _tmpPeça.EsPeça) _tmpPeces.Add((Peça)_tmpPeça);

        _tmpPeça = grid.Get(coordenades.DownLeft());
        if (_tmpPeça != null && _tmpPeça.EsPeça) _tmpPeces.Add((Peça)_tmpPeça);

        _tmpPeça = grid.Get(coordenades.UpLeft());
        if (_tmpPeça != null && _tmpPeça.EsPeça) _tmpPeces.Add((Peça)_tmpPeça);

        return _tmpPeces;
    }

}



