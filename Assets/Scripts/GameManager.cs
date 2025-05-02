using System.Collections;
using System.Linq;
using System.Text;
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
    
    public static readonly WaitForFixedUpdate FixedUpdateDelay = new ();

    private Renderer playerRenderer;

    GameObject gameOver;
    GameObject eventSystem;

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

            if(loadedScene.name == "level0")
                StartCoroutine(WaitForTouch());
            
            playerOffscreenFor = 0;
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if(playerGO != null){
                _player = playerGO.GetComponent<Player>();
                playerRenderer = playerGO.GetComponent<Renderer>();

                var camera = Camera.main;
                camera.gameObject.AddComponent<MoveCamera>();            
            }

            var bg = Instantiate(backgroundPrefab);

            Color color;
            do {
                color = new Color(Random.value, Random.value, Random.value);
            }
            while(IsTooGreenOrDark(color));

            var bgRenderer = bg.GetComponent<Renderer>();
            bgRenderer.material.color = color;
            if(loadedScene.buildIndex == 0)
                StartCoroutine(MainMenuColorLoop(bgRenderer));
        };
    }

    private IEnumerator MainMenuColorLoop(Renderer _bgRenderer){
        Color color1 = _bgRenderer.material.color;
        Color color2;
        while(true) {
            do {
                color2 = new Color(Random.value, Random.value, Random.value);
            }
            while(IsTooGreenOrDark(color2));

            float startTime = Time.time;
            while(Time.time - startTime <= 2.0f) {
                _bgRenderer.material.color = Color.Lerp(color1, color2, (Time.time - startTime) / 2.0f);
                yield return null;
            }

            color1 = color2;
            _bgRenderer.material.color = color1;
        }            
    }   

     bool IsTooGreenOrDark(Color color)
    {
        // Too green: green component much higher than red/blue
        if (color.g > Fractions.ThreeFifths && color.g > color.r + Fractions.OneFifth && color.g > color.b + Fractions.OneFifth)
            return true;


        float luminance = 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
        if (luminance < Fractions.OneThird)
            return true;

        return false;
    }

    private IEnumerator WaitForTouch(){

        yield return new WaitForSeconds((float)Fractions.OneFifth);
        StringBuilder menuInput = new();
        while(Input.touchCount < 1)
        {
            menuInput.Append(Input.inputString);

            if(menuInput.Length <= 0 && Input.GetKeyDown(KeyCode.Space))
                break;

            if(Input.GetKeyDown(KeyCode.Return)){
                var str = menuInput.ToString().ToLower();
                if(str.StartsWith("goto ")){
                    SceneManager.LoadScene(str.Replace("goto ", string.Empty).Trim());
                    yield break;
                }
                else
                    break;
            }
                
            yield return null;
        }

        LoadNextLevel(true);
        yield break;
    }

    public Player Player {
        get => _player;
    }

    Player _player;

    public void LoadMainMenu()
        => SceneManager.LoadScene("level0");
    public void ReloadCurrentLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel(bool async = false){
        var levelNum = SceneManager.GetActiveScene().buildIndex + 1;
        if(!async) SceneManager.LoadScene(levelNum);
        else SceneManager.LoadSceneAsync(levelNum);
    }

    private float playerOffscreenFor;

    private void LateUpdate(){
        if(playerRenderer != null && !playerRenderer.isVisible)
        {
            playerOffscreenFor += Time.deltaTime;
            if(playerOffscreenFor > 1.0f){
                Player.Die();
                playerOffscreenFor = -1000.0f;
            }
        }
        else
            playerOffscreenFor = 0;
    }
}
