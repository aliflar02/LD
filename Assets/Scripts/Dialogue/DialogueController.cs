using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("中文对白数据库（CN DialogueDatabase）。")]
    [FormerlySerializedAs("database")]
    [SerializeField] private DialogueDatabase databaseCN;
    [Tooltip("英文对白数据库（EN DialogueDatabase）。")]
    [SerializeField] private DialogueDatabase databaseEN;

    [Header("Intro")]
    [TextArea(2, 6)]
    [Tooltip("开场旁白中文文本（Intro Narration Text CN），默认显示在 NarrationRoot 上。")]
    [FormerlySerializedAs("introNarrationText")]
    [SerializeField] private string introNarrationTextCN = "你从黑暗中醒来，只记得一个模糊的声音告诉你，去你的意识深处，找到自己丢失的记忆……";
    [TextArea(2, 6)]
    [Tooltip("开场旁白英文文本（Intro Narration Text EN）。")]
    [SerializeField] private string introNarrationTextEN =
        "You wake up in the dark. A faint voice tells you to descend into your mind and recover the memories you lost...";
    [Tooltip("点击开场旁白后要播放的首个对白序列（Intro Sequence Id）。")]
    [SerializeField] private string introSequenceId = "SEQ_01_INTRO_WAKE";
    [Min(0)]
    [Tooltip("开场对白起始行索引（Intro Start Line Index），0 表示从 s1_l1 开始。")]
    [SerializeField] private int introStartLineIndex = 0;
    [Tooltip("启用时是否默认进入开场旁白模式。")]
    [SerializeField] private bool showIntroNarrationOnEnable = true;

    [Header("UI References")]
    [Tooltip("对话总系统根对象（DialogueUI），用于整体显示/隐藏。")]
    [FormerlySerializedAs("dialogueRoot")]
    [SerializeField] private GameObject dialogueUI;
    [Tooltip("标准对白模式根对象（DialogueRoot），与 NarrationRoot 平级。")]
    [FormerlySerializedAs("dialogueBox")]
    [SerializeField] private GameObject dialogueRoot;
    [Tooltip("标准对白文本组件（DialogueText / TMP_Text）。")]
    [SerializeField] private TMP_Text dialogueText;
    [Tooltip("说话人文本组件（SpeakerText / TMP_Text），可选。")]
    [SerializeField] private TMP_Text speakerText;
    [Tooltip("立绘组件（Portrait / Image），可选。")]
    [SerializeField] private Image portraitImage;
    [Tooltip("黑底旁白根对象（NarrationRoot），用于居中旁白模式。")]
    [SerializeField] private GameObject narrationRoot;
    [Tooltip("黑底旁白文本组件（NarrationText / TMP_Text）。")]
    [SerializeField] private TMP_Text narrationText;

    [Header("Input")]
    [Tooltip("是否允许鼠标左键触发推进（advance）。")]
    [SerializeField] private bool allowMouseClickAdvance = true;
    [Tooltip("是否允许空格键触发推进（advance）。")]
    [SerializeField] private bool allowSpaceAdvance = true;
    [Tooltip("是否允许回车键（Enter/KeypadEnter）触发推进（advance）。")]
    [SerializeField] private bool allowEnterAdvance = true;

    [Header("Playback")]
    [Min(0.001f)]
    [Tooltip("默认逐字间隔（defaultCharInterval，秒/字）。")]
    [SerializeField] private float defaultCharInterval = 0.05f;
    [Tooltip("空闲时隐藏（hideWhenIdle）：无对白播放时隐藏 DialogueUI。")]
    [SerializeField] private bool hideDialogueWhenIdle = true;
    [Tooltip("隐藏时清空文本（clearTextWhenHidden）。")]
    [SerializeField] private bool clearTextWhenHidden = true;

    [Header("Interaction Gate")]
    [Tooltip("播放门控对象（disableWhilePlaying）：对白播放中临时禁用，结束后恢复。")]
    [SerializeField] private List<GameObject> disableWhilePlaying = new();

    [Header("Events")]
    [Tooltip("序列开始事件（sequenceStarted），参数为 sequenceId。")]
    public UnityEvent<string> sequenceStarted = new();
    [Tooltip("序列结束事件（sequenceFinished），参数为 sequenceId。")]
    public UnityEvent<string> sequenceFinished = new();
    [Tooltip("信号事件（signalRaised），由序列/行中的 signal 字段触发。")]
    public UnityEvent<string> signalRaised = new();

    private readonly Dictionary<GameObject, bool> cachedActiveStates = new();

    private Coroutine playRoutine;
    private Coroutine introNarrationRoutine;
    private bool advanceRequested;
    private bool isPlaying;
    private bool isShowingIntroNarration;
    private bool isIntroNarrationTyping;
    private string currentSequenceId = string.Empty;
    private MUIEventListener dialogueRootListener;
    private MUIEventListener narrationRootListener;
    private IUnRegister languageChangedUnregister;
    private ELanguage currentLanguage = ELanguage.English;
    private DialogueDatabase activeDatabase;
    private string activeIntroNarrationText = string.Empty;

    public bool IsPlaying => isPlaying;
    public string CurrentSequenceId => currentSequenceId;

    private void Reset()
    {
        if (dialogueUI == null) dialogueUI = gameObject;

        var root = dialogueUI != null ? dialogueUI.transform : transform;
        if (dialogueRoot == null)
        {
            dialogueRoot = FindChildObject(root, "DialogueRoot");
            if (dialogueRoot == null)
            {
                dialogueRoot = FindChildObject(root, "DialogueBox");
            }
        }

        if (narrationRoot == null)
        {
            narrationRoot = FindChildObject(root, "NarrationRoot");
        }
    }

    private void Awake()
    {
        if (dialogueUI == null) dialogueUI = gameObject;
        if (GameManager.Instance != null)
        {
            currentLanguage = GameManager.Instance.Model.CurrentLanguage.Value;
        }

        ApplyLanguage(currentLanguage);
        ResolveMissingReferences();
        HookDialogueRootClick();
        HookNarrationRootClick();

        if (showIntroNarrationOnEnable)
        {
            ShowIntroNarration();
        }
        else if (hideDialogueWhenIdle)
        {
            SetDialogueVisible(false);
        }
    }

    private void Start()
    {
        RegisterLanguageChangedListener();
    }

    private void OnEnable()
    {
        if (!isPlaying)
        {
            if (showIntroNarrationOnEnable)
            {
                ShowIntroNarration();
            }
            else if (hideDialogueWhenIdle)
            {
                SetDialogueVisible(false);
            }
        }
    }

    private void Update()
    {
        if (isShowingIntroNarration && isIntroNarrationTyping)
        {
            if ((allowMouseClickAdvance && Input.GetMouseButtonDown(0)) ||
                (allowSpaceAdvance && Input.GetKeyDown(KeyCode.Space)) ||
                (allowEnterAdvance && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))))
            {
                advanceRequested = true;
            }

            return;
        }

        if (!isPlaying) return;

        if ((allowMouseClickAdvance && Input.GetMouseButtonDown(0)) ||
            (allowSpaceAdvance && Input.GetKeyDown(KeyCode.Space)) ||
            (allowEnterAdvance && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))))
        {
            RequestAdvance();
        }
    }

    private void OnDestroy()
    {
        StopIntroNarrationTyping();
        languageChangedUnregister?.UnRegisterEvent();
        languageChangedUnregister = null;
        UnHookDialogueRootClick();
        UnHookNarrationRootClick();
    }

    public bool PlaySequence(string sequenceId)
    {
        print("PlaySequence: " + sequenceId);
        return PlaySequence(sequenceId, 0);
    }

    public bool PlaySequence(string sequenceId, int startLineIndex)
    {
        var database = GetActiveDatabase();
        if (database == null)
        {
            Debug.LogError($"[{nameof(DialogueController)}] DialogueDatabase is not assigned.");
            return false;
        }

        if (!database.TryGetSequence(sequenceId, out var sequence))
        {
            Debug.LogWarning($"[{nameof(DialogueController)}] Sequence '{sequenceId}' not found.");
            return false;
        }

        return PlaySequence(sequence, startLineIndex);
    }

    public bool PlaySequence(DialogueSequence sequence)
    {
        return PlaySequence(sequence, 0);
    }

    public bool PlaySequence(DialogueSequence sequence, int startLineIndex)
    {
        if (sequence == null)
        {
            Debug.LogWarning($"[{nameof(DialogueController)}] Sequence is null.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(sequence.SequenceId))
        {
            Debug.LogWarning($"[{nameof(DialogueController)}] Sequence id is empty.");
            return false;
        }

        if (sequence.Lines == null || sequence.Lines.Count == 0)
        {
            Debug.LogWarning($"[{nameof(DialogueController)}] Sequence '{sequence.SequenceId}' has no lines.");
            return false;
        }

        var safeStartLineIndex = Mathf.Clamp(startLineIndex, 0, sequence.Lines.Count - 1);
        if (safeStartLineIndex != startLineIndex)
        {
            Debug.LogWarning(
                $"[{nameof(DialogueController)}] Start line index {startLineIndex} is out of range for '{sequence.SequenceId}', fallback to {safeStartLineIndex}.");
        }

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }

        StopIntroNarrationTyping();
        CleanupPlaybackState();
        playRoutine = StartCoroutine(PlaySequenceRoutine(sequence, safeStartLineIndex));
        return true;
    }

    public void StopSequence()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }

        StopIntroNarrationTyping();
        CleanupPlaybackState();
    }

    public void RequestAdvance()
    {
        if (!isPlaying) return;
        advanceRequested = true;
    }

    private IEnumerator PlaySequenceRoutine(DialogueSequence sequence, int startLineIndex)
    {
        isPlaying = true;
        isShowingIntroNarration = false;
        currentSequenceId = sequence.SequenceId;
        advanceRequested = false;

        var firstLine = GetFirstValidLine(sequence, startLineIndex);
        if (firstLine != null)
        {
            ApplyLineVisuals(firstLine);
            if (firstLine.ViewMode == DialogueLineViewMode.CenterBlackNarration && narrationRoot == null)
            {
                Debug.LogWarning($"[{nameof(DialogueController)}] First line is CenterBlackNarration, but narrationRoot is not assigned.");
            }
        }

        SetDialogueVisible(true);
        SetInteractionGate(true);
        RaiseSignal(sequence.OnSequenceStartSignal);
        sequenceStarted.Invoke(currentSequenceId);

        for (var i = startLineIndex; i < sequence.Lines.Count; i++)
        {
            var line = sequence.Lines[i];
            if (line == null) continue;

            ApplyLineVisuals(line);
            RaiseSignal(line.OnLineStartSignal);

            yield return TypeLineRoutine(line);

            RaiseSignal(line.OnLineEndSignal);

            if (line.AutoAdvanceDelay >= 0f)
            {
                yield return WaitAutoAdvance(line.AutoAdvanceDelay);
            }
            else
            {
                yield return WaitForAdvanceInput();
            }
        }

        sequenceFinished.Invoke(currentSequenceId);
        RaiseSignal(sequence.OnSequenceEndSignal);
        CleanupPlaybackState();
    }

    private IEnumerator TypeLineRoutine(DialogueLine line)
    {
        var targetText = GetLineTargetText(line);
        if (targetText == null)
        {
            yield break;
        }

        var content = line.Content ?? string.Empty;
        targetText.text = content;
        targetText.maxVisibleCharacters = 0;
        targetText.ForceMeshUpdate();

        var visibleCharCount = targetText.textInfo.characterCount;
        var interval = line.CharIntervalOverride > 0f ? line.CharIntervalOverride : defaultCharInterval;

        for (var visible = 1; visible <= visibleCharCount; visible++)
        {
            if (advanceRequested)
            {
                advanceRequested = false;
                break;
            }

            targetText.maxVisibleCharacters = visible;
            yield return new WaitForSeconds(interval);
        }

        targetText.maxVisibleCharacters = int.MaxValue;
    }

    private TMP_Text GetLineTargetText(DialogueLine line)
    {
        if (line.ViewMode == DialogueLineViewMode.CenterBlackNarration && narrationText != null)
        {
            return narrationText;
        }

        return dialogueText;
    }

    private IEnumerator WaitForAdvanceInput()
    {
        advanceRequested = false;
        while (!advanceRequested)
        {
            yield return null;
        }

        advanceRequested = false;
    }

    private IEnumerator WaitAutoAdvance(float delay)
    {
        advanceRequested = false;
        if (delay <= 0f) yield break;

        var elapsed = 0f;
        while (elapsed < delay)
        {
            if (advanceRequested)
            {
                advanceRequested = false;
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void ApplyLineVisuals(DialogueLine line)
    {
        var narrationMode = line.ViewMode == DialogueLineViewMode.CenterBlackNarration;

        if (narrationRoot != null)
        {
            narrationRoot.SetActive(narrationMode);
        }

        if (dialogueRoot != null)
        {
            dialogueRoot.SetActive(!narrationMode);
        }

        if (narrationMode)
        {
            if (speakerText != null)
            {
                speakerText.gameObject.SetActive(false);
                speakerText.text = string.Empty;
            }

            if (portraitImage != null)
            {
                portraitImage.gameObject.SetActive(false);
            }

            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
                dialogueText.maxVisibleCharacters = int.MaxValue;
            }

            return;
        }

        if (speakerText != null)
        {
            var hasSpeaker = !string.IsNullOrWhiteSpace(line.Speaker);
            speakerText.gameObject.SetActive(hasSpeaker);
            speakerText.text = hasSpeaker ? line.Speaker : string.Empty;
        }

        if (portraitImage != null)
        {
            if (line.Portrait != null)
            {
                portraitImage.sprite = line.Portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(!line.HidePortraitWhenNull);
            }
        }

        if (narrationText != null)
        {
            narrationText.text = string.Empty;
            narrationText.maxVisibleCharacters = int.MaxValue;
        }
    }

    private void SetDialogueVisible(bool visible)
    {
        var shouldToggleRoot = dialogueUI != null && dialogueUI != gameObject;
        if (hideDialogueWhenIdle && shouldToggleRoot)
        {
            dialogueUI.SetActive(visible);
        }
        else
        {
            if (!visible && dialogueRoot != null)
            {
                dialogueRoot.SetActive(false);
            }

            if (!visible && narrationRoot != null)
            {
                narrationRoot.SetActive(false);
            }
        }

        if (!visible && clearTextWhenHidden)
        {
            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
                dialogueText.maxVisibleCharacters = int.MaxValue;
            }

            if (speakerText != null)
            {
                speakerText.text = string.Empty;
            }

            if (narrationText != null)
            {
                narrationText.text = string.Empty;
                narrationText.maxVisibleCharacters = int.MaxValue;
            }
        }
    }

    private DialogueLine GetFirstValidLine(DialogueSequence sequence, int startLineIndex)
    {
        if (sequence?.Lines == null) return null;

        var startIndex = Mathf.Clamp(startLineIndex, 0, sequence.Lines.Count - 1);
        for (var i = startIndex; i < sequence.Lines.Count; i++)
        {
            if (sequence.Lines[i] != null)
            {
                return sequence.Lines[i];
            }
        }

        return null;
    }

    private void ShowIntroNarration()
    {
        StopIntroNarrationTyping();
        isShowingIntroNarration = true;
        isPlaying = false;
        advanceRequested = false;
        currentSequenceId = string.Empty;

        if (dialogueUI != null && dialogueUI != gameObject)
        {
            dialogueUI.SetActive(true);
        }

        if (narrationRoot != null)
        {
            narrationRoot.SetActive(true);
        }

        if (dialogueRoot != null)
        {
            dialogueRoot.SetActive(false);
        }

        if (narrationText != null)
        {
            narrationText.text = string.Empty;
            narrationText.maxVisibleCharacters = int.MaxValue;
            introNarrationRoutine = StartCoroutine(PlayIntroNarrationRoutine());
        }

        if (dialogueText != null)
        {
            dialogueText.text = string.Empty;
            dialogueText.maxVisibleCharacters = int.MaxValue;
        }

        if (speakerText != null)
        {
            speakerText.text = string.Empty;
            speakerText.gameObject.SetActive(false);
        }

        if (portraitImage != null)
        {
            portraitImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator PlayIntroNarrationRoutine()
    {
        if (narrationText == null)
        {
            isIntroNarrationTyping = false;
            introNarrationRoutine = null;
            yield break;
        }

        isIntroNarrationTyping = true;
        var content = activeIntroNarrationText ?? string.Empty;
        narrationText.text = content;
        narrationText.maxVisibleCharacters = 0;
        narrationText.ForceMeshUpdate();

        var visibleCharCount = narrationText.textInfo.characterCount;
        for (var visible = 1; visible <= visibleCharCount; visible++)
        {
            if (!isShowingIntroNarration)
            {
                break;
            }

            if (advanceRequested)
            {
                advanceRequested = false;
                break;
            }

            narrationText.maxVisibleCharacters = visible;
            yield return new WaitForSeconds(defaultCharInterval);
        }

        narrationText.maxVisibleCharacters = int.MaxValue;
        isIntroNarrationTyping = false;
        advanceRequested = false;
        introNarrationRoutine = null;
    }

    private void StopIntroNarrationTyping()
    {
        if (introNarrationRoutine != null)
        {
            StopCoroutine(introNarrationRoutine);
            introNarrationRoutine = null;
        }

        isIntroNarrationTyping = false;
        advanceRequested = false;
    }

    private void BeginIntroDialogue()
    {
        StopIntroNarrationTyping();
        isShowingIntroNarration = false;

        if (string.IsNullOrWhiteSpace(introSequenceId))
        {
            Debug.LogWarning($"[{nameof(DialogueController)}] Intro sequence id is empty.");
            if (narrationRoot != null)
            {
                narrationRoot.SetActive(false);
            }

            if (dialogueRoot != null)
            {
                dialogueRoot.SetActive(true);
            }

            return;
        }

        PlaySequence(introSequenceId, introStartLineIndex);
    }

    private void ResolveMissingReferences()
    {
        var root = dialogueUI != null ? dialogueUI.transform : transform;

        if (dialogueRoot == dialogueUI)
        {
            dialogueRoot = null;
        }

        if (dialogueRoot == null)
        {
            dialogueRoot = FindChildObject(root, "DialogueRoot");
            if (dialogueRoot == null)
            {
                dialogueRoot = FindChildObject(root, "DialogueBox");
            }
        }

        if (narrationRoot == null)
        {
            narrationRoot = FindChildObject(root, "NarrationRoot");
        }

        if (dialogueText == null && dialogueRoot != null)
        {
            dialogueText = dialogueRoot.GetComponentInChildren<TMP_Text>(true);
        }

        if (narrationText == null && narrationRoot != null)
        {
            narrationText = narrationRoot.GetComponentInChildren<TMP_Text>(true);
        }

        if (speakerText == null)
        {
            var speakerObject = FindChildObject(root, "SpeakerText");
            if (speakerObject != null)
            {
                speakerText = speakerObject.GetComponent<TMP_Text>();
            }
        }

        if (portraitImage == null)
        {
            var portraitObject = FindChildObject(root, "Portrait");
            if (portraitObject != null)
            {
                portraitImage = portraitObject.GetComponent<Image>();
            }
        }
    }

    private GameObject FindChildObject(Transform root, string childName)
    {
        if (root == null || string.IsNullOrWhiteSpace(childName))
        {
            return null;
        }

        var child = FindDeepChild(root, childName);
        return child != null ? child.gameObject : null;
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        if (parent.name == childName) return parent;

        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name == childName) return child;

            var result = FindDeepChild(child, childName);
            if (result != null) return result;
        }

        return null;
    }

    private void SetInteractionGate(bool lockInteraction)
    {
        if (lockInteraction)
        {
            cachedActiveStates.Clear();
            foreach (var go in disableWhilePlaying)
            {
                if (go == null || go == dialogueUI) continue;
                if (cachedActiveStates.ContainsKey(go)) continue;

                cachedActiveStates.Add(go, go.activeSelf);
                go.SetActive(false);
            }
        }
        else
        {
            foreach (var pair in cachedActiveStates)
            {
                if (pair.Key != null)
                {
                    pair.Key.SetActive(pair.Value);
                }
            }

            cachedActiveStates.Clear();
        }
    }

    private void CleanupPlaybackState()
    {
        playRoutine = null;
        isPlaying = false;
        advanceRequested = false;
        currentSequenceId = string.Empty;

        SetInteractionGate(false);
        if (hideDialogueWhenIdle)
        {
            SetDialogueVisible(false);
        }
    }

    private void RaiseSignal(string signalId)
    {
        if (string.IsNullOrWhiteSpace(signalId)) return;
        signalRaised.Invoke(signalId);
    }

    private void HookDialogueRootClick()
    {
        if (dialogueRoot == null) return;

        dialogueRootListener = MUIEventListener.Get(dialogueRoot);
        dialogueRootListener.onClick += OnDialogueRootClicked;
    }

    private void UnHookDialogueRootClick()
    {
        if (dialogueRootListener == null) return;

        dialogueRootListener.onClick -= OnDialogueRootClicked;
        dialogueRootListener = null;
    }

    private void HookNarrationRootClick()
    {
        if (narrationRoot == null) return;

        narrationRootListener = MUIEventListener.Get(narrationRoot);
        narrationRootListener.onClick += OnNarrationRootClicked;
    }

    private void UnHookNarrationRootClick()
    {
        if (narrationRootListener == null) return;

        narrationRootListener.onClick -= OnNarrationRootClicked;
        narrationRootListener = null;
    }

    private void OnDialogueRootClicked(GameObject _)
    {
        RequestAdvance();
    }

    private void OnNarrationRootClicked(GameObject _)
    {
        if (isShowingIntroNarration)
        {
            if (isIntroNarrationTyping)
            {
                advanceRequested = true;
                return;
            }

            BeginIntroDialogue();
            return;
        }

        RequestAdvance();
    }

    private void RegisterLanguageChangedListener()
    {
        if (languageChangedUnregister != null)
        {
            return;
        }

        languageChangedUnregister = GameManager.Instance.Model.CurrentLanguage.RegisterWithInitValue(OnLanguageChanged);
    }

    private void OnLanguageChanged(ELanguage language)
    {
        currentLanguage = language;
        ApplyLanguage(currentLanguage);
    }

    private void ApplyLanguage(ELanguage language)
    {
        activeDatabase = ResolveDatabase(language);
        activeIntroNarrationText = ResolveIntroNarrationText(language);
    }

    private DialogueDatabase GetActiveDatabase()
    {
        if (activeDatabase == null)
        {
            activeDatabase = ResolveDatabase(currentLanguage);
        }

        return activeDatabase;
    }

    private DialogueDatabase ResolveDatabase(ELanguage language)
    {
        return language switch
        {
            ELanguage.English => databaseEN != null ? databaseEN : databaseCN,
            ELanguage.Chinese => databaseCN != null ? databaseCN : databaseEN,
            _ => databaseCN != null ? databaseCN : databaseEN
        };
    }

    private string ResolveIntroNarrationText(ELanguage language)
    {
        return language switch
        {
            ELanguage.English => !string.IsNullOrWhiteSpace(introNarrationTextEN)
                ? introNarrationTextEN
                : introNarrationTextCN,
            ELanguage.Chinese => !string.IsNullOrWhiteSpace(introNarrationTextCN)
                ? introNarrationTextCN
                : introNarrationTextEN,
            _ => !string.IsNullOrWhiteSpace(introNarrationTextCN)
                ? introNarrationTextCN
                : introNarrationTextEN
        };
    }
}
