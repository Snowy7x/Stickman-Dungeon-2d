using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] TMP_Text popupText;
    [SerializeField] TMP_Text titleText;

    private GameObject window;
    [SerializeField] Animator popupAnimator;
    [SerializeField] Animator titleAnimator;

    private Queue<string> popupQueue; //make it different type for more detailed popups, you can add different types, titles, descriptions etc
    private Queue<string> titleQueue; //make it different type for more detailed popups, you can add different types, titles, descriptions etc
    private Coroutine queueChecker;
    private Coroutine titleQueueChecker;

    public static PopupWindow Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("PopupWindow already exists");
            Destroy(gameObject);
            return;
        }
        
        window = transform.GetChild(0).gameObject;
        window.SetActive(false);
        popupQueue = new Queue<string>();
        titleQueue = new Queue<string>();
    }

    public void AddToQueue(string text) {//parameter the same type as queue
        popupQueue.Enqueue(text);
        if (queueChecker == null)
            queueChecker = StartCoroutine(CheckQueue());
    }
    
    public void TitleAddToQueue(string text) {//parameter the same type as queue
        titleQueue.Enqueue(text);
        if (titleQueueChecker == null)
            titleQueueChecker = StartCoroutine(TitleCheckQueue());
    }

    private void ShowPopup(string text) { //parameter the same type as queue
        window.SetActive(true);
        popupText.text = text;
        popupAnimator.Play("PopupAnimation");
    }
    private void ShowTitle(string text) { //parameter the same type as queue
        window.SetActive(true);
        titleText.text = text;
        titleAnimator.Play("ShowAnimation");
    }
    

    private IEnumerator CheckQueue() {
        do {
            ShowPopup(popupQueue.Dequeue());
            do {
                yield return null;
            } while (!popupAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));

        } while (popupQueue.Count > 0);
        window.SetActive(false);
        queueChecker = null;
    }

    private IEnumerator TitleCheckQueue() {
        do {
            ShowTitle(titleQueue.Dequeue());
            do {
                yield return null;
            } while (!titleAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));

        } while (titleQueue.Count > 0);
        window.SetActive(false);
        titleQueueChecker = null;
    }

}