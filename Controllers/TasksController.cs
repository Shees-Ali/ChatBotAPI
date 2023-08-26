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
            var Tasks = await  _context.Tasks.ToListAsync();
            return Ok(new { Status = "Success", Data = Tasks ,Message = "Tasks retreived created successfully!" });
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

 /*       {
  "taskName": "Task Name",
  "taskDescription": "Hello testing Task Description",
  "taskType": "Daily",
  "taskStatus": "In Progress",
  "taskTags": "today,urgent hello",
  "taskDate": "26/08/2023"
}*/
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
            return Ok (new { Status = "Success", Data = task.id, Message = "Task Created !" });
        }

        // PUT api/<TasksController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TasksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
