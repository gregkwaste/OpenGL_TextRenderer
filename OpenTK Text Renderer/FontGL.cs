using System;
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

}

class Glyph
{
    //Position will save top left and bottom right positions
    //In order to define the glyph quad on the texture 
    public Vector2[] pos; 
    public int GLtex;
    public char literal;
}
