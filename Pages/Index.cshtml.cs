using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Services;

namespace WebApplication2.Pages;

public class IndexModel : PageModel
{
    public string? Name => (string?)TempData[nameof(Name)];
    public string? Password => (string?)TempData[nameof(Password)];


    private readonly AppDbContext appDbContext;

    public IndexModel()
    {
        appDbContext = new AppDbContext();
    }

    public IActionResult OnGet()
    {
        var token = Request.Cookies["LoginProject2AppSessionToken"];
        var result2 = appDbContext.Session.Where(session => session.token == token).ToList();
        if (result2.Count > 0)
        {
            return RedirectToPage("ToDo");
        }

        return null;
    }

    public IActionResult OnPost([FromForm] string Name, [FromForm] string password)
    {
        if (string.IsNullOrEmpty(Name))
        {
            TempData["Name.Error"] = " Name cannot be empty";
        }

        if (string.IsNullOrEmpty(password))
        {
            TempData["Password.Error"] = "Password cannot be empty";
        }

        var result = appDbContext.User.ToList();
        foreach (var row in result)
        {
            if (Name == row.userName && password == row.password)
            {
                TempData["Stop"] = "";
                var userId = row.id;
                var token = Helpers.CreateToken();
                var session = new Session();

                session.userId = userId;
                session.token = token;

                appDbContext.Session.Add(session);
                appDbContext.SaveChanges();
                Response.Cookies.Append("LoginProject2AppSessionToken", token);
                return RedirectToPage("ToDo");
            }
            else
            {
                TempData["Stop"] = "username and password are incorrect";
            }
        }


        return RedirectToPage("Index");
    }
}