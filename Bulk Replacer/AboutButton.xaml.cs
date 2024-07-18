using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Bulk_Replacer;

public partial class AboutButton : UserControl
{
    public static readonly DependencyProperty NormalImageProperty =
        DependencyProperty.Register("NormalImage", typeof(BitmapImage), typeof(AboutButton), new PropertyMetadata(null));

    public static readonly DependencyProperty HoverImageProperty =
        DependencyProperty.Register("HoverImage", typeof(BitmapImage), typeof(AboutButton), new PropertyMetadata(null));

    public static readonly DependencyProperty TooltipProperty =
        DependencyProperty.Register("Tooltip", typeof(string), typeof(AboutButton), new PropertyMetadata(null));
    
    public static readonly DependencyProperty LinkProperty =
        DependencyProperty.Register("Link", typeof(string), typeof(AboutButton), new PropertyMetadata(null));
    
    public BitmapImage NormalImage
    {
        get { return (BitmapImage)GetValue(NormalImageProperty); }
        set { SetValue(NormalImageProperty, value); }
    }

    public BitmapImage HoverImage
    {
        get { return (BitmapImage)GetValue(HoverImageProperty); }
        set { SetValue(HoverImageProperty, value); }
    }
    
    public string Tooltip
    {
        get { return (string)GetValue(TooltipProperty); }
        set { SetValue(TooltipProperty, value); }
    }    
    
    public string Link
    {
        get { return (string)GetValue(LinkProperty); }
        set { SetValue(LinkProperty, value); }
    }
    
    public AboutButton()
    {
        InitializeComponent();
    }

    private void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = Link,
            UseShellExecute = true
        });
    }
}