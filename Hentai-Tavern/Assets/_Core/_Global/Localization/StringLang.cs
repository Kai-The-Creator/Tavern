using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringLang
{
    public Language language;
    [TextArea(0, 20)]
    public string Text;

    public StringLang(Language language, string text)
    {
        this.language = language;
        this.Text = text;
    }
}
