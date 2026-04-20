using Framework.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartUIController : MonoBehaviour
{
    [Header("按钮引用")]
    [Tooltip("开始按钮（Btn_Start）。")]
    [SerializeField] private Button btnStart;
    [Tooltip("退出按钮（Btn_Esc）。")]
    [SerializeField] private Button btnEsc;
    [Tooltip("中文按钮。")]
    [SerializeField] private Button chineseButton;
    [Tooltip("英文按钮。")]
    [SerializeField] private Button englishButton;

    [Header("显示与输入")]
    [Tooltip("点击开始后隐藏 StartUI。")]
    [SerializeField] private bool hideStartUIWhenStart = true;
    [Tooltip("点击开始后锁定开始按钮，防止重复触发。")]
    [SerializeField] private bool lockStartAfterClick = true;
    [Tooltip("允许回车键触发开始。")]
    [SerializeField] private bool allowEnterStart = true;
    [Tooltip("允许 ESC 键直接退出游戏。")]
    [SerializeField] private bool allowEscQuit = true;

    [Header("事件回调")]
    [Tooltip("点击开始后的事件回调。")]
    public UnityEvent onStartRequested = new();

    private bool hasStarted;
    private IUnRegister languageChangedUnregister;

    private void OnEnable()
    {
        if (btnStart != null) btnStart.onClick.AddListener(OnStartClicked);
        if (btnEsc != null) btnEsc.onClick.AddListener(OnEscClicked);
        if (chineseButton != null) chineseButton.onClick.AddListener(OnChineseClicked);
        if (englishButton != null) englishButton.onClick.AddListener(OnEnglishClicked);
    }

    private void OnDisable()
    {
        if (btnStart != null) btnStart.onClick.RemoveListener(OnStartClicked);
        if (btnEsc != null) btnEsc.onClick.RemoveListener(OnEscClicked);
        if (chineseButton != null) chineseButton.onClick.RemoveListener(OnChineseClicked);
        if (englishButton != null) englishButton.onClick.RemoveListener(OnEnglishClicked);
    }

    private void Start()
    {
        languageChangedUnregister = GameManager.Instance.Model.CurrentLanguage.RegisterWithInitValue(UpdateLanguageButtonState);
    }

    private void OnDestroy()
    {
        languageChangedUnregister?.UnRegisterEvent();
        languageChangedUnregister = null;
    }

    private void Update()
    {
        if (allowEnterStart && Input.GetKeyDown(KeyCode.Return))
        {
            OnStartClicked();
            return;
        }

        if (allowEscQuit && Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscClicked();
        }
    }

    public void OnStartClicked()
    {
        if (hasStarted) return;

        if (lockStartAfterClick)
        {
            hasStarted = true;
            if (btnStart != null) btnStart.interactable = false;
        }

        onStartRequested.Invoke();

        if (hideStartUIWhenStart)
        {
            gameObject.SetActive(false);
        }

        UIManager.Instance.ShowUI("OverlayUI");
        UIManager.Instance.ShowUI("MainUI");
        UIManager.Instance.ShowUI("PersistentUI");
    }

    public void OnEscClicked()
    {
        QuitGame();
    }

    public void OnChineseClicked()
    {
        SwitchLanguage(ELanguage.Chinese);
    }

    public void OnEnglishClicked()
    {
        SwitchLanguage(ELanguage.English);
    }

    private void SwitchLanguage(ELanguage targetLanguage)
    {
        if (GameManager.Instance.Model.CurrentLanguage.Value == targetLanguage)
        {
            return;
        }

        GameManager.Instance.Model.CurrentLanguage.Value = targetLanguage;
    }

    private void UpdateLanguageButtonState(ELanguage currentLanguage)
    {
        if (chineseButton != null)
        {
            chineseButton.interactable = currentLanguage != ELanguage.Chinese;
        }

        if (englishButton != null)
        {
            englishButton.interactable = currentLanguage != ELanguage.English;
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
