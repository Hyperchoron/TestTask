using OpenTK.Graphics.OpenGL4;

namespace LineStrings.Framework
{
    /// <summary>
    /// Класс-враппер для фреймбуфера и его текстуры
    /// </summary>
    class Framebuffer : IDisposable
    {
        public Framebuffer(int width, int height)
        {
            _fbo = GL.GenFramebuffer();
            _fboTex = GL.GenTexture();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            GL.BindTexture(TextureTarget.Texture2D, _fboTex);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgb,
                width,
                height,
                0,
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                0
            );

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _fboTex, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("Framebuffer is incomplete!");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void BindAsTarget()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        }

        public void BindToDraw()
        {
            GL.BindTexture(TextureTarget.Texture2D, _fboTex);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private int _fbo;
        private int _fboTex;

        ~Framebuffer()
        {
            if (!_isDisposed) Dispose();
        }

        private bool _isDisposed = false;

        public void Dispose()
        {
            GL.DeleteFramebuffer(_fbo);
            GL.DeleteTexture(_fboTex);

            _isDisposed = true;
        }
    }
}