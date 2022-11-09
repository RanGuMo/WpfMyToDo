using Microsoft.AspNetCore.Mvc;
using MyToDo.API.Service;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToDo.API.Controllers
{
    /// <summary>
    /// 待办事项控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService service;

        public ToDoController(IToDoService service)
        {
            this.service = service;
        }
        /// <summary>
        /// 通过Id 获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> Get(int id) => await service.GetSingleAsync(id);
        /// <summary>
        /// 根据ToDoParameter 传入的参数查询所有数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetAll([FromQuery] ToDoParameter param) => await service.GetAllAsync(param);

        [HttpGet]
        public async Task<ApiResponse> Summary() => await service.Summary();
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> Add([FromBody] ToDoDto model) => await service.AddAsync(model);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> Update([FromBody] ToDoDto model) => await service.UpdateAsync(model);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ApiResponse> Delete(int id) => await service.DeleteAsync(id);
    }
}
