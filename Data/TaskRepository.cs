using Microsoft.EntityFrameworkCore;
using Storage.API.DTOs;
using Storage.API.Helpers;
using Storage.API_CAN.DTOs;
using Storage.API_CAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DataContext _context;
        public TaskRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<TaskName> GetTask(string name)
        {
            var taskName = await _context.TaskName.Include(u => u.TaskList).FirstOrDefaultAsync(u => u.Name == name);
            return taskName;
        }

        public async Task<IEnumerable<TaskName>> GetTasks()
        {
            var tasks = await _context.TaskName.ToListAsync();

            return tasks;
        }

        public async Task<TaskName> RegisterTask(TaskName taskName)
        {

            await _context.TaskName.AddAsync(taskName);
            await _context.SaveChangesAsync();



            var bomName = await _context.BomName.Include(u => u.BomList).FirstOrDefaultAsync(u => u.Name == taskName.BomName);
            var bomList = await _context.BomList.Where(u => u.BomNameId == bomName.Id).ToListAsync();






            foreach (var element in bomList)
            {
                var taskList = new TaskList
                {
                    BuhNr = element.BuhNr,
                    BomNameId = element.BomNameId,
                    ManufPartNr = element.ManufPartNr,
                    ComponentasId = element.ComponentasId,
                    Qty = taskName.Qty,
                    Status = "Created",
                    TaskNameId = taskName.Id
                };

                await _context.TaskList.AddAsync(taskList);
                await _context.SaveChangesAsync();
            }




            return taskName;
        }

        public async  Task<List<TaskList>> CheckDB(TaskName taskName)
        {
            var suma = 0;
            var listas = new List<TaskList>();

            foreach (var item in taskName.TaskList)
            {
                if (item.Status != "Stored")
                {
                    var componentas = await _context.Componentass.Include(u => u.Reels).FirstOrDefaultAsync(u => u.BuhNr == item.BuhNr);
                    if (componentas != null)
                    {
                        foreach (var itemas in componentas.Reels)
                        {
                            suma = suma + itemas.QTY;
                        }

                    }
                    if (suma > item.Qty)
                    {
                        listas.Add(new TaskList
                        {
                            Id = item.Id,
                            BuhNr = item.BuhNr,
                            Qty = item.Qty,
                            BomNameId = item.BomNameId,
                            TaskNameId = taskName.Id,
                            Status = "Checked",
                            ComponentasId = item.ComponentasId,
                            ManufPartNr = item.ManufPartNr
                        });
                        suma = 0;
                    }
                }


            }

            return listas;
        }
         public async  Task<List<TaskList>> CheckDBShort(TaskName taskName)
        {
            var suma = 0;
            var listas = new List<TaskList>();

            foreach (var item in taskName.TaskList)
            {
                if (item.Status != "Stored")
                {
                    var componentas = await _context.Componentass.Include(u => u.Reels).FirstOrDefaultAsync(u => u.BuhNr == item.BuhNr);
                    if (componentas != null)
                    {
                        foreach (var itemas in componentas.Reels)
                        {
                            suma = suma + itemas.QTY;
                        }

                    }
                    if (suma <= item.Qty)
                    {
                        listas.Add(new TaskList
                        {
                            Id = item.Id,
                            BuhNr = item.BuhNr,
                            Qty = item.Qty,
                            BomNameId = item.BomNameId,
                            TaskNameId = taskName.Id,
                            Status = "Checked",
                            ComponentasId = item.ComponentasId,
                            ManufPartNr = item.ManufPartNr
                        });
                        
                    }
                }
             suma = 0;

            }

            return listas;
        }
    }
}
