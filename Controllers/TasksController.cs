using ChatBotAPI.Models;
using ChatBotAPI.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatBotAPI.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            this._context = context;
        }

        // GET: api/<TasksController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Tasks = await _context.Tasks.ToListAsync();
            return Ok(new { Status = "Success", Data = Tasks, Message = "Tasks retreived created successfully!" });
        }

        // GET api/<TasksController>/5
        [HttpGet("{id}")]
        [ActionName("GetByID")]
        public async Task<IActionResult> GetByID(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.id == id);
            if (task != null)
            {
                return Ok(new { Status = "Success", Data = task, Message = "Task Found !" });
            }
            return NotFound(new { Status = "Error", Message = "Not Found !" });
        }

        /// <remarks>
        /// Sample request:
        ///
        ///     POST /task
        ///     {
        ///        "taskName": "Task Name",
        ///        "taskDescription":  "Hello testing Task Description",
        ///        "taskType": "Daily",
        ///        "taskStatus" : "In Progress" ,
        ///        "taskTags" : "today,urgent hello",
        ///        "taskDate" : "26/08/2023"
        ///     }
        ///
        /// </remarks>
        /// 
        // POST api/<TasksController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskItemRequest model)
        {
            if (model == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Task Creation Failed!." });
            }

            var task = new TaskItem()
            {
                TaskName = model.TaskName,
                TaskDescription = model.TaskDescription,
                TaskDate = model.TaskDate,
                TaskStatus = model.TaskStatus,
                TaskTags = model.TaskTags,
                TaskType = model.TaskType,
                CreatedAt = DateTime.Now,
            };
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return Ok(new { Status = "Success", Data = task.id, Message = "Task Created !" });
        }

        // PUT api/<TasksController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int? id, [FromBody] TaskItemRequest model)
        {
            if (model == null || id == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Task Update Failed!." });
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                task.TaskName = model.TaskName;
                task.TaskDescription = model.TaskDescription;
                task.TaskDate = model.TaskDate;
                task.TaskStatus = model.TaskStatus;
                task.TaskTags = model.TaskTags;
                task.TaskType = model.TaskType;

                await _context.SaveChangesAsync();
                return Ok(new { Status = "Success", Message = "Task Updated Sucessfully" });
            }
            return NotFound(new { Status = "Failed", Message = "Task Not Found" });
        }

        // DELETE api/<TasksController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Task Creation Failed!." });
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return Ok(new { Status = "Success", Message = "Task Deleted Sucessfully" });
            }
            return NotFound(new { Status = "Failed", Message = "Task Not Found" });
        }
    }
}
