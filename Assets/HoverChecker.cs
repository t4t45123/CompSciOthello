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
            boardManager.mainPlace(boardManager.currentMousePos, boardManager.currentTurn, boardManager.pieceArr);
            //boardManager.ChangeTurn(boardManager.currentTurn);
        }
        if (Input.GetMouseButtonUp(1)) {
            Debug.Log (boardManager.LogBoard(boardManager.startBoard));
        }
    }
}
