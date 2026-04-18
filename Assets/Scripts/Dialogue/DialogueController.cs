using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("对白数据库（DialogueDatabase），提供 sequenceId 对应的对白序列。")]
    [SerializeField] private DialogueDatabase database;

    [Header("UI References")]
    [Tooltip("对话界面根对象（DialogueUI root），用于整体显示/隐藏。")]
    [SerializeField] private GameObject dialogueRoot;
    [Tooltip("对白点击区域（DialogueBox），点击后请求推进对白。")]
    [SerializeField] private GameObject dialogueBox;
    [Tooltip("对白文本组件（DialogueText / TMP_Text），显示当前对白内容。")]
    [SerializeField] private TMP_Text dialogueText;
    [Tooltip("说话人文本组件（speakerText / TMP_Text），可选。")]
    [SerializeField] private TMP_Text speakerText;
    [Tooltip("立绘组件（portraitImage / Image），可选。")]
    [SerializeField] private Image portraitImage;

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
    [Tooltip("空闲时隐藏（hideWhenIdle）：无对白播放时隐藏 DialogueUI / DialogueBox。")]
    [SerializeField] private bool hideDialogueWhenIdle = true;
    [Tooltip("隐藏时清空文本（clearTextWhenHidden）：清除对白文本与说话人文本。")]
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
    private bool advanceRequested;
    private bool isPlaying;
    private string currentSequenceId = string.Empty;
    private MUIEventListener dialogueBoxListener;

    public bool IsPlaying => isPlaying;
    public string CurrentSequenceId => currentSequenceId;

    private void Reset()
    {
        if (dialogueRoot == null) dialogueRoot = gameObject;
        if (dialogueBox == null) dialogueBox = gameObject;
    }

    private void Awake()
    {
        if (dialogueRoot == null) dialogueRoot = gameObject;
        HookDialogueBoxClick();
        if (hideDialogueWhenIdle) SetDialogueVisible(false);
    }

    private void Update()
    {
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
        UnHookDialogueBoxClick();
    }

    public bool PlaySequence(string sequenceId)
    {
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

        return PlaySequence(sequence);
    }

    public bool PlaySequence(DialogueSequence sequence)
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

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }

        CleanupPlaybackState();
        playRoutine = StartCoroutine(PlaySequenceRoutine(sequence));
        return true;
    }

    public void StopSequence()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }

        CleanupPlaybackState();
    }

    public void RequestAdvance()
    {
        if (!isPlaying) return;
        advanceRequested = true;
    }

    private IEnumerator PlaySequenceRoutine(DialogueSequence sequence)
    {
        isPlaying = true;
        currentSequenceId = sequence.SequenceId;
        advanceRequested = false;

        SetDialogueVisible(true);
        SetInteractionGate(true);
        RaiseSignal(sequence.OnSequenceStartSignal);
        sequenceStarted.Invoke(currentSequenceId);

        for (var i = 0; i < sequence.Lines.Count; i++)
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
        if (dialogueText == null)
        {
            yield break;
        }

        var content = line.Content ?? string.Empty;
        dialogueText.text = content;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();

        var visibleCharCount = dialogueText.textInfo.characterCount;
        var interval = line.CharIntervalOverride > 0f ? line.CharIntervalOverride : defaultCharInterval;

        for (var visible = 1; visible <= visibleCharCount; visible++)
        {
            if (advanceRequested)
            {
                advanceRequested = false;
                break;
            }

            dialogueText.maxVisibleCharacters = visible;
            yield return new WaitForSeconds(interval);
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;
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
    }

    private void SetDialogueVisible(bool visible)
    {
        var shouldToggleRoot = dialogueRoot != null && dialogueRoot != gameObject;
        if (hideDialogueWhenIdle && shouldToggleRoot)
        {
            dialogueRoot.SetActive(visible);
        }
        else if (dialogueBox != null)
        {
            dialogueBox.SetActive(visible);
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
        }
    }

    private void SetInteractionGate(bool lockInteraction)
    {
        if (lockInteraction)
        {
            cachedActiveStates.Clear();
            foreach (var go in disableWhilePlaying)
            {
                if (go == null || go == dialogueRoot) continue;
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

    private void HookDialogueBoxClick()
    {
        if (dialogueBox == null) return;

        dialogueBoxListener = MUIEventListener.Get(dialogueBox);
        dialogueBoxListener.onClick += OnDialogueBoxClicked;
    }

    private void UnHookDialogueBoxClick()
    {
        if (dialogueBoxListener == null) return;
        dialogueBoxListener.onClick -= OnDialogueBoxClicked;
        dialogueBoxListener = null;
    }

    private void OnDialogueBoxClicked(GameObject _)
    {
        RequestAdvance();
    }
}
