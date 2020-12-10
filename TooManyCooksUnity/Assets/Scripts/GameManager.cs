using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager S;
    public GameObject SmokePrefab;
    public bool isMobile;
    public Dialog dialog;

    [SyncVar]
    public int dishesServed = 0;
    
    [SyncVar]
    public int guestsPoisoned = 0;

    public int maxDishes = 10;
    public int maxGuestsPoisoned = 5;
    public GameObject statsButton;
    public DialogManager dialogManager;
    
    private void Awake()
    {
        if (S == null)
        {
            isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
            S = this;
        } else if (S != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WaiterButton.S.GetComponent<Button>().onClick.AddListener(ShowGameStats);
    }

    public string checkGameWinCondition()
    {
        if (dishesServed >= maxDishes)
        {
            return "chef";
        } 
        
        if (guestsPoisoned >= maxGuestsPoisoned)
        {
            return "imposter";
        }

        return "none";
    }

    public void ShowGameStats()
    {
        string statsString = $"Pizzas served: {dishesServed}\n" +
                             $"Guests poisoned: {guestsPoisoned}\n";
        
        showDialogHelper(statsString);
        
    }

    public void showDialogHelper(string dialogString)
    {
        WaiterButton.S.gameObject.SetActive(false);
        dialog.name = "Waiter";
        dialog.sentences.Clear();
        dialog.sentences.Add(dialogString);
        dialogManager.StartDialog(dialog);
    }
    
    
    public void ShowDialog(string dialogType)
    {
        string dialogString = "";
        switch (dialogType)
        {
            case "default":
                dialogString = $"Great Pizza!\n" +
                               $"They loved it!\n";
                showDialogHelper(dialogString);
                break;
            case "burnt":
                dialogString = $"Eww! Burnt Food!\n" +
                               $"We can't serve this!";
                showDialogHelper(dialogString);
                break;
            
            case "invalid":
                dialogString = $"Eww! This is raw!\n" +
                               $"We can't serve this!";
                showDialogHelper(dialogString);
                break;
            case "poisonedServed":
                dialogString = $"OMG! Poison!\n" +
                               $"Imposter Alert!\n";
                VoteButton.S.gameObject.SetActive(true);
                showDialogHelper(dialogString);
                break;
            case "chefWin":
                dialogString = $"The Chefs Win!\n" +
                               $"All guests served!\n";
                showDialogHelper(dialogString);
                break;
            case "imposterWin":
                dialogString = $"The Imposter Wins!\n" +
                               $"Guests poisoned: {maxGuestsPoisoned}\n";
                showDialogHelper(dialogString);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!dialogManager.isOpen)
            {
                ShowGameStats();
            }
            else
            {
                dialogManager.EndDialog();
            }
        }
    }
}
