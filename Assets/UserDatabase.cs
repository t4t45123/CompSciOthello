using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class UserDatabase : MonoBehaviour
{
    public string userName;
    public string pass;
    public int userId;
    int maxId;
    string adminUserName = "ADMIN";
    string adminPass = "admin";
    void Start() {
        //Debug.Log (Application.persistentDataPath);
        InitializeDatabase();
    }
    void InitializeDatabase() { // this Creates the database and is called when the script is loaded
        IDbConnection dbConnection = OpenDatabase(); 
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY, Username TEXT, Password TEXT)"; 
        dbCommandCreateTable.ExecuteScalar(); 
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        dbCommandInsertValue.CommandText = "INSERT OR REPLACE INTO Users (Id, Username, Password) VALUES (1, '" + adminUserName + "', '" + adminPass +"')";
        dbCommandInsertValue.ExecuteScalar(); 
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS leaderboard (Id INTEGER PRIMARY KEY, Wins INTEGER, Losses INTEGER)";
        dbCommandCreateTable.ExecuteScalar();
        dbConnection.Close();
    }
    IDbConnection OpenDatabase() // opens the database connection
    {
        // Open a connection to the database.
        string dbUri = "URI=file:" + Application.persistentDataPath + @"\userData.db"; 
        IDbConnection dbConnection = new SqliteConnection(dbUri); 
        dbConnection.Open(); 
        return dbConnection;
    }
    int GetMaxId() { // returns the bigest ID in the users table 
        IDbConnection dbConnection = OpenDatabase(); 
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = "SELECT MAX(Id) FROM Users"; 
        IDataReader dataReader = dbCommandReadValues.ExecuteReader(); 
        while (dataReader.Read()){ 
            maxId = dataReader.GetInt16(0);
        }
        dbConnection.Close();
        return maxId;
        
    }


    public bool compareUser(string user, string pass) { // this function finds an id that is linked to both a user and a password that is inputted
        userId = 0;
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = "SELECT Id FROM Users WHERE Username = '" + user + "' AND Password = '" + pass + "'"; 
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        
        while (dataReader.Read()) {
            userId = (dataReader.GetInt32(0));
            //Debug.Log(userId);
        }
        dbConnection.Close();
        if (userId == 0) {
            return false;
        } else {
            return true;
        }
        
    }
    public bool CheckIfNewUser(string user) { // returns a boolean indicating whether the user is new
        userId = 0;
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = ("SELECT Id FROM Users WHERE Username = '" + user + "'");
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        while (dataReader.Read()) {
            userId = (dataReader.GetInt32(0));
            Debug.Log(userId);
        }
        dbConnection.Close();
        if (userId == 0) {
            return true;
        } else {
            return false;
        }
    }
    public void inputUserInformation(string userName, string pass) { // takes a username and password and inputs it into the database
        Debug.Log("input UserInformation Called with " + userName + " " + pass);
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        dbCommandInsertValue.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (" + (GetMaxId()+ 1) + ",'" + userName + "','" + pass + "')";
        dbCommandInsertValue.ExecuteNonQuery(); 
        dbConnection.Close(); 
        
    }
    public int getUserId(string user) { // gets the user Id based on the username 
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = ("SELECT Id FROM Users WHERE Username = '" + user + "'");
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        while (dataReader.Read()) {
            userId = (dataReader.GetInt32(0));
            //Debug.Log(userId);
        }
        dbConnection.Close();
        return userId;
    }
    public Vector2 getwl(int userId) { // gets wins and the losses from a specific user id 
        int wins = 0;
        int losses = 0;
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = "SELECT Wins, Losses From leaderboard WHERE Id = '" + userId.ToString() + "'";
        IDataReader dataReader = command.ExecuteReader();
        while (dataReader.Read()){
            wins =  dataReader.GetInt32(0);
            losses = dataReader.GetInt32(1);
        }
        dbConnection.Close();
        return new Vector2(wins,losses);
    }
    public void insertWins(int wl,int userId){ // insets a win or loss into the database based on a user id
        int wins = 0;
        int losses = 0;
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand command = dbConnection.CreateCommand();
        switch (wl) {
            case 0:
            wins = 1;
            break;
            case 1:
            losses = 1;
            break;
        }
        command.CommandText = "INSERT OR REPLACE INTO leaderboard (Id,Wins,Losses) VALUES ("+ userId + ","+ (getwl(userId).x + wins) + ", " + (getwl(userId).y+losses) +  ")";
        command.ExecuteScalar();                    
        dbConnection.Close();
    }
    public string returnAllWl() { // retunrs all the users, wins and losses based on the id
        string ids = "";
        string wins = "";
        string losses = "";
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = "SELECT * FROM leaderboard ORDER by Wins DESC";
        IDataReader reader = command.ExecuteReader();
        while (reader.Read()) {
            ids = (ids + reader.GetInt32(0).ToString() + ",");
            wins = (wins + reader.GetInt32(1).ToString() + ",");
            losses = (losses + reader.GetInt32(2).ToString() + ",");
        }
        dbConnection.Close();
        //Debug.Log (ids + " || "+ wins + " || " + losses);
        return (ids + " || "+ wins + " || " + losses);
        
    }
    public string GetUserFromID(string id) {// gets the user string from the id 
        string user = "";
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = "SELECT Username FROM USERS WHERE (Id = '" + id + "')";
        IDataReader reader = command.ExecuteReader();
        while (reader.Read()) {
            user = reader.GetString(0);
        }
        return user;
    }
} 
