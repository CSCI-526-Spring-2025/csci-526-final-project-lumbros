using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeroInfo : MonoBehaviour
{
    private GameObject HeroTextInfo; 
    bool isDisplay = false;
    // Start is called before the first frame update
    void Start()
    {
            HeroTextInfo = GameObject.Find("HeroTextUI");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick(){
        if(isDisplay){
            HeroTextInfo.SetActive(false);
        }
        else{
            HeroTextInfo.SetActive(true);
        }
        isDisplay = !isDisplay;

    }
}
