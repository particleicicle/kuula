using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    float lastInput;

    public float horizontalOffset = 4.0f;

    // Update is called once per frame
    void LateUpdate()
    {
        var player = GameManager.Instance.Player;
        if (player == null) return;

        // Update last input direction if there is any input
        if (!Mathf.Approximately(player.pInput, 0.0f))
            lastInput = player.pInput;

        Vector3 playerPos = player.position;
        Vector3 cameraPos = transform.position;

        // If lastInput is positive (moving right), look ahead to the right
        // If lastInput is negative (moving left), look ahead to the left
        float targetX =  playerPos.x;
        if(!Mathf.Approximately(lastInput, 0.0f))
            targetX += lastInput < 0.0f ? horizontalOffset : -horizontalOffset;

        // Smoothly move the camera's X position towards the targetX
        cameraPos.x = Mathf.Lerp(cameraPos.x, targetX, 6 * Time.deltaTime); // 0.1f is smoothing, you can tweak

        transform.position = cameraPos;
    }

}
