using System;

using System.Windows;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace PngAnimator;

public class BindableSKElement : SKElement
{
    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register(nameof(Image), typeof(SKBitmap), typeof(BindableSKElement),
            new PropertyMetadata(null, OnImageChanged));

    public SKBitmap Image
    {
        get => (SKBitmap)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BindableSKElement element && e.NewValue is SKBitmap)
        {
            element.InvalidateVisual();
        }
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        if (Image == null) return;

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        float scaleX = e.Info.Width / (float)Image.Width;
        float scaleY = e.Info.Height / (float)Image.Height;
        var scale = Math.Min(scaleX, scaleY);

        float scaledWidth = Image.Width * scale;
        float scaledHeight = Image.Height * scale;

        float x = (e.Info.Width - scaledWidth) / 2;
        float y = (e.Info.Height - scaledHeight) / 2;

        var rect = new SKRect(x, y, x + scaledWidth, y + scaledHeight);
        canvas.DrawBitmap(Image, rect);
    }

}