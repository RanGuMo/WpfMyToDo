using AutoMapper;
using MaterialDesignThemes.Wpf;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using WpfMyToDo.Common;
using WpfMyToDo.Extensions;
using WpfMyToDo.Service;

namespace WpfMyToDo.ViewModels
{
    public class ToDoViewModel : NavigationViewModel
    {

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ToDoDto> SelectedCommand { get; private set; }
        public DelegateCommand<ToDoDto> DeleteCommand { get; private set; }

        private readonly IDialogHostService dialogHost;
        public ToDoViewModel(IContainerProvider provider)
         : base(provider)
        {
            ToDoDtos = new ObservableCollection<ToDoDto>();
            
            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectedCommand = new DelegateCommand<ToDoDto>(Selected);
            DeleteCommand = new DelegateCommand<ToDoDto>(Delete);
            this.dialogHost = provider.Resolve<IDialogHostService>();
            this.service = provider.Resolve<IToDoService>();


        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增": Add(); break;
                case "查询": GetDataAsync(); break;
                case "保存": Save(); break;
            }
        }

       

        private ObservableCollection<ToDoDto> toDoDtos;
        public ObservableCollection<ToDoDto> ToDoDtos
        {
            get { return toDoDtos; }
            set { toDoDtos = value; RaisePropertyChanged(); }
        }

        private int selectedIndex;

        /// <summary>
        /// 下拉列表选中状态值
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 右侧编辑窗口是否展开
        /// </summary>
        private bool isRightDrawerOpen;
        private readonly IToDoService service;

        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }
        }



        private ToDoDto currentDto;

        /// <summary>
        /// 编辑选中/新增时对象
        /// </summary>
        public ToDoDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }

        private string search;

        /// <summary>
        /// 搜索条件
        /// </summary>
        public string Search
        {
            get { return search; }
            set { search = value; RaisePropertyChanged(); }
        }







        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="obj"></param>
        private async void Delete(ToDoDto obj)
        {
            try
            {
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除待办事项:{obj.Title} ?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;

                UpdateLoading(true);
                var deleteResult = await service.DeleteAsync(obj.Id);
                if (deleteResult.Status)
                {
                    var model = ToDoDtos.FirstOrDefault(t => t.Id.Equals(obj.Id));
                    if (model != null)
                        ToDoDtos.Remove(model); //移除ToDotos集合中的数据
                }
            }
            finally
            {
                UpdateLoading(false);
            }
        }

        /// <summary>
        /// 添加待办的按钮命令
        /// </summary>
        private void Add()
        {
            CurrentDto = new ToDoDto();
           IsRightDrawerOpen = true;
        }

        //添加待办
        private async void Save()
        {
            //如果 标题 或者 内容 为空
            if (string.IsNullOrWhiteSpace(CurrentDto.Title)|| string.IsNullOrWhiteSpace(CurrentDto.Content))
            {
                return;
            }
            UpdateLoading(true);
            try
            {
                //代表是更新数据
                if (CurrentDto.Id > 0)
                {
                    var updateResult = await service.UpdateAsync(CurrentDto);
                    if (updateResult.Status)
                    {
                        var todo = ToDoDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                        if (todo != null)
                        {
                            todo.Title = CurrentDto.Title;
                            todo.Content = CurrentDto.Content;
                            todo.Status = CurrentDto.Status;

                        }
                        IsRightDrawerOpen = false; //关闭折叠
                    }
                }
                else  //新增
                {
                    var addResult = await service.AddAsync(CurrentDto);
                    if (addResult.Status)
                    {
                        ToDoDtos.Add(addResult.Result);
                        IsRightDrawerOpen = false;
                    }
                }
            }
            catch (Exception)
            {


            }
            finally
            {
                UpdateLoading(false);
            }
          



        }


        /// <summary>
        /// 点击item项,展开右边的编辑,并填充编辑数据
        /// </summary>
        /// <param name="obj"></param>
        private async void Selected(ToDoDto obj)
        {
            try
            {
                UpdateLoading(true);
                var todoResult = await service.GetFirstOfDefaultAsync(obj.Id);
                if (todoResult.Status)
                {
                    CurrentDto = todoResult.Result;
                    IsRightDrawerOpen = true;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                UpdateLoading(false);
            }

        }

        /// <summary>
        /// 获取数据
        /// </summary>
        async void GetDataAsync()
        {
            //for (int i = 0; i < 20; i++)
            //{
            //    ToDoDtos.Add(new ToDoDto()
            //    {
            //        Id = i,
            //        Title = "标题" + i,
            //        Content ="测试数据。。。"
            //    });
            //}
            UpdateLoading(true);//开始加载数据
            // 0 代表全部  2代表已完成,如果已完成,就更改状态为1否则为0
            int? Status = SelectedIndex == 0 ? null : SelectedIndex == 2 ? 1 : 0;

            var todoResult = await service.GetAllFilterAsync(new ToDoParameter()
            {
                PageIndex = 0,
                PageSize = 100,
                Search = Search,
                Status = Status
            });
            if (todoResult.Status)
            {
                ToDoDtos.Clear();
                foreach (var item in todoResult.Result.Items)
                {
            
                    ToDoDtos.Add(item);
                }
            }

            UpdateLoading(IsOpen: false);//停止加载数据

        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if (navigationContext.Parameters.ContainsKey("Value"))
                SelectedIndex = navigationContext.Parameters.GetValue<int>("Value");
            else
                SelectedIndex = 0;
            //获取数据
            GetDataAsync();
        }


    }
}
