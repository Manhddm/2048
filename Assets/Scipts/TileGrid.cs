using UnityEngine;
using Random = UnityEngine.Random;

public class TileGrid : MonoBehaviour
{
    public TileRow[] Rows { get; set; }
    public TileCell[] Cells { get; set; }

    public int size => Cells.Length;
    public int height => Rows.Length ;
    public int width => size/height;

    private void Awake()
    {
        Rows = GetComponentsInChildren<TileRow>();
        Cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for (int y = 0; y < Rows.Length; y++)
        {
            for (int x = 0; x < Rows[y].Cells.Length; x++)
            {
                Rows[y].Cells[x].Coordinates = new Vector2Int(x, y);
            }
        }
        Debug.Log($"vip {Rows.Length}");
        Debug.Log(Cells.Length);
    }

    public TileCell GetCell(int x, int y)
    {

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return Rows[y].Cells[x];
        }
        return null;
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.Coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;
        return GetCell(coordinates.x, coordinates.y);
        
    }
    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, Cells.Length);
        int start = index;
        while (Cells[index].Occupied)
        {
            index++;
            if (index >= Cells.Length)
            {
                index = 0;
            }

            if (index == start)
            {
                return null;
            }
        }
        return Cells[index];
    }
}
