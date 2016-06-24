using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TileGraphics : MonoBehaviour {

    const int SPRITE_MAX = 2048; //2048

    public GameObject spriteRendererPrefab;

    public TileTextureDefs tileDefs;
    private Dictionary<Tile.TYPE, TileTypeGraphics> tileTypesDefinitions;

    int sizeX = 0;
    int sizeY = 0;

    // remove all of the sprite renderers that were already on the gameObject
    public void RemoveSprites()
    {
        if (Application.isEditor)
            foreach (SpriteRenderer spriteR in GetComponentsInChildren<SpriteRenderer>())
            {
                if(spriteR.gameObject != gameObject)
                    DestroyImmediate(spriteR.gameObject);
            }
        else
            foreach (SpriteRenderer spriteR in GetComponentsInChildren<SpriteRenderer>())
            {
                if (spriteR.gameObject != gameObject)
                    Destroy(spriteR.gameObject);
            }
    }

    public void SpriteToggle(bool on)
    {
        if (on)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void BuildSprite(TileMap tileMap, float tileSize)
    {
        sizeX = tileMap.sizeX;
        sizeY = tileMap.sizeY;

        int texWidth = sizeX * tileDefs.tileResolution;
        int texHeight = sizeY * tileDefs.tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHeight);

        tileTypesDefinitions = new Dictionary<Tile.TYPE, TileTypeGraphics>();
        foreach (TileTypeGraphics def in tileDefs.tileTypes)
        {
            tileTypesDefinitions[def.type] = def;
        }

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                Sprite sprite = tileTypesDefinitions[tileMap.GetTile(x, y).type].sprite;
                Color[] p = sprite.texture.GetPixels((int)(sprite.textureRect.x), (int)(sprite.textureRect.y), tileDefs.tileResolution, tileDefs.tileResolution);
                texture.SetPixels(x * tileDefs.tileResolution, y * tileDefs.tileResolution, tileDefs.tileResolution, tileDefs.tileResolution, p);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        AddTextureToSprite(tileSize, texture);
    }

    private void AddTextureToSprite(float tileSize, Texture2D texture)
    {
        RemoveSprites();

        if (sizeX * tileDefs.tileResolution <= SPRITE_MAX && sizeY * tileDefs.tileResolution <= SPRITE_MAX)
        {
            SpriteToggle(true);
            SpriteRenderer sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
            sprite_renderer.sprite = Sprite.Create(texture, new Rect(0, 0, sizeX * tileDefs.tileResolution, sizeY * tileDefs.tileResolution),
                                                new Vector2(0, 0), tileDefs.tileResolution / tileSize);
        }
        else
        {
            int numX = (sizeX * tileDefs.tileResolution) / SPRITE_MAX + 1;
            int numY = (sizeY * tileDefs.tileResolution) / SPRITE_MAX + 1;

            SpriteToggle(false);

            for (int y = 0; y < numY; y++)
            {
                for(int x = 0; x < numX; x++)
                {
                    int texWidth = SPRITE_MAX;
                    if (x == numX - 1)
                        texWidth = (sizeX * tileDefs.tileResolution) % SPRITE_MAX;
                    int texHeight = SPRITE_MAX;
                    if (y == numY - 1)
                        texHeight = (sizeY * tileDefs.tileResolution) % SPRITE_MAX;

                    GameObject child_object = GameObject.Instantiate<GameObject>(spriteRendererPrefab);
                    child_object.transform.SetParent(transform);
                    child_object.transform.localPosition = new Vector3((x * SPRITE_MAX) / (tileDefs.tileResolution / tileSize), (y * SPRITE_MAX) / (tileDefs.tileResolution / tileSize), transform.position.z);
                    SpriteRenderer sprite_renderer = child_object.GetComponent<SpriteRenderer>();
                    sprite_renderer.sprite = Sprite.Create(texture, new Rect(x * SPRITE_MAX, y * SPRITE_MAX, texWidth, texHeight),
                                                        new Vector2(0, 0), tileDefs.tileResolution / tileSize, 0, SpriteMeshType.FullRect);
                }
            }
        }
    }
    
    //useless, now using sprites
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

        BuildTexture(tileMap, tileSize);
    }

    public void BuildTexture(TileMap tileMap, float tileSize)
    {
        sizeX = tileMap.sizeX;
        sizeY = tileMap.sizeY;

        int texWidth = sizeX * tileDefs.tileResolution;
        int texHeight = sizeY * tileDefs.tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHeight);

        tileTypesDefinitions = new Dictionary<Tile.TYPE, TileTypeGraphics>();
        foreach (TileTypeGraphics def in tileDefs.tileTypes)
        {
            tileTypesDefinitions[def.type] = def;
        }

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                Sprite sprite = tileTypesDefinitions[tileMap.GetTile(x, y).type].sprite;
                Color[] p = sprite.texture.GetPixels((int)(sprite.textureRect.x), (int)(sprite.textureRect.y), tileDefs.tileResolution, tileDefs.tileResolution);
                texture.SetPixels(x * tileDefs.tileResolution, y * tileDefs.tileResolution, tileDefs.tileResolution, tileDefs.tileResolution, p);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        MeshRenderer mesh_rederer = GetComponent<MeshRenderer>();
        Material mat = Material.Instantiate(mesh_rederer.sharedMaterial);
        mat.mainTexture = texture;
        mesh_rederer.sharedMaterial = mat;
    }
}
