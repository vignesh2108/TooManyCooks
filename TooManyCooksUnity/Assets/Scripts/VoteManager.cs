using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : NetworkBehaviour
{
    public static VoteManager S;
    // Start is called before the first frame update
    
    [SyncVar] 
    public int numPlayers = 3;
    
    [SyncVar] 
    public int imposterIndex = -1;
    
    [SyncVar(hook =  nameof(VoteCounter))] 
    public int p1Votes = 0;
    
    [SyncVar(hook =  nameof(VoteCounter))] 
    public int p2Votes = 0;
    
    [SyncVar(hook =  nameof(VoteCounter))] 
    public int p3Votes = 0;
    
    [SyncVar(hook =  nameof(VoteCounter))] 
    public int p4Votes = 0;

    public GameObject VoteDialogBox;
    public GameObject VoteDialogButtons;
    public Text voteInstructionText;

    public bool localVoted = false;

    public GameObject voteButton1;
    public GameObject voteButton2;
    public GameObject voteButton3;
    public GameObject voteButton4;

    public GameObject closeButton;

    public PlayerScript player;
    
    [SyncVar(hook = nameof(voteModeChange))]
    public bool isVotingMode = false;
    private void Start()
    {
        if (S == null)
        {
            S = this;
        } else if (S != this)
        {
            Destroy(this);
        }
    }
    

    public void VoteCounter(int _old, int _new)
    {
        
        if (p1Votes + p2Votes + p3Votes + p4Votes >= numPlayers)
        {   
            Debug.Log($"p1: {p1Votes}, p2: {p2Votes}, p3: {p3Votes}, p4: {p4Votes}");
            int[] votes = {p1Votes, p2Votes, p3Votes, p4Votes};
            int votedIndex = Array.IndexOf(votes, votes.Max());
            if (votes[votedIndex] <= numPlayers / 2)
            {
                voteInstructionText.text = $"Voting Complete!\nThe Chefs failed to identify P{imposterIndex + 1} as the imposter!";
                
                // To make draw uncomment here
                /*voteInstructionText.text = "Voting Complete! Its a draw!";
                player.DisableControls(false);
                WaiterButton.S.gameObject.SetActive(true);
                UseButton.S.gameObject.SetActive(true);
                GrabButton.S.gameObject.SetActive(true);
                closeButton.SetActive(true);*/
            }
            else
            {
                if (votedIndex == imposterIndex)
                {
                    voteInstructionText.text = "Voting Complete!\n" +
                                               "The chefs win!\n" +
                                               $"P{imposterIndex + 1} was the imposter!";
                }
                else
                {
                    voteInstructionText.text = $"Voting Complete!\nThe Chefs failed to identify P{imposterIndex + 1} as the imposter!" +
                                               $"\nP{imposterIndex + 1} wins!";
                }
                // End Game here.
            }
        }
        Debug.Log($"p1: {p1Votes}, p1: {p2Votes}, p1: {p3Votes}, p1: {p4Votes}");
    }


    public void voteModeChange(bool _old, bool _new)
    {
        if (_new == true)
        {
            player.DisableControls();
            GameManager.S.dialogManager.EndDialog();
            WaiterButton.S.gameObject.SetActive(false);
            UseButton.S.gameObject.SetActive(false);
            GrabButton.S.gameObject.SetActive(false);
            VoteDialogBox.SetActive(true);
            VoteDialogButtons.SetActive(true);
            
            if (numPlayers < 4)
            {
                voteButton4.SetActive(false);
            }
            else
            {
                voteButton4.SetActive(true);
            }
        }
        else
        {
            player.DisableControls(false);
            GameManager.S.dialogManager.EndDialog();
            WaiterButton.S.gameObject.SetActive(true);
            UseButton.S.gameObject.SetActive(true);
            GrabButton.S.gameObject.SetActive(true);
            VoteDialogBox.SetActive(false);
            VoteDialogButtons.SetActive(false);
            closeButton.SetActive(false);
            
        }
    }
    
    
    // SERVER FUNCTION
    public void StartVote(bool enable)
    {
        isVotingMode = enable;
        p1Votes = 0;
        p2Votes = 0;
        p3Votes = 0;
        p4Votes = 0;
        RpcTriggerVote(enable);
    }

    public void UpdateVoteCount(int i)
    {
        switch (i)
        {
            case 0:
                p1Votes += 1;
                break;
            case 1:
                p2Votes += 1;
                break;
            case 2:
                p3Votes += 1;
                break;
            case 3:
                p4Votes += 1;
                break;
        }
        RpcTriggerVote(true);
    }
    
    
    
    [ClientRpc]
    void RpcTriggerVote(bool enable)
    {    
        player.DisableControls(enable);
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
