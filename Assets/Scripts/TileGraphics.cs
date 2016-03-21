using UnityEngine;
using System.Collections;

public class TileGraphics : MonoBehaviour {

    [Header("Size of Tile Map")]
    public int sizeX = 0;
    public int sizeY = 0;
    public float tileSize = 1f;
    [Space(10)]
    public Texture2D TileTexture;
    public int tileResolution;

	// Use this for initialization
	void Start () {
        BuildMesh();
	}

    Color[][] ChopUpTiles()
    {
        int numTilesPerRow = TileTexture.width / tileResolution;
        int numRows = TileTexture.height / tileResolution;

        Color[][] tiles = new Color[numTilesPerRow * numRows][];

        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numTilesPerRow; x++)
            {
                tiles[y * numTilesPerRow + x] = TileTexture.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
            }
        }

        return tiles;
    }

    void BuildTexture()
    {
        int texWidth = sizeX * tileResolution;
        int texHeight = sizeY * tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHeight);

        Color[][] tiles = ChopUpTiles();

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                Color[] p = tiles[Random.Range(0, tiles.Length)];
                texture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, p);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterials[0].mainTexture = texture;
    }

    [ContextMenu ("Rebuild Mesh")]
    void BuildMesh()
    {
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
        
        //Prepare for polygon collider setting
        PolygonCollider2D poly_collider = GetComponent<PolygonCollider2D>();
        
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

                //Poly collider section
                Vector2[] box = new Vector2[4];
                box[0] = new Vector2(verticies[triangles[triOffset + 0]].x, verticies[triangles[triOffset + 0]].y);
                box[1] = new Vector2(verticies[triangles[triOffset + 1]].x, verticies[triangles[triOffset + 1]].y);
                box[2] = new Vector2(verticies[triangles[triOffset + 2]].x, verticies[triangles[triOffset + 2]].y);
                box[3] = new Vector2(verticies[triangles[triOffset + 5]].x, verticies[triangles[triOffset + 5]].y);
                poly_collider.pathCount++;
                poly_collider.SetPath(y * sizeX + x, box);
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

        BuildTexture();
    }

}
