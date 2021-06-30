using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Data;
using Storage.API.DTOs;
using Storage.API_CAN.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _repo;
        private readonly ISearchRepository _search;
        private readonly IMapper _mapper;
        public TaskController(ITaskRepository repo, ISearchRepository search, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _search = search;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tasks = await _repo.GetTasks();
            return Ok(tasks);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetTask(string name)
        {
            var task = await _repo.GetTask(name);

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> PostTask([FromForm] TaskForRegisterDto taskForRegisterDto)
        {

            var taskName = new TaskName
            {
                Name = taskForRegisterDto.Name,
                Status = "Created",
                BomName = taskForRegisterDto.BomName,
                Qty = taskForRegisterDto.QTY,
                DateAdded = DateTime.Now,
                LastModified = DateTime.Now
            };

            var result = await _repo.RegisterTask(taskName);


            return Ok();
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> Update(string name, TaskForUpdateDto taskForUpdateDto)
        {
            var taskFromRepo = await _repo.GetTask(name);
            taskForUpdateDto.LastModified = DateTime.Now;

            _mapper.Map(taskForUpdateDto, taskFromRepo);

            if (await _search.SaveAll())
                return NoContent();

            throw new Exception($"Updating task {name} failed on save");
        }


        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteBom(string name)
        {
            var taskName = await _repo.GetTask(name);
            if (taskName != null) _search.Delete(taskName);

            if (await _search.SaveAll()) return Ok();

            return BadRequest("Failed to delete");

        }

        [HttpGet("{name}/check")]
        public async Task<IActionResult> GetTaskListDB(string name)
        {
            var TaskName = await _repo.GetTask(name);
            var listas = await _repo.CheckDB(TaskName);

            return Ok(listas);
        }

        [HttpGet("{name}/check/short")]
        public async Task<IActionResult> GetTaskListDBforShort(string name)
        {
            var TaskName = await _repo.GetTask(name);
            var listas = await _repo.CheckDBShort(TaskName);

            return Ok(listas);
        }
    }
}
