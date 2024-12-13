using LineStrings.Framework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LineStrings.Painter.Impl
{
    public class OpenGLPainter : GameWindow, ILineStringPainter
    {
        public OpenGLPainter(int width, int height, string title = "LINESTRING Painter")
            : base(
                GameWindowSettings.Default,
                new NativeWindowSettings() { ClientSize = (width, height), Title = title, WindowBorder = WindowBorder.Fixed, StartVisible = false }
                )
        {
            _width = width;
            _height = height;

            _fbo = new Framebuffer(width, height);

            GL.Viewport(0, 0, width, height);

            // xy-координаты и uv-координаты вершин прямоугольника-холста (В нормализованных координатах)
            float[] vertices = {
                -1.0f, 1.0f, 0.0f, 0.0f, // Top-Left
                1.0f, 1.0f, 1.0f, 0.0f, // Top-Right
                1.0f, -1.0f, 1.0f, 1.0f, // Bottom-Right
                -1.0f, -1.0f, 0.0f, 1.0f, // Bottom-Left
            };

            // Индексы вершин для построения прямоугольника
            uint[] indices = {
                0, 1, 2,
                2, 3, 0,
            };

            // Буферы холста

            _fboMesh = new VertexBuffer();
            _fboMesh.SetData(vertices);

            _fboMeshIndices = new IndexBuffer();
            _fboMeshIndices.SetData(indices);

            _fboMesh.Bind();

            _fboAttribs = new VertexAttributes()
                .AddAttribute(2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0)
                .AddAttribute(2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            // Шейдеры для отрисовки холста

            string fboVertShdrSrc = @"
                #version 460 core
                layout (location = 0) in vec2 aPosition;
                layout (location = 1) in vec2 aTexCoords;

                out vec2 TexCoords;

                void main()
                {
                    gl_Position = vec4(aPosition, 0.0, 1.0);
                    TexCoords = aTexCoords;
                }
            ";

            string fboFragShdrSrc = @"
                #version 460 core
                out vec4 FragColor;

                in vec2 TexCoords;

                uniform sampler2D screenTexture;

                void main()
                {
                    FragColor = texture(screenTexture, TexCoords);
                }
            ";

            var fboVertShdr = new VertexShader().SetSource(fboVertShdrSrc).Compile();
            var fboFragShdr = new FragmentShader().SetSource(fboFragShdrSrc).Compile();

            _fboShader = new ShaderProgram()
                .Attach(fboVertShdr)
                .Attach(fboFragShdr)
                .Link();

            // Вершинный буфер для отрисовки линий:
            _lineStringVertices = new VertexBuffer();
            _lineStringVertices.Bind();

            _lineStringAttribs = new VertexAttributes()
                .AddAttribute(2, VertexAttribPointerType.Float, false, 2 * sizeof(float));

            // Простые шейдеры для линий:

            string lsVertShdrSrc = @"
                #version 460 core
                layout (location = 0) in vec2 aPosition;

                void main()
                {
                    gl_Position = vec4(aPosition, 0.0, 1.0);
                }
            ";

            string lsFragShdrSrc = @"
                #version 460 core
                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(1.0, 0.0, 0.0, 1.0);
                }
            ";

            var lsVertShdr = new VertexShader().SetSource(lsVertShdrSrc).Compile();
            var lsFragShdr = new FragmentShader().SetSource(lsFragShdrSrc).Compile();

            _lineStringShader = new ShaderProgram()
                .Attach(lsVertShdr)
                .Attach(lsFragShdr)
                .Link();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        /// <summary>
        /// Здесь происходит отрисовка холста
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _fbo.BindToDraw(); // Здесь холст используется в качестве источника картинки

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _fboAttribs.Bind();
            _fboMesh.Bind();
            _fboMeshIndices.Bind();
            _fboShader.Use();

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        public void BeginLineString()
        {
            _lineStringPoints.Clear();
        }

        public void EndLineString()
        {
            if (_lineStringPoints.Count() <= 2) return;
            
            _lineStringVertices.SetData(_lineStringPoints.ToArray());

            _fbo.BindAsTarget(); // Холст используется в качестве цели
            
            _lineStringAttribs.Bind();
            _lineStringVertices.Bind();
            _lineStringShader.Use();

            GL.Viewport(0, 0, _width, _height);

            GL.DrawArrays(PrimitiveType.LineStrip, 0, _lineStringPoints.Count() / 2); // Просто отрисовка линий

            GL.Flush();

            _fbo.Unbind();
        }

        public void LineStringPoint(float x, float y)
        {
            _lineStringPoints.Add(x / _width * 2.0f - 1.0f);
            _lineStringPoints.Add(y / _height * 2.0f - 1.0f);
        }

        public void PrepareImage()
        {
            _fbo.BindAsTarget(); // Здесь холст используется в качестве цели

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Flush();

            _fbo.Unbind();
        }

        private readonly int _width, _height;

        private Framebuffer _fbo;
        private VertexBuffer _fboMesh;
        private IndexBuffer _fboMeshIndices;
        private VertexAttributes _fboAttribs;
        private ShaderProgram _fboShader;

        private List<float> _lineStringPoints = new List<float>();
        private VertexBuffer _lineStringVertices;
        private VertexAttributes _lineStringAttribs;
        private ShaderProgram _lineStringShader;
    }
}