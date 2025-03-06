using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
using Random = UnityEngine.Random;

public class MetricManager : MonoBehaviour
{
    public static MetricManager Instance { get; private set; }

    public MetricAbstract[] metrics = {
        new MetricExample(),
        new MetricWaveLength(),
        new MetricChosenUpgrade(),
        new MetricTowerPlaced()
    };

    public long _sessionID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate instances
            return;
        }

        Instance = this; // Assign the singleton instance
        _sessionID = DateTime.Now.Ticks;
        Debug.Log("awake metric + " + _sessionID);
        //Send();

        Debug.Log("size of metrics: " + metrics.Length);
        foreach (var metric in metrics)
        {
            metric.post += Send;
        }
    }

    private void Send(string url, List<string> formTags, List<string> formValues)
    {
        StartCoroutine(Post(url, formTags, formValues));
    }

    private IEnumerator Post(string url, List<string> formTags, List<string> formValues)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        //form.AddField("entry.1828615863", sessionID);
        //form.AddField("entry.866404936", testInt);
        formValues[0] = _sessionID.ToString();

        for (int i = 0; i < formTags.Count; i++)
        {
            form.AddField(formTags[i], formValues[i]);
        }

        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("starting metric + " + _sessionID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void Send()
    //{
    //    _testInt = Random.Range(0, 101);
    //    StartCoroutine(Post(_sessionID.ToString(), _testInt.ToString()));
    //}

    //private IEnumerator Post(string sessionID, string testInt)
    //{
    //    // Create the form and enter responses
    //    WWWForm form = new WWWForm();
    //    form.AddField("entry.1828615863", sessionID);
    //    form.AddField("entry.866404936", testInt);
    //    // Send responses and verify result
    //    using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
    //    {
    //        yield return www.SendWebRequest();
    //        if (www.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log(www.error);
    //        }
    //        else
    //        {
    //            Debug.Log("Form upload complete!");
    //        }
    //    }
    //}

}
