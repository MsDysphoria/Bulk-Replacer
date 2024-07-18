using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.TitleBar.ShowAboutSection += TitleBarOnShowAboutSection;
            this.AboutSection.HideSection += AboutSectionOnHideSection;
        }

        private void AboutSectionOnHideSection()
        {
            this.Replacer.Visibility = Visibility.Visible;
            this.TitleBar.HideSection();
        }

        private void TitleBarOnShowAboutSection()
        {
            this.Replacer.Visibility = Visibility.Collapsed;
            this.AboutSection.ShowInformation();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        { if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); } }
    }
}