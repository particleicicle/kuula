using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using TMPro;

public class GenerateLevelSelect : MonoBehaviour
{

    [SerializeField]
    GameObject startButton;
    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    Transform buttonContainer;
    public float columnSpacing = 450f;
    public float rowSpacing = 150f;
    public float startY = -125f;
    public int columns = 3;

    public void Generate()
    {
        if(startButton != null){
            startButton.SetActive(false);
        }

        int index = 0;

        foreach (var set in GameManager.Instance.levelSetData.levelSets)
        {
            if (set.levelSceneNames.Count == 0) continue;

            string firstLevel = set.levelSceneNames[0];

            var button = Instantiate(buttonPrefab, buttonContainer);

            // Set button label using TextMeshPro
            var tmpText = button.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) {
                tmpText.text = $"Set {set.setNumber}";
            }

            // Calculate position
            int col = index % columns;
            int row = index / columns;

            float x = col * columnSpacing;
            float y = startY - row * rowSpacing;

            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            var buttonMono = button.GetComponent<Button>();
            buttonMono.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(firstLevel);
            });

            index++;
        }
    }
}
