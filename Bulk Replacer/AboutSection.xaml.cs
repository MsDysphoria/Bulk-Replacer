using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Bulk_Replacer;

public partial class AboutSection : UserControl
{
    public static readonly DependencyProperty CancelTokenProperty =
        DependencyProperty.Register("CancelToken", typeof(CancellationTokenSource), typeof(AboutSection), new PropertyMetadata(null));

    public event Action HideSection;
    public CancellationTokenSource CancellationToken
    {
        get { return (CancellationTokenSource)GetValue(CancelTokenProperty); }
        set { SetValue(CancelTokenProperty, value); }
    }
    
    public AboutSection()
    {
        InitializeComponent();
    }

    private void Return_MouseDown(object sender, MouseButtonEventArgs e)
    {
        CancellationToken?.Cancel();
        CancellationToken?.Dispose();
        CancellationToken = null;
        Storyboard fadeOutStoryboard = this.FindResource("FadeOut_Info") as Storyboard;
        fadeOutStoryboard?.Begin();
        HideSection?.Invoke();
    }
    
    public void ShowInformation()
    {
        Storyboard fadeInStoryboard = this.FindResource("FadeIn_Info") as Storyboard;
        fadeInStoryboard.Begin();
    }
}