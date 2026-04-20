using UnityEngine;

public readonly struct DialogueLanguageChangedEvent
{
    public readonly SystemLanguage Language;

    public DialogueLanguageChangedEvent(SystemLanguage language)
    {
        Language = language;
    }
}
