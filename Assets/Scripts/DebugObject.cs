using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObject : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (GameManager.Instance.Model.CurrentLanguage.Value == ELanguage.English)
                GameManager.Instance.Model.CurrentLanguage.Value = ELanguage.Chinese;
            else
                GameManager.Instance.Model.CurrentLanguage.Value = ELanguage.English;
        }
    }
}
