using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Controller : MonoBehaviour {

    public InputField weightMonth;
    public InputField weightDay;
    public InputField weightYear;
    public InputField weightWeight;
    public GameObject mainScreen;
    public GameObject loginScreen;
    private string sysdateMonth = System.DateTime.Now.ToString("MMM");
    private string sysdateDay = System.DateTime.Now.ToString("dd");
    private string sysdateYear = System.DateTime.Now.ToString("yyyy");
    private bool requestFinished;
    private bool requestErrorOccurred;

    void Start () {
        // get data from /weightwatch/lastweights
        StartCoroutine(GetRequest("https://apex.oracle.com/pls/apex/mqstest/weightwatch/lastweights"));
        weightMonth.text = sysdateMonth;
        weightDay.text = sysdateDay;
        weightYear.text = sysdateYear;
        weightWeight.text = "104.4";
	}

    public void SaveData()
    {
        Debug.Log("SaveData()");
        mainScreen.GetComponent<Image>().CrossFadeColor(Color.blue, 0f, false, true);
        mainScreen.GetComponent<Image>().CrossFadeColor(Color.black, 2.0f, false, true);
        StartCoroutine(Upload());
    }

    public void SwitchScreen()
    {
        loginScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    [System.Serializable]
    public class WWData
    {
        public Items[] items;
    }

    [System.Serializable]
    public class Items
    {
        public int ww_id;
        public string ww_raw;
        public string ww_date;
        public float ww_weight;
        public string ww_created;
        public string ww_created_by;
        public string ww_updated;
        public string ww_updated_by;
    }

    IEnumerator Upload()
    {

        string weightMonthMM = "05";

        WWWForm form = new WWWForm();
        form.AddField("ww_weight", weightWeight.text);
        form.AddField("username", "Unity");

        

        using (UnityWebRequest www = UnityWebRequest.Post("https://apex.oracle.com/pls/apex/mqstest/weightwatch/weight/" + weightYear.text + "-" + weightMonthMM + "-" + weightDay.text, form))
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
            // Get results from json
            WWData wwData = JsonUtility.FromJson<WWData>(request.downloadHandler.text);
            // Set last weight as actual weight
            weightWeight.text = wwData.items[0].ww_weight.ToString("n1");
            //Debug.Log(wwData.items[0].ww_id);

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
