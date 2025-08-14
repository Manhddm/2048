
using UnityEngine;

public class TileRow : MonoBehaviour
{
    public TileCell[] Cells { get; set; }

    private void Awake()
    {
        Cells = GetComponentsInChildren<TileCell>();
    }
}
