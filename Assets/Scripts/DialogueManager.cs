﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public GameObject dBox;

    public Text sText;
    public Text dText;
    public bool isFrozen = false;

    public bool diaglogActive;

    public string[] dialogLines;
    public int currentLine;

    public Rigidbody2D[] playerBody;
    public Vector2[] linearBackups;

    // Use this for initialization
    void Start()
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        playerBody = new Rigidbody2D[2];
        linearBackups = new Vector2[2];

        playerBody[0] = players[0].GetComponent<Rigidbody2D>();
        playerBody[1] = players[1].GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (diaglogActive && Input.GetKeyDown(KeyCode.Space))
        {
            //dBox.SetActive(false);
            //diaglogActive = false;

            currentLine++;

        }

        if (currentLine >= dialogLines.Length)
        {
            closeDialogue();
            currentLine = 0;
        }

        if (dialogLines.Length > 0)
        {
            dText.text = dialogLines[currentLine];
        }
    }

    public void showBox(string source, string dialogue)
    {

        
        diaglogActive = true;
        dBox.SetActive(true);
        sText.text = source;
        dText.text = dialogue;


    }

    public void showDialogue(string source)
    {
        if (!isFrozen)
        {
            isFrozen = true;
           // freezePlayer();
        }

        diaglogActive = true;
        dBox.SetActive(true);
        sText.text = source;

    }

    public void closeDialogue()
    {
        diaglogActive = false;
        dBox.SetActive(false);

        if (isFrozen)
        {
            isFrozen = false;
           // unfreezePlayer();
        }

    }

    private void freezePlayer()
    {
        linearBackups[0] = playerBody[0].velocity;

        linearBackups[1] = playerBody[1].velocity;
        
        playerBody[0].velocity = Vector2.zero;
        playerBody[1].velocity = Vector2.zero;

        playerBody[0].constraints = RigidbodyConstraints2D.FreezeAll;
        playerBody[1].constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void unfreezePlayer()
    {
        playerBody[0].constraints = RigidbodyConstraints2D.None;
        playerBody[0].velocity = linearBackups[0];

        playerBody[1].constraints = RigidbodyConstraints2D.None;
        playerBody[1].velocity = linearBackups[1];
    }

}
