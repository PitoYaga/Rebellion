using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueScene : MonoBehaviour
{
    [SerializeField] private float textSpeed = 0.1f;
    [SerializeField] private Text dialogue;
    [SerializeField] private string[] sentences;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject coninueButton;
    [SerializeField] private GameObject finishButton;
    
    private int _index;
    int _sceneIndex;
    private float timer;

    private int clicked;

    void Start()
    {
        _sceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(TypeSentences());

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    
    void Update()
    {
        if (dialogue.text == sentences[_index])
        {
            coninueButton.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            timer += Time.deltaTime;
            if (timer >= 1.7f)
            {
                SceneManager.LoadScene(_sceneIndex + 1);
            }
        }
    }

    IEnumerator TypeSentences()
    {
        foreach (char letter in sentences[_index].ToCharArray())
        {
            dialogue.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void Continue()
    {
        coninueButton.SetActive(false);

        if (_index < sentences.Length - 1)
        {
            _index++;
            dialogue.text = "";
            StartCoroutine(TypeSentences());
        }
        else
        {
            dialogue.text = sentences[_index];
            restartButton.SetActive(true);
            finishButton.SetActive(true);
        }
    }
    
    public void Restart()
    {
        _index = 0;
        dialogue.text = "";
        StartCoroutine(TypeSentences());
    }
    
    public void LoadNextScene()
    {
        SceneManager.LoadScene(_sceneIndex + 1);
    }
}
