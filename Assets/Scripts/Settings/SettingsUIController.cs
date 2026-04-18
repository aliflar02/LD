using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [Header("按钮引用")]
    [Tooltip("返回按钮（Btn_Back）。")]
    [SerializeField] private Button btnBack;
    [Tooltip("退出按钮（Btn_Quit）。")]
    [SerializeField] private Button btnQuit;

    [Header("界面引用")]
    [Tooltip("返回时要恢复显示的界面对象（StartUI）。")]
    [SerializeField] private GameObject startUI;

    [Header("行为配置")]
    [Tooltip("Awake 时隐藏 SettingsUI。")]
    [SerializeField] private bool hideOnAwake = true;
    [Tooltip("点击返回时自动显示 StartUI。")]
    [SerializeField] private bool showStartUIOnBack = true;
    [Tooltip("允许 ESC 键执行返回。")]
    [SerializeField] private bool allowEscBack = true;

    [Header("事件回调")]
    [Tooltip("点击返回后的事件回调。")]
    public UnityEvent onBackRequested = new();
    [Tooltip("点击退出后的事件回调。")]
    public UnityEvent onQuitRequested = new();

    private void Awake()
    {
        if (hideOnAwake)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (btnBack != null) btnBack.onClick.AddListener(OnBackClicked);
        if (btnQuit != null) btnQuit.onClick.AddListener(OnQuitClicked);
    }

    private void OnDisable()
    {
        if (btnBack != null) btnBack.onClick.RemoveListener(OnBackClicked);
        if (btnQuit != null) btnQuit.onClick.RemoveListener(OnQuitClicked);
    }

    private void Update()
    {
        if (!allowEscBack) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackClicked();
        }
    }

    public void ShowFromStartUI()
    {
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnBackClicked()
    {
        onBackRequested.Invoke();
        Hide();

        if (showStartUIOnBack && startUI != null)
        {
            startUI.SetActive(true);
        }
    }

    public void OnQuitClicked()
    {
        onQuitRequested.Invoke();
        QuitGame();
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
