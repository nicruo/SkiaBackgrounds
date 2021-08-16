using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SkiaBackgrounds
{
    public class Bubble
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Speed { get; set; }
        public int TimeToLive { get; set; }
    }

    public sealed partial class MainPage : Page
    {
        private int maxBubbles = 100;
        private int spawnRate = 600;
        private int minBubblesSpeed = 4;
        private int maxBubblesSpeed = 7;
        private List<Bubble> bubbles = new List<Bubble>();
        private Random random = new Random();

        public MainPage()
        {
            InitializeComponent();

            StartTimer(TimeSpan.FromMilliseconds(spawnRate), () =>
            {
                if(bubbles.Count < maxBubbles)
                {
                    bubbles.Add(new Bubble { X = random.NextDouble(), Y = 1.1, TimeToLive = 1400 , Speed = random.Next(minBubblesSpeed, maxBubblesSpeed) });
                }
                return true;
            });

            StartTimer(TimeSpan.FromMilliseconds(10), () =>
            {
                for (int i = 0; i < bubbles.Count; i++)
                {
                    var bubble = bubbles[i];
                    bubble.Y -= bubble.Speed / 1000f;
                    bubble.TimeToLive -= 10;
                    if(bubble.TimeToLive <= 0)
                    {
                        bubbles.Remove(bubble);
                    }
                }
                canvas.Invalidate();
                return true;
            });
        }

        private void SKXamlCanvas_PaintSurface(object sender, SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            using (SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 1,
                Shader = SKShader.CreateLinearGradient(new SKPoint(0, 80), new SKPoint(0, info.Height), new SKColor[] { new SKColor(21, 142, 242), new SKColor(13, 90, 153) }, SKShaderTileMode.Repeat)
            })
            {
                canvas.DrawRect(0, 80, info.Width, info.Height, paint);
            };

            foreach (var bubble in bubbles)
            {
                using (SKPaint paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    StrokeWidth = 1,
                    Color = new SKColor(183, 222, 234, (byte)(bubble.Y * 255)),
                    IsAntialias = true
                })
                {
                    canvas.DrawCircle((float)(info.Width * bubble.X), (float)(info.Height * bubble.Y), 10, paint);
                };
            }

        }

        private void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            var timerTick = 0L;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            void renderingFrameEventHandler(object sender, object args)
            {
                var newTimerTick = stopWatch.ElapsedMilliseconds / (long)interval.TotalMilliseconds;
                if (newTimerTick == timerTick)
                    return;
                timerTick = newTimerTick;
                if (!callback())
                    CompositionTarget.Rendering -= renderingFrameEventHandler;
            }
            CompositionTarget.Rendering += renderingFrameEventHandler;
        }
    }
}