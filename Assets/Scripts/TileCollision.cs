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
        poly_collider = new PolygonCollider2D();
        poly_collider.CreatePrimitive(3, new Vector2(0.1f,0.1f));

        ArrayList doneTiles = new ArrayList();
        int x = 0;
        int y = 0;
        while (tileMap.sizeX * tileMap.sizeY > doneTiles.Count)
        {
            if (!(doneTiles.Contains(tileMap.GetTile(x, y))))
            {
                ArrayList island = tileMap.Percolate(x, y);
                island.Sort();
                foreach (var tile in island)
                {
                    Debug.Log(tile.ToString());
                    doneTiles.Add(tile);
                }
                poly_collider.pathCount++;
                poly_collider.SetPath(poly_collider.pathCount, GetBox());
            }
            x++;
            if(x >= tileMap.sizeX)
            {
                x = 0;
                y++;
            }
        }
        
        
    }

    Vector2[] GetBox()
    {
        List<Vector2> box = new List<Vector2>();
        return box.ToArray();
    }
}