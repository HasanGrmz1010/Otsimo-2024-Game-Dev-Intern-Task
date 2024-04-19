using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

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

    [SerializeField] List<Sprite> tool_images = new List<Sprite>();
    [SerializeField] List<RectTransform> toolButtons = new List<RectTransform>();
    [SerializeField] Button toolMenuButton;
    [SerializeField] Button colorWheelButton;
    [SerializeField] RectTransform colorPicker;
    [SerializeField] Image currentTool_img;

    bool toolMenu_status = false, colorWheel_status = false;
    static bool button_tweening = false;

    private void Start()
    {
        toolMenuButton.onClick.AddListener(Button_ToolChooseMenu);
        colorWheelButton.onClick.AddListener(Button_ColorWheelMenu);
    }

    #region Canvas Button Functions
    public void Button_ToolChooseMenu()
    {
        if (!button_tweening)
        {
            toolMenuButton.transform.DOPunchScale(Vector3.one / 5f, .2f, 1, .25f);
            int temp_offset;
            switch (toolMenu_status)
            {
                case true: // CLOSE THE MENU
                    temp_offset = 240;
                    button_tweening = true;
                    var close_seq = DOTween.Sequence();
                    foreach (RectTransform button in toolButtons)
                    {
                        button.DOMove(button.parent.position, .1f);
                        temp_offset -= 60;
                    }
                    close_seq.OnComplete(() =>
                    {
                        button_tweening = false;
                    });
                    toolMenu_status = false;
                    break;

                case false: // OPEN THE MENU
                    temp_offset = 240;
                    button_tweening = true;
                    var open_seq = DOTween.Sequence();
                    foreach (RectTransform button in toolButtons)
                    {
                        open_seq.Append(button.DOMoveX(button.position.x - temp_offset, .075f));
                        temp_offset += 240;
                    }
                    open_seq.OnComplete(() =>
                    {
                        button_tweening = false;
                    });
                    toolMenu_status = true;
                    break;
            }
        }
    }

    public void Button_Tool(string _tag)
    {

        switch (_tag)
        {
            case "pen":
                currentTool_img.sprite = tool_images[0];
                break;

            case "bucket":
                currentTool_img.sprite = tool_images[1];
                break;

            case "stamp":
                currentTool_img.sprite = tool_images[2];
                break;

            case "eraser":
                currentTool_img.sprite = tool_images[3];
                break;

            default:
                break;
        }
    }
    #endregion

    public void Button_ColorWheelMenu()
    {
        if (!button_tweening)
        {
            button_tweening = true;
            colorWheelButton.transform.DOPunchScale(Vector3.one / 5f, .2f, 1, .25f);
            switch (colorWheel_status)
            {
                case true:// Close Color Wheel
                    colorPicker.DOMove(colorWheelButton.transform.position, .2f);
                    colorPicker.DOScale(.01f, .2f).OnComplete(() =>
                    {
                        colorPicker.gameObject.SetActive(false);
                        colorWheel_status = false;
                        button_tweening = false;
                    });
                    break;

                case false:// Open Color Wheel
                    colorPicker.gameObject.SetActive(true);
                    colorPicker.DOMoveX(colorWheelButton.transform.position.x + 150, .2f);
                    colorPicker.DOMoveY(colorWheelButton.transform.position.y + 150f, .2f);
                    colorPicker.DOScale(1f, .2f).OnComplete(() =>
                    {
                        colorWheel_status = true;
                        button_tweening = false;
                    });
                    break;
            }
        }
        
    }

    private void OnDisable()
    {
        toolMenuButton.onClick.RemoveAllListeners();
        colorWheelButton.onClick.RemoveAllListeners();
    }
}
