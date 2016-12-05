using System.IO;
using OpenTK.Graphics.OpenGL;

namespace gImage
{
    class gImage
    {
        public int width;
        public int height;
        public int GLid;
        public byte[] pixels;
    }


    class BMPImage : gImage
    {
        

        public BMPImage(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            br.BaseStream.Seek(0x12, SeekOrigin.Begin);

            //Get width,height
            width = br.ReadInt32();
            height = br.ReadInt32();
            br.ReadInt16(); //Skip number of images possibly??
            int bitrate = br.ReadInt32();

            //Seek to pixel data start
            br.BaseStream.Seek(0x36, SeekOrigin.Begin);
            //Fetch pixels
            
            pixels = br.ReadBytes(width * height * bitrate / 8);
            
            //Close Handle
            br.Close();
            fs.Close();

            createGLTex(); //Load texture to the GPU


        }

        private void createGLTex()
        {
            GLid = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, GLid);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);

        }

    }
}
