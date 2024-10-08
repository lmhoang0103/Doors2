using MEC;
using System.Collections.Generic;
using UnityEngine;

public class TestEfficientCoroutine : MonoBehaviour
{
    private CoroutineHandle coroutineHandle;
    void Start()
    {
        Timing.CallDelayed(2f, delegate { Timing.RunCoroutine(RunTestCoroutine()); });

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Pause");
            Timing.PauseCoroutines(coroutineHandle);
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            Debug.Log("Resume");
            Timing.ResumeCoroutines(coroutineHandle);
        }
    }
    private IEnumerator<float> RunTestCoroutine()
    {
        for (int i = 0; i < 10; i++)
        {
            if (gameObject != null && gameObject.activeInHierarchy)
            {
                yield return Timing.WaitForSeconds(1f);
            }
            Debug.Log(i);
        }

    }





}
