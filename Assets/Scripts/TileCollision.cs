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

        //this resets the collider
        poly_collider.CreatePrimitive(3, new Vector2(0.1f,0.1f));
        poly_collider.pathCount = 0;

        //run through percolation of all tiles, set collision for all islands
        // TODO: right now collision is made for outer edges only, being inside will bounce you out.
        ArrayList doneTiles = new ArrayList();
        int x = 0;
        int y = 0;
        while (tileMap.sizeX * tileMap.sizeY > doneTiles.Count)
        {
            if (!(doneTiles.Contains(tileMap.GetTile(x, y))))
            {
                Tile[] island = tileMap.Percolate(x, y, new Tile.TYPE[] { Tile.TYPE.ground });
                if (island.Length > 0)
                {
                    Array.Sort(island);
                    foreach (Tile tile in island)
                    {
                        doneTiles.Add(tile);
                    }
                    poly_collider.SetPath(poly_collider.pathCount++, GetBox(island, tileSize));
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

    Vector2[] GetBox(Tile[] islandY, float tileSize)
    {
        // Get another array sorted on the other axis
        Tile[] islandX = new Tile[islandY.Length];
        Array.Copy(islandY, islandX, islandY.Length);
        Array.Sort(islandY);

        // Swap the Comparison method so ICompare interface can binary search properly
        for (int i = 0; i < islandX.Length; i++)
        {
            islandX[i].SwapCompare();
        }

        Array.Sort(islandX);

        // Start the collision box with the first point
        List<Vector2> box = new List<Vector2>();
        box.Add(new Vector2(islandX[0].x * tileSize, islandX[0].y * tileSize));

        // initialize some helpers
        String direction = "+x";
        int length = 0;
        int startx = 0;
        int starty = 0;

        // the crazy loop of doom, under construction
        do
        {
            switch (direction)
            {
                case "+x":
                    for (length = 0; (startx + length < islandX.Length)
                        && (islandX[startx].y == islandX[startx + length].y)
                        && (islandX[startx].x == islandX[startx + length].x - length)
                        && (Array.BinarySearch(islandX, new Tile(islandX[startx + length].x, islandX[startx + length].y - 1, islandX[startx].type)) < 0); length++);
                    box.Add(new Vector2(box[box.Count - 1].x + (length * tileSize), box[box.Count - 1].y));
                    startx += length - 1;
                    if (Array.BinarySearch(islandY, new Tile(islandX[startx].x, islandX[startx].y - 1, islandX[startx].type)) > 0)
                        direction = "-y";
                    else
                        direction = "+y";
                    starty = Array.BinarySearch(islandY, new Tile(islandX[startx].x, islandX[startx].y, islandX[startx].type));
                    break;
                case "+y":
                    for (length = 0; (starty + length < islandY.Length)
                        && (islandY[starty].x == islandY[starty + length].x)
                        && (islandY[starty].y == islandY[starty + length].y - length)
                        && (Array.BinarySearch(islandY, new Tile(islandY[starty + length].x - 1, islandY[starty + length].y, islandY[starty].type)) < 0); length++)
                        print((starty + length < islandY.Length)
                        && (islandY[starty].x == islandY[starty + length].x)
                        && (islandY[starty].y == islandY[starty + length].y - length)
                        && (Array.BinarySearch(islandY, new Tile(islandY[starty + length].x - 1, islandY[starty + length].y, islandY[starty].type)) < 0));
                    box.Add(new Vector2(box[box.Count - 1].x, box[box.Count - 1].y + (length * tileSize)));
                    starty += length - 1;
                    if (Array.BinarySearch(islandX, new Tile(islandY[starty].x - 1, islandY[starty].y, islandY[starty].type)) > 0)
                        direction = "+x";
                    else
                        direction = "-x";
                    startx = Array.BinarySearch(islandX, new Tile(islandY[starty].x, islandY[starty].y, islandY[starty].type));
                    break;
                case "-x":
                    for (length = 0; (startx - length >= 0)
                        && (islandX[startx].y == islandX[startx - length].y)
                        && (islandX[startx].x == islandX[startx - length].x + length)
                        && (Array.BinarySearch(islandY, new Tile(islandX[startx - length].x, islandX[startx - length].y + 1, islandX[startx].type)) < 0); length++);
                    box.Add(new Vector2(box[box.Count - 1].x - (length * tileSize), box[box.Count - 1].y));
                    startx -= length;
                    if (Array.BinarySearch(islandY, new Tile(islandX[startx].x, islandX[startx].y + 1, islandX[startx].type)) > 0)
                        direction = "+y";
                    else
                        direction = "-y";
                    starty = Array.BinarySearch(islandY, new Tile(islandX[startx].x, islandX[startx].y, islandX[startx].type));
                    break;
                case "-y":
                    for (length = 0; (starty - length >= 0)
                        && (islandY[starty].x == islandY[starty - length].x)
                        && (islandY[starty].y == islandY[starty - length].y + length)
                        && (Array.BinarySearch(islandX, new Tile(islandY[starty - length].x + 1, islandY[starty - length].y, islandY[starty].type)) < 0); length++);
                    box.Add(new Vector2(box[box.Count - 1].x, box[box.Count - 1].y - (length * tileSize)));
                    starty -= length;
                    if (Array.BinarySearch(islandX, new Tile(islandY[starty].x + 1, islandY[starty].y, islandY[starty].type)) > 0)
                        direction = "-x";
                    else
                        direction = "+x";
                    startx = Array.BinarySearch(islandX, new Tile(islandY[starty].x, islandY[starty].y, islandY[starty].type));
                    break;
            }
        } while (box[box.Count - 1] != box[0] && box.Count < 1000);
        box.RemoveAt(box.Count - 1);
        return box.ToArray();
    }

    //for testing purposes only, remove when GetBox functions properly
    Vector2[] GetBax(Tile[] island, float tileSize)
    {
        List<Vector2> box = new List<Vector2>();
        box.Add(new Vector2(0, 0));
        box.Add(new Vector2(1, 0));
        box.Add(new Vector2(1, 1));
        box.Add(new Vector2(0, 1));
        return box.ToArray();
    }
}