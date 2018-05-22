using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour {

    public Text debugText;

    public void Log(string output)
    {
        Debug.Log(output);
        debugText.text = output + "\n" + debugText.text;
    }
}
