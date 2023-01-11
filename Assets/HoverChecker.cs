using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverChecker : MonoBehaviour
{
    public Transform HoverTransform;
    public GameObject HoverObject;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] Manager boardManager;
    Ray ray;
    RaycastHit hit;
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        if (Physics.Raycast(ray, out hit, hitLayer)) {
            HoverTransform = hit.transform;
            HoverObject = hit.transform.gameObject;
        }
        if (Input.GetMouseButtonDown(0) && Physics.Raycast (ray, out hit, hitLayer) && boardManager.CheckPlace360(boardManager.currentMousePos, boardManager.currentTurn, boardManager.pieceArr)) { // checks for a mouse input to place.
            boardManager.instancePiece(boardManager.currentMousePos, boardManager.currentTurn, boardManager.pieceArr);
            boardManager.ChangeTurn(boardManager.currentTurn);
        }
        if (Input.GetMouseButtonUp(1)) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    Debug.Log(boardManager.pieceArr[i,j].colour);
                }
            }
        }
    }
}
