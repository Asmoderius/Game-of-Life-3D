using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour
{
    public string seed;
    public float delay = 100f;
    public short size_X, size_Y, size_Z;
    public bool randomBoard = false;
    public bool loopEdges = true;
    public short startDensity = 1;
    public bool topLayer = false;
    public Grid nextGrid;
    private Cell[, ,] grid;
    private bool paused;
    private float timeStamp = 0f;

    // Use this for initialization
    void Start()
    {
        if (topLayer)
        {
            if (!seed.Equals(string.Empty))
            {
                Random.seed = seed.GetHashCode();
            }
            else
            {
                Random.seed = Random.Range(0, 10000);
            }
            this.grid = new Cell[size_X, size_Y, size_Z];
            this.paused = false;
            if (randomBoard)
            {
                InitializeRandomGrid();
            }
            else
            {
                grid = InitializeEmptyGrid(grid);
            }
            GetComponent<GridMeshBuilder>().UpdateMesh(grid, size_X, size_Y, size_Z);
            timeStamp = Time.fixedTime;
          
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (topLayer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                paused = !paused;
            }
        }
    }

    void FixedUpdate()
    {
        if(topLayer)
        {
            if(!paused)
            {
                float checkTime = Time.fixedTime;
                if (checkTime - timeStamp > delay)
                {
                    NextGeneration();
                    timeStamp = checkTime;
                }
            }
  
        }

    }

    private Cell[,,] InitializeEmptyGrid(Cell[,,] generation)
    {

        for (short z = 0; z <= size_Z - 1; z++)
        {
            for (short x = 0; x <= size_X - 1; x++)
            {
                for (short y = 0; y <= size_Y - 1; y++)
                {
                    generation[x, y, z] = new Cell(false, 0);
                }
            }
        }
        return generation;
    }

    public void InitializeRandomGrid()
    {
        for (short z = 0; z <= size_Z - 1; z++)
        {
            for (short x = 0; x <= size_X - 1; x++)
            {
                for (short y = 0; y <= size_Y - 1; y++)
                {
                    bool state = Random.Range(0, startDensity) == 0;
                    grid[x, y, z] = new Cell(state, 0);
                }
            }
        }
        NextGeneration();
    }


    public bool SetCell(bool state, short grid_X, short grid_Y, short grid_Z)
    {
        if (grid_X < 0 || grid_X > (short)(size_X - 1) || grid_Y < 0 || grid_Y > (short)(size_Y - 1) || grid_Z < 0 || grid_Z > (short)(size_Z - 1))
        {
            return false;
        }
        else
        {
            grid[grid_X, grid_Y, grid_Z].cellState = state;
            return true;
        }
    }

    public bool GetCellState(short grid_X, short grid_Y, short grid_Z)
    {
        return grid[grid_X, grid_Y, grid_Z].cellState;
    }

    public short GetCellAge(short grid_X, short grid_Y, short grid_Z)
    {
        return grid[grid_X, grid_Y, grid_Z].cellAge;
    }

    public void UpdateMesh()
    {
        GetComponent<GridMeshBuilder>().UpdateMesh(grid, size_X, size_Y, size_Z);
    }

    public void NextGeneration()
    {

            Cell[, ,] newGeneration = new Cell[size_X, size_Y, size_Z];
            newGeneration = InitializeEmptyGrid(newGeneration);
            for (short z = 0; z <= size_Z - 1; z++)
            {
                for (short x = 0; x <= size_X - 1; x++)
                {
                    for (short y = 0; y <= size_Y - 1; y++)
                    {
                        Cell c = grid[x, y, z];
                        short neighbourCount = CountNeighbours(x, y, z);
                        newGeneration[x, y, z].cellState = (c.cellState && (neighbourCount == 2 || neighbourCount == 3)) || (!c.cellState && neighbourCount == 3);
                  
                    }
                }
            }
            UpdateHistory();
            grid = newGeneration;
            GetComponent<GridMeshBuilder>().UpdateMesh(this.grid, size_X, size_Y, size_Z);
        
    }

    private void UpdateHistory()
    {
        if (nextGrid != null && GetComponent<GridMeshBuilder>().meshBuilt)
        {
            nextGrid.SetMesh(this.GetMesh());
        }
    }

    private short CountNeighbours(short x, short y, short z)
    {
        short neighbourCount = 0;

        for (var j = -1; j <= 1; j++)
        {
            if (!loopEdges && z + j < 0 || z + j >= size_Z)
            {
                continue;
            }

            int k = (z + j + size_Z) % size_Z;
            for (var i = -1; i <= 1; i++)
            {
                if (!loopEdges && x + i < 0 || x + i >= size_X)
                {
                    continue;
                }
                int h = (x + i + size_X) % size_X;
                neighbourCount += (short)(grid[h,0, k].cellState ? 1 : 0);
            }
        }
        return (short)(neighbourCount - (grid[x, 0, z].cellState ? 1 : 0));
    }

    public Mesh GetMesh()
    {
        return GetComponent<GridMeshBuilder>().GetMesh();
    }

    public void SetMesh(Mesh mesh)
    {
        if (nextGrid != null && GetComponent<GridMeshBuilder>().meshBuilt)
        {
            nextGrid.SetMesh(this.GetMesh());
            GetComponent<GridMeshBuilder>().SetMesh(mesh);
        }
        else
        {
            GetComponent<GridMeshBuilder>().SetMesh(mesh);
        }

    }
 

}
