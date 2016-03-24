using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
public class TileCollision : MonoBehaviour {
    
    public void BuildCollider(TileMap tileMap, float tileSize)
    {
        //Prepare for polygon collider setting
        PolygonCollider2D poly_collider = GetComponent<PolygonCollider2D>();
        //poly_collider = new PolygonCollider2D();
        poly_collider.CreatePrimitive(3, new Vector2(0.1f,0.1f));
        poly_collider.pathCount = 0;

        ArrayList doneTiles = new ArrayList();
        int x = 0;
        int y = 0;
        while (tileMap.sizeX * tileMap.sizeY > doneTiles.Count)
        {
            if (!(doneTiles.Contains(tileMap.GetTile(x, y))))
            {
                List<Tile> island = tileMap.Percolate(x, y, new Tile.TYPE[] { Tile.TYPE.ground });
                if (island.Count > 0)
                {
                    island.Sort();
                    foreach (Tile tile in island)
                    {
                        Debug.Log(tile.ToString());
                        doneTiles.Add(tile);
                    }
                    poly_collider.SetPath(poly_collider.pathCount++, GetBax());
                }
                else
                {
                    doneTiles.Add(tileMap.GetTile(x, y));
                }
            }
            x++;
            if(x >= tileMap.sizeX)
            {
                x = 0;
                y++;
            }
        }
        
        
    }

    Vector2[] GetBox(List<Tile> island, float tileSize)
    {
        List<Vector2> box = new List<Vector2>();
        box.Add(new Vector2(island[0].x * tileSize, island[0].y * tileSize));

        int direction = 0;
        int length = 0;
        do
        {
            switch (direction)
            {
                case 0:
                    box.Add(new Vector2(box[box.Count - 1].x, box[box.Count - 1].y));
                    length = 0;
                    do
                    {
                        box[box.Count - 1].Set(box[box.Count - 1].x + tileSize, box[box.Count - 1].y);
                    } while ((length + 1) < island.Count && island[length].y == island[length].y);
                    break;
                default:
                    break;
            }
        } while (box[box.Count - 1] != box[0]);
        return box.ToArray();
    }

    Vector2[] GetBax()
    {
        List<Vector2> box = new List<Vector2>();
        box.Add(new Vector2(0, 0));
        box.Add(new Vector2(1, 0));
        box.Add(new Vector2(1, 1));
        box.Add(new Vector2(0, 1));
        return box.ToArray();
    }
}