using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Controller : MonoBehaviour {

    public InputField weightDate;
    public InputField weightWeight;
    public GameObject panel;
    private string sysdate = System.DateTime.Now.ToString("yyyy-dd-MM");

    void Start () {
        Debug.Log("I am here!");
        weightDate.text = sysdate;
        weightWeight.text = "104.4";
	}

    public void SaveData()
    {
        Debug.Log("SaveData()");
        panel.GetComponent<Image>().CrossFadeColor(Color.blue, 0f, false, true);
        panel.GetComponent<Image>().CrossFadeColor(Color.black, 2.0f, false, true);
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("?ww_weight=" + weightWeight.text + "&username=Unity"));
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post("https://apex.oracle.com/pls/apex/mqstest/weightwatch/weight/" + weightDate.text, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
