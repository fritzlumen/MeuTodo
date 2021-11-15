using MeuTodo.Data;
using MeuTodo.Models;
using MeuTodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeuTodo.Controllers
{
    [ApiController]
    [Route("v1")]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        [Route("todos/skip/{skip:int}/take/{take:int}")]
        public async Task<IActionResult> GetAsync(
            [FromServices] AppDbContext context,
            [FromRoute]int skip = 0,
            [FromRoute]int take = 25)
        {
            if (take > 1000)
                return BadRequest();

            var total = await context.Todos.CountAsync();

            var todos = await context
                .Todos
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return Ok(new
            {
                total,
                skip,
                take,
                data = todos
            });
        }

        [HttpGet]
        [Route("todos/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todo = await context
                .Todos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return todo == null
                ? NotFound()
                : Ok(todo);
        }
        [HttpPost("todos")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = new ToDo
            {
                Date = DateTime.Now,
                Done = false,
                Title = model.Title
            };

            try
            {
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
                return Created($"v1/todos/{todo.Id}", todo);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        [HttpPut("todos/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = await context
                .Todos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (todo == null)
                NotFound();

            try
            {
                todo.Title = model.Title;

                context.Todos.Update(todo);
                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpDelete("todos/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todo = await context
                .Todos
                .FirstOrDefaultAsync(x => x.Id == id);

            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }
        [HttpGet("todos/load")]
        public async Task<IActionResult> LoadAsync(
            [FromServices] AppDbContext context)
        {
            for (int i = 0; i < 1500; i++)
            {
                var todo = new ToDo()
                {
                    Id = i + 1,
                    Done = false,
                    Date = DateTime.Now,
                    Title = $"Tarefa {i}"
                };
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
            }

            return Ok();
        }
    }

}

