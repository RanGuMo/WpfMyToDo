
using MyToDo.Shared.Dtos;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMyToDo.Extensions;
using WpfMyToDo.Service;

namespace WpfMyToDo.ViewModels.Dialogs
{

  
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public LoginViewModel(ILoginService loginService, IEventAggregator aggregator)
        {
            UserDto = new ResgiterUserDto();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.loginService = loginService;
            this.aggregator = aggregator;
        }
        private readonly ILoginService loginService;
        private readonly IEventAggregator aggregator;
        public DelegateCommand<string> ExecuteCommand { get; private set; }

        

        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// 取消关闭对话框
        /// </summary>
        /// <returns></returns>
        public bool CanCloseDialog()
        {
            return true;
        }
        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void OnDialogClosed()
        {
            LoginOut();//注销登录
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        #region Login 属性
        public string Title { get; set; } = "ToDo";  //登录页面左上角的标题
        /// <summary>
        ///  选择索引 0为登录页面,1为注册页面
        /// </summary>
        private int selectIndex;
        public int SelectIndex
        {
            get { return selectIndex; }
            set { selectIndex = value; RaisePropertyChanged(); }
        }

        /// <summary>
        ///  登录用户名
        /// </summary>
        private string userName; 
        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }
        /// <summary>
        ///  登录密码
        /// </summary>
        private string passWord;
        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; RaisePropertyChanged(); }
        }

        #endregion

        private ResgiterUserDto userDto;

        public ResgiterUserDto UserDto
        {
            get { return userDto; }
            set { userDto = value; RaisePropertyChanged(); }
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                //登录
                case "Login": Login(); break;
                //注销
                case "LoginOut": LoginOut(); break;
                //注册
                case "Resgiter": Resgiter(); break;
                //跳转到注册页面
                case "ResgiterPage": SelectIndex = 1; break;
                //返回到登录页面
                case "Return": SelectIndex = 0; break;
            }
        }

      
        /// <summary>
        /// 登录
        /// </summary>
        async void Login()
        {
            if (string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrWhiteSpace(PassWord))
            {
                return;
            }

            var loginResult = await loginService.Login(new MyToDo.Shared.Dtos.UserDto()
            {
                Account = UserName,
                PassWord = PassWord
            });

            if (loginResult != null && loginResult.Status)
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));

            }
            else
            {
                //登录失败提示...
                aggregator.SendMessage("登录失败,用户名或密码错误!", "Login");
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        private async void Resgiter()
        {
            if (string.IsNullOrWhiteSpace(UserDto.Account) ||
                string.IsNullOrWhiteSpace(UserDto.UserName) ||
                string.IsNullOrWhiteSpace(UserDto.PassWord) ||
                string.IsNullOrWhiteSpace(UserDto.NewPassWord))
            {
                aggregator.SendMessage("请输入完整的注册信息！", "Login");
                return;
            }

            if (UserDto.PassWord != UserDto.NewPassWord)
            {
                aggregator.SendMessage("密码不一致,请重新输入！", "Login");
                return;
            }

            var resgiterResult = await loginService.Resgiter(new MyToDo.Shared.Dtos.UserDto()
            {
                Account = UserDto.Account,
                UserName = UserDto.UserName,
                PassWord = UserDto.PassWord
            });

            if (resgiterResult != null && resgiterResult.Status)
            {
                aggregator.SendMessage("注册成功", "Login");
                //注册成功,返回登录页页面
                SelectIndex = 0;
            }
            else
                aggregator.SendMessage(resgiterResult.Message, "Login");
        }
        /// <summary>
        /// 注销
        /// </summary>
        void LoginOut()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.No));
        }

     
    }
}
