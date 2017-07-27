using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GridMeshBuilder : MonoBehaviour {

    public bool meshBuilt = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateMesh(Cell[, ,] newGeneration, short size_X, short size_Y, short size_Z)
    {
 
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (short z = 0; z <= size_Z-1; z++)
        {
            for (short x = 0; x <= size_X - 1; x++)
            {
                for (short y = 0; y <= size_Y - 1; y++)
                {
                    if (newGeneration[x, y, z].cellState == false) continue;
                    BuildCube(newGeneration, new Vector3(x, y, z), vertices, indices, uvs, size_X, size_Y, size_Z);
                }
            }
        }
 

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        GetComponent<BoxCollider>().size = new Vector3(size_X, size_Y, size_Z);
        GetComponent<BoxCollider>().center = new Vector3(size_X/2f, size_Y/2f, size_Z/2f);
        GetComponent<MeshFilter>().mesh = mesh;
        meshBuilt = true;
    }

    private void BuildCube(Cell[, ,] newGeneration, Vector3 position, List<Vector3> vertices, List<int> indices, List<Vector2> uvs, short size_X, short size_Y, short size_Z)
    {
        if (!CullFace(newGeneration, new Vector3(position.x - 1, position.y, position.z), size_X, size_Y, size_Z))
        {
            BuildFace(position, Vector3.up, Vector3.forward, false, vertices, indices, uvs);
        }
        if (!CullFace(newGeneration, new Vector3(position.x + 1, position.y, position.z), size_X, size_Y, size_Z))
        {
            BuildFace(new Vector3(position.x + 1, position.y, position.z), Vector3.up, Vector3.forward, true, vertices, indices, uvs);
        }

        BuildFace(new Vector3(position.x, position.y + 1, position.z), Vector3.forward, Vector3.right, true, vertices, indices, uvs);
        if (!CullFace(newGeneration, new Vector3(position.x, position.y, position.z-1), size_X, size_Y, size_Z))
        {
            BuildFace(new Vector3(position.x, position.y, position.z), Vector3.up, Vector3.right, true, vertices, indices, uvs);
        }
        if (!CullFace(newGeneration, new Vector3(position.x, position.y, position.z+1), size_X, size_Y, size_Z))
        {
            BuildFace(new Vector3(position.x, position.y, position.z + 1), Vector3.up, Vector3.right, false, vertices, indices, uvs);
        }
    }

    private void BuildFace(Vector3 corner,Vector3 up, Vector3 right, bool reversed, List<Vector3> vertices, List<int> indices, List<Vector2> uvs)
    {
        int index = vertices.Count;
        vertices.Add(corner);
        vertices.Add(corner + up);
        vertices.Add(corner + up + right);
        vertices.Add(corner + right);

        uvs.Add(new Vector2(0f, 0f));
        uvs.Add(new Vector2(0f, 1f));
        uvs.Add(new Vector2(1f, 1f));
        uvs.Add(new Vector2(1f, 0f));

        if (reversed)
        {
            indices.Add(index + 0);
            indices.Add(index + 1);
            indices.Add(index + 2);

            indices.Add(index + 2);
            indices.Add(index + 3);
            indices.Add(index + 0);
        }
        else
        {
            indices.Add(index + 1);
            indices.Add(index + 0);
            indices.Add(index + 2);

            indices.Add(index + 3);
            indices.Add(index + 2);
            indices.Add(index + 0);
        }
    }

    public bool CullFace(Cell[, ,] grid, Vector3 position, short size_X, short size_Y, short size_Z)
    {
        if (position.x < 0 || position.x > size_X - 1 || position.y < 0 || position.y > size_Y - 1 || position.z < 0 || position.z > size_Z - 1)
        {
            return false;
        }
        return grid[(short)position.x, (short)position.y, (short)position.z].cellState;
    }

    public Mesh GetMesh()
    {
        return GetComponent<MeshFilter>().mesh;
    }

    public void SetMesh(Mesh mesh)
    {
        GetComponent<MeshFilter>().mesh = mesh;
        meshBuilt = true;
    }
}
