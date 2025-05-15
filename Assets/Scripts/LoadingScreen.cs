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
        StopAllCoroutines();
        StartCoroutine(LoadingTextLoop());
        onEnableCalled = true;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        onEnableCalled = false;
    }

    public float changeDelay = (float)Fractions.OneThird;
    IEnumerator LoadingTextLoop(){
        int index = 0;
        WaitForSeconds delay = new(changeDelay);
        while(true) {
            textMesh.text = loadingTexts[index++];
            index %= loadingTexts.Length;
            yield return delay;
        }
    }
}
