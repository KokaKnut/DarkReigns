using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileGraphics : MonoBehaviour {
    
    public TileTextureDefs tileDefs;
    private Dictionary<Tile.TYPE, TileTypeGraphics> tileTypesDefinitions;

    int sizeX = 0;
    int sizeY = 0;

    Color[][] ChopUpTiles()
    {
        int numTilesPerRow = tileDefs.spriteSheet.width / tileDefs.tileResolution;
        int numRows = tileDefs.spriteSheet.height / tileDefs.tileResolution;

        Color[][] tiles = new Color[numTilesPerRow * numRows][];

        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numTilesPerRow; x++)
            {
                tiles[y * numTilesPerRow + x] = tileDefs.spriteSheet.GetPixels(x * tileDefs.tileResolution, y * tileDefs.tileResolution, tileDefs.tileResolution, tileDefs.tileResolution);
            }
        }

        return tiles;
    }

    public void BuildTexture(TileMap tileMap)
    {
        int texWidth = sizeX * tileDefs.tileResolution;
        int texHeight = sizeY * tileDefs.tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHeight);

        Color[][] tiles = ChopUpTiles();

        tileTypesDefinitions = new Dictionary<Tile.TYPE, TileTypeGraphics>();
        foreach (TileTypeGraphics def in tileDefs.tileTypes)
        {
            tileTypesDefinitions[def.type] = def;
        }

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                Color[] p = tiles[tileTypesDefinitions[tileMap.GetTile(x, y).type].top1];
                texture.SetPixels(x * tileDefs.tileResolution, y * tileDefs.tileResolution, tileDefs.tileResolution, tileDefs.tileResolution, p);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterials[0].mainTexture = texture;
    }
    
    public void BuildMesh(TileMap tileMap, float tileSize)
    {
        sizeX = tileMap.sizeX;
        sizeY = tileMap.sizeY;

        int numTiles = sizeX * sizeY;
        int numTris = numTiles * 2;

        int vsizeX = sizeX + 1;
        int vsizeY = sizeY + 1;
        int numVerts = vsizeX * vsizeY;

        //Generate the mesh data
        Vector3[] verticies = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uvs = new Vector2[numVerts];

        int[] triangles = new int[numTris * 3];

        int x, y;
        //Caluculate all verts
        for (y = 0; y < vsizeY; y++)
        {
            for (x = 0; x < vsizeX; x++)
            {
                verticies[y * vsizeX + x] = new Vector3(x * tileSize, y * tileSize, 0);
                normals[y * vsizeX + x] = Vector3.back;
                uvs[y * vsizeX + x] = new Vector2((float)x / sizeX, (float)y / sizeY);
            }
        }
        
        //Calculate all tris
        for (y = 0; y < sizeY; y++)
        {
            for (x = 0; x < sizeX; x++)
            {
                //Triangle section
                int squareIndex = y * sizeX + x;
                int triOffset = squareIndex * 6;
                triangles[triOffset + 0] = y * vsizeX + x + 0;
                triangles[triOffset + 1] = y * vsizeX + x + vsizeX + 0;
                triangles[triOffset + 2] = y * vsizeX + x + vsizeX + 1;

                triangles[triOffset + 3] = y * vsizeX + x + 0;
                triangles[triOffset + 4] = y * vsizeX + x + vsizeX + 1;
                triangles[triOffset + 5] = y * vsizeX + x + 1;
            }
        }

        //Create new mesh with our data
        Mesh mesh = new Mesh();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;

        //Assign our mesh to the GameObject
        MeshFilter mesh_filter = GetComponent<MeshFilter>();
        mesh_filter.mesh = mesh;

        BuildTexture(tileMap);
    }

}
