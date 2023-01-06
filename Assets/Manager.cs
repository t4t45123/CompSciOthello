using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class pieces {
        public GameObject piece;
        public int colour;
        public Vector2 position;
        public pieces (GameObject Piece, int col, Vector2 pos) {
            piece = Piece;
            colour = col;
        }
    }

public class Manager : MonoBehaviour
{
    // vars
    public Material[] pieceMat = new Material[2];
    [SerializeField] Material blackMat;
    [SerializeField] Material whiteMat;
    [SerializeField] Transform whiteParent;
    [SerializeField] Transform blackParent;
    [SerializeField] GameObject piece;
    public pieces[,] pieceArr = new pieces[8,8];
    public Vector2 currentMousePos;
    public int currentTurn = 0;

    // scripts
    [SerializeField] HoverChecker hoverChecker;
    
    void Start()
    {
        
    }
    void Update() // called every frame
    {
        posSetter();
    }
    public void posSetter() { // sets the current mouse position var to the location on the board that it is currently hovering over.
        Cell cell = hoverChecker.HoverObject.GetComponentInParent<Cell>();
        if (cell) {
            currentMousePos = cell.cellPos;
        }
    }
    void Awake() { // called at te very beginning, and initializes the piece array with null instances of a class of pieces.
        pieces tempPiece = new pieces(null, 0 , new Vector2 (-1, -1));
        
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                pieceArr[x,y] = tempPiece;
            }
        }
        pieceMat[0] = whiteMat;
        pieceMat[1] = blackMat;
    }
    public void instancePiece(Vector2 pos, int colour) {
        GameObject lastCreated = Instantiate(piece ,new Vector3 ( pos.x,1.5f, pos.y), Quaternion.identity, currentTurn == 0 ? whiteParent : blackParent); // creates a new peice based on the inputed location.
        lastCreated.GetComponent<Renderer>().material = pieceMat[colour]; // sets the colour of the piece to the colour inputed.
        
    }
    public void ChangeTurn(int turn) { // changes the current tune var to the opposite turn
        currentTurn = turn == 0 ? 1 : 0;
    }
}
