using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //Script refernece
    [SerializeField] Manager boardManager;
    [SerializeField] AccountManager accountManager;
    public float errorDisplayDuration = 5f;
    TextMeshProUGUI errorText;
    public TextMeshProUGUI winText;
    [SerializeField] float lastDisplayTime;
    public float winDisplayDuration =  5f;
    public float lastWinDisplayTime = -5f;
    public bool winTextActive;


    void DisableAllScreens() {
        loginScreen.SetActive(false);
        MainMenu.SetActive(false);
        PreGameMenu.SetActive(false);
        playMenu.SetActive(false);
    }
    public void EnableLoginScreen() {
        DisableAllScreens();
        loginScreen.SetActive(true);
    }
    public void EnableMainMenu() {
    DisableAllScreens();
    MainMenu.SetActive(true);
    }
    public void EnablePreGameMenu() {
        DisableAllScreens();
        PreGameMenu.SetActive(true);
    }
    public void EnablePlayMenu() {
        DisableAllScreens();
        playMenu.SetActive(true);
        if (accountManager.playerId == 1) {
            adminButtonParent.SetActive(true);
        }else {
            adminButtonParent.SetActive(false);
        }
    }
    public void LogPossibleMoves() {
        for (int i = 0; i < 64; i++) {
            Debug.Log (boardManager.getPossibleMoves(boardManager.pieceArr, boardManager.currentTurn)[i]);
        }
    }
    public void shadowPlace() {
        boardManager.placeShadows(boardManager.getPossibleMoves(boardManager.pieceArr,boardManager.currentTurn));
    } 
    void Start() {
        EnableLoginScreen();
        errorText = ErrorTextObject.GetComponent<TextMeshProUGUI>();
        lastDisplayTime = Time.time;
    }
    public void delMainBoard() {
        boardManager.delBoard(boardManager.pieceArr);
    }
    public void ResetMainBoard() {
        boardManager.loadBoard(boardManager.startBoard, boardManager.pieceArr);
    }
    public void Logboard() {
        Debug.Log (boardManager.LogBoard(boardManager.pieceArr));
    }
    public void DisplayErrorText (string message) {
        errorText.color = new Color(255, 0,0,255);
        lastDisplayTime = Time.time;
        errorText.text = message;
        ErrorTextObject.SetActive(true);
        
        
    }
    public void DisplayPositiveMessage(string message) {
        errorText.color = new Color(0,255,0,255);
        lastDisplayTime = Time.time;
        errorText.text = message;
        ErrorTextObject.SetActive(true);
    }
    void Update() {
        if (lastDisplayTime + errorDisplayDuration < Time.time) {
            ErrorTextObject.SetActive(false);
        }
        if (lastWinDisplayTime + winDisplayDuration < Time.time) {
            winText.gameObject.SetActive(false);
            winTextActive = false;
        }
    }
    public void closeGame() {
        Application.Quit();
    }
    public void displayWinText(int colour){
        lastDisplayTime = Time.time;
        winTextActive = true;
        winText.text = ((colour == 1 ? "black " : "white ") + "wins");
        winText.gameObject.SetActive(true);
        if (lastWinDisplayTime + winDisplayDuration > Time.time) {
            EnablePreGameMenu();
        }
    }

}
