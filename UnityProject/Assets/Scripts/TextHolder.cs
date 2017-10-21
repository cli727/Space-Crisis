﻿using UnityEngine;
using System.Collections;

public class TextHolder : MonoBehaviour
{
    public TextAsset textFile;
    public string[] textLines;
    public int lineToBreak;
    // public Item item to check
    public bool autoDialog;

    // Use this for initialization
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string[] getTextLines() { 
  
        if (textFile != null)
        {
           textLines = textFile.text.Split('\n');
        }
        else {
            textLines = this.GetComponent<TextHolder>().textLines;
        }
        return textLines;
    }
}