using System.Collections;
using NUUI;
using UnityEngine;

public class Starter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        StartCoroutine(CallDelayed());
    }

    private IEnumerator CallDelayed()
    {
        yield return new WaitForSeconds(0.2f);
        ScreenManager.Instance.StartNewQueue("Screen1");
    }
}
