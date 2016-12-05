using System;
using OpenTK.Graphics.OpenGL;


namespace GLHelpers
{
    class GLShaderHelper
    {
        //Shader Creation
        static public void CreateShaders(string vs, string fs, out int vertexObject,
            out int fragmentObject, out int program)
        {
            int status_code;
            string info;

            vertexObject = GL.CreateShader(ShaderType.VertexShader);
            fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            //Compile vertex Shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out status_code);
            if (status_code != 1)
                throw new ApplicationException(info);

            //Compile fragment Shader
            GL.ShaderSource(fragmentObject, fs);

            //HANDLE INCLUDES
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out status_code);
            if (status_code != 1)
                throw new ApplicationException(info);

            program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);
            GL.LinkProgram(program);
            //GL.UseProgram(program);

        }
    }
}
