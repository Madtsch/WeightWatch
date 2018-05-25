using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Controller : MonoBehaviour {

    #region Vars

    public Debugger Debugger;

    public InputField weightMonth;
    public InputField weightDay;
    public InputField weightYear;
    public InputField weightWeight;
    public GameObject mainScreen;
    public GameObject loginScreen;
    private string sysdateMonth = System.DateTime.Now.ToString("MMM");
    private string sysdateDay = System.DateTime.Now.ToString("dd");
    private string sysdateYear = System.DateTime.Now.ToString("yyyy");
    private bool requestErrorOccurred;

    #endregion

    #region Default Functions

    void Start () {
        // get data from /weightwatch/lastweights
        StartCoroutine(GetRequest("https://apex.oracle.com/pls/apex/mqstest/weightwatching/lastweights"));
        weightMonth.text = sysdateMonth;
        weightDay.text = sysdateDay;
        weightYear.text = sysdateYear;
        weightWeight.text = "Please wait for loading!";
        Debugger.Log(weightMonth.text + " " + weightDay.text + " " + weightYear.text);
	}

    #endregion

    #region Button Functions

    public void SaveData()
    {
        Debugger.Log("SaveData()");
        //mainScreen.GetComponent<Image>().CrossFadeColor(Color.blue, 0f, false, true);
        //mainScreen.GetComponent<Image>().CrossFadeColor(Color.black, 2.0f, false, true);
        StartCoroutine(Upload());
    }

    public void SwitchScreen()
    {
        loginScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    public void PlusMonth()
    {
        int tmpMonth = GetInt(System.DateTime.Parse("1. " + weightMonth.text + " 2000").ToString("MM"), 0);
        Debugger.Log("tmpMonth: " + tmpMonth);

        if (tmpMonth == 12)
        {
            tmpMonth = 0;
        }
        weightMonth.text = GetMonth(tmpMonth + 1);
        Debugger.Log("GetMonth(tmpMonth): " + GetMonth(tmpMonth + 1));
    }

    public void MinusMonth()
    {
        int tmpMonth = GetInt(System.DateTime.Parse("1. " + weightMonth.text + " 2000").ToString("MM"), 0);
        Debugger.Log("tmpMonth: " + tmpMonth);
        if (tmpMonth == 1)
        {
            tmpMonth = 13;
        }
        weightMonth.text = GetMonth(tmpMonth - 1);
        Debugger.Log("GetMonth(tmpMonth): " + GetMonth(tmpMonth -1));
    }

    public void PlusDay()
    {
        int tmpDay = GetInt(weightDay.text, 0);
        if (tmpDay == 31)
        {
            tmpDay = 0;
        }
        weightDay.text = (tmpDay + 1).ToString();
    }

    public void MinusDay()
    {
        int tmpDay = GetInt(weightDay.text, 0);
        if (tmpDay == 1)
        {
            tmpDay = 32;
        }
        weightDay.text = (tmpDay - 1).ToString();
    }

    public void PlusYear()
    {
        int tmpYear = GetInt(weightYear.text, 0);
        weightYear.text = (tmpYear + 1).ToString();
    }

    public void MinusYear()
    {
        int tmpYear = GetInt(weightYear.text, 0);
        weightYear.text = (tmpYear - 1).ToString();
    }

    public void PlusWeight()
    {
        float tmpWeight = GetFloat(weightWeight.text, 0.0F);
        weightWeight.text = (tmpWeight + 0.1f).ToString();
    }

    public void MinusWeight()
    {
        float tmpWeight = GetFloat(weightWeight.text, 0.0F);
        weightWeight.text = (tmpWeight - 0.1f).ToString();
    }

    #endregion

    #region Helper Functions

    private float GetFloat(string stringValue, float defaultValue)
    {
        float result = defaultValue;
        float.TryParse(stringValue, out result);
        return result;
    }

    private int GetInt(string stringValue, int defaultValue)
    {
        int result = defaultValue;
        int.TryParse(stringValue, out result);
        return result;
    }

    private string GetMonth(int intMonth)
    {
        string stringMonth;
        switch (intMonth)
        {
            case (1):
                stringMonth = "Jan";
                return stringMonth;
            case (2):
                stringMonth = "Feb";
                return stringMonth;
            case (3):
                stringMonth = "Mar";
                return stringMonth;
            case (4):
                stringMonth = "Apr";
                return stringMonth;
            case (5):
                stringMonth = "May";
                return stringMonth;
            case (6):
                stringMonth = "Jun";
                return stringMonth;
            case (7):
                stringMonth = "Jul";
                return stringMonth;
            case (8):
                stringMonth = "Aug";
                return stringMonth;
            case (9):
                stringMonth = "Sep";
                return stringMonth;
            case (10):
                stringMonth = "Oct";
                return stringMonth;
            case (11):
                stringMonth = "Nov";
                return stringMonth;
            case (12):
                stringMonth = "Dec";
                return stringMonth;
            default:
                Debugger.Log("Error on month calculation");
                return "Err";
        }
    }

    #endregion

    #region Data Classes

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

    #endregion

    #region Restful Calls

    IEnumerator Upload()
    {
        // fixme - hardcode may
        int weightMonthMM = GetInt(System.DateTime.Parse("1. " + weightMonth.text + " 2000").ToString("MM"), 0);

        WWWForm form = new WWWForm();
        form.AddField("ww_weight", weightWeight.text);
        form.AddField("username", "Unity");

        using (UnityWebRequest www = UnityWebRequest.Post("https://apex.oracle.com/pls/apex/mqstest/weightwatching/weight/" + weightYear.text + "-" + weightMonthMM.ToString("00") + "-" + weightDay.text, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debugger.Log(www.error);
                Debugger.Log("Request failed (status: " + www.responseCode + ") - " + www.downloadHandler.text);
            }
            else
            {
                Debugger.Log("Form upload complete: " + www.responseCode);
            }
        }
    }

    IEnumerator GetRequest(string uri)
    {
        requestErrorOccurred = false;

        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debugger.Log("Something went wrong, and returned error: " + request.error);
            requestErrorOccurred = true;
        }
        else
        {
            // Get results from json
            WWData wwData = JsonUtility.FromJson<WWData>(request.downloadHandler.text);
            // Set last weight as actual weight
            weightWeight.text = wwData.items[0].ww_weight.ToString("n1");
            Debugger.Log("LastWeight: " + weightWeight.text);

            // display last five dates and weight
            for (int i = 0; i < wwData.items.Length; i++)
            {
                Debugger.Log(wwData.items[i].ww_date + " - " + wwData.items[i].ww_weight);
            }

            if (request.responseCode == 200)
            {
                Debugger.Log(uri + " - Request finished successfully!");
            }
            else
            {
                Debugger.Log(uri + "Request failed (status:" + request.responseCode + ")");
                requestErrorOccurred = true;
            }

            if (!requestErrorOccurred)
            {
                yield return null;
                // process results
            }
        }
    }
    #endregion
}
