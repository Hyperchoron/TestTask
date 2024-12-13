using System.Text.RegularExpressions;
using LineString.Parser;
using LineStrings.Painter.Impl;

namespace LineStrings
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StreamReader sr;

            if (args.Length > 0) sr = new StreamReader(args[0]);
            else sr = new StreamReader(Console.OpenStandardInput());

            using (var app = new OpenGLPainter(800, 600))
            {
                app.PrepareImage();

                LineStringParser parser = new(sr);

                parser.ParseAndPaintTo(app);

                app.BeginLineString();

                app.LineStringPoint(100, 100);
                app.LineStringPoint(200, 200);

                app.EndLineString();

                app.IsVisible = true;

                app.Run();
            }
        }
    }
}