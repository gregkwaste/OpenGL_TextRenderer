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
            vvs = GLSL_Preprocessor.Parser("Shaders/Simple_VS.glsl");
            ffs = GLSL_Preprocessor.Parser("Shaders/Simple_FS.glsl");
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

            glControl1.SwapBuffers();

        }

        private void renderText(string text)
        {
            /*
             * THIS FUNCTION WILL IMPLEMENT TEXT RENDERING
             * 
             */
            float space = 0.14f;
            float width = 0.14f;
            float height = 0.14f;

            //Check if font exists
            if (mainfont == null) return;

            //Create text objects
            GLText gtex = new GLText();
            //Load Image
            gtex.GLImage = sampleTex;
            gtex.program = shader_program;
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
                Glyph glph = mainfont.char_dict[c];
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
            tobs.Add(gtex);

        }

        private void prepareFont()
        {

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

            //Add some text for rendering
            renderText("Fuck you Shaw!");
        }
    }


    class GLText
    {
        public float[] pverts;
        public float[] puvs;
        public int[] pints;

        public int GLImage;

        //GL Buffers
        public int vbo;
        public int uvbo;
        public int ebo;
        public int program;
        

        public void render()
        {
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


}
