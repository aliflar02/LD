using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "LD/Dialogue/Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    [Tooltip("对白序列列表（sequences），存放当前数据库内可播放的全部对白序列。")]
    [SerializeField] private List<DialogueSequence> sequences = new();

    private readonly Dictionary<string, DialogueSequence> cache = new(StringComparer.Ordinal);
    private bool cacheBuilt;

    public IReadOnlyList<DialogueSequence> Sequences => sequences;

    public bool TryGetSequence(string sequenceId, out DialogueSequence sequence)
    {
        EnsureCache();
        return cache.TryGetValue(sequenceId?.Trim() ?? string.Empty, out sequence);
    }

    public void RebuildCache()
    {
        cache.Clear();
        cacheBuilt = true;

        if (sequences == null) return;

        foreach (var sequence in sequences)
        {
            if (sequence == null) continue;

            var key = sequence.SequenceId?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"[{nameof(DialogueDatabase)}] Empty sequence id found in '{name}'.");
                continue;
            }

            if (cache.ContainsKey(key))
            {
                Debug.LogWarning($"[{nameof(DialogueDatabase)}] Duplicate sequence id '{key}' in '{name}'.");
                continue;
            }

            cache.Add(key, sequence);
        }
    }

    private void EnsureCache()
    {
        if (!cacheBuilt)
        {
            RebuildCache();
        }
    }

    private void OnEnable()
    {
        cacheBuilt = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        cacheBuilt = false;
    }
#endif
}
