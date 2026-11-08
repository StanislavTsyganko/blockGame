using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

//public enum EndLevelType
//{
//    Win,
//    Lose,
//    End,
//}

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _backgroundTilemap;
    [Header("Menu")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject endGameMenu;
    [Header("Level target")]
    [SerializeField] private GameObject levelTargetPanel;
    [SerializeField] private GameObject levelTargetBackground;
    [SerializeField] private GameObject levelTargetString;
    [Header("End game")]
    [SerializeField] private GameObject endGameBackground;
    [SerializeField] public TextMeshProUGUI endGameText;
    [Header("UI")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject levelTargetUI;

    [Header("Buttons")]
    public Button playButton;
    public Button exitButton;
    public Button nextButton;
    public Button restartButton;
    public Button continueButton;
    public Button menuButton;

    public Sprite piecePlacementFrame;
    public GameObject placementObject;
    private List<GameObject> levelTargetStrings = new List<GameObject>(); 
    private List<GameObject> levelTargetStringsUI = new List<GameObject>(); 

    public void ShowLoading()
    {

    }

    public void ShowMainMenu()
    {
        inGameUI.SetActive(false);
        endGameMenu.SetActive(false);
        endGameBackground.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void GenerateLevelTarget(Dictionary<TileInfo, int> uniqTiles)
    {
        foreach (var (tile,count) in uniqTiles)
        {
            // start game target
            GameObject newObj = Instantiate(levelTargetString, levelTargetBackground.transform, false);
            TileSlotUI slotScript = newObj.GetComponent<TileSlotUI>();
            slotScript.Setup(tile.tile, tile.color, count);
            levelTargetStrings.Add(newObj);

            // ui target
            GameObject newObjUI = Instantiate(levelTargetString, levelTargetUI.transform, false);
            TileSlotUI slotScriptUI = newObjUI.GetComponent<TileSlotUI>();
            slotScriptUI.Setup(tile.tile, tile.color, count);
            levelTargetStringsUI.Add(newObjUI);
        }
    }

    public void ShowLevelTarget()
    {
        levelTargetPanel.SetActive(true);
        levelTargetUI.SetActive(true);
    }

    public void UpdateLevelTargetUI(Dictionary<TileInfo, int> uniqTiles)
    {
        foreach (var levelString in levelTargetStringsUI)
        {
            TileSlotUI slotScriptUI = levelString.GetComponent<TileSlotUI>();
            if (!slotScriptUI)
                continue;
            int uniqCount = uniqTiles.TryGetValue(new TileInfo(slotScriptUI.tileBase, slotScriptUI.tileColor), out var count) ? count : 0;
            slotScriptUI.SetCount(uniqCount);
        }
    }

    public void CloseLevelTarget()
    {
        levelTargetPanel.SetActive(false);
        foreach(var targetSting in levelTargetStrings)
            Destroy(targetSting);
    }

    public void ClearLevelTargetUI()
    {
        levelTargetUI.SetActive(false);
        foreach (var targetSting in levelTargetStringsUI)
            Destroy(targetSting);
        levelTargetStringsUI.Clear();
    }

    public void ShowEndGameMenu(bool isSuccess)
    {
        mainMenu.SetActive(false);
        endGameMenu.SetActive(true);
        endGameBackground.SetActive(true);
        continueButton.gameObject.SetActive(false);
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
        endGameText.text = "";
        endGameMenu.SetActive(false);
        endGameBackground.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void OnPlayButtonClick()
    {
        CloseAllMenu();
        inGameUI.SetActive(true);
    }

    public void OnRestartButtonClick()
    {
        CloseAllMenu();
        ClearLevelTargetUI();
    }

    public void OnNextLevelButtonClick()
    {
        CloseAllMenu();
        ClearLevelTargetUI();
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

    public void OnPause()
    {
        endGameBackground.SetActive(true);
        endGameMenu.SetActive(true);
        restartButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    public void OnLose()
    {
        endGameText.text = "Lose";
        ShowEndGameMenu(false);
    }

    public void OnNoMoreLevels()
    {
        Debug.Log("No more level");
        endGameMenu.SetActive(true);
        playButton.enabled = false;
        endGameText.text = "No more levels";
        endGameText.enabled = true;
    }


    public GameObject SpawnFigurePlacementFrame(float height, float width, Vector3 position)
    {
        if (piecePlacementFrame == null)
        {
            Debug.LogWarning("[PieceManager] piecePlacementFrame = null!");
            return null;
        }
        if (placementObject)
            Destroy(placementObject);

        GameObject frame = new GameObject("Frame");
        SpriteRenderer sr = frame.AddComponent<SpriteRenderer>();
        sr.sprite = piecePlacementFrame;
        Vector2 spriteSize = sr.sprite.bounds.size;

        frame.transform.localScale = new Vector3(width / spriteSize.x, height / spriteSize.y, 1f);
        frame.transform.position = position;

        placementObject = frame;

        return frame;
    }

    public void ClearAll()
    {
        if (placementObject)
            Destroy(placementObject);
        ClearLevelTargetUI();
    }
}
