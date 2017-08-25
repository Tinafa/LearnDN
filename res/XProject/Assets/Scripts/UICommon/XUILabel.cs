﻿using UILib;
using UnityEngine;
using XUtliPoolLib;
using System;

public class XUILabel : XUIObject,IXUILabel
{
    public float m_fAlphaVar;

    [System.NonSerialized]
    private int m_id = 0;
    public float AlphaVar
    {
        get { return m_fAlphaVar; }
    }
    public float Alpha
    {
        get
        {
            if (null != m_uiLabel)
            {
                return m_uiLabel.alpha;
            }
            return 1;
        }
        set
        {
            if (null != m_uiLabel)
            {
                m_uiLabel.alpha = value;
            }
        }
    }
    public int spriteWidth
    {
        get
        {
            return m_uiLabel.width;
        }
        set
        {
            m_uiLabel.width = value;
        }
    }

    public int spriteHeight
    {
        get
        {
            return m_uiLabel.height;
        }
    }

    public int spriteDepth
    {
        get
        {
            return m_uiLabel.depth;
        }
        set
        {
            m_uiLabel.depth = value;
        }
    }

    public int fontSize
    {
        get
        {
            return m_uiLabel.fontSize;
        }
        set
        {
            m_uiLabel.fontSize = value;
        }
    }

    public void SetIdentity(int i)
    {
        m_id = i;
    }

    public bool HasIdentityChanged(int i)
    {
        return m_id != i;
    }

    public void RegisterLabelClickEventHandler(LabelClickEventHandler eventHandler)
    {
        UIEventListener.Get(this.gameObject).onClick = OnLabelClick;
        m_labelClickEventHandler = eventHandler;
    }

    void OnLabelClick(GameObject button)
    {
        m_labelClickEventHandler(this);
    }

    public string GetText()
    {
        return m_uiLabel.text;
    }

    public Color GetColor()
    {
        return m_uiLabel.color;
    }

    public void SetText(string strText)
    {
        if (m_uiLabel != null)
        {
            m_uiLabel.text = Localization.Get(strText);
        }
    }

    public void SetColor(Color c)
    {
        m_uiLabel.color = c;
    }
   
    public void SetEffectColor(Color c)
    {
        m_uiLabel.effectColor = c;
    }

    public void SetGradient(bool bEnable, Color top, Color bottom)
    {
        m_uiLabel.applyGradient = bEnable;
        m_uiLabel.gradientTop = top;
        m_uiLabel.gradientBottom = bottom;
    }
    public void ToggleGradient(bool bEnable)
    {
        m_uiLabel.applyGradient = bEnable;
    }

    public void SetEnabled(bool bEnabled)
    {
        m_bEnabled = bEnabled;

        if (m_bEnabled)
        {
            m_uiLabel.color = m_sourceColor;
            m_uiLabel.gradientTop = m_sourceTop;
            m_uiLabel.gradientBottom = m_sourceBottom;
            m_uiLabel.effectColor = m_effectColor;
        }
        else
        {
            m_uiLabel.color = new Color(1.0f, 1.0f, 1.0f, 1);
            float grey = m_sourceTop.r * 0.3f + m_sourceTop.g * 0.587f + m_sourceTop.b * 0.114f;
            m_uiLabel.gradientTop = new Color(grey, grey, grey);
            grey = m_sourceBottom.r * 0.3f + m_sourceBottom.g * 0.587f + m_sourceBottom.b * 0.114f;
            m_uiLabel.gradientBottom = new Color(grey, grey, grey);
            grey = m_effectColor.r * 0.3f + m_effectColor.g * 0.587f + m_effectColor.b * 0.114f;
            m_uiLabel.effectColor = new Color(grey, grey, grey);
        }
    }

    public void SetDepthOffset(int d)
    {
        UIWidget[] w = gameObject.GetComponentsInChildren<UIWidget>();

        for (int i = 0; i < w.Length; i++)
        {
            w[i].depth += d;
        }
    }

    public void MakePixelPerfect()
    {
        if (m_uiLabel == null)
        {
            return;
        }

        m_uiLabel.MakePixelPerfect();
    }

    public Vector2 GetPrintSize()
    {
        return m_uiLabel.printedSize;
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        m_uiLabel = GetComponent<UILabel>();
        if (null == m_uiLabel)
        {
            Debug.LogError("null == m_uiLabel");
        }

        m_sourceColor = m_uiLabel.color;
        m_sourceTop = m_uiLabel.gradientTop;
        m_sourceBottom = m_uiLabel.gradientBottom;
        m_effectColor = m_uiLabel.effectColor;
    }

    private LabelClickEventHandler m_labelClickEventHandler = null;
    private UILabel m_uiLabel = null;
    private bool m_bEnabled = true;
    private Color m_sourceColor;
    private Color m_sourceTop;
    private Color m_sourceBottom;
    private Color m_effectColor;
}
