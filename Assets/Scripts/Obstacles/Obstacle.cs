using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("moi");
        if(other.attachedRigidbody && other.attachedRigidbody.gameObject.layer == LayerMask.NameToLayer("Player")) {
            GameManager.Instance.ReloadCurrentLevel();
        }
    }
}
