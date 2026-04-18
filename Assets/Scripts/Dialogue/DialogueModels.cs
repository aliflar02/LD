using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [Tooltip("对白行标识（lineId），可选，主要用于调试定位。")]
    [SerializeField] private string lineId = "line";
    [Tooltip("说话人名称（speaker），显示到说话人文本（speakerText）。")]
    [SerializeField] private string speaker = "";
    [TextArea(2, 6)]
    [Tooltip("对白内容（content），由逐字机（typewriter）按字显示。")]
    [SerializeField] private string content = "";
    [Tooltip("立绘图片（portrait）；留空时按下方规则决定显示或隐藏。")]
    [SerializeField] private Sprite portrait;
    [Tooltip("当立绘（portrait）为空时：true=隐藏立绘组件（portraitImage）。")]
    [SerializeField] private bool hidePortraitWhenNull = true;
    [Tooltip("逐字间隔覆盖值（charIntervalOverride）：<= 0 时使用默认值（defaultCharInterval）。")]
    [SerializeField] private float charIntervalOverride = -1f;
    [Tooltip("自动推进延迟（autoAdvanceDelay）：>= 0 自动推进；< 0 等待玩家手动推进。")]
    [SerializeField] private float autoAdvanceDelay = -1f;
    [Tooltip("行开始信号（onLineStartSignal），可选，用于事件联动。")]
    [SerializeField] private string onLineStartSignal = "";
    [Tooltip("行结束信号（onLineEndSignal），可选，用于事件联动。")]
    [SerializeField] private string onLineEndSignal = "";

    public string LineId => lineId;
    public string Speaker => speaker;
    public string Content => content;
    public Sprite Portrait => portrait;
    public bool HidePortraitWhenNull => hidePortraitWhenNull;
    public float CharIntervalOverride => charIntervalOverride;
    public float AutoAdvanceDelay => autoAdvanceDelay;
    public string OnLineStartSignal => onLineStartSignal;
    public string OnLineEndSignal => onLineEndSignal;
}

[Serializable]
public class DialogueSequence
{
    [Tooltip("对白序列标识（sequenceId），用于 PlaySequence(sequenceId) 调用。")]
    [SerializeField] private string sequenceId = "sequence_id";
    [TextArea(1, 3)]
    [Tooltip("序列说明（description），仅用于编辑器备注。")]
    [SerializeField] private string description = "";
    [Tooltip("序列开始信号（onSequenceStartSignal），可选。")]
    [SerializeField] private string onSequenceStartSignal = "";
    [Tooltip("序列结束信号（onSequenceEndSignal），可选。")]
    [SerializeField] private string onSequenceEndSignal = "";
    [Tooltip("对白行列表（lines），按顺序依次播放。")]
    [SerializeField] private List<DialogueLine> lines = new();

    public string SequenceId => sequenceId;
    public string Description => description;
    public string OnSequenceStartSignal => onSequenceStartSignal;
    public string OnSequenceEndSignal => onSequenceEndSignal;
    public List<DialogueLine> Lines => lines;
}
