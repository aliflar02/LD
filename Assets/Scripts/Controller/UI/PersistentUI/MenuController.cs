using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    [SerializeField] private Button btnOpenMenu;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;
    // Start is called before the first frame update
    void Start()
    {
        menuPanel.SetActive(false);
        btnOpenMenu.onClick.AddListener(() =>
        {
            Debug.Log("Open Menu");
            menuPanel.SetActive(true);
        });
        btnYes.onClick.AddListener(() =>
        {
            Debug.Log("Yes");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        btnNo.onClick.AddListener(() =>
        {
            Debug.Log("No");
            menuPanel.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
