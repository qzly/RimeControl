using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace RimeControl.UserControl
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingPage : System.Windows.Controls.UserControl
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        #region 加载圆圈的margin
        [Description("加载圆圈的margin"), Category("扩展"), DefaultValue(0)]
        public string LoadCirclesMargin
        {
            get { return (string)GetValue(LoadCirclesMarginProperty); }
            set { SetValue(LoadCirclesMarginProperty, value); }
        }


        public static readonly DependencyProperty LoadCirclesMarginProperty =
            DependencyProperty.Register("LoadCirclesMargin", typeof(string), typeof(LoadingPage),
            new FrameworkPropertyMetadata("50"));
        #endregion

        #region 加载中的提示
        [Description("加载中的提示"), Category("扩展"), DefaultValue(0)]
        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }


        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register("LoadingText", typeof(string), typeof(LoadingPage),
            new FrameworkPropertyMetadata("加载中"));
        #endregion

        #region 加载中的提示的字体大小
        [Description("加载中的提示的字体大小"), Category("扩展"), DefaultValue(0)]
        public int LoadingTextFontSize
        {
            get { return (int)GetValue(LoadingTextFontSizeProperty); }
            set { SetValue(LoadingTextFontSizeProperty, value); }
        }


        public static readonly DependencyProperty LoadingTextFontSizeProperty =
            DependencyProperty.Register("LoadingTextFontSize", typeof(int), typeof(LoadingPage),
            new FrameworkPropertyMetadata(12));
        #endregion

        #region 圆圈的颜色
        [Description("圆圈的颜色"), Category("扩展"), DefaultValue(0)]
        public Brush CirclesBrush
        {
            get { return (Brush)GetValue(CirclesBrushProperty); }
            set { SetValue(CirclesBrushProperty, value); }
        }


        public static readonly DependencyProperty CirclesBrushProperty =
            DependencyProperty.Register("CirclesBrush", typeof(Brush), typeof(LoadingPage),
            new FrameworkPropertyMetadata(Brushes.Black));
        #endregion

        #region 加载中的提示的字体颜色
        [Description("加载中的提示的字体颜色"), Category("扩展"), DefaultValue(0)]
        public Brush LoadingTextForeground
        {
            get { return (Brush)GetValue(LoadingTextForegroundProperty); }
            set { SetValue(LoadingTextForegroundProperty, value); }
        }


        public static readonly DependencyProperty LoadingTextForegroundProperty =
            DependencyProperty.Register("LoadingTextForeground", typeof(Brush), typeof(LoadingPage),
            new FrameworkPropertyMetadata(Brushes.DarkSlateGray));
        #endregion
    }
}
