using UnityEngine;

public class Obstacle : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("moi");
        if(other.attachedRigidbody && other.attachedRigidbody.gameObject.layer == LayerMask.NameToLayer("Player")) {
            GameManager.Instance.ReloadCurrentLevel();
        }
    }
}
