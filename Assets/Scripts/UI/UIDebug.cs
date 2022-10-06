using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour
{
    public Text textBox;
    private static Text _textBox;
    private static List<string> texts = new List<string>();

    public bool debugToUI = false;
    private static bool _debugToUI;


    private void Awake()
    {
        _textBox = textBox;
        _debugToUI = debugToUI;
    }

    private void Update()
    {
        if (_debugToUI)
        {
            if (texts.Count > 0)
            {
                foreach (string text in texts)
                {
                    _textBox.text += text + "\r\n";
                }
                texts.Clear();
            }
        }
    }

    public static void Log(string text)
    {
        if (_textBox == null) return;
        texts.Add(text);
        Debug.Log(text);
    }
}
