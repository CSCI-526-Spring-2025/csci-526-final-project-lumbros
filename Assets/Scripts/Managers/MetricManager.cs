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
    public long _sessionID;
    public int _testInt;
    public string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfURU3mpWxk__Sm_sX44O7Pj2LEhJVbxcpuMnm0dfvBJ0QIeA/formResponse";


    private void Awake()
    {
        _sessionID = DateTime.Now.Ticks;
        Debug.Log("awake metric + " + _sessionID);
        Send();
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

    public void Send()
    {
        _testInt = Random.Range(0, 101);
        StartCoroutine(Post(_sessionID.ToString(), _testInt.ToString()));
    }

    private IEnumerator Post(string sessionID, string testInt)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.1828615863", sessionID);
        form.AddField("entry.866404936", testInt);
        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
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

}
