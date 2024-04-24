using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System;
using static System.Net.WebRequestMethods;
using UnityEngine.EventSystems;
using TMPro;

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
            //DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    public static Action onAnyMenuOpened;
    public static Action onAllMenusClosed;

    [SerializeField] gameData gameData;
    [SerializeField] LineGenerator lineGenerator;
    //[SerializeField] Transform canvasItemHolder;
    [SerializeField] GameObject canvasPlane;
    [SerializeField] Camera mainCam;

    [SerializeField] List<Sprite> tool_images = new List<Sprite>();
    [SerializeField] List<RectTransform> toolButtons = new List<RectTransform>();

    public List<Canvas> canvases = new List<Canvas>();
    [SerializeField] RectTransform colorPicker;
    [SerializeField] Image currentTool_img;
    [SerializeField] Image fade_img;
    [SerializeField] TextMeshProUGUI greet_text;

    [Header("-------------- BUTTONS) --------------")]
    [SerializeField] Button saveCanvasButton;
    [SerializeField] Button projectileButton;
    [SerializeField] Button cleanCanvasButton;
    [SerializeField] Button toolMenuButton;
    [SerializeField] Button colorWheelButton;
    [Header("-------------------------------------------------------------")]
    [SerializeField] Button newCanvasButton;
    [SerializeField] Button exitGameButton;

    bool toolMenu_status = false, colorWheel_status = false;

    private void Start()
    {
        fade_img.DOFade(0f, 3f).OnComplete(() =>
        {
            if (!gameData.hasDrawn)
            {
                fade_img.gameObject.SetActive(false);
                greet_text.gameObject.SetActive(false);
                newCanvasButton.gameObject.SetActive(true);
                exitGameButton.gameObject.SetActive(true);
            }
            else
            {
                SaveManager.instance.LoadSavedCanvas();
                fade_img.gameObject.SetActive(true);
                fade_img.DOFade(1f, .5f).OnComplete(() =>
                {
                    canvases[0].gameObject.SetActive(true);
                    canvases[1].gameObject.SetActive(false);
                    mainCam.transform.DORotate(Vector3.zero, .1f, RotateMode.FastBeyond360);
                    fade_img.DOFade(0f, .25f).OnComplete(() =>
                    {
                        fade_img.gameObject.SetActive(false);
                        GameManager.instance.state = GameManager.State.paint;
                    });
                });
            }
        });
    }

    #region Paint Canvas Button Functions
    public void P_Button_ToolChooseMenu()
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

    public void P_Button_ColorWheelMenu()
    {
        if ((GameManager.instance.mode == GameManager.Mode.pen || GameManager.instance.mode == GameManager.Mode.bucket || GameManager.instance.mode == GameManager.Mode.projectile))
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

    public void P_Button_CleanCanvas()
    {
        onAnyMenuOpened();
        cleanCanvasButton.interactable = false;
        cleanCanvasButton.transform.DOPunchScale(Vector3.one / 5, .2f, 1, .2f).OnComplete(() =>
        {
            cleanCanvasButton.interactable = true;
            if (!toolMenu_status && !colorWheel_status)
                onAllMenusClosed();
        });
        foreach (Transform item in SaveManager.instance.canvasItemHolder.transform)
        {
            Destroy(item.gameObject);
        }
        canvasPlane.GetComponent<MeshRenderer>().material.color = Color.white;
        GameManager.instance.canvasLayerCounter = 1;
        lineGenerator.SetLineGenerating(true);
    }

    public void P_Button_SaveCanvas()
    {
        SaveManager.instance.SaveCurrentCanvas();
    }

    public void P_Button_Projectile()
    {
        GameManager.instance.mode = GameManager.Mode.projectile;
        if (!toolMenu_status && !colorWheel_status) onAllMenusClosed();
    }

    public void P_Button_Tool(string _tag)
    {
        GameObject currentClickedObj;
        switch (_tag)
        {
            case "pen":
                currentClickedObj = EventSystem.current.currentSelectedGameObject;
                currentClickedObj.GetComponent<Button>().transform.DOPunchScale(Vector3.one / 5f, .2f, 1, .25f);
                currentTool_img.sprite = tool_images[0];
                GameManager.instance.mode = GameManager.Mode.pen;
                break;

            case "bucket":
                currentClickedObj = EventSystem.current.currentSelectedGameObject;
                currentClickedObj.GetComponent<Button>().transform.DOPunchScale(Vector3.one / 5f, .2f, 1, .25f);
                currentTool_img.sprite = tool_images[1];
                GameManager.instance.mode = GameManager.Mode.bucket;
                break;

            case "stamp":
                currentClickedObj = EventSystem.current.currentSelectedGameObject;
                currentClickedObj.GetComponent<Button>().transform.DOPunchScale(Vector3.one / 5f, .2f, 1, .25f);
                currentTool_img.sprite = tool_images[2];
                GameManager.instance.mode = GameManager.Mode.stamp;
                break;

            case "eraser":
                currentClickedObj = EventSystem.current.currentSelectedGameObject;
                currentClickedObj.GetComponent<Button>().transform.DOPunchScale(Vector3.one / 5f, .2f, 1, .25f);
                currentTool_img.sprite = tool_images[3];
                GameManager.instance.mode = GameManager.Mode.eraser;
                break;

            default:
                break;
        }
    }
    #endregion

    #region Menu Canvas Button Functions
    void M_Button_NewCanvasButton()
    {
        gameData.hasDrawn = true;
        fade_img.gameObject.SetActive(true);
        fade_img.DOFade(1f, .5f).OnComplete(() =>
        {
            canvases[0].gameObject.SetActive(true);
            canvases[1].gameObject.SetActive(false);
            mainCam.transform.DORotate(Vector3.zero, .1f, RotateMode.FastBeyond360);
            fade_img.DOFade(0f, .25f).OnComplete(() =>
            {
                fade_img.gameObject.SetActive(false);
                GameManager.instance.state = GameManager.State.paint;
            });
        });
        
    }

    void M_ExitGameButton()
    {
        Application.Quit();
    }
    #endregion

    private void OnEnable()
    {
        toolMenuButton.onClick.AddListener(P_Button_ToolChooseMenu);
        colorWheelButton.onClick.AddListener(P_Button_ColorWheelMenu);
        cleanCanvasButton.onClick.AddListener(P_Button_CleanCanvas);
        projectileButton.onClick.AddListener(P_Button_Projectile);
        saveCanvasButton.onClick.AddListener(P_Button_SaveCanvas);

        newCanvasButton.onClick.AddListener(M_Button_NewCanvasButton);
        exitGameButton.onClick.AddListener(M_ExitGameButton);
    }

    private void OnDisable()
    {
        toolMenuButton.onClick.RemoveAllListeners();
        colorWheelButton.onClick.RemoveAllListeners();
        cleanCanvasButton.onClick.RemoveAllListeners();
        projectileButton.onClick.RemoveAllListeners();
        saveCanvasButton.onClick.RemoveAllListeners();

        newCanvasButton.onClick.RemoveAllListeners();
        exitGameButton.onClick.RemoveAllListeners();
    }
}
