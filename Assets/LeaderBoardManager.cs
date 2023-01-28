using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    class leaderData {
        int ids;
        int wins;
        int losses;
        leaderData (int id, int win, int loss) {
            ids = id;
            wins = win;
            losses = loss;
        }
    }
    [SerializeField] UserDatabase database;
    [SerializeField] TMP_Text _winText;
    [SerializeField] TMP_Text _lossText;
    [SerializeField] TMP_Text _userText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        splitData();
    }
    void splitData() {
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
