﻿using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using OpenTK;
using gImage;

class FontGL
{
    public Dictionary<char, Glyph> char_dict = new Dictionary<char, Glyph>();
    public string alphabet;

    //Text Settings
    public float space;
    public float width;
    public float height;

    //Charmap Texture
    public int tex;
    //Text Program
    public int program;
    
    public FontGL()
    {
        alphabet = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        foreach (char c in alphabet)
            Debug.WriteLine(c);

    }

    public void initFromImage(gImage.gImage im)
    {
        /*  This function should take a full charmap already loaded image
         *  and parse-store all glyphs into the dictionary as textures
         */

        int cw = 86;
        int ch = 140;

        //Charmap is 10x10
        int imagepitch = cw * 10 * 3;

        int clength = alphabet.Length;
        int count = 0;

        for (int i = 0; i < clength; i++)
        {
            char c = alphabet[i];
            Debug.WriteLine(c);
            int x_id = i % 10;
            int y_id = i / 10;

            int xoffset = cw * x_id;
            int yoffset = ch * y_id;

            //Letter masking
            int horl = 20;
            int horr = 20;
            int vertu = 40;
            int vertd = 30;

            //Masked sizes
            int mcw = cw - horl - horr;
            int mch = ch - vertu - vertd;

            //AlphaThresh
            int alphathreshold = 200;


            //Get Top left corner of glyph
            int tl_x = (xoffset + horl);
            int tl_y = (yoffset + vertu);
            //Get Bottom right corner of glyph
            int br_x = tl_x + mcw;
            int br_y = tl_y + mch;

            ////Prepate byte arrays
            //byte[] subpixels = new byte[mcw * mch * 4];
            //for (int j=horl; j < cw-horr; j++)
            //    for (int k=vertu; k < ch-vertd; k++)
            //    {
            //        int pix_off = (k + yoffset) * (imagepitch) + (xoffset + j)*3;
            //        int local_off = j-horr + (k-vertu) * mcw;
            //        subpixels[local_off * 4 + 0] = im.pixels[pix_off + 0];
            //        subpixels[local_off * 4 + 1] = im.pixels[pix_off + 1];
            //        subpixels[local_off * 4 + 2] = im.pixels[pix_off + 2];
            //        if ((im.pixels[pix_off + 0] > alphathreshold)
            //            && (im.pixels[pix_off + 1] > alphathreshold)
            //            && (im.pixels[pix_off + 2] > alphathreshold))
            //            subpixels[local_off * 4 + 3] = 0;
            //        else
            //            subpixels[local_off * 4 + 3] = 255;
            //    }
            
            ////Parse Glyph
            //int gtex = GL.GenTexture();
            //GL.BindTexture(TextureTarget.Texture2D, gtex);

            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, cw, ch, 0, PixelFormat.Rgb, PixelType.UnsignedByte, subpixels);

            //Create Glyph
            Glyph glph = new Glyph();
            glph.GLtex = im.GLid;
            glph.literal = c;
            glph.pos = new Vector2[2];
            glph.pos[0] = new Vector2(((float)tl_x) / im.width,
                                      ((float)tl_y) / im.height);
            glph.pos[1] = new Vector2(((float)br_x) / im.width,
                                      ((float)br_y) / im.height);

            Debug.WriteLine("TL Corner " + tl_x + " " + tl_y);
            Debug.WriteLine("BR Corner " + br_x + " " + br_y);
            //Store to dict
            char_dict[c] = glph;

        }
        
    }

    public GLText renderText(string text, Vector2 pos, float scale)
    {
        /*
         * THIS FUNCTION WILL IMPLEMENT TEXT RENDERING
         * 
         */
        float space = this.space;
        float width = this.width;
        float height = this.height;

        //Check if font exists
        if (this == null) return null;

        //Create text objects
        GLText gtex = new GLText();
        //Save text
        gtex.text = text;
        gtex.pos = pos;
        gtex.scale = scale;
        //Load Image & Program
        gtex.GLImage = tex;
        gtex.program = program;
        //Allocate arrays
        gtex.pints = new int[text.Length * 6];
        gtex.puvs = new float[text.Length * 6 * 2];
        gtex.pverts = new float[text.Length * 6 * 3];

        //Construct float arrays
        float startx = 0.0f;
        float startid = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            Glyph glph = this.char_dict[c];
            //Store a quad for every char

            //Handle positions
            // 0 
            gtex.pverts[6 * 3 * i + 0] = i * space + 0.0f;
            gtex.pverts[6 * 3 * i + 1] = height;
            gtex.pverts[6 * 3 * i + 2] = 0.2f;
            // 1 
            gtex.pverts[6 * 3 * i + 3] = i * space + 0.0f;
            gtex.pverts[6 * 3 * i + 4] = 0.0f;
            gtex.pverts[6 * 3 * i + 5] = 0.2f;
            // 2 
            gtex.pverts[6 * 3 * i + 6] = i * space + width;
            gtex.pverts[6 * 3 * i + 7] = height;
            gtex.pverts[6 * 3 * i + 8] = 0.2f;
            // 3 
            gtex.pverts[6 * 3 * i + 9] = i * space + width;
            gtex.pverts[6 * 3 * i + 10] = height;
            gtex.pverts[6 * 3 * i + 11] = 0.2f;
            // 4 
            gtex.pverts[6 * 3 * i + 12] = i * space + 0.0f;
            gtex.pverts[6 * 3 * i + 13] = 0.0f;
            gtex.pverts[6 * 3 * i + 14] = 0.2f;
            // 5 
            gtex.pverts[6 * 3 * i + 15] = i * space + width;
            gtex.pverts[6 * 3 * i + 16] = 0.0f;
            gtex.pverts[6 * 3 * i + 17] = 0.2f;


            //Handle Indices
            gtex.pints[6 * i + 0] = 4 * i + 0;
            gtex.pints[6 * i + 1] = 4 * i + 1;
            gtex.pints[6 * i + 2] = 4 * i + 2;
            gtex.pints[6 * i + 3] = 4 * i + 3;
            gtex.pints[6 * i + 4] = 4 * i + 4;
            gtex.pints[6 * i + 5] = 4 * i + 5;

            //Handle Uvs
            //0
            gtex.puvs[6 * 2 * i + 0] = glph.pos[0].X;
            gtex.puvs[6 * 2 * i + 1] = glph.pos[0].Y;
            //1
            gtex.puvs[6 * 2 * i + 2] = glph.pos[0].X;
            gtex.puvs[6 * 2 * i + 3] = glph.pos[1].Y;
            //2
            gtex.puvs[6 * 2 * i + 4] = glph.pos[1].X;
            gtex.puvs[6 * 2 * i + 5] = glph.pos[0].Y;
            //3
            gtex.puvs[6 * 2 * i + 6] = glph.pos[1].X;
            gtex.puvs[6 * 2 * i + 7] = glph.pos[0].Y;
            //4
            gtex.puvs[6 * 2 * i + 8] = glph.pos[0].X;
            gtex.puvs[6 * 2 * i + 9] = glph.pos[1].Y;
            //5
            gtex.puvs[6 * 2 * i + 10] = glph.pos[1].X;
            gtex.puvs[6 * 2 * i + 11] = glph.pos[1].Y;
        }

        //Create OpenGL buffers

        //Generate Geometry VBOs
        GL.GenBuffers(1, out gtex.vbo);
        GL.GenBuffers(1, out gtex.ebo);
        GL.GenBuffers(1, out gtex.uvbo);

        //Vertex Buffer
        int vsize = sizeof(float) * gtex.pverts.Length;
        //Upload vertex buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, gtex.vbo);
        //Allocate to NULL
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vsize), (IntPtr)null, BufferUsageHint.StaticDraw);
        //Add verts data
        GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)vsize, gtex.pverts);

        //UV Buffer
        int uvsize = sizeof(float) * gtex.puvs.Length;
        //Upload uv buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, gtex.uvbo);
        //Allocate to NULL
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvsize), (IntPtr)null, BufferUsageHint.StaticDraw);
        //Add verts data
        GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)uvsize, gtex.puvs);

        //Upload index buffer
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, gtex.ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(int) * gtex.pints.Length), gtex.pints, BufferUsageHint.StaticDraw);

        //Store text object
        return gtex;

    }
}

class GLText
{
    public float[] pverts;
    public float[] puvs;
    public int[] pints;

    public int GLImage;
    public string text;

    //GL Buffers
    public int vbo;
    public int uvbo;
    public int ebo;
    public int program;

    //Transforms
    public Vector2 pos;
    public float scale;


    public void render()
    {
        //Upload Uniforms
        int loc;
        loc = GL.GetUniformLocation(program, "pos");
        GL.Uniform2(loc, pos);
        loc = GL.GetUniformLocation(program, "scale");
        GL.Uniform1(loc, scale);


        // Attach to Shaders
        int vpos, uvpos;
        //Vertex attribute
        //Bind vertex buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
        vpos = GL.GetAttribLocation(program, "vPosition");
        GL.VertexAttribPointer(vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(vpos);

        //Bind uv buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, this.uvbo);
        uvpos = GL.GetAttribLocation(program, "uvPosition");
        GL.VertexAttribPointer(uvpos, 2, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(uvpos);

        //Bind elem buffer
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, this.GLImage);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);


        //RENDERING PHASE
        //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        GL.DrawArrays(PrimitiveType.Triangles, 0, this.pints.Length);
        GL.DisableVertexAttribArray(vpos);
        GL.DisableVertexAttribArray(uvpos);

    }

    

}

class Glyph
{
    //Position will save top left and bottom right positions
    //In order to define the glyph quad on the texture 
    public Vector2[] pos; 
    public int GLtex;
    public char literal;
}
