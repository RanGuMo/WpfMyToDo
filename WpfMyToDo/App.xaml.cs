using DryIoc;
using WpfMyToDo.Views.Dialogs;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfMyToDo.Common;
using WpfMyToDo.Service;
using WpfMyToDo.ViewModels;
using WpfMyToDo.ViewModels.Dialogs;
using WpfMyToDo.Views;
using Prism.Services.Dialogs;

namespace WpfMyToDo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        
        //protected override void OnInitialized()
        //{
        //    var service = App.Current.MainWindow.DataContext as IConfigureService;
        //    if (service != null)
        //    {
        //        service.Configure();
        //    }
        //    base.OnInitialized();
        //}
        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="containerProvider"></param>
        public static void LoginOut(IContainerProvider containerProvider)
        {
            Current.MainWindow.Hide();//主页面隐藏

            var dialog = containerProvider.Resolve<IDialogService>();

            dialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }

                Current.MainWindow.Show();
            });
        }
        /// <summary>
        /// 初始化完成后
        /// </summary>
        protected override void OnInitialized()
        {
            var dialog = Container.Resolve<IDialogService>();

            dialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }

                var service = App.Current.MainWindow.DataContext as IConfigureService;
                if (service != null)
                    service.Configure();
                base.OnInitialized();
            });
        }



        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //注入容器并给构造函数起名为 webUrl
            containerRegistry.GetContainer()
                .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            //注册实例
            containerRegistry.GetContainer().RegisterInstance(@"http://localhost:4745/", serviceKey: "webUrl");
            
            containerRegistry.Register<ILoginService, LoginService>();
            containerRegistry.Register<IToDoService, ToDoService>();
            containerRegistry.Register<IMemoService, MemoService>();
            //containerRegistry.Register<IDialogHostService, DialogHostService>();

            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();


            //对话服务
            containerRegistry.Register<IDialogHostService, DialogHostService>();

            containerRegistry.RegisterForNavigation<AddToDoView,AddToDoViewModel>();
            containerRegistry.RegisterForNavigation<AddMemoView,AddMemoViewModel>();

            containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();


            containerRegistry.RegisterForNavigation<IndexView,IndexViewModel>();
            containerRegistry.RegisterForNavigation<MemoView,MemoViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView,SettingsViewModel>();
            containerRegistry.RegisterForNavigation<ToDoView,ToDoViewModel>();
            containerRegistry.RegisterForNavigation<SkinView,SkinViewModel>();
            containerRegistry.RegisterForNavigation<AboutView>();
        }
    }
}
