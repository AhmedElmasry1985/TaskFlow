using System.Security.Claims;
using AutoMapper;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksApi.Data;
using TasksApi.DTOs;
using TasksApi.Models;

namespace TasksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController(ITasksUnitOfWork tasksUnitOfWork, IUserRepository userRepository, IMapper mapper)
    : ControllerBase
{
    
    [HttpGet("GetMine")]
    public async Task<ActionResult<GetTasksCreatedByUserResponseDto>> GetTasksCreatedByUser(
        [FromBody] GetTasksCreatedByUserRequestDto requestDto)
    {
        var responseDto = new GetTasksCreatedByUserResponseDto();
        var userIdFromClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var parseResult = int.TryParse(userIdFromClaim, out var userId);
        User? creatorUser = null;
        if(!parseResult)
            ModelState.AddModelError("UserId", "Creator user not found");
        else
        {
            creatorUser = await userRepository.GetUserByExternalId(userId);
            if (creatorUser == null)
                ModelState.AddModelError("UserId", "Creator user not found");
        }
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            responseDto.Result = new ValidationResult { IsSuccess = false, Message = string.Join(", ", errors) };
            return BadRequest(responseDto);
        }

        var tasks = await tasksUnitOfWork.TaskRepository.GetTasksCreatedByUser(creatorUser.Id);
        responseDto.Tasks = mapper.Map<List<TaskDto>>(tasks);
        responseDto.Result = new ValidationResult { IsSuccess = true, Message = "Tasks retrieved successfully" };
        return Ok(responseDto);
    }

    [HttpGet("GetAssignedToMe")]
    public async Task<ActionResult<GetTasksAssignedToUserResponseDto>> GetTasksAssignedToUser(
        [FromBody] GetTasksAssignedToUserRequestDto requestDto)
    {
        var responseDto = new GetTasksAssignedToUserResponseDto();
        var userIdFromClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var parseResult = int.TryParse(userIdFromClaim, out var userId);
        User? assignedUser = null;
        if(!parseResult)
            ModelState.AddModelError("UserId", "Assigned user not found");
        else
        {
            assignedUser = await userRepository.GetUserByExternalId(userId);
            if (assignedUser == null)
                ModelState.AddModelError("UserId", "Assigned user not found");
        }
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            responseDto.Result = new ValidationResult { IsSuccess = false, Message = string.Join(", ", errors) };
            return BadRequest(responseDto);
        }

        var tasks = await tasksUnitOfWork.TaskRepository.GetTasksAssignedToUser(assignedUser.Id);
        responseDto.Tasks = mapper.Map<List<TaskDto>>(tasks);
        responseDto.Result = new ValidationResult { IsSuccess = true, Message = "Tasks retrieved successfully" };
        return Ok(responseDto);
    }

    [HttpPost("CreateTask")]
    public async Task<ActionResult<CreateNewTaskResponseDto>> CreateNewTask(
        [FromBody] CreateNewTaskRequestDto requestDto)
    {
        var responseDto = new CreateNewTaskResponseDto();
        var userIdFromClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var parseResult = int.TryParse(userIdFromClaim, out var userId);
        User? creatorUser = null;
        if(!parseResult)
            ModelState.AddModelError("CreatorUserId", "Creator user not found");
        else
        { creatorUser = await userRepository.GetUserByExternalId(userId);
            if (creatorUser == null)
                ModelState.AddModelError("CreatorUserId", "Creator user not found");
        }
        User? assignedUser = creatorUser; // Default to creator
        if (creatorUser.Id !=
            requestDto.AssignedUserId) //prevents double check if the creator is the assigned user
        {
            assignedUser = await userRepository.GetUserByExternalId(requestDto.AssignedUserId);
            if (assignedUser == null)
                ModelState.AddModelError("AssignedUserId", "Assigned user not found");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            responseDto.Result = new ValidationResult { IsSuccess = false, Message = string.Join(", ", errors) };
            return BadRequest(responseDto);
        }

        var newTask = new Models.Task
        {
            Title = requestDto.Title,
            Description = requestDto.Description,
            StatusId = (int)Models.TaskStatus.Created,
            CreatorUserId = creatorUser.Id,
            AssignedUserId = assignedUser.Id,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow
        };

        await tasksUnitOfWork.TaskRepository.Add(newTask);
        await tasksUnitOfWork.SaveChangesAsync();

        // Reload the task with all related data
        var createdTask = await tasksUnitOfWork.TaskRepository.GetTasksCreatedByUser(creatorUser.Id);
        var task = createdTask.FirstOrDefault(t => t.Id == newTask.Id);

        responseDto.Task = mapper.Map<TaskDto>(task);
        responseDto.Result = new ValidationResult { IsSuccess = true, Message = "Task created successfully" };
        return Ok(responseDto);
    }

    [HttpPost("AddNote")]
    public async Task<ActionResult<AddNoteToTaskResponseDto>> AddNoteToTask(
        [FromBody] AddNoteToTaskRequestDto requestDto)
    {
        var responseDto = new AddNoteToTaskResponseDto();
        var userIdFromClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var parseResult = int.TryParse(userIdFromClaim, out var userId);
        User? creatorUser = null;
        if(!parseResult)
            ModelState.AddModelError("CreatorUserId", "Creator user not found");
        else
        {
            creatorUser = await userRepository.GetUserByExternalId(userId);
            if (creatorUser == null)
                ModelState.AddModelError("CreatorUserId", "Creator user not found");
        }
        var task = await tasksUnitOfWork.TaskRepository.FindById(requestDto.TaskId);
        if (task == null)
            ModelState.AddModelError("TaskId", "Task not found");
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            responseDto.Result = new ValidationResult { IsSuccess = false, Message = string.Join(", ", errors) };
            return BadRequest(responseDto);
        }

        var newNote = new Models.Note
        {
            Content = requestDto.Content,
            TaskId = requestDto.TaskId,
            CreatorUserId = creatorUser.Id,
            CreationDate = DateTime.UtcNow
        };
        task.ModificationDate = DateTime.UtcNow;
        await tasksUnitOfWork.NoteRepository.Add(newNote);
        await tasksUnitOfWork.SaveChangesAsync();

        // Reload the note with all related data
        var createdNotes = await tasksUnitOfWork.NoteRepository.GetNotesByTaskId(requestDto.TaskId);
        var note = createdNotes.FirstOrDefault(n => n.Id == newNote.Id);

        responseDto.Note = mapper.Map<NoteDto>(note);
        responseDto.Result = new ValidationResult { IsSuccess = true, Message = "Note added successfully" };
        return Ok(responseDto);
    }

    [HttpPut("ChangeStatus")]
    public async Task<ActionResult<ChangeTaskStatusResponseDto>> ChangeTaskStatus(
        [FromBody] ChangeTaskStatusRequestDto requestDto)
    {
        var responseDto = new ChangeTaskStatusResponseDto();

        var task = await tasksUnitOfWork.TaskRepository.FindById(requestDto.TaskId);
        if (task == null)
            ModelState.AddModelError("TaskId", "Task not found");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            responseDto.Result = new ValidationResult { IsSuccess = false, Message = string.Join(", ", errors) };
            return BadRequest(responseDto);
        }

        task.StatusId = requestDto.NewStatusId;
        task.ModificationDate = DateTime.UtcNow;

        await tasksUnitOfWork.SaveChangesAsync();

        // Reload the task with all related data
        var updatedTasks = await tasksUnitOfWork.TaskRepository.GetTasksCreatedByUser(task.CreatorUserId);
        var updatedTask = updatedTasks.FirstOrDefault(t => t.Id == task.Id);

        responseDto.Task = mapper.Map<TaskDto>(updatedTask);
        responseDto.Result = new ValidationResult { IsSuccess = true, Message = "Task status changed successfully" };
        return Ok(responseDto);
    }
}