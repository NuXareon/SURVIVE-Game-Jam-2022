using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public enum GameColor
    {
        White,
        Blue,
        Orange,
        Purple,
        Green
    }

    public enum Gravity
    {
        Up,
        Down,
        Left,
        Right
    }

    public Color whiteColor;
    public Color blueColor;
    public Color orangeColor;
    public Color purpleColor;
    public Color greenColor;

    public Color GetColor(GameColor gamecolor)
    {
        switch (gamecolor)
        {
            case GameColor.White:
                return whiteColor;
            case GameColor.Blue:
                return blueColor;
            case GameColor.Orange:
                return orangeColor;
            case GameColor.Purple:
                return purpleColor;
            case GameColor.Green:
                return greenColor;
        }

        return whiteColor;
    }
}
