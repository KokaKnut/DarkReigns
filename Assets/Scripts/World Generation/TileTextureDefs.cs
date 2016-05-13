using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileTypeGraphics
{
    public Tile.TYPE type = Tile.TYPE.none;
    //TODO: add the abillity for a sprite sheet to have multiple choices for the same tile type, used at the discretion of tile graphics
    public Sprite sprite = null;
}

public class TileTextureDefs : MonoBehaviour
{

    //public Texture2D spriteSheet = null;
    public int tileResolution = 0;
    public TileTypeGraphics[] tileTypes;
}
