using OpenTK.Graphics.OpenGL4;

namespace LineStrings.Framework
{
    /// <summary>
    ///  Обобщенный класс-враппер для буфера
    /// </summary>
    /// <typeparam name="T">Тип элемента буфера</typeparam>
    internal class Buffer<T> : IDisposable
        where T : struct
    {
        public Buffer(BufferTarget type, int typesize)
        {
            _vbo = GL.GenBuffer();
            _type = type;
            _typesize = typesize;
        }

        public Buffer<T> Bind()
        {
            GL.BindBuffer(_type, _vbo);

            return this;
        }

        public Buffer<T> SetData(T[] array)
        {
            Bind();

            GL.BufferData(_type, array.Length * _typesize, array, BufferUsageHint.DynamicDraw);

            return this;
        }

        private int _vbo;
        private BufferTarget _type;
        private int _typesize;

        ~Buffer()
        {
            if (!_isDisposed) Dispose();
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);

            _isDisposed = true;
        }

        private bool _isDisposed = false;
    }

    internal class VertexBuffer : Buffer<float>
    {
        public VertexBuffer()
         : base(BufferTarget.ArrayBuffer, sizeof(float))
        {
        }
    }

    internal class IndexBuffer : Buffer<uint>
    {
        public IndexBuffer()
         : base(BufferTarget.ElementArrayBuffer, sizeof(uint))
        {
        }
    }

    /// <summary>
    /// Класс-враппер над массивом атрибутов
    /// </summary>
    internal class VertexAttributes : IDisposable
    {
        public VertexAttributes()
        {
            _vao = GL.GenVertexArray();
        }

        public VertexAttributes Bind()
        {
            GL.BindVertexArray(_vao);

            return this;
        }

        public VertexAttributes AddAttribute(int size, VertexAttribPointerType type, bool normalized, int stride, int offset = 0)
        {
            Bind();

            GL.VertexAttribPointer(_attribCount, size, type, normalized, stride, offset);
            GL.EnableVertexAttribArray(_attribCount);

            _attribCount++;

            return this;
        }

        private int _vao;
        private int _attribCount = 0;

        ~VertexAttributes()
        {
            if (!_isDisposed) Dispose();
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vao);

            _isDisposed = true;
        }

        private bool _isDisposed = false;
    }
}