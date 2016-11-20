using System;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_Text_Renderer
{
    public partial class Form1 : Form
    {
        float[] quadverts = new float[] {0.0f, 0.0f, 0.0f,
                                         0.0f, 1.0f, 0.0f,
                                         1.0f, 1.0f, 0.0f,
                                         1.0f, 0.0f, 0.0f};

        int[] quadindices = new int[] {0, 2, 1,
                                       2, 1, 3};
        
        public Form1()
        {
            InitializeComponent();

            //Opening PNG file
            Image im = Image.FromFile("courier_charmap.png");
            MemoryStream ms = new MemoryStream();

            im.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            


            //Testing some inits       
            FontGL cm = new FontGL();

        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Black);

            //I should compile the very default shaders here
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Debug.WriteLine("Painting");
            render();
            //glControl1.SwapBuffers();

            //glControl1.Invalidate();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            Debug.WriteLine("Resizing");
            glControl1.Invalidate();
        }

        private void render()
        {
            /*
             * THIS FUNCTION WILL IMPLEMENT TEXT RENDERING
             * 
             */
            string text = "Testing";


            renderText(text);
            /*
            foreach (char c in text)
            {
                Debug.WriteLine(c);
            }
            */
            
        }

        private void renderText(string text)
        {
            /*
             * THIS FUNCTION WILL IMPLEMENT TEXT RENDERING
             * 
             */
        
        }

        private void prepareFont()
        {
            
        }



        class gImage
        {
            public int width;
            public int height;
            public int GLid;
        }


        class BMPImage: gImage
        {
            public byte[] pixels;
            
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

            }

            private void createGLTex()
            {
                GLid = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, GLid);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            }

        }
        
    }
}
