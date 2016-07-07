﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TileCollision : MonoBehaviour {

    public int splitX;
    public int splitY;
    public Tile.TYPE[] solid;
    public Tile.TYPE[] air;
    public GameObject ropePrefab;

    // remove all of the edge colliders that were already on the gameObject
    public void RemoveCollision()
    {
        if (Application.isEditor)
        {
            foreach (GameObject rope in GameObject.FindGameObjectsWithTag("Rope"))
                DestroyImmediate(rope.gameObject);
            foreach (EdgeCollider2D edge in GetComponents<EdgeCollider2D>())
            {
                DestroyImmediate(edge);
            }
        }
        else
        {
            foreach (GameObject rope in GameObject.FindGameObjectsWithTag("Rope"))
                Destroy(rope.gameObject);
            foreach (EdgeCollider2D edge in GetComponents<EdgeCollider2D>())
            {

                Destroy(edge);
            }
        }
    }

    public void BuildColliderFast(TileMap tileMap, float tileSize)
    {
        TileMap[] maps = tileMap.Split(splitX, splitY);

        RemoveCollision();

        foreach (TileMap map in maps)
        {
            BuildCollider(map, tileSize, false);
        }
    }

    public void BuildCollider(TileMap tileMap, float tileSize, bool bRemoveColliders)
    {
        // remove all of the edge colliders that were already on the gameObject
        if(bRemoveColliders)
            RemoveCollision();

        //run through percolation of all tiles, set collision for all islands
        ArrayList doneTiles = new ArrayList();
        int x = 0;
        int y = 0;
        while (tileMap.sizeX * tileMap.sizeY > doneTiles.Count || y < tileMap.sizeY)
        {
            if (!(doneTiles.Contains(tileMap.GetTile(x, y))))
            {
                Tile[] island = tileMap.Percolate(x, y, solid);

                if (island.Length > 0) // if island returned empty, our start must be air.
                {
                    EdgeCollider2D edge_collider = gameObject.AddComponent<EdgeCollider2D>();
                    edge_collider.hideFlags = HideFlags.HideInInspector;
                    edge_collider.points = GetBox(island, tileSize);
                }
                else
                {
                    island = tileMap.Percolate(x, y, air);
                    //EdgeCollider2D edge_collider = gameObject.AddComponent<EdgeCollider2D>();
                    //edge_collider.hideFlags = HideFlags.HideInInspector;
                    //edge_collider.points = GetBox(island, tileSize);
                }

                foreach (Tile tile in island)
                {
                    doneTiles.Add(tile);
                }
            }
            x++;
            if (x >= tileMap.sizeX)
            {
                x = 0;
                y++;
            }
        }
    }

    Vector2[] GetBox(Tile[] islandY, float tileSize)
    {
        // Get another array to sort on the other axis
        Tile[] islandX = new Tile[islandY.Length];

        for (int i = 0; i < islandY.Length; i++)
        {
            islandY[i].SetCompare('X');
        }

        // Copy the array and swap the Comparison method so ICompare interface can binary search properly
        for (int i = 0; i < islandX.Length; i++)
        {
            islandX[i] = new Tile(islandY[i].x, islandY[i].y, islandY[i].type);
            islandX[i].SetCompare('Y');
        }

        Array.Sort(islandY);
        Array.Sort(islandX);

        // Start the collision box with the first point
        List<Vector2> box = new List<Vector2>();
        box.Add(new Vector2(islandX[0].x * tileSize, islandX[0].y * tileSize));

        // Initialize some helpers
        String direction = "+x";
        int length = 0;
        int startx = 0;
        int starty = 0;
        // The crazy loop of doom. Ask Marcus if questions. TODO: probably room for optimization.
        do
        {
            switch (direction)
            {
                case "+x":
                    for (length = 0; (startx + length < islandX.Length)
                        && (islandX[startx].y == islandX[startx + length].y)
                        && (islandX[startx].x == islandX[startx + length].x - length)
                        && (Array.BinarySearch(islandX, new Tile(islandX[startx + length].x, islandX[startx + length].y - 1, islandX[startx].type, 'Y')) < 0); length++);
                    box.Add(new Vector2(box[box.Count - 1].x + (length * tileSize), box[box.Count - 1].y));
                    length--;
                    if (Array.BinarySearch(islandX, new Tile(islandX[startx + length].x + 1, islandX[startx].y - 1, islandX[startx].type, 'Y')) >= 0)
                    {
                        startx = Array.BinarySearch(islandX, new Tile(islandX[startx + length].x + 1, islandX[startx].y - 1, islandX[startx].type, 'Y'));
                        direction = "-y";
                    }
                    else
                    {
                        startx += length;
                        direction = "+y";
                    }
                    starty = Array.BinarySearch(islandY, new Tile(islandX[startx].x, islandX[startx].y, islandX[startx].type));
                    break;
                case "+y":
                    for (length = 0; (starty + length < islandY.Length)
                        && (islandY[starty].x == islandY[starty + length].x)
                        && (islandY[starty].y == islandY[starty + length].y - length)
                        && (Array.BinarySearch(islandY, new Tile(islandY[starty + length].x + 1, islandY[starty + length].y, islandY[starty].type)) < 0); length++) ;
                    box.Add(new Vector2(box[box.Count - 1].x, box[box.Count - 1].y + (length * tileSize)));
                    length--;
                    if (Array.BinarySearch(islandY, new Tile(islandY[starty].x + 1, islandY[starty + length].y + 1, islandY[starty].type)) >= 0)
                    {
                        starty = Array.BinarySearch(islandY, new Tile(islandY[starty].x + 1, islandY[starty + length].y + 1, islandY[starty].type));
                        direction = "+x";
                    }
                    else
                    {
                        starty += length;
                        direction = "-x";
                    }
                    startx = Array.BinarySearch(islandX, new Tile(islandY[starty].x, islandY[starty].y, islandY[starty].type, 'Y'));
                    break;
                case "-x":
                    for (length = 0; (startx - length >= 0)
                        && (islandX[startx].y == islandX[startx - length].y)
                        && (islandX[startx].x == islandX[startx - length].x + length)
                        && (Array.BinarySearch(islandX, new Tile(islandX[startx - length].x, islandX[startx - length].y + 1, islandX[startx].type, 'Y')) < 0); length++) ;
                    box.Add(new Vector2(box[box.Count - 1].x - (length * tileSize), box[box.Count - 1].y));
                    length--;
                    if (Array.BinarySearch(islandX, new Tile(islandX[startx - length].x - 1, islandX[startx].y + 1, islandX[startx].type, 'Y')) >= 0)
                    {
                        startx = Array.BinarySearch(islandX, new Tile(islandX[startx - length].x - 1, islandX[startx].y + 1, islandX[startx].type, 'Y'));
                        direction = "+y";
                    }
                    else
                    {
                        startx -= length;
                        direction = "-y";
                    }
                    starty = Array.BinarySearch(islandY, new Tile(islandX[startx].x, islandX[startx].y, islandX[startx].type));
                    break;
                case "-y":
                    for (length = 0; (starty - length >= 0)
                        && (islandY[starty].x == islandY[starty - length].x)
                        && (islandY[starty].y == islandY[starty - length].y + length)
                        && (Array.BinarySearch(islandY, new Tile(islandY[starty - length].x - 1, islandY[starty - length].y, islandY[starty].type)) < 0); length++) ;
                    box.Add(new Vector2(box[box.Count - 1].x, box[box.Count - 1].y - (length * tileSize)));
                    length--;
                    if (Array.BinarySearch(islandY, new Tile(islandY[starty].x - 1, islandY[starty - length].y - 1, islandY[starty].type)) >= 0)
                    {
                        starty = Array.BinarySearch(islandY, new Tile(islandY[starty].x - 1, islandY[starty - length].y - 1, islandY[starty].type));
                        direction = "-x";
                    }
                    else
                    {
                        starty -= length;
                        direction = "+x";
                    }
                    startx = Array.BinarySearch(islandX, new Tile(islandY[starty].x, islandY[starty].y, islandY[starty].type, 'Y'));
                    break;
            }
        } while (box[box.Count - 1] != box[0] && box.Count < 4000);
        return box.ToArray();
    }

    public void BuildRopeColliders(TileMap tileMap, float tileSize)
    {

        //run through percolation of all tiles, set collision for all ropes
        ArrayList doneTiles = new ArrayList();
        int x = 0;
        int y = 0;
        while (tileMap.sizeX * tileMap.sizeY > doneTiles.Count || y < tileMap.sizeY)
        {
            if (!(doneTiles.Contains(tileMap.GetTile(x, y))))
            {
                if(tileMap.GetTile(x, y).type == Tile.TYPE.rope)
                {
                    List<Tile> rope = new List<Tile>();
                    Vector2[] ropeLine = new Vector2[2];

                    rope.Add(tileMap.GetTile(x, y));
                    ropeLine[0] = Vector2.zero;
                    ropeLine[1] = new Vector2(ropeLine[0].x, ropeLine[0].y + tileSize / 3);

                    int i = 1;
                    while(tileMap.GetTile(x, y + i).type == Tile.TYPE.rope)
                    {
                        rope.Add(tileMap.GetTile(x, y + i));
                        ropeLine[1] = new Vector2(ropeLine[1].x, ropeLine[1].y + tileSize);
                        i++;
                    }

                    GameObject child_object = GameObject.Instantiate<GameObject>(ropePrefab);
                    child_object.transform.SetParent(transform);
                    child_object.transform.localPosition = new Vector3(x * tileSize + tileSize / 2, y * tileSize + tileSize / 3, 0);
                    child_object.GetComponent<EdgeCollider2D>().points = ropeLine;

                    GameObject child_child_object = child_object.transform.GetChild(0).gameObject;
                    child_child_object.transform.localPosition = new Vector3(-tileSize / 2, i * tileSize - tileSize / 3, 0);
                    child_child_object.GetComponent<EdgeCollider2D>().points = new Vector2[] { Vector2.zero, new Vector2(tileSize, 0) };

                    foreach (Tile tile in rope)
                    {
                        doneTiles.Add(tile);
                    }
                }
                else
                {
                    doneTiles.Add(tileMap.GetTile(x, y));
                }
            }
            x++;
            if (x >= tileMap.sizeX)
            {
                x = 0;
                y++;
            }
        }
    }
}