using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Red,
        Green,
        Blue,
        Purple
    }

    [HideInInspector]
    public string name;
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

    float beatAccumulatedTime = 0.0f;
    float beatTimeS;
    int beatIndex = 0; 

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

        // Execute first beat!
        beats[beatIndex].Play();
    }

    // Update is called once per frame
    void Update()
    {
        beatAccumulatedTime += Time.deltaTime;

        if (beatAccumulatedTime >= beatTimeS)
        {
            beats[beatIndex].Play();

            ++beatIndex;
            beatIndex %= beats.Count;
            beatAccumulatedTime -= beatTimeS;
        }
    }
}
