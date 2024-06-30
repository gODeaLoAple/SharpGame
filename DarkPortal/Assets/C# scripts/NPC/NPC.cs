using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public Button button;
    public Button buttonInShop;
    public Player player;
    private bool triggeringPlayer;
    private bool shopEnabled;
    public Canvas canvasForButtonShop;
    public Canvas canvasShop;
    public Canvas CanvasForDialog;                // review(30.06.2024): Почему-то нейминг разных канвасов отличается
    [SerializeField] private AudioSource buysmth; // review(30.06.2024): Интересное название
    public TriggetText triggetDialogue;
    private int beginDilogue; // review(30.06.2024): Я бы переименвал в Stage, а вместо int использовал enum DialogStage, чтобы было более понятно
    
    public TextMeshProUGUI name;
    public TextMeshProUGUI text;
    
    public TextMeshProUGUI totalCoins;

    void Start()
    {
        canvasForButtonShop.enabled = false;
        canvasShop.enabled = false;
        CanvasForDialog.enabled = false;
        var btn = button.GetComponent<Button>(); // review(30.06.2024): Не экономьте на символах
        btn.onClick.AddListener(TaskOnClick);
        var btnInShop = buttonInShop.GetComponent<Button>();
        btnInShop.onClick.AddListener(TaskOnClickEnd);
    }
    
    void Update()
    {
        totalCoins.text = player.inventory.coins.ToString();
        switch (beginDilogue)
        {
            case 0:
                if (triggetDialogue.end)
                {
                    beginDilogue = 1;
                    triggetDialogue.end = false;
                    buysmth.Play();
                }
                return;
            case 1:
                CanvasForDialog.enabled = false;
                canvasShop.enabled = true;
                player.speed = 0;
                break;
            case 2:
                beginDilogue = 0;
                shopEnabled = false;
                canvasForButtonShop.enabled = true;
                player.speed = 5f;
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canvasForButtonShop.enabled = true;
        }
    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canvasForButtonShop.enabled = false;
        }
    }

    private void TaskOnClick()
    {
        triggetDialogue.TriggerDialog(CanvasForDialog, name, text);
        beginDilogue = 0;
        CanvasForDialog.enabled = true;
        canvasForButtonShop.enabled = false;
        player.canvasDefault.enabled = false;
        player.speed = 0f;

    }
    
    private void TaskOnClickEnd()
    {
        beginDilogue = 2;
        canvasForButtonShop.enabled = false;
        canvasShop.enabled = false;
        player.canvasDefault.enabled = true;
        buysmth.Play();
    }
}