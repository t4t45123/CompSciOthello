using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject cell;
    Vector3 cellPos;
    public Transform cellParent;
    
    void Start()
    {
        float cellWidth = cell.transform.localScale.x;
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) { 
                cellPos = new Vector3((cellWidth * i), 1.0f, (cellWidth * j));
                GameObject cellObj = Instantiate(cell, cellPos, Quaternion.identity, cellParent);
                Cell cellScript = cellObj.GetComponent<Cell>();
                cellScript.cellPos = new Vector2(i,j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
