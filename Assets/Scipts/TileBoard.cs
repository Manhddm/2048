using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    
    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;
    
    private TileGrid _grid;
    private List<Tile> _tiles;

    private bool _waiting;


    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);

        
    }

    private void Start()
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            tileStates[i].textColor.a = tileStates[i].backgroundColor.a = 1f;
        }

    }

    public void CreateTile(int  number = 2)
    {
        Tile tile = Instantiate(tilePrefab, _grid.transform);
        tile.SetState(tileStates[0],number);
        tile.Spawn(_grid.GetRandomEmptyCell());
        _tiles.Add(tile);
    }

    public void ClearBoard()
    {
        foreach (var cell in _grid.Cells)
        {
            cell.Tile = null;
        }
        foreach (var tile in _tiles)
        {
            Destroy(tile.gameObject);
        }
        _tiles.Clear();
    }

    private void Update()
    {
        if (!_waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1); // Sửa startY từ 0, incrementY = 1
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, _grid.height - 2, -1); // Sửa startY từ _grid.height - 1
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1); // Sửa startX từ 0
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, _grid.width - 2, -1, 0, 1); // Sửa startX từ _grid.width - 1
            }
        }

    }

    private void MoveTiles(Vector2Int direction, int startX,int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int x = startX; x>=0 && x < _grid.width; x+=incrementX)
        {
            for (int y = startY; y>=0 &&  y < _grid.height; y+=incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);
                if (cell!= null && cell.Occupied)
                {
                   changed |=  MoveTile(cell.Tile, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacentCell = _grid.GetAdjacentCell(tile.Cell, direction);
        while (adjacentCell != null)
        {
            if (adjacentCell.Occupied)
            {
                if (CanMerge(tile, adjacentCell.Tile))
                {
                    Merge(tile, adjacentCell.Tile);
                    return true;
                }   
                break;
            }

            newCell = adjacentCell;
            adjacentCell = _grid.GetAdjacentCell(adjacentCell, direction);
        }

        if (newCell != null)
        {
          tile.MoveTo(newCell);
     
          return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.Number == b.Number && !b.Locked;
    }

    private void Merge(Tile a, Tile b)
    {
        _tiles.Remove(a);
        a.Merge(b.Cell);
        int index = Mathf.Clamp(IndexOf(b.State) + 1, 0, tileStates.Length - 1);
        int number = b.Number * 2;
        b.SetState(tileStates[index], number);
        gameManager.IncreaseScore(number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (tileStates[i] == state)
            {
                return i;
            }
        }
        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        _waiting = true;
        yield return new WaitForSeconds(0.12f);
        _waiting = false;
        foreach (var tile in _tiles)
        {
            tile.Locked = false;
        }
        if (_tiles.Count != _grid.size)
            CreateTile();
        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }
    }

    public bool CheckForGameOver()
    {
        if (this._tiles.Count != _grid.size)
        {
            return false;
        }
        foreach(var tile in this._tiles)
        {
            TileCell up = _grid.GetAdjacentCell(tile.Cell, Vector2Int.up);
            TileCell down = _grid.GetAdjacentCell(tile.Cell, Vector2Int.down);
            TileCell right = _grid.GetAdjacentCell(tile.Cell, Vector2Int.right);
            TileCell left = _grid.GetAdjacentCell(tile.Cell, Vector2Int.left);
            if (up != null && CanMerge(tile, up.Tile))
            {
                return false;
            }

            if (down != null && CanMerge(tile, down.Tile))
            {
                return false;
            }

            if (right != null && CanMerge(tile, right.Tile))
            {
                return false;
            }

            if (left != null && CanMerge(tile, left.Tile))
            {
                return false;
            }
        }

        return true;
    }
}
