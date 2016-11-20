using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using OpenTK;

class FontGL
{
    public Dictionary<char, Glyph> char_dict = new Dictionary<char, Glyph>();

    public FontGL()
    {
        string alphabet = " !\"#$%&'()*+-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVXYZ[\\]^_`abcdefghijklmnopqrstuvxyz{|}~";
        foreach (char c in alphabet)
            Debug.WriteLine(c);

    }

}

class Glyph
{
    public Vector2 pos;
    public int GLtex;
    public char literal;
}
