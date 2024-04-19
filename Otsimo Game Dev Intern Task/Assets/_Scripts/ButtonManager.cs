using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System;

public class ButtonManager : MonoBehaviour
{
    #region Singleton
    public static ButtonManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    public static Action onAnyMenuOpened;
    public static Action onAllMenusClosed;

    [SerializeField] LineGenerator lineGenerator;
    [SerializeField] Transform canvasItemHolder;
    [SerializeField] Camera mainCam;

    [SerializeField] List<Sprite> tool_images = new List<Sprite>();
    [SerializeField] List<RectTransform> toolButtons = new List<RectTransform>();
    
    [SerializeField] RectTransform colorPicker;
    [SerializeField] Image currentTool_img;

    [Header("======= BUTTONS) =======")]
    [SerializeField] Button cleanCanvasButton;
    [SerializeField] Button toolMenuButton;
    [SerializeField] Button colorWheelButton;

    bool toolMenu_status = false, colorWheel_status = false;

    private void Start()
    {
        toolMenuButton.onClick.AddListener(Button_ToolChooseMenu);
        colorWheelButton.onClick.AddListener(Button_ColorWheelMenu);
        cleanCanvasButton.onClick.AddListener(Button_CleanCanvas);
    }

    #region Canvas Button Functions
    public void Button_ToolChooseMenu()
    {
        toolMenuButton.interactable = false;
        toolMenuButton.transform.DOPunchScale(Vector3.one / 5f, .2f, 1, .25f).OnComplete(() =>
        {
            toolMenuButton.interactable = true;
        });

        int temp_offset;
        switch (toolMenu_status)
        {
            case true: // CLOSE THE MENU
                temp_offset = 240;
                var close_seq = DOTween.Sequence();
                foreach (RectTransform button in toolButtons)
                {
                    button.DOMove(button.parent.position, .1f);
                    temp_offset -= 60;
                }
                close_seq.OnComplete(() =>
                {
                    toolMenu_status = false;
                    if (!colorWheel_status) onAllMenusClosed();
                });
                    
                break;

            case false: // OPEN THE MENU
                temp_offset = 240;
                var open_seq = DOTween.Sequence();
                foreach (RectTransform button in toolButtons)
                {
                    open_seq.Append(button.DOMoveX(button.position.x - temp_offset, .075f));
                    temp_offset += 240;
                }
                open_seq.OnComplete(() =>
                {
                    onAnyMenuOpened();
                });
                toolMenu_status = true;
                break;
        }
    }

    public void Button_ColorWheelMenu()
    {
        if ((GameManager.instance.mode == GameManager.Mode.pen || GameManager.instance.mode == GameManager.Mode.bucket))
        {
            colorWheelButton.interactable = false;
            colorWheelButton.transform.DOPunchScale(Vector3.one / 5f, .2f, 1, .25f).OnComplete(() =>
            {
                colorWheelButton.interactable = true;
            });
            switch (colorWheel_status)
            {
                case true:// Close Color Wheel
                    colorPicker.DOMove(colorWheelButton.transform.position, .2f);
                    colorPicker.DOScale(.01f, .2f).OnComplete(() =>
                    {
                        colorPicker.gameObject.SetActive(false);
                        colorWheel_status = false;
                        if (!toolMenu_status) onAllMenusClosed();
                    });
                    break;

                case false:// Open Color Wheel
                    onAnyMenuOpened();
                    colorPicker.gameObject.SetActive(true);
                    colorPicker.DOMoveX(colorWheelButton.transform.position.x + 150, .2f);
                    colorPicker.DOMoveY(colorWheelButton.transform.position.y + 150f, .2f);
                    colorPicker.DOScale(1f, .2f).OnComplete(() =>
                    {
                        colorWheel_status = true;
                    });
                    break;
            }
        }

    }

    public void Button_CleanCanvas()
    {
        onAnyMenuOpened();
        cleanCanvasButton.interactable = false;
        cleanCanvasButton.transform.DOPunchScale(Vector3.one / 5, .2f, 1, .2f).OnComplete(() =>
        {
            cleanCanvasButton.interactable = true;
            if (!toolMenu_status && !colorWheel_status)
                onAllMenusClosed();
        });
        foreach (Transform item in canvasItemHolder.transform)
        {
            Destroy(item.gameObject);
        }
        mainCam.backgroundColor = Color.white;
        lineGenerator.SetLineGenerating(true);
    }

    public void Button_Tool(string _tag)
    {

        switch (_tag)
        {
            case "pen":
                currentTool_img.sprite = tool_images[0];
                GameManager.instance.mode = GameManager.Mode.pen;
                break;

            case "bucket":
                currentTool_img.sprite = tool_images[1];
                GameManager.instance.mode = GameManager.Mode.bucket;
                break;

            case "stamp":
                currentTool_img.sprite = tool_images[2];
                GameManager.instance.mode = GameManager.Mode.stamp;
                break;

            case "eraser":
                currentTool_img.sprite = tool_images[3];
                GameManager.instance.mode = GameManager.Mode.eraser;
                break;

            default:
                break;
        }
    }
    #endregion

    

    private void OnDisable()
    {
        toolMenuButton.onClick.RemoveAllListeners();
        colorWheelButton.onClick.RemoveAllListeners();
        cleanCanvasButton.onClick.RemoveAllListeners();
    }
}
