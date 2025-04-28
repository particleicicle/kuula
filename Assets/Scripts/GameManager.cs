using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get {
            if(_instance == null)
                return new GameObject("GameManager").AddComponent<GameManager>();
            
            return _instance;
        }
    }
    
    public static readonly WaitForFixedUpdate FixedUpdateDelay = new ();

    private Renderer playerRenderer;


    void Awake(){
        if(_instance != null && _instance != this){
            Destroy(this);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);

        

        SceneManager.activeSceneChanged += delegate(Scene _, Scene loadedScene) {
            if(loadedScene.name == "level0")
                StartCoroutine(WaitForTouch());
            else 
                Camera.main.gameObject.AddComponent<MoveCamera>();
            
            playerOffscreenFor = 0;
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if(playerGO != null){
                _player = playerGO.GetComponent<Player>();
                playerRenderer = playerGO.GetComponent<Renderer>();
            }
        };
    }

    private IEnumerator WaitForTouch(){
        
        while(Input.touchCount < 1 && !Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.Space))
            yield return null;

        LoadNextLevel();
        yield break;
    }

    public Player Player {
        get => _player;
    }

    Player _player;

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
