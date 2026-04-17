using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

public class GameModel : ISingleton
{
    public EItemType CurrentItemType { get; set; } = EItemType.None;
    public FocusPanelType CurrentFocusPanelType { get; set; } = FocusPanelType.None;

    public List<EItemType> GotItems = new();

    public Dictionary<string, EItemType> NoteItemDic = new();

    public void OnSingletonInit()
    {

    }

}
