using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{

    [SerializeField] UserDatabase database;
    [SerializeField] TMP_Text _winText;
    [SerializeField] TMP_Text _lossText;
    [SerializeField] TMP_Text _userText;
    void Update()
    {
        splitData();
    }
    void splitData() { // splits the data from retun all wl function in the user database script, and places all of the text into text on the leaderboard
        string userText = "";
        string winText = "";
        string lossesText = "";
        string leaderData = database.returnAllWl();
        string[] data = leaderData.Split(" || ");
        string[] wins = data[1].Split(",");
        string[] Ids = data[0].Split(",");
        string[] losses = data[2].Split(",");
        foreach (string Id in Ids){
            userText = (userText + "\n" + database.GetUserFromID(Id));
        }
        foreach (string win in wins) {
            winText = (winText + "\n" + win);
        }
        foreach (string loss in losses) {
            lossesText = (lossesText + "\n" + loss);
        }
        Debug.Log("users:" +userText + "\nwins:" + winText + "\n losses:" + lossesText);
        _winText.text = winText;
        _lossText.text = lossesText;
        _userText.text = userText;
    }
}
