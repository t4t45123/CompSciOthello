using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UImanager : MonoBehaviour
{
    //gameObject reference
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject ErrorTextObject;
    [SerializeField] GameObject PreGameMenu;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject adminButtonParent;
    [SerializeField] GameObject difficultyParent;
    [SerializeField] GameObject boardParent;


    //Script refernece
    [SerializeField] Manager boardManager;
    [SerializeField] AccountManager accountManager;
    [SerializeField] UserDatabase userDatabase;
    public float errorDisplayDuration = 5f;
    TextMeshProUGUI errorText;
    public TextMeshProUGUI winText;
    [SerializeField] float lastDisplayTime;
    public bool winTextActive = false;

    //UI elements
    public TextMeshProUGUI whiteCountText;
    public TextMeshProUGUI blackCountText;
    public TextMeshProUGUI turnText;
    public TMPro.TMP_Dropdown difficultyDropdown;
    public TMPro.TMP_Dropdown playerSelector;
    public Toggle shadowMoveToggle;
    void Update() { // called every frame used to check how long the error has been displayed for
        if (lastDisplayTime + errorDisplayDuration < Time.time) {
            ErrorTextObject.SetActive(false);
        }
        difficultydisplay();

        
    }
    void difficultydisplay() {
        if (playerSelector.value == 0) {
            difficultyParent.SetActive(false); 
            boardManager.isMonteEnabled = false;
        }else{
            difficultyParent.SetActive(true);
            boardManager.isMonteEnabled = true;
        }
    }
    void DisableAllScreens() { // this is used to hide all screens, so not more than one screen is displayed at once
        loginScreen.SetActive(false);
        MainMenu.SetActive(false);
        PreGameMenu.SetActive(false);
        playMenu.SetActive(false);
        boardParent.SetActive(false);
    }
    public void EnableLoginScreen() { // this is used to enable the login screen at the start of the game
        DisableAllScreens();
        loginScreen.SetActive(true);
    }
    public void EnableMainMenu() { // used to enable the main menu this is called when the player logs in
    DisableAllScreens();
    MainMenu.SetActive(true);
    }
    public void EnablePreGameMenu() { // used to enable the pre-game menu this is called when the player presses the play button in the main menu
        DisableAllScreens();
        PreGameMenu.SetActive(true);
    }
    public void EnablePlayMenu() { // used to display the play menu and also show the admin buttons is the admin account is logged in
        DisableAllScreens();
        playMenu.SetActive(true);
        if (accountManager.playerId == 1) {
            adminButtonParent.SetActive(true);
        }else {
            adminButtonParent.SetActive(false);
        }
        boardParent.SetActive(true);
        ResetMainBoard();
        boardManager.currentTurn = 0;
    }
    public void LogPossibleMoves() { // used to log all possible moves into the console for debugging
        for (int i = 0; i < 64; i++) {
            Debug.Log (boardManager.getPossibleMoves(boardManager.pieceArr, boardManager.currentTurn)[i]);
        }
    }
    public void shadowPlace() { // used to call the shadow place function again for debugging purposes
        boardManager.placeShadows(boardManager.getPossibleMoves(boardManager.pieceArr,boardManager.currentTurn));
    } 
    void Start() {
        EnableLoginScreen();
        errorText = ErrorTextObject.GetComponent<TextMeshProUGUI>();
        lastDisplayTime = Time.time;
    }
    public void delMainBoard() { // used to remove all of the pieces on the board 
        boardManager.delBoard(boardManager.pieceArr);
    }
    public void ResetMainBoard() { // used to reset the board
        boardManager.loadBoard(boardManager.startBoard, boardManager.pieceArr);
    }
    public void Logboard() { // used to log the board for debugging purposes
        Debug.Log (boardManager.LogBoard(boardManager.pieceArr));
    }
    public void LogWins(){
        Debug.Log(userDatabase.getwl(userDatabase.userId));
    }
    public void DisplayErrorText (string message) {
        errorText.color = new Color(255, 0,0,255);
        lastDisplayTime = Time.time;
        errorText.text = message;
        ErrorTextObject.SetActive(true);
        
        
    }
    public void DisplayPositiveMessage(string message) { // used to display when an account has been created
        errorText.color = new Color(0,255,0,255);
        lastDisplayTime = Time.time;
        errorText.text = message;
        ErrorTextObject.SetActive(true);
    }

    public void closeGame() { // close the game 
        Application.Quit();
    }
    public IEnumerator displayWinText(int colour) { // displays the win text at the end of a game
        winTextActive = true;
        winText.text = ((colour == 1 ? "black " : "white ") + "wins");
        winText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f); // waits for 5 seconds
        winTextActive = false;
        winText.gameObject.SetActive(false);
        boardManager.loadBoard(boardManager.startBoard, boardManager.pieceArr);
        EnablePreGameMenu();
    }
    public void updateCountAndTurn(int whiteCount, int blackCount, int turn) { // updates the current ui count and current turn
        turnText.text = turn == 0 ? "White's Turn" : "Black's Turn";
        whiteCountText.text = "White Count \n  " +whiteCount;
        blackCountText.text = "Black Count \n  " +blackCount;
    }

}



