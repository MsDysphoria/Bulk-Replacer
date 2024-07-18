using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Bulk_Replacer;

public partial class TitleBar : UserControl
{
    public event Action ShowAboutSection;

    private Storyboard fadeInStoryboard;
    private Storyboard fadeOutStoryboard;
    
    public TitleBar()
    {
        InitializeComponent();
        fadeInStoryboard = FindResource("FadeIn_Info") as Storyboard;
        fadeOutStoryboard = FindResource("FadeOut_Info") as Storyboard;
    }

    private void BtnInfo_OnMouseDown(object sender, RoutedEventArgs routedEventArgs)
    {
        ShowAboutSection?.Invoke();
        fadeOutStoryboard?.Begin();
    }

    private void BtnClose_OnMouseDown(object sender, RoutedEventArgs routedEventArgs)
    {
        Application.Current.Shutdown();
    }

    public void HideSection()
    {
        fadeInStoryboard?.Begin();
    }

}