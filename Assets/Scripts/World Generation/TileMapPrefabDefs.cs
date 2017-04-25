using UnityEngine;
using System.Collections;

[System.Serializable]
public struct TileMapPrefabDef
{

    public string Name;
    public TileMapPrefabBuilder tileMapPrefab;
    public int rarity;
    public Vector2 coords;
    public int linkageNumber;

    public prefabType type;
    public enum prefabType
    {
        normal,
        start,
        end,
        unique
    }

    private int _linkages;
    public int linkages
    {
        get
        {
            return _linkages;
        }
        set
        {
            _linkages = value;
        }
    }
}

public class TileMapPrefabDefs : MonoBehaviour
{

    public TileMapPrefabDef[] prefabTypes;
}
