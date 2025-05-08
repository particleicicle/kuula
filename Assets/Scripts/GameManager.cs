using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject gameOverPrefab;  
    [SerializeField]
    GameObject eventSystemPrefab;
    [SerializeField]
    GameObject backgroundPrefab;


    private static GameManager _instance;

    public static GameManager Instance
    {
        get {
            if(_instance == null)
                return new GameObject("GameManager").AddComponent<GameManager>();
            
            return _instance;
        }
    }

    public bool enableTimer = true;
    
    public static readonly WaitForFixedUpdate FixedUpdateDelay = new ();

    public LevelSetData levelSetData;

    GameObject gameOver;
    GameObject eventSystem;

    public readonly List<string> levelCompletionTimes = new ();

    public void GameOver(){
        gameOver = Instantiate(gameOverPrefab);
        //Destroy(_player.gameObject);
    }

    void Awake(){
        if(_instance != null && _instance != this){
            Destroy(this);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);

        if(!eventSystem){
            eventSystem = Instantiate(eventSystemPrefab);
            DontDestroyOnLoad(eventSystem);
        }
        

        SceneManager.activeSceneChanged += delegate(Scene _, Scene loadedScene) {
            StopAllCoroutines();

            bool inMainMenu = loadedScene.buildIndex <= 0;

            if(inMainMenu){
                _currentLevelSetName = string.Empty;
                levelCompletionTimes.Clear();
            }


            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if(playerGO != null){
                _player = playerGO.GetComponent<Player>();
                var camera = Camera.main;
                if(camera.gameObject.GetComponent<MoveCamera>() == null)
                    camera.gameObject.AddComponent<MoveCamera>();            
            }

            // var bg = Instantiate(backgroundPrefab);

            // Color color;
            // do {
            //     color = new Color(Random.value, Random.value, Random.value);
            // }
            // while(IsTooGreenOrDark(color));

            // var bgRenderer = bg.GetComponent<Renderer>();
            // bgRenderer.material.color = color;

            // if(inMainMenu)
            //     StartCoroutine(MainMenuColorLoop(bgRenderer));
        };
    }
    // private IEnumerator MainMenuColorLoop(Renderer _bgRenderer){
    //     Color color1 = _bgRenderer.material.color;
    //     Color color2;
    //     while(true) {
    //         do {
    //             color2 = new Color(Random.value, Random.value, Random.value);
    //         }
    //         while(IsTooGreenOrDark(color2));

    //         float startTime = Time.time;
    //         while(Time.time - startTime <= 2.0f) {

    //             if(_bgRenderer == null)
    //                 yield break;

    //             _bgRenderer.material.color = Color.Lerp(color1, color2, (Time.time - startTime) / 2.0f);

    //             yield return null;
    //         }

    //         color1 = color2;
    //         _bgRenderer.material.color = color1;
    //     }            
    // }   

    //  bool IsTooGreenOrDark(Color color)
    // {
    //     // Too green: green component much higher than red/blue
    //     if (color.g > Fractions.ThreeFifths && color.g > color.r + Fractions.OneFifth && color.g > color.b + Fractions.OneFifth)
    //         return true;

    //     double luminance = (0.2126 * color.r) + (0.7152 * color.g) + (0.0722 * color.b);
    //     if (luminance < Fractions.OneHalf)
    //         return true;

    //     return false;
    // }

    public Player Player {
        get => _player;
    }

    Player _player;

    public void LoadMainMenu()
        => SceneManager.LoadScene(0);
    public void ReloadCurrentLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    string _currentLevelSetName;
    public string CurrentLevelSetName 
    {
        get => _currentLevelSetName;
    }
    
    public void LoadNextLevel(bool async = false)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        foreach (var set in levelSetData.levelSets)
        {
            int index = set.levelSceneNames.IndexOf(currentScene);
            if (index != -1)
            {
                _currentLevelSetName = set.name;
                int nextIndex = index + 1;
                string nextScene = nextIndex >= set.levelSceneNames.Count ? "victory" : set.levelSceneNames[nextIndex % set.levelSceneNames.Count];
                if (async)
                    SceneManager.LoadSceneAsync(nextScene);
                else
                    SceneManager.LoadScene(nextScene);
                return;
            }
        }

        Debug.LogWarning("Current scene not found in any level set.");
    }

    const float DEATH_Y_THRESHOLD = -20.0f;

    private void LateUpdate(){
        if(Player != null && !Player.isDead && Player.position.y < DEATH_Y_THRESHOLD)
            Player.Die();
    }
}
