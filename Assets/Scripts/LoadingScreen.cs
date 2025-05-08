using UnityEngine;
using TMPro;
using System.Collections;
public class LoadingScreen : MonoBehaviour
{
    
    [SerializeField]
    TMP_Text textMesh;
    [SerializeField]
    string[] loadingTexts = new string[] {
        "Loading.",
        "Loading..",
        "Loading..."
    };

    void Start()
    {
        if(!onEnableCalled){
            OnEnable();
        }
    }
    bool onEnableCalled;
    void OnEnable()
    {
        onEnableCalled = true;
        StopAllCoroutines();
        StartCoroutine(LoadingTextLoop());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        onEnableCalled = false;
    }


    IEnumerator LoadingTextLoop(){
        int index = 0;
        WaitForSeconds delay = new(1.0f);
        while(true) {
            textMesh.text = loadingTexts[index++];
            index %= loadingTexts.Length;
            yield return delay;
        }
    }
}
