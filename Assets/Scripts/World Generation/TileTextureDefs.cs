using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileTypeGraphics
{
    public Tile.TYPE type = Tile.TYPE.none;
    //TODO: add the abillity for a sprite sheet to have multiple choices for the same tile type, used at the discretion of tile graphics
    public Texture2D spritesheet = null;
    [Header("Indexes")]
    public int top1 = 0;
    public int top2 = 0;
    public int top3 = 0;
    public int top4 = 0;
    public int top5 = 0;
    public int mid1 = 0;
    public int mid2 = 0;
    public int mid3 = 0;
    public int mid4 = 0;
    public int left1 = 0;
    public int left2 = 0;
    public int left3 = 0;
    public int right1 = 0;
    public int right2 = 0;
    public int right3 = 0;
    public int bot1 = 0;
    public int bot2 = 0;
    public int bot3 = 0;
    public int bot4 = 0;
    public int bot5 = 0;
}

public class TileTextureDefs : MonoBehaviour
{

    public TileTypeGraphics[] tileTypes;
}
