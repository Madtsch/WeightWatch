using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour {

    public InputField weightDate;
    public InputField weightWeight;
    private string sysdate = System.DateTime.Now.ToString("yyyy-dd-MM");

    void Start () {
        Debug.Log("I am here!");
        weightDate.text = sysdate;
        weightWeight.text = "104.4";
	}

    public void SaveData()
    {

    }
}
