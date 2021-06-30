using Storage.API.DTOs;
using Storage.API.Helpers;
using Storage.API_CAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Data
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskName>> GetTasks();
        Task<TaskName> RegisterTask (TaskName taskName);
        Task<TaskName> GetTask(string name);
        Task<List<TaskList>> CheckDB(TaskName taskName);
        Task<List<TaskList>> CheckDBShort(TaskName taskName);
    }
}
