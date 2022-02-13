using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SequenceBeat
{
    public enum GravityChange
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public enum ColorChange
    {
        None,
        Blue,
        Orange,
        Purple,
        Green
    }

    [HideInInspector]
    public string name;
    [HideInInspector]
    public GameObject UI;
    [HideInInspector]
    public GameObject UIHighlight;
    public GravityChange gravityChange;
    public ColorChange colorChange;

    static public Utils.GameColor ColorChangeToColor(ColorChange colorChange)
    {
        switch (colorChange)
        {
            case ColorChange.None:
                return Utils.GameColor.White;
            case ColorChange.Blue:
                return Utils.GameColor.Blue;
            case ColorChange.Orange:
                return Utils.GameColor.Orange;
            case ColorChange.Purple:
                return Utils.GameColor.Purple;
            case ColorChange.Green:
                return Utils.GameColor.Green;
        }
        return Utils.GameColor.White;
    }
    public void Play(PlayerComponent playerComponent)
    {
        ApplyGravity();
        ApplyColor(playerComponent);
    }

    void ApplyGravity()
    {
        if (gravityChange == GravityChange.None)
        {
            return;
        }

        float gravityPull = Physics.gravity.magnitude;

        switch (gravityChange)
        {
            case GravityChange.Up:
                Physics.gravity = Vector3.up * gravityPull;
                break;
            case GravityChange.Down:
                Physics.gravity = Vector3.down * gravityPull;
                break;
            case GravityChange.Left:
                Physics.gravity = Vector3.left * gravityPull;
                break;
            case GravityChange.Right:
                Physics.gravity = Vector3.right * gravityPull;
                break;
        }
    }
    void ApplyColor(PlayerComponent playerComponent)
    {
        if (colorChange == ColorChange.None)
        {
            return;
        }

        playerComponent.SetColor(ColorChangeToColor(colorChange));
    }
}

public class SequenceComponent : MonoBehaviour
{
    public int bpm = 60;
    public List<SequenceBeat> beats;
    public GameObject ArrowImage;
    public GameObject ArrowHighlightImage;
    public GameObject SquareImage;
    public GameObject SquareHighlightImage;
    public AudioSource lowBeat;
    public AudioSource highBeat;

    float beatAccumulatedTime = 0.0f;
    float beatTimeS;
    int beatIndex = 0;
    int lastBeatAudioPlayed = -1;
    Utils utils;
    PlayerComponent playerComponent;
    GoalComponent goalComponent;
    GameFlow flow;

    void OnValidate()
    {
        foreach (SequenceBeat beat in beats)
        {
            beat.name = "";
            if (beat.gravityChange != SequenceBeat.GravityChange.None)
            {
                beat.name += beat.gravityChange.ToString();
            }

            if (beat.colorChange != SequenceBeat.ColorChange.None)
            {
                beat.name += beat.colorChange.ToString();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameLogic = GameObject.FindGameObjectWithTag("GameController");
        utils = gameLogic.GetComponent<Utils>();
        flow = gameLogic.GetComponent<GameFlow>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerComponent = player.GetComponent<PlayerComponent>();

        GameObject goal = GameObject.FindGameObjectWithTag("Finish");
        goalComponent = goal.GetComponent<GoalComponent>();

        beatTimeS = 60.0f / (float)bpm;

        int index = 0;
        const float elementSpacing = 20.0f;
        float elementWidth = ArrowImage.GetComponent<RectTransform>().rect.width;
        float initialUIPositionX = (beats.Count - 1.0f) * elementWidth / 2.0f + (beats.Count - 1.0f) * elementSpacing / 2.0f;
        foreach (SequenceBeat beat in beats)
        {
            if (beat.gravityChange != SequenceBeat.GravityChange.None)
            {
                beat.UI = RenderArrowUI(index, elementSpacing, elementWidth, initialUIPositionX, beat.gravityChange);
                beat.UIHighlight = RenderArrowHighlightUI(index, elementSpacing, elementWidth, initialUIPositionX, beat.UI);
            }
            else if (beat.colorChange != SequenceBeat.ColorChange.None)
            {
                beat.UI = RenderSquareUI(index, elementSpacing, elementWidth, initialUIPositionX);
                beat.UIHighlight = RenderSquareHighlightUI(index, elementSpacing, elementWidth, initialUIPositionX, beat.UI);
            }

            ApplyUIColor(beat.UI, beat.colorChange);

            ++index;
        }

        // Force first beat to be played on the first update tick
        beatAccumulatedTime = beatTimeS;
    }

    // Update is called once per frame
    void Update()
    {
        if (flow.IsGamePaused())
        {
            return;
        }

        beatAccumulatedTime += Time.deltaTime;

        if (beats.Count > 1)
        {
            ProcessBeatAudio();
        }

        if (beatAccumulatedTime >= beatTimeS)
        {
            if (beats.Count > 0)
            {
                PlayBeat();
            }

            beatAccumulatedTime -= beatTimeS;
        }
    }

    GameObject RenderArrowUI(int index, float arrowSpacing, float arrowWidth, float initialUIPositionX, SequenceBeat.GravityChange gravityChange)
    {
        GameObject newArrow = Instantiate(ArrowImage, GetComponentInChildren<Canvas>().transform);
        RectTransform arrowTransform = newArrow.GetComponent<RectTransform>();

        switch (gravityChange)
        {
            case SequenceBeat.GravityChange.Up:
                {
                    Vector3 rotation = new Vector3(0.0f, 0.0f, 90.0f);
                    arrowTransform.Rotate(rotation);
                    break;
                }
            case SequenceBeat.GravityChange.Down:
                {
                    Vector3 rotation = new Vector3(0.0f, 0.0f, -90.0f);
                    arrowTransform.Rotate(rotation);
                    break;
                }
            case SequenceBeat.GravityChange.Left:
                {
                    Vector3 rotation = new Vector3(0.0f, 0.0f, 180.0f);
                    arrowTransform.Rotate(rotation);
                    break;
                }
            case SequenceBeat.GravityChange.Right:
                // Default orientation
                break;
        }

        Canvas arrowCanvas = newArrow.GetComponent<Canvas>();
        arrowCanvas.overrideSorting = true;

        Vector2 arrowPosition = new Vector2(-initialUIPositionX + ((float)index * (arrowWidth + arrowSpacing)), arrowTransform.anchoredPosition.y);
        arrowTransform.anchoredPosition = arrowPosition;

        return newArrow;
    }

    GameObject RenderSquareUI(int index, float arrowSpacing, float arrowWidth, float initialUIPositionX)
    {
        GameObject newSquare = Instantiate(SquareImage, GetComponentInChildren<Canvas>().transform);
        RectTransform squareTransform = newSquare.GetComponent<RectTransform>();

        Vector2 squarePosition = new Vector2(-initialUIPositionX + ((float)index * (arrowWidth + arrowSpacing)), squareTransform.anchoredPosition.y);
        squareTransform.anchoredPosition = squarePosition;

        Canvas squareCanvas = newSquare.GetComponent<Canvas>();
        squareCanvas.overrideSorting = true;


        return newSquare;
    }

    GameObject RenderSquareHighlightUI(int index, float arrowSpacing, float arrowWidth, float initialUIPositionX, GameObject parentUIElement)
    {
        GameObject newSquare = Instantiate(SquareHighlightImage, parentUIElement.transform);

        Canvas squareCanvas = newSquare.GetComponent<Canvas>();
        squareCanvas.overrideSorting = true;

        newSquare.SetActive(false);

        return newSquare;
    }

    GameObject RenderArrowHighlightUI(int index, float arrowSpacing, float arrowWidth, float initialUIPositionX, GameObject parentUIElement)
    {
        GameObject newArrow = Instantiate(ArrowHighlightImage, parentUIElement.transform);

        Canvas arrowCanvas = newArrow.GetComponent<Canvas>();
        arrowCanvas.overrideSorting = true;

        newArrow.SetActive(false);

        return newArrow;
    }

void ApplyUIColor(GameObject UIObject, SequenceBeat.ColorChange color)
    {
        if (color != SequenceBeat.ColorChange.None)
        {
            Color UIColor = utils.GetColor(SequenceBeat.ColorChangeToColor(color));
            UIObject.GetComponent<Image>().color = UIColor;
        }
    }

    // TODO make highlights pretty
    void HighlightCurrentBeat(int beatIndex)
    {
        int previousIndex = beatIndex - 1;
        if (previousIndex < 0)
        {
            previousIndex = beats.Count - 1;
        }
        
        // TODO do this prettier
        if (beats[previousIndex].UI)
        {
            //beats[previousIndex].UI.GetComponent<Image>().color = Color.white;
        }

        if (beats[beatIndex].UI)
        {
            //beats[beatIndex].UI.GetComponent<Image>().color = Color.yellow;
        }

        if (beats[previousIndex].UIHighlight)
        {
            beats[previousIndex].UIHighlight.SetActive(false);
            //beats[previousIndex].UI.GetComponent<Image>().color = Color.white;
        }

        if (beats[beatIndex].UIHighlight)
        {
            beats[beatIndex].UIHighlight.SetActive(true);
            //beats[beatIndex].UI.GetComponent<Image>().color = Color.yellow;
        }
    }

    void ProcessBeatAudio()
    {
        float beatSectionProgress = beatAccumulatedTime / beatTimeS;
        int beatSection = (int)Mathf.Floor(beatSectionProgress*4.0f);

        if (beatSection > lastBeatAudioPlayed)
        {
            if (beatSection % 4 == 0)
            {
                highBeat.Play();
            }
            else
            {
                lowBeat.Play();
            }

            playerComponent.OnAudioBeat(beatSection % 4 == 0);
            goalComponent.OnAudioBeat(beatSection % 4 == 0);

            lastBeatAudioPlayed = beatSection;
        }
    }

    void PlayBeat()
    {
        beats[beatIndex].Play(playerComponent);
        lastBeatAudioPlayed = 0;

        HighlightCurrentBeat(beatIndex);

        ++beatIndex;
        beatIndex %= beats.Count;
    }
}