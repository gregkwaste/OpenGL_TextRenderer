using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GLHelpers;
using gImage;
using GLSL_Preproc;


namespace OpenTK_Text_Renderer
{
    public partial class Form1 : Form
    {

        //Rendering Program
        int shader_program;

        //Texture Quad Stuff
        float[] quadverts = new float[4 * 3] {-1.0f, 1.0f, 0.0f,
                                            -1.0f, -1.0f, 0.0f,
                                            1.0f, 1.0f, 0.0f,
                                            1.0f, -1.0f, 0.0f};

        int[] quadindices = new Int32[4] { 1, 0, 2, 3 };


        int quad_vbo, quad_ebo;

        //GLContext Stuff
        bool glloaded = false;
        int sampleTex = 0;


        FontGL mainfont = new FontGL();
        List<GLText> tobs = new List<GLText>();
        

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);
            GL.ClearColor(System.Drawing.Color.Black);
            GL.Enable(EnableCap.DepthTest);
            //glControl1.SwapBuffers();
            //glControl1.Invalidate();
            Debug.WriteLine("GL Cleared");
            Debug.WriteLine(GL.GetError());

            this.glloaded = true;

            //Generate Geometry VBOs
            GL.GenBuffers(1, out quad_vbo);
            GL.GenBuffers(1, out quad_ebo);

            //Bind Geometry Buffers
            int arraysize = sizeof(float) * 4 * 3;
            //Upload vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, quad_vbo);
            //Allocate to NULL
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(arraysize), (IntPtr)null, BufferUsageHint.StaticDraw);
            //Add verts data
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)arraysize, quadverts);

            //Upload index buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quad_ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(Int32) * 6), quadindices, BufferUsageHint.StaticDraw);


            //Compile Shaders
            string vvs, ffs;
            int vertex_shader_ob, fragment_shader_ob;
            vvs = GLSL_Preprocessor.Parser("Shaders/Text_VS.glsl");
            ffs = GLSL_Preprocessor.Parser("Shaders/Text_FS.glsl");
            //Compile Texture Shaders
            GLShaderHelper.CreateShaders(vvs, ffs, out vertex_shader_ob,
                    out fragment_shader_ob, out shader_program);


            //Setup default program
            GL.UseProgram(shader_program);


            //Load Sample texture
            //Test BMP Image Class
            BMPImage bm = new BMPImage("courier.bmp");
            sampleTex = bm.GLid;


            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!glloaded)
                return;

            glControl1.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.ClearColor(System.Drawing.Color.Black);
            render();

            glControl1.SwapBuffers();
            glControl1.Invalidate();

        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!this.glloaded)
                return;
            if (glControl1.ClientSize.Height == 0)
                glControl1.ClientSize = new System.Drawing.Size(glControl1.ClientSize.Width, 1);
            Debug.WriteLine("GLControl Resizing");
            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);
        }

        private void render()
        {
            /*
             * THIS FUNCTION WILL IMPLEMENT TEXT RENDERING
             * 
             */

            //Load uniforms
            int loc;
            loc = GL.GetUniformLocation(shader_program, "w");
            GL.Uniform1(loc, (float)glControl1.Width);
            loc = GL.GetUniformLocation(shader_program, "h");
            GL.Uniform1(loc, (float)glControl1.Height);

            loc = GL.GetUniformLocation(shader_program, "projMat");
            //((float)glControl1.Width) / glControl1.Height
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView((float) Math.PI / 2.0f, 1.0f, 0.0001f, 1.0f);
            GL.UniformMatrix4(loc, false, ref proj);


            foreach (GLText t in tobs)
                t.render();
        }

        private void prepareFont()
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            mainfont.space = (float) numericUpDown1.Value;

            GLText t = tobs[0];
            tobs.RemoveAt(0);
            tobs.Add(mainfont.renderText(t.text,t.pos,t.scale));

            glControl1.Invalidate();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            mainfont.width = (float)numericUpDown2.Value;

            GLText t = tobs[0];
            string text = t.text;
            tobs.RemoveAt(0);
            tobs.Add(mainfont.renderText(t.text, t.pos, t.scale));

            glControl1.Invalidate();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            mainfont.height = (float)numericUpDown3.Value;

            GLText t = tobs[0];
            string text = t.text;
            tobs.RemoveAt(0);
            tobs.Add(mainfont.renderText(t.text, t.pos, t.scale));

            glControl1.Invalidate();
        }

        private void loadCharmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Test BMP Image Class
            BMPImage bm = new BMPImage("courier.bmp");


            /* PNG CHARMAP CONVERT TO BMP
            
            //Opening PNG file
            //Image im = Image.FromFile("courier_charmap.png");
            //MemoryStream ms = new MemoryStream();
            //im.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            
            */

            //Testing some inits       
            mainfont.initFromImage(bm);
            mainfont.tex = bm.GLid;
            mainfont.program = shader_program;

            //Set default settings
            mainfont.space =  0.14f;
            mainfont.width =  0.14f;
            mainfont.height = 0.14f;

            //Add some text for rendering
            tobs.Add(mainfont.renderText("Life Sucks :(", new Vector2(0.2f,0.2f), 2.0f));
        }
    }


    


}
