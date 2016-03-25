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

        Test();

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
                        doneTiles.Add(tile);
                    }
                    poly_collider.SetPath(poly_collider.pathCount++, GetBax(island, tileSize));
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
        box.Add(new Vector2(island[1].x * tileSize, island[1].y * tileSize));

        int direction = 0;
        int length = 0;
        int start = 0;
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
                        length++;
                    } while ((start + length + 1) < island.Count && island[start].y == island[start + length].y);
                    return null;
                    break;
                default:
                    break;
            }
        } while (box[box.Count - 1] != box[0]);
        return box.ToArray();
    }

    Vector2[] GetBax(List<Tile> island, float tileSize)
    {
        List<Vector2> box = new List<Vector2>();
        box.Add(new Vector2(0, 0));
        box.Add(new Vector2(1, 0));
        box.Add(new Vector2(1, 1));
        box.Add(new Vector2(0, 1));
        return box.ToArray();
    }

    Stack<Vector2> Test()
    {
        Stack<Vector2> box = new Stack<Vector2>();
        box.Push(new Vector2(0,0));
        Vector2 point = box.Peek();
        for (int i = 0; i < 20; i++)
        {
            box.Peek().Set(point.x + 1, point.y - 1);
            point.Set(point.x + 1, point.y - 1);
        }
        return box;
    }
}