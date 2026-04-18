using UnityEngine;

public class DialogueSequenceTrigger : MonoBehaviour
{
    [Tooltip("目标对话控制器（DialogueController），用于执行 PlaySequence。")]
    [SerializeField] private DialogueController dialogueController;
    [Tooltip("要触发的对白序列标识（sequenceId），需与 DialogueDatabase 一致。")]
    [SerializeField] private string sequenceId;
    [Tooltip("启动即播（playOnStart）：在 Start 阶段自动触发。")]
    [SerializeField] private bool playOnStart;
    [Tooltip("点击触发（triggerByClick）：对象被点击时通过 MUIEventListener 触发。")]
    [SerializeField] private bool triggerByClick;

    private MUIEventListener listener;

    private void Start()
    {
        if (triggerByClick)
        {
            listener = MUIEventListener.Get(gameObject);
            listener.onClick += OnClicked;
        }

        if (playOnStart)
        {
            Trigger();
        }
    }

    private void OnDestroy()
    {
        if (listener != null)
        {
            listener.onClick -= OnClicked;
            listener = null;
        }
    }

    public void Trigger()
    {
        if (dialogueController == null)
        {
            Debug.LogWarning($"[{nameof(DialogueSequenceTrigger)}] DialogueController is not assigned on '{name}'.");
            return;
        }

        if (string.IsNullOrWhiteSpace(sequenceId))
        {
            Debug.LogWarning($"[{nameof(DialogueSequenceTrigger)}] sequenceId is empty on '{name}'.");
            return;
        }

        dialogueController.PlaySequence(sequenceId);
    }

    private void OnClicked(GameObject _)
    {
        Trigger();
    }
}
