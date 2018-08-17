using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class TileMapPrefabBuilder : MonoBehaviour
{

    public TileMap tileMap = null;
    public bool preview = false;
    public TileGraphics tg;
    public float tileSize = 0f;
    public TileTextureDefs tileDefs = null;
    
    public bool painting = false;
    public Tile.TYPE tilePaint;
    public Vector2 tileHighlight;

    Mesh grid;
    Mesh cursor;

    void Awake()
    {
        Disable();
        tileMap.ResetLinkages();
    }

    public void PaintTile(int x, int y, Tile.TYPE type)
    {
        tileMap.SetTile(x, y, type);
        tg.BuildSprite(tileMap, tileSize);
    }

    void OnDrawGizmosSelected()
    {
        if (painting)
        {
            if (grid == null)
                MakeGrid();
            Gizmos.color = new Color(.2f, .2f, 1f, .3f);
            Gizmos.DrawWireMesh(grid, transform.position);
            
            if (!(Mathf.Approximately(tileHighlight.x, -1) && Mathf.Approximately(tileHighlight.y, -1)))
            {
                Gizmos.color = new Color(1f, 1f, 1f, .6f);
                Gizmos.DrawWireMesh(cursor, new Vector3(transform.position.x + (tileHighlight.x * tileSize), transform.position.y + (tileHighlight.y * tileSize)));
            }
        }
    }

    void MakeGrid()
    {
        grid = TileGraphics.MakeTileMesh(tileMap, tileSize);
        cursor = TileGraphics.MakeTileMesh(new TileMap(1, 1), tileSize);
        tileHighlight = new Vector2(-1, -1);
    }

    [ContextMenu("Hide Contents")]
    public void Disable()
    {
        gameObject.GetComponent<TileGraphics>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        enabled = false;
    }

    [ContextMenu("Show Contents")]
    public void Enable()
    {
        gameObject.GetComponent<TileGraphics>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        enabled = true;
    }
}
