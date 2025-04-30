using UnityEngine;
using System.Collections;
public class MoveCamera : MonoBehaviour
{
    float lastInput;

    public float horizontalOffset = 4.0f;

    void Start(){
        OnEnable();
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(MoveCameraLoop());
    }

    void OnDisable()
        => StopAllCoroutines();

    // Update is called once per frame
    private IEnumerator MoveCameraLoop(){
        var player = GameManager.Instance.Player;
        if (player == null) yield break;

        while(true){
            if (!Mathf.Approximately(player.PInput, 0.0f))
                lastInput = player.PInput;

            Vector3 playerPos = player.position;
            Vector3 cameraPos = transform.position;

            float targetX =  playerPos.x;
            if(!Mathf.Approximately(lastInput, 0.0f))
                targetX += lastInput < 0.0f ? horizontalOffset : -horizontalOffset;

            cameraPos.x = Mathf.Lerp(cameraPos.x, targetX, 6 * Time.deltaTime);
            transform.position = cameraPos;

            yield return GameManager.FixedUpdateDelay; 
        }
    }

}
