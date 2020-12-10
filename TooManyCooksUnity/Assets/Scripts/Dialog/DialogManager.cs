using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text nameText;
    public Text dialogText;
    public Animator animator;
    private Queue<string> sentences;
    public bool isOpen = false;
    void Start()
    {
        sentences= new Queue<string>();
    }

    public void StartDialog(Dialog dialog)
    {    
        animator.SetBool("IsOpen", true);
        isOpen = true;
        nameText.text = dialog.name;
        sentences.Clear();
        foreach (string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialog();
            WaiterButton.S.gameObject.SetActive(true);
            return;
        }

        string sentence = sentences.Dequeue();
        dialogText.text = sentence;
    }

    public void EndDialog()
    {    
        isOpen = false;
        animator.SetBool("IsOpen", false);
        WaiterButton.S.gameObject.SetActive(true);
     
    }
    

    // Update is called once per frame
    
}
