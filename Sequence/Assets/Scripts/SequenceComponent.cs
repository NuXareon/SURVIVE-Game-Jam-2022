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
    public GravityChange gravityChange;
    public ColorChange colorChange;

    public void Play()
    {
        ApplyGravity();
        ApplyColor();
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
    void ApplyColor()
    {
        // TODO
    }
}


public class SequenceComponent : MonoBehaviour
{
    public int bpm = 60;
    public List<SequenceBeat> beats;
    public GameObject ArrowImage;
    public AudioSource lowBeat;
    public AudioSource highBeat;

    float beatAccumulatedTime = 0.0f;
    float beatTimeS;
    int beatIndex = 0;
    int lastBeatAudioPlayed = -1;

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
        beatTimeS = 60.0f / (float)bpm;

        int index = 0;
        const float elementSpacing = 20.0f;
        float elementWidth = ArrowImage.GetComponent<RectTransform>().rect.width;
        float initialUIPositionX = (beats.Count - 1.0f) * elementWidth / 2.0f + (beats.Count-1.0f) * elementSpacing / 2.0f;
        foreach (SequenceBeat beat in beats)
        {
            if (beat.gravityChange != SequenceBeat.GravityChange.None)
            {
                beat.UI = RenderArrowUI(index, elementSpacing, elementWidth, initialUIPositionX, beat.gravityChange);
            }

            ++index;
        }

        // Execute first beat!
        if (beats.Count > 0)
        {
            ProcessBeatAudio();
            PlayBeat();
        }
    }

    // Update is called once per frame
    void Update()
    {
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

        Vector2 arrowPosition = new Vector2(-initialUIPositionX + ((float)index * (arrowWidth + arrowSpacing)), arrowTransform.anchoredPosition.y);
        arrowTransform.anchoredPosition = arrowPosition;

        return newArrow;
    }

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
            beats[previousIndex].UI.GetComponent<Image>().color = Color.white;
        }

        if (beats[beatIndex].UI)
        {
            beats[beatIndex].UI.GetComponent<Image>().color = Color.yellow;

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

            lastBeatAudioPlayed = beatSection;
        }
    }

    void PlayBeat()
    {
        beats[beatIndex].Play();
        lastBeatAudioPlayed = 0;

        HighlightCurrentBeat(beatIndex);

        ++beatIndex;
        beatIndex %= beats.Count;
    }
}
