using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using Framework.Utility;
using UnityEngine;

public enum ELanguage
{
    Chinese,
    English
}
public class GameModel : ISingleton
{
    public EItemType CurrentItemType { get; set; } = EItemType.None;
    public FocusPanelType CurrentFocusPanelType { get; set; } = FocusPanelType.None;
    public BindableProperty<ELanguage> CurrentLanguage { get; } = new BindableProperty<ELanguage>(ELanguage.English);

    public List<EItemType> GotItems = new();

    public Dictionary<string, EItemType> NoteItemDic = new();

    public void OnSingletonInit()
    {
        CurrentLanguage.Value = ELanguage.English;
    }

}
