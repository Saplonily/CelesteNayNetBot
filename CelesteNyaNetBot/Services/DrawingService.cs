using System.Drawing;
using System.Numerics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using PointF = SixLabors.ImageSharp.PointF;

namespace CelesteNyaNetBot.Services;

public class DrawingService
{
    protected Font fontForDraw;

    public DrawingService()
    {
        FontCollection fontCollection = new();
        fontCollection.AddSystemFonts();
        FontFamily fontFamily = fontCollection.Get("SimSun");
        fontForDraw = fontFamily.CreateFont(36);
    }

    public string Draw(string prefixAndNameStr, Color[] colors)
    {
        string fname = Path.GetTempFileName();
        using FileStream fs = new(fname, FileMode.Create, FileAccess.Write);

        TextOptions textOptions = new(fontForDraw)
        {
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        FontRectangle rect = TextMeasurer.Measure($"#12 - {prefixAndNameStr}", textOptions);
        int width = (int)(rect.Width * 1.1);
        int height = (int)(rect.Height * 1.5);
        textOptions.Origin = new PointF(width / 2f, height / 2f);

        Image<Rgba32> image = new(width, height * colors.Length);

        image.Mutate(p =>
        {
            for (int i = 0; i < colors.Length; i++)
            {
                p.DrawText(textOptions, $"#{i + 1} {prefixAndNameStr}", colors[i]);
                textOptions.Origin += new Vector2(0f, height);
            }
        });
        image.SaveAsPng(fs);

        return fname;
    }
}
