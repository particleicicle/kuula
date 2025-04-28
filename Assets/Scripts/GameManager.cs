using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get {
            return _instance;
        }
    }

    private Renderer playerRenderer;


    void Awake(){
        if(_instance != null && _instance != this){
            Destroy(this);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += delegate(Scene _, Scene _) {
            playerOffscreenFor = 0;
            playerRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>();
        };
    }

    void Start(){
        LoadNextLevel();
    }

    public void ReloadCurrentLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel(){
        var currentLevel = SceneManager.GetActiveScene().name.ToLower();
        //Debug.Log(currentLevel);
        //Get level number from the level name
        var levelNum = int.Parse(currentLevel.Replace("level", string.Empty)) + 1;
        SceneManager.LoadScene("level" + levelNum);
    }

    private float playerOffscreenFor;

    private void LateUpdate(){
        if(playerRenderer != null && !playerRenderer.isVisible)
        {
            playerOffscreenFor += Time.deltaTime;
            if(playerOffscreenFor > 2.0f)
                ReloadCurrentLevel();
        }
        else
            playerOffscreenFor = 0;
    }
}
