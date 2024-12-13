namespace LineStrings.Painter
{
    public interface ILineStringPainter
    {
        /// <summary>
        /// Подготовить холст (изображение)
        /// </summary>
        public void PrepareImage();

        /// <summary>
        /// Начать ломанную
        /// </summary>
        public void BeginLineString();

        /// <summary>
        /// Завершить ломанную
        /// </summary>
        public void EndLineString();

        /// <summary>
        /// Добавить вершину ломанной
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        public void LineStringPoint(float x, float y);
    }
}