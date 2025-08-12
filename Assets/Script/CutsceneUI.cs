using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneUI : MonoBehaviour
{
    public GameObject[] cutsceneUI;

    public void ShowCutsceneUI(int index)
    {
        cutsceneUI[index].SetActive(true);
    }
}
