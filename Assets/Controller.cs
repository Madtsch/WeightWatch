using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Controller : MonoBehaviour {

    public InputField weightDate;
    public InputField weightWeight;
    public GameObject panel;
    private string sysdate = System.DateTime.Now.ToString("yyyy-MM-dd");
    private bool requestFinished;
    private bool requestErrorOccurred;

    void Start () {
        // get data from /weightwatch/lastweights
        StartCoroutine(GetRequest("https://apex.oracle.com/pls/apex/mqstest/weightwatch/lastweights"));
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

    [System.Serializable]
    public class WWData
    {
        public float ww_weight;
        public string ww_date;
        public string ww_created_by;
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("ww_weight", weightWeight.text);
        form.AddField("username", "Unity");

        using (UnityWebRequest www = UnityWebRequest.Post("https://apex.oracle.com/pls/apex/mqstest/weightwatch/weight/" + weightDate.text, form))
        {
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

    IEnumerator GetRequest(string uri)
    {
        requestFinished = false;
        requestErrorOccurred = false;

        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        requestFinished = true;
        if (request.isNetworkError)
        {
            Debug.Log("Something went wrong, and returned error: " + request.error);
            requestErrorOccurred = true;
        }
        else
        {
            // Show results as text
            //Debug.Log(request.downloadHandler.text);
            WWData wwData = new WWData();
            wwData = JsonUtility.FromJson<WWData>(request.downloadHandler.text);
            Debug.Log(wwData.ww_date);
            //Debug.Log("Debug: " + wwData["0"].ww_weight);

            if (request.responseCode == 200)
            {
                Debug.Log("Request finished successfully!");
            }
            else
            {
                Debug.Log("Request failed (status:" + request.responseCode + ")");
                requestErrorOccurred = true;
            }

            if (!requestErrorOccurred)
            {
                yield return null;
                // process results
            }
        }
    }
}
