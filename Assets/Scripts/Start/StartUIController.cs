using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartUIController : MonoBehaviour
{
    [Header("按钮引用")]
    [Tooltip("开始按钮（Btn_Start）。")]
    [SerializeField] private Button btnStart;
    [Tooltip("设置入口按钮（Btn_Esc）。")]
    [SerializeField] private Button btnEsc;

    [Header("界面引用")]
    [Tooltip("设置界面控制器（SettingsUIController），优先使用该引用。")]
    [SerializeField] private SettingsUIController settingsUIController;
    [Tooltip("设置界面备用对象（SettingsUI），未绑定控制器时使用。")]
    [SerializeField] private GameObject settingsUIFallback;

    [Header("显示与输入")]
    [Tooltip("Awake 时自动隐藏设置界面。")]
    [SerializeField] private bool autoHideSettingsOnAwake = true;
    [Tooltip("点击开始后隐藏 StartUI。")]
    [SerializeField] private bool hideStartUIWhenStart = true;
    [Tooltip("打开设置时隐藏 StartUI。")]
    [SerializeField] private bool hideStartUIWhenOpenSettings = true;
    [Tooltip("点击开始后锁定开始按钮，防止重复触发。")]
    [SerializeField] private bool lockStartAfterClick = true;
    [Tooltip("允许回车键触发开始。")]
    [SerializeField] private bool allowEnterStart = true;
    [Tooltip("允许 ESC 键打开设置。")]
    [SerializeField] private bool allowEscOpenSettings = true;

    [Header("事件回调")]
    [Tooltip("点击开始后的事件回调。")]
    public UnityEvent onStartRequested = new();
    [Tooltip("点击设置入口后的事件回调。")]
    public UnityEvent onOpenSettingsRequested = new();

    private bool hasStarted;

    private void Awake()
    {
        if (autoHideSettingsOnAwake)
        {
            HideSettingsUI();
        }
    }

    private void OnEnable()
    {
        if (btnStart != null) btnStart.onClick.AddListener(OnStartClicked);
        if (btnEsc != null) btnEsc.onClick.AddListener(OnEscClicked);
    }

    private void OnDisable()
    {
        if (btnStart != null) btnStart.onClick.RemoveListener(OnStartClicked);
        if (btnEsc != null) btnEsc.onClick.RemoveListener(OnEscClicked);
    }

    private void Update()
    {
        if (allowEnterStart && Input.GetKeyDown(KeyCode.Return))
        {
            OnStartClicked();
            return;
        }

        if (allowEscOpenSettings && Input.GetKeyDown(KeyCode.Escape))
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
    }

    public void OnEscClicked()
    {
        onOpenSettingsRequested.Invoke();

        ShowSettingsUI();

        if (hideStartUIWhenOpenSettings)
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowSettingsUI()
    {
        if (settingsUIController != null)
        {
            settingsUIController.ShowFromStartUI();
            return;
        }

        if (settingsUIFallback != null)
        {
            settingsUIFallback.SetActive(true);
        }
    }

    private void HideSettingsUI()
    {
        if (settingsUIController != null)
        {
            settingsUIController.Hide();
            return;
        }

        if (settingsUIFallback != null)
        {
            settingsUIFallback.SetActive(false);
        }
    }
}
