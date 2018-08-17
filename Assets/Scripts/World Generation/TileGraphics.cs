using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TileGraphics : MonoBehaviour {

    const int SPRITE_MAX = 2048; //2048
    const int MESH_MAX = 2048; //arbitrary

    public GameObject spriteRendererPrefab;
    public GameObject meshRendererPrefab;

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

    public void RemoveMeshes()
    {
        if (Application.isEditor)
            foreach (MeshRenderer spriteR in GetComponentsInChildren<MeshRenderer>())
            {
                if (spriteR.gameObject != gameObject)
                    DestroyImmediate(spriteR.gameObject);
            }
        else
            foreach (MeshRenderer spriteR in GetComponentsInChildren<MeshRenderer>())
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

    public void MeshToggle(bool on)
    {
        if (on)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
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
                Color[] p = sprite.texture.GetPixels((int)(sprite.textureRect.x), (int)(sprite.textureRect.y), tileDefs.tileResolution, tileDefs.tileResolution); //TODO: switch to Color32
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
    
    public void BuildMeshOld(TileMap tileMap, float tileSize)
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

    public void BuildMesh(TileMap tileMap, float tileSize)
    {
        Texture2D texture = BuildTexture(tileMap, tileSize);
        //sizeX and sizeY were set in BuildTexture call

        RemoveMeshes();
                
        if (sizeX * tileDefs.tileResolution <= MESH_MAX && sizeY * tileDefs.tileResolution <= MESH_MAX)
        {
            MeshToggle(true);

            //Generate the mesh data
            Vector3[] verticies = new Vector3[4];
            Vector3[] normals = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            int[] triangles = new int[6];

            verticies[0] = transform.position;
            verticies[1] = new Vector3(transform.position.x + sizeX * tileDefs.tileResolution, transform.position.y, transform.position.z);
            verticies[2] = new Vector3(transform.position.x, transform.position.y + sizeY * tileDefs.tileResolution, transform.position.z);
            verticies[3] = new Vector3(transform.position.x + sizeX * tileDefs.tileResolution, transform.position.y + sizeY * tileDefs.tileResolution, transform.position.z);

            normals[0] = Vector3.back;
            normals[1] = Vector3.back;
            normals[2] = Vector3.back;
            normals[3] = Vector3.back;

            uvs[0] = Vector2.zero;
            uvs[1] = new Vector2(1, 0);
            uvs[2] = new Vector2(0, 1);
            uvs[3] = new Vector2(1, 1);

            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 3;
            triangles[3] = 0;
            triangles[4] = 3;
            triangles[5] = 1;

            //Create new mesh with our data
            Mesh mesh = new Mesh();
            mesh.vertices = verticies;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uvs;

            //Assign our mesh to the GameObject
            MeshFilter mesh_filter = GetComponent<MeshFilter>();
            mesh_filter.mesh = mesh;

            //Assign our texture to an instanced material, then give that to our renderer
            MeshRenderer mesh_rederer = GetComponent<MeshRenderer>();
            Material mat = Material.Instantiate(mesh_rederer.sharedMaterial);
            mat.mainTexture = texture;
            mesh_rederer.material = mat;
        }
        else
        {
            int numX = (sizeX * tileDefs.tileResolution) / MESH_MAX + 1;
            int numY = (sizeY * tileDefs.tileResolution) / MESH_MAX + 1;

            MeshToggle(false);

            for (int y = 0; y < numY; y++)
            {
                for (int x = 0; x < numX; x++)
                {
                    int texWidth = MESH_MAX;
                    if (x == numX - 1)
                        texWidth = (sizeX * tileDefs.tileResolution) % MESH_MAX;
                    int texHeight = MESH_MAX;
                    if (y == numY - 1)
                        texHeight = (sizeY * tileDefs.tileResolution) % MESH_MAX;

                    GameObject child_object = GameObject.Instantiate<GameObject>(meshRendererPrefab);
                    child_object.transform.SetParent(transform);
                    child_object.transform.localPosition = new Vector3((x * MESH_MAX) / (tileDefs.tileResolution / tileSize), (y * MESH_MAX) / (tileDefs.tileResolution / tileSize), transform.position.z);

                    //Generate the mesh data
                    Vector3[] verticies = new Vector3[4];
                    Vector3[] normals = new Vector3[4];
                    Vector2[] uvs = new Vector2[4];
                    int[] triangles = new int[6];

                    verticies[0] = transform.position;
                    verticies[1] = new Vector3(transform.position.x + texWidth / (tileDefs.tileResolution / tileSize), transform.position.y, transform.position.z);
                    verticies[2] = new Vector3(transform.position.x, transform.position.y + texHeight / (tileDefs.tileResolution / tileSize), transform.position.z);
                    verticies[3] = new Vector3(transform.position.x + texWidth / (tileDefs.tileResolution / tileSize), transform.position.y + texHeight / (tileDefs.tileResolution / tileSize), transform.position.z);

                    normals[0] = Vector3.back;
                    normals[1] = Vector3.back;
                    normals[2] = Vector3.back;
                    normals[3] = Vector3.back;

                    uvs[0] = new Vector2(((float)x * MESH_MAX) / texture.width, ((float)y * MESH_MAX) / texture.height);
                    uvs[1] = new Vector2(((float)x * MESH_MAX + texWidth) / texture.width, ((float)y * MESH_MAX) / texture.height);
                    uvs[2] = new Vector2(((float)x * MESH_MAX) / texture.width, ((float)y * MESH_MAX + texHeight) / texture.height);
                    uvs[3] = new Vector2(((float)x * MESH_MAX + texWidth) / texture.width, ((float)y * MESH_MAX + texHeight) / texture.height);

                    triangles[0] = 0;
                    triangles[1] = 2;
                    triangles[2] = 3;
                    triangles[3] = 0;
                    triangles[4] = 3;
                    triangles[5] = 1;

                    //Create new mesh with our data
                    Mesh mesh = new Mesh();
                    mesh.vertices = verticies;
                    mesh.triangles = triangles;
                    mesh.normals = normals;
                    mesh.uv = uvs;

                    //Assign our mesh to the GameObject
                    MeshFilter mesh_filter = child_object.GetComponent<MeshFilter>();
                    mesh_filter.mesh = mesh;

                    //Assign our texture to an instanced material, then give that to our renderer
                    MeshRenderer mesh_rederer = child_object.GetComponent<MeshRenderer>();
                    Material mat = Material.Instantiate(mesh_rederer.sharedMaterial);
                    mat.mainTexture = texture;
                    mesh_rederer.material = mat;
                }
            }
        }
    }

    public Texture2D BuildTexture(TileMap tileMap, float tileSize)
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

        return texture;
    }

    public static Mesh MakeTileMesh(TileMap tileMap, float tileSize)
    {
        int sizeX = tileMap.sizeX;
        int sizeY = tileMap.sizeY;

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

        return mesh;
    }
}
