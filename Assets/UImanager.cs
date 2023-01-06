using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UImanager : MonoBehaviour
{
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject ErrorTextObject;
    [SerializeField] GameObject PreGameMenu;
    public float errorDisplayDuration = 5f;
    TextMeshProUGUI errorText;
    [SerializeField] float lastDisplayTime;
    void DisableAllScreens() {
        loginScreen.SetActive(false);
        MainMenu.SetActive(false);
        PreGameMenu.SetActive(false);
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
    void Start() {
        EnableLoginScreen();
        errorText = ErrorTextObject.GetComponent<TextMeshProUGUI>();
        lastDisplayTime = Time.time;
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
    }
    public void closeGame() {
        Application.Quit();
    }

}
