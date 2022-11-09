using Prism.Events;
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
using System.Windows.Shapes;
using WpfMyToDo.Common;
using WpfMyToDo.Extensions;

namespace WpfMyToDo.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            InitializeComponent();

            //注册等待消息窗口
            aggregator.Resgiter(arg =>
            {
                DialogHost.IsOpen = arg.IsOpen;

                if (DialogHost.IsOpen)  
                    DialogHost.DialogContent = new ProgressView();
            });
            //注册 全局 提示消息
            aggregator.ResgiterMessage(arg =>
            {
                Snackbar.MessageQueue.Enqueue(arg.Message);
            });

          

            //最小化事件
            btnMin.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            //最大化事件
            btnMax.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
            };
            //关闭窗口事件
            btnClose.Click += async (s, e) => {
                //退出系统前,询问窗口
                var dialogResult = await dialogHostService.Question("温馨提示", "确认退出系统?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                this.Close(); };
            //窗口可拖拽事件 DragMove(拖拽移动)
            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }

            };
            //窗口双击事件
            ColorZone.MouseDoubleClick += (s, e) =>
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
            };
            //菜单选择项改变事件（当选择某一项后，自动收起）
            menuBar.SelectionChanged += (s, e) => 
            {
                drawerHost.IsLeftDrawerOpen = false; 
            };
            




        }
    }
}
