using UnityEngine;
using UnityEngine.UI;

public class GUIContoller : MonoBehaviour
{
    [SerializeField]
    private Transform pressActionKey, hud;

    [SerializeField]
    Image staminaFullIcon;

    [SerializeField]
    RawImage gameOver;

    [SerializeField]
    Text quantBombText, quantTrapText;

    bool isActionKeyActive, isGameOver;
    string displayText;
    int quantBomb = 0, quantTrap = 0;

    private float deathScreenTimer = 4.0f;

    public int CurrQuantBomb { set { quantBomb = value; UpdateAmountItens(); } }
    public int CurrQuantTrap { set { quantTrap = value; UpdateAmountItens(); } }
    public bool IsActionKeyActive { set { isActionKeyActive = value; DisplayMessage(); } }
    public string DisplayText { set { displayText = value; } }

    private void Awake()
    {
        isActionKeyActive = false;
        GlobalSettings.gGUI = this;
    }

    private void Update()
    {
        staminaFullIcon.fillAmount = GlobalSettings.gPlayer.CurrStamina / 100;

        if (GlobalSettings.gGameState == (int)GlobalSettings.GAME_STATE.DEATH)
        {
            gameOver.gameObject.SetActive(true);
            hud.gameObject.SetActive(false);
            isGameOver = true;
        }

        if (isGameOver)
        {
            deathScreenTimer -= 1 * Time.deltaTime;
            Debug.Log(deathScreenTimer);
            if (deathScreenTimer <= 0.0f)
            {
                GlobalSettings.gCurrentScene = 0;
                UnityEngine.SceneManagement.SceneManager.LoadScene(GlobalSettings.gCurrentScene);
            }
        }

    }

    private void DisplayMessage()
    {
        Text _text = pressActionKey.gameObject.GetComponentInChildren<Text>();

        _text.text = displayText;
        pressActionKey.gameObject.SetActive(isActionKeyActive);
    }

    private void UpdateAmountItens()
    {
        quantBombText.text = quantBomb.ToString();
        quantTrapText.text = quantTrap.ToString();
    }
}
