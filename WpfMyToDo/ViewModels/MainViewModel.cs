using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMyToDo.Common;
using WpfMyToDo.Common.Models;
using WpfMyToDo.Extensions;

namespace WpfMyToDo.ViewModels
{
    public class MainViewModel :BindableBase, IConfigureService
    {
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<MenuBar> menuBars;
        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 导航命令
        /// </summary>
        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
        /// <summary>
        /// 导航日志
        /// </summary>
        public IRegionNavigationJournal journal;

        /// <summary>
        /// 导航回退
        /// </summary>
        public DelegateCommand GoBackCommand { get; private set; }
        /// <summary>
        /// 导航前进
        /// </summary>
        public DelegateCommand GoForwardCommand { get; private set; }
        /// <summary>
        /// 注销命令
        /// </summary>
        public DelegateCommand LoginOutCommand { get; private set; }

        private readonly IRegionManager regionManager;
        private readonly IContainerProvider containerProvider;
       

        public MainViewModel(IContainerProvider containerProvider, IRegionManager regionManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            this.regionManager = regionManager;
            this.containerProvider = containerProvider;
            //后退
            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal!=null && journal.CanGoBack)
                {
                    journal.GoBack();
                }
            });
            //前进
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (journal != null &&  journal.CanGoForward)
                    journal.GoForward();
            });

            LoginOutCommand = new DelegateCommand(() =>
            {
                //注销当前用户
                App.LoginOut(containerProvider);
            });


        }

        private void Navigate(MenuBar obj)
        {
            //如果obj为空或者 命名空间没有，直接返回
            if (obj==null||string.IsNullOrWhiteSpace(obj.NameSpace))
            {
                return;
            }
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace, back =>
            {
                journal = back.Context.NavigationService.Journal;
            });
          
        }

       
        //创建菜单
        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookOutline", Title = "待办事项", NameSpace = "ToDoView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookPlus", Title = "备忘录", NameSpace = "MemoView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
        }
        /// <summary>
        /// 配置首页初始化参数
        /// </summary>
        public void Configure()
        {
            UserName = AppSession.UserName;
            CreateMenuBar();
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
        }
    }
}
