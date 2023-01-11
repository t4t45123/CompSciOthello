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
    public class placeCheck {
        public bool canPlace;
        public int endPos;
        public Vector2 startPos;
        public int direction;
        public int Colour;
        public placeCheck (bool placeable, int end, Vector2 start, int dir, int col) {
            canPlace = placeable;
            endPos = end;
            startPos = start;
            direction =  dir;
            Colour = col;
        }
    }
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
    Vector2 intToDir(int dir) {
        switch(dir) {
            case 0:
            return new Vector2 (-1,1);
            case 1:
            return new Vector2 (0,1);
            case 2: 
            return new Vector2 (1,1);
            case 3:
            return new Vector2 (1,0);
            case 4:
            return new Vector2 (1,-1);
            case 5:
            return new Vector2 (0,-1);
            case 6:
            return new Vector2 (-1,-1);
            case 7:
            return new Vector2 (-1,0);
        }
        return new Vector2 (0,0);
    }
    public int[] getLine(int dir, Vector2 pos, pieces[,] board) { // returns an array of ints, which correlate to colours in the row.
        int[] coloursInRow = new int[8];
        for (int i = 0; i < 8; i++) {
            if (pos.x >= 0 && pos.x <= 7 && pos.y >= 0 && pos.y <= 7) {
                coloursInRow[i] = board[(int)pos.x, (int)pos.y].colour;
            }
            pos += intToDir(dir);
        }
        return coloursInRow;
    }
    public placeCheck CheckPlace(int dir, Vector2 pos, int colour, pieces[,] board) 
    {
        int[] row = new int[8];
        int oppCol = colour == 2?1:2;
        placeCheck tempPlaceCheck = new placeCheck(false,0,pos,dir,colour);

        row = getLine(dir, pos, board);
        if (row[1] == 0) {
            return tempPlaceCheck;
        }
        else if(row[1] == oppCol) {
            bool notFound = true;
            int i = 2;
            while(notFound && i < 8) {
                if (row[i] == colour) {
                    notFound = false;
                    placeCheck placecheck = new placeCheck(true, i , pos,dir,colour);
                }
                else if (row[i] == oppCol) {
                    i++;
                }else {
                    return tempPlaceCheck;
                }
            }
        }
        return tempPlaceCheck;

    } 
    public bool CheckPlace360 (Vector2 pos,int colour, pieces[,] board) {
        for (int i = 0; i < 8; i++) {
            if (CheckPlace(i,pos, colour, board).canPlace) {
                return true;
            }
        }
        return false;
    }

}
