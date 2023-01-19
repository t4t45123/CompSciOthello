using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccountManager : MonoBehaviour
{
    public GameObject passInput;
    public GameObject userInput;
    TMP_InputField userInputField;
    TMP_InputField passwordInputField;
    public GameObject userDatabaseManager;
    UserDatabase databaseScript;
    public GameObject loginScreenParent;
    public int playerId;
    public string playerUsername;
    UImanager uiManager;
    public GameObject UIManagerObject;

    // Start is called before the first frame update
    void Start()
    {
        userInputField = userInput.GetComponent<TMP_InputField>();
        passwordInputField = passInput.GetComponent<TMP_InputField>();
        databaseScript = userDatabaseManager.GetComponent<UserDatabase>();
        uiManager = UIManagerObject.GetComponent<UImanager>();
    }
    public string GetPassInput() { // gets the text that is entered in the pass input field
        return (passwordInputField.text);
    }
    public string GetUserInput() { // gets the text that is entered in the user input field
        return (userInputField.text); 
    }
    public void CheckUserData() { // calls the compare user function to determine if the login is correct
        
        
        if (databaseScript.compareUser(GetUserInput(),GetPassInput())) {
            Debug.Log(GetUserInput() + " "+ GetPassInput());
            //Debug.Log("correct");
            playerId = databaseScript.getUserId(GetUserInput());
            playerUsername = GetUserInput();
            
            uiManager.EnableMainMenu();
        } else {
            uiManager.DisplayErrorText("Incocrect Username or Password");
        }
    }
    public void CreateAccount() { // uses the two check functions and then if both of those are true then it would call the input user informaiton function
        string password = GetPassInput();
        string userName = GetUserInput();
        if (CheckPass(password) && CheckUser(userName)) {
            uiManager.DisplayPositiveMessage("Account created successfully");
            databaseScript.inputUserInformation(userName, password);
        } 
    }
    bool CheckPass(string pass) { // checks if the inputed password is a valid input
        if (pass.Length <= 16 & pass.Length >= 5) {
            return true;
        }
        uiManager.DisplayErrorText("Inputted Password is invalid");
        return false;
    }
    bool CheckUser(string user) { // checks if the inputed username is a valid input
        if (user.Length <= 15 & user.Length >= 3 & databaseScript.CheckIfNewUser(user) & checkUserWords(user)) {
            return true;
        }
        uiManager.DisplayErrorText("Inputted Username is invalid");
        return false;
    }
    bool checkUserWords(string user) { // checks if the inputted username contains a bad word
        bool found = false;
        string badWords = new string ("john,bad"); // add to this string for all of the words that are not allowed in the inputted username
        foreach (string word in badWords.Split(",")) {
            if (user.Contains(word)) {
                found = true;
            }
        }
        if (!found) {
            return true;
        }else {
            return false;
        }
    }
}
