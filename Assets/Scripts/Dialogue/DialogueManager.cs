using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject close;
    
    private Queue<string> sentences;

    public Text dialogueText;
    
    public Dialogue dialogue;
    
    
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;

        if (sentences.Count <= 0)
        {
            return;
        }
    }
}
