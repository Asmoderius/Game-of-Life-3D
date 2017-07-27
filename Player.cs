using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


    public float maxInteractionRange = 10f;
    public Grid grid;
    public Selector selector;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxInteractionRange))
        {
            Vector3 position = new Vector3(Mathf.FloorToInt(hit.point.x) + 0.5f, Mathf.FloorToInt(hit.point.y)-0.5f, Mathf.FloorToInt(hit.point.z) + 0.5f);
            selector.transform.position = position;
            if (Input.GetMouseButtonDown(0))
            {
                grid.SetCell(true, (short)position.x, (short)position.y, (short)position.z);
                grid.UpdateMesh();
            }

        }
	}

    public void ToggleRandomBoard()
    {
        grid.InitializeRandomGrid();

    }
}
