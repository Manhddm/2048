using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Tile : MonoBehaviour
{
    public TileState State {get; private set;}
    public TileCell Cell {get; private set;}
    public int Number {get; private set;}
    public bool Locked {get;  set;}
    
    private Image background;
    private TextMeshProUGUI text;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetState(TileState state, int number)
    {
        State = state;
        Number = number;
        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if (Cell != null)
        {
            this.Cell.Tile = null;
        }
        Cell = cell;
        Cell.Tile  = this;
        transform.position = cell.transform.position;
    }
    public void MoveTo(TileCell cell)
    {
        if (Cell != null)
        {
            this.Cell.Tile = null;
        }
        Cell = cell;
        Cell.Tile  = this;
       StartCoroutine(Animate(Cell.transform.position, false));
    }

    public void Merge(TileCell cell)
    {
        if (Cell != null)
        {
            this.Cell.Tile = null;
        }

        if (cell != null && cell.Tile != null)
        {
            cell.Tile.Locked = true;
        }
        StartCoroutine(Animate(cell.transform.position, true));
        Cell = null;
    }
    private IEnumerator Animate(Vector3 to, bool mergeing)
    {
        float elapsed = 0f;
        float duration = 0.12f;
        Vector3 from =  transform.position;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        

        transform.position = to;
        if (mergeing)
        {
            Destroy(gameObject);
        }
    }
}
