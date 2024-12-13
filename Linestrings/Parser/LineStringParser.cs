using System.Text.RegularExpressions;
using LineStrings.Painter;

namespace LineString.Parser
{
    public class LineStringParser
    {
        public LineStringParser(StreamReader reader)
        {
            _sr = reader;
        }

        public void ParseAndPaintTo(ILineStringPainter painter)
        {
            string? line;
            while ((line = _sr.ReadLine()) is not null)
            {
                if (line.Length == 0) break;

                var tokens = line.Split([' ', '(', ')'], StringSplitOptions.RemoveEmptyEntries); // Без сложного парсера и валидации...

                painter.BeginLineString();

                for (int i = 1; i < tokens.Length - 1; i += 2)
                {
                    float xCoord, yCoord;

                    if (!float.TryParse(tokens[i], out xCoord)) xCoord = 0.0f;
                    if (!float.TryParse(tokens[i + 1], out yCoord)) yCoord = 0.0f;

                    painter.LineStringPoint(xCoord, yCoord);
                }

                painter.EndLineString();
            }
        }

        private readonly StreamReader _sr;
    }
}