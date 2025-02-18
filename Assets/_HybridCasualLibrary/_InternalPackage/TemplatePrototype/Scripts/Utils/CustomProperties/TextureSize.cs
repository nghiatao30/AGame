using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TextureSize
{
    public TextureSize(int width, int height)
    {
        m_Width = width;
        m_Height = height;
        m_OptionValue = TextureSizeOption.CustomSize;
    }

    public TextureSize(TextureSizeOption sizeOption)
    {
        if (sizeOption == TextureSizeOption.CustomSize)
            throw new Exception("Custom size Bruh??? Are u kidding me??? Use the first constructor instead Bruh!!!");
        m_Width = sizeOption == TextureSizeOption.TextureFullscreen ? Screen.width : (int) sizeOption;
        m_Height = sizeOption == TextureSizeOption.TextureFullscreen ? Screen.height : (int) sizeOption;
        m_OptionValue = sizeOption;
    }

    public enum TextureSizeOption
    {
        Texture128x128 = 128,
        Texture256x256 = 256,
        Texture512x512 = 512,
        Texture1024x1024 = 1024,
        Texture2048x2048 = 2048,
        Texture4096x4096 = 4096,
        TextureFullscreen = -1,
        CustomSize = 0
    }

    [SerializeField]
    private int m_Width;
    [SerializeField]
    private int m_Height;
    [SerializeField]
    private TextureSizeOption m_OptionValue;

    public int width
    {
        get
        {
            if (m_OptionValue == TextureSizeOption.TextureFullscreen)
                return Screen.width;
            return m_Width;
        }
    }
    public int height
    {
        get
        {
            if (m_OptionValue == TextureSizeOption.TextureFullscreen)
                return Screen.height;
            return m_Height;
        }
    }
    public Vector2Int sizeAsVector2
    {
        get
        {
            return new Vector2Int(width, height);
        }
    }
}