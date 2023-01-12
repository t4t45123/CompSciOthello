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
    public pieces[,] startBoard = new pieces[8,8];
    public Vector2 currentMousePos;
    public int currentTurn = 0;

    // scripts
    [SerializeField] HoverChecker hoverChecker;
    
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
        pieces tempPiece = new pieces(null, 9 , new Vector2 (-1, -1));
        
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                pieceArr[x,y] = tempPiece;
            }
        }
        pieceMat[0] = whiteMat;
        pieceMat[1] = blackMat;
    }
    public GameObject instancePiece(Vector2 pos, int colour, pieces[,] board) {
        GameObject lastCreated = Instantiate(piece ,new Vector3 ( pos.x,1.5f, pos.y), Quaternion.identity, colour == 0 ? whiteParent : blackParent); // creates a new peice based on the inputed location.
        lastCreated.GetComponent<Renderer>().material = pieceMat[colour]; // sets the colour of the piece to the colour inputed.
        board[(int)pos.x,(int)pos.y] = new pieces(lastCreated, colour,pos);
        return lastCreated;
    }
    public void mainPlace(Vector2 pos, int colour, pieces[,] board) {
        if (CheckPlace360(pos, colour,board)) {
        GameObject lastCreated = Instantiate(piece ,new Vector3 ( pos.x,1.5f,pos.y),Quaternion.identity, colour == 0? whiteParent : blackParent);
        lastCreated.GetComponent<Renderer>().material = pieceMat[colour]; 
        board[(int)pos.x,(int)pos.y] = new pieces (lastCreated, colour,pos);
        
        ChangeTurn(colour);    
        }
        for (int i =0; i < 8; i++) {
            placeCheck check = CheckPlace(i, pos, colour, board);
            changePeices(check,board);
        }
    }
    public void ChangeTurn(int turn) { // changes the current tune var to the opposite turn
        currentTurn = turn == 0 ? 1 : 0;
    }
    Vector2 intToDir(int dir) { // returns a vector based on a int that is inputed
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
            if (pos.x >= 0 && pos.x <= 7 && 0 <= pos.y && pos.y <= 7) {
                coloursInRow[i] = board[(int)pos.x, (int)pos.y].colour;
            }
            pos += intToDir(dir);
        }
        string tempString = "";
        foreach (int item in coloursInRow) {
            tempString += item;
        }
        //Debug.Log(tempString + "\n" + dir + "\n\n");
        return coloursInRow;
    }
    public placeCheck CheckPlace(int dir, Vector2 pos, int colour, pieces[,] board) // checks if there is a piece that outflanks another peice.
    {
        int[] row = new int[8];
        int oppCol = colour == 1?0:1;
        placeCheck tempPlaceCheck = new placeCheck(false,0,pos,dir,colour);

        row = getLine(dir, pos, board);
        if (row[1] == 9) {
            return tempPlaceCheck;
        }
        else if(row[1] == oppCol) {
            bool notFound = true;
            int i = 2;
            while(notFound && i < 8) {
                if (row[i] == colour) {
                    notFound = false;
                    placeCheck placecheck = new placeCheck(true, i , pos,dir,colour);
                    return placecheck;
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
    public bool CheckPlace360 (Vector2 pos,int colour, pieces[,] board) { // performs the checkplace function for each direction.
        for (int i = 0; i < 8; i++) {
            if (CheckPlace(i,pos, colour, board).canPlace) {
                return true;
            }
        }
        return false;
    }
    void changePeices(placeCheck check, pieces[,] board)  {
        Vector2 tempPos = check.startPos;
        for (int i = 0; i < check.endPos; i++) {
            board[(int)tempPos.x, (int)tempPos.y].colour = check.Colour;
            board[(int)tempPos.x,(int)tempPos.y].piece.transform.GetComponent<Renderer>().material = check.Colour == 0 ? whiteMat:blackMat; 
            tempPos += intToDir(check.direction);

        }
    }
    void delPiece(pieces[,] board, Vector2 pos) {
        if (board[(int)pos.x,(int)pos.y].piece != null) {
            Destroy(board[(int)pos.x,(int)pos.y].piece);
            board[(int)pos.x,(int)pos.y] = new pieces (null, 9, new Vector2(-1,-1));
        }
    }
    public void delBoard(pieces[,] board) {
        for (int i = 0; i < 8; i++) { 
            for (int j = 0; j < 8; j++) {
                delPiece(board, new Vector2(i,j));
            }
        }
    }
    
    public void loadBoard(pieces[,] newBoard, pieces[,] oldBoard) {
        delBoard(oldBoard);
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (newBoard[i,j].colour != 9) {
                    instancePiece(new Vector2(i,j), newBoard[i,j].colour, pieceArr);
                }
            }
        }

    }
    
    public string LogBoard(pieces[,] board) {
        string output = "";
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (j == 0) {
                    output = output + "\n";
                }
                output = (output + board[j,i].colour + " ");
            }
        }
        return output;
    }
    
    public void saveBoard(pieces[,] newBoard, pieces[,] oldBoard) {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                newBoard[i,j] = new pieces(null, oldBoard[i,j].colour, oldBoard[i,j].position);
            }
        }
    }
    
}
