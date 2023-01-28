using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class pieces { // pieces class used to hold data about the pieces on a board.
    public GameObject piece;
    public int colour;
    public Vector2 position;
    public pieces (GameObject Piece, int col, Vector2 pos) {
        piece = Piece;
        colour = col;
    }
    }


public class boardState { // class handling board states for the monete carlo tree search class
    Manager manager = new Manager();
    public pieces[,] board = new pieces[8,8];
    public int turn;
    public boardState (pieces[,] Board, int Col) {
        board = Board;
        turn = Col;
    }
    public boardState Move(Vector2 pos) {
        boardState temp = new boardState ((manager.placeOnArray(pos, this.turn, this.board)), this.turn);
        return temp;
    }
    public bool isGameOver() {

        //Debug.Log (manager.LogBoard(this.board));
        //Debug.Log("moveAmount" + manager.getPossibleMoveCount(manager.getPossibleMoves(this.board,this.turn),this.turn,this.board));
        if (manager.getPossibleMoveCount(manager.getPossibleMoves(this.board,this.turn),this.turn,this.board) == 0) {
            return true;
        }else {
            return false;
        }
    }
    public Vector2[] getLegalActions() {
        Vector2[] tempArr = manager.getPossibleMoves(this.board, this.turn);
        Vector2[] legalActions = new Vector2[64];
        int i = 0;
        foreach (Vector2 temp in tempArr) {
            if (temp != new Vector2(-1,-1)) {
                if (this.board[(int)temp.x,  (int)temp.y].colour == 9) {
                    legalActions[i] = (temp);
                }else {
                    legalActions[i] = new Vector2(-1,-1);
                }
            }
            
            i++;
        }
        
        return legalActions;
    }
    public int gameResult() {
        if (manager.getPossibleMoveCount(manager.getPossibleMoves(this.board,this.turn),turn,this.board) > (manager.getPossibleMoveCount(manager.getPossibleMoves(this.board,this.turn == 1 ? 0: 1),this.turn == 1 ? 0: 1,this.board))) {
            return 1;
        }else {
            return 2;
        }
    }

}



    public class monteCarloNode{ // class handling monte carlo nodes
        Manager manager = new Manager();
        public boardState state;
        public monteCarloNode parent = null;
        public Vector2 ParentAction = new Vector2 (-1,-1);
        List <monteCarloNode> children = new List<monteCarloNode>();
        int numberOfVisits = 0;
        int[] results = new int[3];
        bool hasGottenUntriedActions = false;
        public List<Vector2> untriedActions = new List<Vector2>();
        public int itterations = 100;
        
        List<Vector2> performedActions = new List<Vector2>();
        int currentTurn;
        public monteCarloNode(boardState _state, monteCarloNode _parent, Vector2 _parentAction, int _Itterations ) { // constructer for the class
            state = _state;
            parent = _parent;
            ParentAction = _parentAction;
            itterations = _Itterations;
        }
        List<Vector2> untried_Actions() { // all of the possible actions on the board
            this.untriedActions = new List<Vector2>(manager.getPossibleMoves(state.board, state.turn));
            int total = this.untriedActions.Count;
            for (int i = 0; i < total; i++) {
                untriedActions.Remove(new Vector2(-1,-1));
            }
            //Debug.Log(this.untriedActions.Count);
            foreach(Vector2 v in this.untriedActions) {
                //Debug.Log(v);
            }
            return untriedActions;
            }

        int q() { //returns the differnce betweens the wins and losses of the current move
            int wins = this.results[1];
            int losses = this.results[2];
            return wins - losses;
        }
        int n() { // returns the number of visits a piece has
            return this.numberOfVisits;
        }
        monteCarloNode expand() { // expands the tree by creating a new child node, and taking out of the untried actions, and adds that child to the list of children
            //Debug.Log("called Expand");
            if (!hasGottenUntriedActions) {
                this.untriedActions = this.untried_Actions();
                hasGottenUntriedActions = true;
            }
            Vector2 action = this.untriedActions[0];
            this.untriedActions.RemoveAt(0);
            monteCarloNode childNode = null;

            pieces[,] board = new pieces[8,8];
            boardState nextState =  new boardState(board,0);
            for (int i = 0; i <8; i++) {
                for (int j = 0; j < 8; j++) {
                    nextState.board[i,j] =  new pieces(null, this.state.board[i,j].colour, new Vector2 (i,j));
                }
            }
            //nextState.board = this.state.board;
            //nextState = nextState.saveState(nextState, this.state);
            nextState = nextState.Move(action);
            //Debug.Log(manager.LogBoard(nextState.board));
            //Debug.Log(manager.LogBoard(this.state.board));
            //Debug.Log(this.state.board == nextState.board);
            nextState.turn = nextState.turn == 1 ? 2:1;
            childNode = new monteCarloNode(nextState,this, action, itterations);
            this.children.Add (childNode);
            return childNode;
        }
        public bool isTerminalNode() { // returns true if the game is over
            return this.state.isGameOver();
        }
        int rollout() { // used to simulate the game until the game is over from the current state.
            boardState currentRolloutState = this.state;
            while (!currentRolloutState.isGameOver()) {
                Vector2[] possibleMoves = currentRolloutState.getLegalActions();
                Vector2 action = this.rolloutPolicy(possibleMoves);
                //Debug.Log(action);
                currentRolloutState = currentRolloutState.Move(action);
            }
            return currentRolloutState.gameResult();
            
            
        }
        void backpropagate(int result) { // used to go back up the tree and change the results of the node.
            this.numberOfVisits += 1;
            this.results[result] += 1;
            if (this.parent != null){
                this.parent.backpropagate(result);
            }
        }

        bool isFullyExpanded() { // used to check if the tree is fully expanded
            return this.untriedActions.Count == 0;
        }

monteCarloNode bestChild(float cParam = 0.1f) { // used to find the best child out of the list of children.
    
    //this.untriedActions = this.untried_Actions();            
    double temp = -1000f;
    int bestIndex = 0;
    //Debug.Log(this.children.Count + " - child count");
    List<double> choicesWeights = new List<double>();
    foreach (monteCarloNode c in this.children) {
        choicesWeights.Add((c.q() / c.n()) + cParam * Mathf.Sqrt((2*Mathf.Log(this.n()) / c.n())));
        //Debug.Log(c.n() + " n || q " + c.q() + " || " + (c.q() / c.n()) + cParam * Mathf.Sqrt((2*Mathf.Log(this.n()) / c.n())));
    }
    
    for (int i = 0; i < choicesWeights.Count; i++) { // finds the index of the largest number 
        
        if (temp <= choicesWeights[i]) {
            temp = choicesWeights[i];
            bestIndex = i;
        }
    }
    //Debug.Log(choicesWeights.Count + " length");
    //Debug.Log (children[0]);
    //Debug.Log(bestIndex + " - index Chosen");
    return children[bestIndex];
}

Vector2 rolloutPolicy(Vector2[] possibleMoves) { // used to randomly select a move out of the possible moves.
    Vector2 possibleMove = possibleMoves[UnityEngine.Random.Range(0,possibleMoves.Length)];
    return possibleMove;
}

monteCarloNode treePolicy() { // selected a node to rollout
    monteCarloNode current = new monteCarloNode(null,null,new Vector2 (-1,-1), itterations);
    current = this;
    
    //Debug.Log(this.isFullyExpanded() +"fullyExpanded");
    if (!hasGottenUntriedActions) {
        this.untriedActions = this.untried_Actions();
        hasGottenUntriedActions = true;
    }
    // Debug.Log(this.isFullyExpanded() +"fullyExpanded");
    // Debug.Log(this.state.isGameOver() +"is game Over");
    //Debug.Log (manager.LogBoard(this.state.board) + "\n \n" +this.isFullyExpanded() +"fullyExpanded" + "\n \n " +this.state.isGameOver() +"is game Over");
    //Debug.Log(this.ParentAction + "parent action");
    while (! current.isTerminalNode()) {
        if (!current.isFullyExpanded()) {
            return current.expand();
        }else {
            current = current.bestChild();
        }
    }
    return current;
}

public monteCarloNode bestAction() { // find the best action to play.
    monteCarloNode v;
    int simulationNum = itterations;
    for (int i = 0; i < simulationNum; i++) {
        v = this.treePolicy();
        if (v != null) {
            int reward = v.rollout();
            v.backpropagate(reward);
        }
    }
    monteCarloNode finalNode =  this.bestChild(1.4142f);
    //performedActions.Add (finalNode.ParentAction);
    return finalNode;
}
}

public class Manager : MonoBehaviour 
{
    
    public bool isMonteEnabled = false;
    public Vector2 selectedMove;
    public class placeCheck {
        public bool canPlace;
        public int endPos;
        public Vector2 startPos;
        public int direction;
        public int Colour;
        public placeCheck (bool placeable, int end, Vector2 start, int dir, int col) { // place check class used for returning the information to check the pieces that need to be changed, and if a piece can be placed.
            canPlace = placeable;
            endPos = end;
            startPos = start;
            direction =  dir;
            Colour = col;
        }
    }


    // vars
    public GameObject shadow;
    public Transform shadowParent;
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
    int monteItterations;


    // scripts
    [SerializeField] HoverChecker hoverChecker;
    [SerializeField] UserDatabase database;
    boardState state;
    monteCarloNode MonteCarloNode;
    [SerializeField] UImanager ui;
    
    void Update() // called every frame
    {
        if (ui.shadowMoveToggle.isOn) {
            placeShadows(getPossibleMoves(pieceArr,currentTurn));
        }else {
            delShadowChildren();
        }
        posSetter();
        //Debug.Log(getPossibleMoveCount(getPossibleMoves(pieceArr,currentTurn), currentTurn, pieceArr));
        StartCoroutine (winCheck(pieceArr,currentTurn));
        ui.updateCountAndTurn(getPieceCount(pieceArr, 0), getPieceCount(pieceArr,1), currentTurn);
        aiDifficultySelector(ui.difficultyDropdown.value);
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
    public void mainPlace(Vector2 pos, int colour, pieces[,] board, bool changeTurn) {
        if (CheckPlace360(pos, colour,board)) {
            GameObject lastCreated = Instantiate(piece ,new Vector3 ( pos.x,1.5f,pos.y),Quaternion.identity, colour == 0? whiteParent : blackParent);
            lastCreated.GetComponent<Renderer>().material = pieceMat[colour]; 
            board[(int)pos.x,(int)pos.y] = new pieces (lastCreated, colour,pos);
            if (changeTurn){
                ChangeTurn(colour); 
            }
            if (ui.playerSelector.value == 1) {
                Debug.Log("playerSelector == 1");
                if (currentTurn == 1){
                    StartCoroutine(StartAiMove());
                }
                
            }
        }
        for (int i =0; i < 8; i++) {
            placeCheck check = CheckPlace(i, pos, colour, board);
            changePeices(check,board);
            placeShadows(getPossibleMoves(board, colour));
            
        }
        
    }
    
    public pieces[,] placeOnArray(Vector2 pos, int colour, pieces[,] board) {
        if (pos.x >= 0 & pos.x < 8 & pos.y >= 0 & pos.y <8) {

            
            board[(int)pos.x, (int)pos.y] = new pieces(null, colour, pos);
            for (int i = 0; i < 8; i++) {
                placeCheck check  = CheckPlace(i, pos, colour, board);
                changePieceArr(check, board);
            }
        }
        return board;

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
            }else {
                coloursInRow[i] = -1;
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
            if (CheckPlace(i,pos, colour, board).canPlace & board[(int)pos.x,(int)pos.y].colour == 9) { // 9 is the null value for the board.
                return true;
            }
        }
        return false;
    }
    void changePeices(placeCheck check, pieces[,] board)  { // used to change the colour of the pieces when a piece is placed.
        Vector2 tempPos = check.startPos;
        for (int i = 0; i < check.endPos; i++) {
            board[(int)tempPos.x, (int)tempPos.y].colour = check.Colour;
            board[(int)tempPos.x,(int)tempPos.y].piece.transform.GetComponent<Renderer>().material = check.Colour == 0 ? whiteMat:blackMat; 
            tempPos += intToDir(check.direction);

        }
    }
    void changePieceArr(placeCheck check, pieces[,] board) {
        Vector2 tempPos = check.startPos;
        for (int i = 0; i < check.endPos; i++) {
            board[(int)tempPos.x, (int)tempPos.y].colour = check.Colour;
            tempPos += intToDir(check.direction);
        }
    }
    void delPiece(pieces[,] board, Vector2 pos) { // used to delete a specific piece from the board.
        if (board[(int)pos.x,(int)pos.y].piece != null) {
            Destroy(board[(int)pos.x,(int)pos.y].piece);
            board[(int)pos.x,(int)pos.y] = new pieces (null, 9, new Vector2(-1,-1));
        }
    }
    public void delBoard(pieces[,] board) { // used to remove all the pieces on the board
        for (int i = 0; i < 8; i++) { 
            for (int j = 0; j < 8; j++) {
                delPiece(board, new Vector2(i,j));
            }
        }
    }
    
    public void loadBoard(pieces[,] newBoard, pieces[,] oldBoard) { // used to generate a new board from an array
        if (newBoard == oldBoard) {
            pieces[,] tempBoard = new pieces[8,8];
            saveBoard(tempBoard, oldBoard);
            delBoard(newBoard);
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (tempBoard[i,j].colour != 9) {
                        instancePiece(new Vector2(i,j), tempBoard[i,j].colour, pieceArr);
                    }
                }
            }
        }else {
            delBoard(oldBoard);
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (newBoard[i,j].colour != 9) {
                        instancePiece(new Vector2(i,j), newBoard[i,j].colour, pieceArr);
                    }
                }
            }
            
        }
        

    }
    
    public string LogBoard(pieces[,] board) { // used for debugging the board, to see if there is any desyncs with the board data and what is shown visually
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
    
    /*this is used to save the board, so it cna be loaded again later, and this is needed due to the boards being linked by gameObjects
    which were causing problems with both of the boards being changed simultaneously*/
    public void saveBoard(pieces[,] newBoard, pieces[,] oldBoard) { 
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                newBoard[i,j] = new pieces(null, oldBoard[i,j].colour, oldBoard[i,j].position);
            }
        }
    }
    public Vector2[] getPossibleMoves(pieces[,] board, int colour) { // used to get the next possible moves
        Vector2[] possibleMoves = new Vector2[64];

        int totalMoves = 0;
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (CheckPlace360(new Vector2(i,j), colour, board) & board[i,j].colour == 9) {
                    possibleMoves[totalMoves] = (new Vector2(i,j));
                    totalMoves +=1;
                } 
            }
        }
        for (int k = 0; k < 64; k++) { // setting the possible moves to a null value so it does not alway place at the positon 0,0
            if (k >= totalMoves) {
                possibleMoves[k] = (new Vector2(-1,-1));
            }
        }
        
        return possibleMoves;
    }
    public void placeShadows(Vector2[] positions) { // used to display a shadow on all possible locations so the player knows where they can place.
        delShadowChildren();
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                for (int k = 0; k < 64; k++) {
                    if (positions[k] == new Vector2(i,j) &&  positions[k] != new Vector2 (-1, -1)) {
                    //Debug.Log(positions[i * j]);
                    Instantiate(shadow, new Vector3(i, 1.5f, j), Quaternion.identity, shadowParent);
                    }
                }
            }
        }
    }
    void delShadowChildren() { // used to delete all of the children of the shadow parent, so a new set can be created 
        foreach(Transform child in shadowParent) {
            Destroy(child.gameObject);
        }
    }
    int getPieceCount(pieces[,] board, int colour) { // gets the number of pieces of a specific colour on the board
        int count = 0;
        for (int i =0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (board[i,j].colour == colour) {
                    count += 1;
                }
            }
        }
        return count;
    }
    public int getPossibleMoveCount(Vector2[] possibleMoveArr, int colour, pieces[,] board) { // gets the amount of possible moves that can be played
        Vector2 nullMove = new Vector2 (-1, -1);
        int count = 0;
        foreach (Vector2 move in possibleMoveArr) {
            if (move != nullMove) {
                count += 1;
            }
        }
    return count;
    }
    IEnumerator winCheck(pieces[,] board, int colour) { // win check function checks for if a player has won and if so then displays that the player has won and after 5 seconds take them back to the pre game menu
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (getPossibleMoveCount(getPossibleMoves(board,currentTurn),currentTurn,board) == 0 & ui.winTextActive == false) {
            
            StartCoroutine(ui.displayWinText((getPieceCount(board,currentTurn) > getPieceCount(board, currentTurn == 1 ? 0 : 1)) ? currentTurn: currentTurn ==1 ? 0: 1));
            if (ui.playerSelector.value == 1) {
                database.insertWins((getPieceCount(board,0) > getPieceCount(board, 1)) ?0: 1 ,database.userId);
            }
            
            
    }
    }

    public void aiDifficultySelector(int difficulty) {// used to chage the amount of itterations done by the monte carlo tree search, depending on the difficulty dropdown
        int returnItterations = 0;
        switch ( difficulty ) {
            case 0:
                returnItterations = 50;
                break;
            case 1:
                returnItterations = 1000;
                break;
            case 2:
                returnItterations =  3000;
                break;
            default:
            returnItterations = 500;
            break;
        }
        monteItterations = returnItterations;
    }
    public void AIMove() {// used to call create an instance of the monte carlo class, and then get the best move from the instance and play that move.
        pieces[,] tempBoard = new pieces[8,8];
        saveBoard(tempBoard, pieceArr);
        monteCarloNode root = new monteCarloNode((new boardState(tempBoard, currentTurn)), null, new Vector2 (-1,-1), monteItterations);
        Debug.Log("monteStarted" + monteItterations + " -  Itterations");
        monteCarloNode selectedNode = root.bestAction();
        Debug.Log (selectedNode.ParentAction +"monte move");
        mainPlace(selectedNode.ParentAction, currentTurn, pieceArr, true);
        
        //ChangeTurn(currentTurn);
        }
    IEnumerator StartAiMove(){
        Debug.Log("startedAiMove");
        yield return new WaitForSeconds(1);
        AIMove();

    }
    }
