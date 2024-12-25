using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using WebApplication2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Pages;

public class ToDo : PageModel
{
    public List<Todo> Yapılacaklar { get; set; } = new List<Todo>();
    public List<Todo> Yapılıyor { get; set; } = new List<Todo>();
    public List<Todo> Bitti { get; set; } = new List<Todo>();

    private readonly AppDbContext appDbContext;

    public ToDo()
    {
        appDbContext = new AppDbContext();
    }

    public string Name { get; set; }
    public string Gender { get; set; }

    public IActionResult OnGet()
    {
        var token = Request.Cookies["LoginProject2AppSessionToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("Index");
        }

        var session = appDbContext.Session
            .Include(session => session.User)
            .Include(session => session.User.Gender)
            .SingleOrDefault(session => session.token == token);

        if (session == null)
        {
            return RedirectToPage("Index");
        }

        Name = session.User.userName;
        if (session.User.Gender.name == "female")
        {
            Gender = " hanım";
        }
        else
        {
            Gender = " bey";
        }

        var todos = appDbContext.Todo
            .Where(todo => todo.userId == session.User.id)
            .ToList();
        if (todos == null || !todos.Any())
        {
            return Page();
        }

        foreach (var todo in todos)
        {
            switch (todo.status)
            {
                case 0:
                    Yapılacaklar.Add(todo);
                    break;
                case 1:
                    Yapılıyor.Add(todo);
                    break;
                case 2:
                    Bitti.Add(todo);
                    break;
            }
        }

        return Page();
        
    }

    public RedirectToPageResult OnPost([FromForm] int id, [FromForm] string title, [FromForm] int status,
        bool Delete = false)
    {
        var token = Request.Cookies["LoginProject2AppSessionToken"];
        var session = appDbContext.Session
            .Include(session => session.User)
            .Include(session => session.User.Gender)
            .SingleOrDefault(session => session.token == token);
        if (session == null)
        {
            return RedirectToPage("Index");
        }

        if (id == 0)
        {
            var newTodo = new Todo
            {
                userId = session.User.id,
                title = title,
                status = status
            };
            appDbContext.Todo.Add((newTodo));
        }
        else
        {
            var todo = appDbContext.Todo.SingleOrDefault(todo => todo.id == id && todo.userId == session.User.id);
            if (todo != null)
            {
                if (Delete)
                {
                    appDbContext.Todo.Remove(todo);
                }
                else
                {
                    todo.title = title;
                    todo.status = status;
                }
            }
        }

        appDbContext.SaveChanges();
        
        return RedirectToPage("ToDo");
    }
    [HttpPost]
    public IActionResult OnPostUpdateTodoStatus([FromForm] int id, [FromForm] int status)
    {
        var token = Request.Cookies["LoginProject2AppSessionToken"];
        var session = appDbContext.Session
            .Include(session => session.User)
            .SingleOrDefault(session => session.token == token);

        if (session == null)
        {
            return new JsonResult(new { success = false, message = "Unauthorized" }) { StatusCode = 401 };
        }

        var todoItem = appDbContext.Todo.FirstOrDefault(t => t.id == id && t.userId == session.User.id);
        if (todoItem != null)
        {
            todoItem.status = status;
            appDbContext.SaveChanges();
            return new JsonResult(new { success = true });
        }

        return new JsonResult(new { success = false, message = "Item not found" }) { StatusCode = 404 };
    }


    public IActionResult OnGetLogout()
    {
        Response.Cookies.Delete("LoginProject2AppSessionToken");
        return RedirectToPage("Index");
    }
}