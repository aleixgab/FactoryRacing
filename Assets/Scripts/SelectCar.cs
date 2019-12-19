using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCar : MonoBehaviour
{
    public GameObject[] cars;
    private short current = 0;

    private void Start()
    {
        cars[current].SetActive(true);
        CustomNetworkManager.playerPrefabIndex = current;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnButtonNext();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnButtonPrev();
        }

        transform.RotateAround(cars[current].transform.position, Vector3.up, -15 * Time.deltaTime);
        transform.LookAt(cars[current].transform.position);
    }

    public void OnButtonNext()
    {
        cars[current].SetActive(false);

        current++;
        if (current >= cars.Length)
            current = 0;

        cars[current].SetActive(true);
        CustomNetworkManager.playerPrefabIndex = current;
    }

    public void OnButtonPrev()
    {
        cars[current].SetActive(false);

        current--;
        if (current < 0)
            current = (short)(cars.Length - 1);

        cars[current].SetActive(true);
        CustomNetworkManager.playerPrefabIndex = current;
    }


    private void OnDestroy()
    {
        PlayerPrefs.SetInt("CarID", current);
    }
}
