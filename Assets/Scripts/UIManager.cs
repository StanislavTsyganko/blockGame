using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.Audio.ProcessorInstance;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _backgroundTilemap;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject endGameMenu;
    [SerializeField] private GameObject endGameBackground;
    public TextMeshProUGUI endGameText;

    public Button playButton;
    public Button exitButton;
    public Button nextButton;
    public Button restartButton;
    public Button menuButton;

    public Sprite piecePlacementFrame;

    public void ShowLoading()
    {

    }

    public void ShowMainMenu()
    {
        endGameMenu.SetActive(false);
        endGameBackground.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowEndGameMenu(bool isSuccess)
    {
        mainMenu.SetActive(false);
        endGameMenu.SetActive(true);
        endGameBackground.SetActive(true);
        if (isSuccess)
        {
            restartButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(true);
        }

    }

    public void CloseAllMenu()
    {
        endGameMenu.SetActive(false);
        endGameBackground.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void OnPlayButtonClick()
    {
        CloseAllMenu();
    }

    public void OnRestartButtonClick()
    {
        CloseAllMenu();
    }

    public void OnMenuButtonClick()
    {
        ShowMainMenu();
    }

    public void OnNextLevelButtonClick()
    {
        CloseAllMenu();
    }

    public void OnExitButtonClick()
    {
        CloseAllMenu();
    }

    public void OnWin()
    {
        endGameText.text = "Win!";
        ShowEndGameMenu(true);
    }

    public void OnLose()
    {
        endGameText.text = "Lose";
        ShowEndGameMenu(false);
    }

    public GameObject SpawnFigurePlacementFrame(float height, float width, Vector3 position)
    {
        if (piecePlacementFrame == null)
        {
            Debug.LogWarning("[PieceManager] piecePlacementFrame = null!");
            return null;
        }

        GameObject frame = new GameObject("Frame");
        SpriteRenderer sr = frame.AddComponent<SpriteRenderer>();
        sr.sprite = piecePlacementFrame;
        Vector2 spriteSize = sr.sprite.bounds.size;

        frame.transform.localScale = new Vector3(width / spriteSize.x, height / spriteSize.y, 1f);
        frame.transform.position = position;

        return frame;
    }
}
