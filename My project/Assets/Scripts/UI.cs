using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class UI : NetworkBehaviour
{
    // Start is called before the first frame update
    [SyncVar(hook = "ChangeTop")]
    public string topText;

    [SyncVar(hook = "ChangeMiddle")]
    public string middleText;

    void ChangeTop(string oldText, string newText)
    {
        transform.GetChild(0).GetComponent<Text>().text = newText;
    }

    void ChangeMiddle(string oldText, string newText)
    {
        transform.GetChild(1).GetComponent<Text>().text = newText;
    }
}
