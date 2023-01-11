using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject cell;
    Vector3 cellPos;
    public Transform cellParent;
    public Manager boardManager;
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
        boardManager.instancePiece (new Vector2(3,3), 1, boardManager.pieceArr);
        boardManager.instancePiece (new Vector2(3,4), 0, boardManager.pieceArr);
        boardManager.instancePiece (new Vector2(4,3), 0, boardManager.pieceArr);
        boardManager.instancePiece (new Vector2(4,4), 1, boardManager.pieceArr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
