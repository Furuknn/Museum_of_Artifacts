using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class UIManager : MonoBehaviour, IGameStateListener
{
    public static UIManager instance { get; private set; }
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameIntro;
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject skillTreePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverWinPanel;
    [SerializeField] private GameObject gameOverLosePanel;

    [Header("Player Health Bar In Game UI")]
    [SerializeField] private Image playerHealthBarInGameUI;
    [SerializeField] private Image playerHealthBarFadeUI;
    [Header("Player XP Bar In Game UI")]
    [SerializeField] private Image playerXpBarInGameUI;
    [SerializeField] private TextMeshProUGUI playerXpPointTextUI;

    void Awake()
    {
        instance = this;
    }
    public void GameStateChangedCallBack(EGameState gameState)
    {
        menuPanel.SetActive(gameState == EGameState.MAINMENU);
        gameIntro.SetActive(gameState == EGameState.GAMEINTRO);
        characterSelectionPanel.SetActive(gameState == EGameState.CHARACTERSELECTION);
        inGamePanel.SetActive(gameState == EGameState.INGAME);
        skillTreePanel.SetActive(gameState == EGameState.INSKILLTREE);
        pausePanel.SetActive(gameState == EGameState.PAUSE);
        gameOverWinPanel.SetActive(gameState == EGameState.GAMEOVERWIN);
        gameOverLosePanel.SetActive(gameState == EGameState.GAMEOVERLOSE);
    }
    private bool isGamePaused = false;
    public void PauseMenuToggle()
    {
        if (GameManager.instance.gameState == EGameState.INSKILLTREE)
        {
            ToggleSkillTree();
            return;
        }
        

        if (!isGamePaused && GameManager.instance.gameState == EGameState.INGAME)
        {
            GameManager.instance.StopGame(EGameState.PAUSE);
        }
        else if (isGamePaused && GameManager.instance.gameState == EGameState.PAUSE)
        {
            GameManager.instance.ContinueGame();
        }

        isGamePaused = !isGamePaused;

    }
    public void ToggleSkillTree()
    {
        if (GameManager.instance.gameState == EGameState.INSKILLTREE) //if skill tree is already open, close it and set the game state to in game
        {
            GameManager.instance.ContinueGame();
        }
        else
        {
            GameManager.instance.StopGame(EGameState.INSKILLTREE);
        }
    }
    public Image GetInGameHealthBar()
    {
        return playerHealthBarInGameUI;
    }
    public Image GetInGameHealthBarFade()
    {
        return playerHealthBarFadeUI;
    }

    public Image GetInGameXpBar()
    {
        return playerXpBarInGameUI;
    }
    public TextMeshProUGUI GetInGameLevelPointText()
    {
        return playerXpPointTextUI;
    }
}
