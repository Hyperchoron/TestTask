using OpenTK.Graphics.OpenGL4;

namespace LineStrings.Framework
{
    /// <summary>
    /// Клас-враппер для шейдеров (стадии)
    /// </summary>
    internal class Shader : IDisposable
    {
        public Shader(ShaderType type)
        {
            _shader = GL.CreateShader(type);
        }

        public Shader SetSource(string source)
        {
            GL.ShaderSource(_shader, source);

            return this;
        }

        public Shader Compile()
        {
            GL.CompileShader(_shader);

            GL.GetShader(_shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string message = GL.GetShaderInfoLog(_shader);

                Console.WriteLine(message);
            }

            return this;
        }

        internal void AttachToProgram(int program)
        {
            GL.AttachShader(program, _shader);
        }

        private int _shader;

        ~Shader()
        {
            if (!_isDisposed) Dispose();
        }

        private bool _isDisposed = false;

        public void Dispose()
        {
            GL.DeleteShader(_shader);

            _isDisposed = true;
        }
    }

    internal class VertexShader : Shader
    {
        public VertexShader()
        : base(ShaderType.VertexShader)
        {
        }
    }

    internal class FragmentShader : Shader
    {
        public FragmentShader()
        : base(ShaderType.FragmentShader)
        {
        }
    }

    /// <summary>
    /// Класс-враппер шейдера (программы)
    /// </summary>
    internal class ShaderProgram : IDisposable
    {
        public ShaderProgram()
        {
            _program = GL.CreateProgram();
        }

        public ShaderProgram Attach(Shader shader)
        {
            shader.AttachToProgram(_program);

            return this;
        }

        public ShaderProgram Link()
        {
            GL.LinkProgram(_program);

            GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string message = GL.GetProgramInfoLog(_program);

                Console.WriteLine(message);
            }

            return this;
        }

        public ShaderProgram Use()
        {
            GL.UseProgram(_program);

            return this;
        }

        private int _program;

        ~ShaderProgram()
        {
            if (!_isDisposed) Dispose();
        }

        private bool _isDisposed = false;

        public void Dispose()
        {
            GL.DeleteProgram(_program);

            _isDisposed = true;
        }
    }
}