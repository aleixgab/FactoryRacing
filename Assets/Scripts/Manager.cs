using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject finishText;
    public GameObject[] cars;
    public GameObject finishCamera;
    public static bool raceFinished = false;
    public static int winner = 0;
    private bool text = false;

    private void Start()
    {
        raceFinished = false;
        finishText.SetActive(false);
    }
    private void Update()
    {
        if (raceFinished && !text)
        {
            Camera.main.gameObject.SetActive(false);
            finishCamera.SetActive(true);
            text = true;
            finishText.SetActive(true);
            cars[winner].SetActive(true);
        }
    }
}
