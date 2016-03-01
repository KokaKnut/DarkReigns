using UnityEngine;
using System.Collections;

public class TileMap : MonoBehaviour {

    [Header("Size of Tile Map")]
    public int sizeX = 0;
    public int sizeY = 0;
    public float tileSize = 1f;
    //[Space(10)]

	// Use this for initialization
	void Start () {
        BuildMesh();
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
        //caluculate all verts
        for (y = 0; y < vsizeY; y++)
        {
            for (x = 0; x < vsizeX; x++)
            {
                verticies[y * vsizeX + x] = new Vector3(x * tileSize, y * tileSize, 0);
                normals[y * vsizeX + x] = Vector3.back;
                uvs[y * vsizeX + x] = new Vector2((float)x / vsizeX, (float)y / vsizeY);
            }
        }
        //calculate all tris
        PolygonCollider2D poly_collider = GetComponent<PolygonCollider2D>();
        if (poly_collider == null)
            Debug.Log("box");

        for (y = 0; y < sizeY; y++)
        {
            for (x = 0; x < sizeX; x++)
            {
                int squareIndex = y * sizeX + x;
                int triOffset = squareIndex * 6;
                triangles[triOffset + 0] = y * vsizeX + x + 0;
                triangles[triOffset + 1] = y * vsizeX + x + vsizeX + 0;
                triangles[triOffset + 2] = y * vsizeX + x + vsizeX + 1;

                triangles[triOffset + 3] = y * vsizeX + x + 0;
                triangles[triOffset + 4] = y * vsizeX + x + vsizeX + 1;
                triangles[triOffset + 5] = y * vsizeX + x + 1;

                Vector2[] box = new Vector2[4];
                box[0] = new Vector2(verticies[triangles[triOffset + 0]].x, verticies[triangles[triOffset + 0]].y);
                box[1] = new Vector2(verticies[triangles[triOffset + 1]].x, verticies[triangles[triOffset + 1]].y);
                box[2] = new Vector2(verticies[triangles[triOffset + 2]].x, verticies[triangles[triOffset + 2]].y);
                box[3] = new Vector2(verticies[triangles[triOffset + 5]].x, verticies[triangles[triOffset + 5]].y);
                Debug.Log("box: " + x + " |paths: " + poly_collider.pathCount);
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
        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();

        mesh_filter.mesh = mesh;
    }

}
