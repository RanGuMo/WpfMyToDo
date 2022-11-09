using Microsoft.EntityFrameworkCore;
using MyToDo.API;
using MyToDo.API.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToDo.API.Context.Repository
{ 
    public class ToDoRepository : Repository<ToDo>, IRepository<ToDo>
    {
        public ToDoRepository(MyToDoContext dbContext) : base(dbContext)
        {
        }
    }
}
