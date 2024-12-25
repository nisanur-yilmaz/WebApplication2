using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Diagnostics.SymbolStore;
using WebApplication2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace WebApplication2.Pages;

public class SıgnUp : PageModel
{
    public IActionResult OnGet()
    {
        var token = Request.Cookies["LoginProject2AppSessionToken"];
        var result2 =
            appDbContext.Session.Where(session => session.token == token).ToList();
        if (result2.Count > 0)
        {
            return RedirectToPage("ToDo");
        }

        return null;
    }

    public string? Name => (string?)TempData[nameof(Name)];
    public string? Password => (string?)TempData[nameof(Password)];
    public string? PasswordAgain => (string?)TempData[nameof(PasswordAgain)];
    public string? Gender => (string?)TempData[nameof(Gender)];
    private readonly AppDbContext appDbContext;

    public SıgnUp()
    {
        appDbContext = new AppDbContext();
    }

    public RedirectToPageResult OnPost([FromForm] string name, [FromForm] string password,
        [FromForm] string passwordAgain, [FromForm] string gender)
    {
        var token = Request.Cookies["LoginProject2AppSessionToken"];
        var result2 =
            appDbContext.Session.Where(session => session.token == token).ToList();
        if (result2.Count > 0)
        {
            return RedirectToPage("ToDo");
        }

        TempData["Name"] = name;
        TempData["Password"] = password;
        TempData["PasswordAgain"] = passwordAgain;
        TempData["Gender"] = gender;
        var problemYok = true;
        if (string.IsNullOrEmpty(name))
        {
            TempData["Name.Error"] = "Name cannot be empty";
            problemYok = false;
        }

        if (string.IsNullOrEmpty(password))
        {
            TempData["Password.Error"] = "Password cannot be empty";
            problemYok = false;
        }
        else
        {
            var problemsiz = Helpers.IsPasswordLengthValid(password);
            if (!problemsiz)
            {
                TempData["Password.Error"] = "Password length should not be less than 6 digits";
                problemYok = false;
            }
            else
            {
                var sifreProblemsiz = Helpers.IsPasswordValid(password);
                if (!sifreProblemsiz)
                {
                    TempData["Password.Error"] =
                        "The password must contain at least 1 number, 1 lowercase letter, 1 uppercase letter and 1 symbol";
                    problemYok = false;
                }
            }
        }

        if (string.IsNullOrEmpty(passwordAgain))
        {
            TempData["PasswordAgain.Error"] = "Password Again  cannot be empty";
            problemYok = false;
        }

        if (string.IsNullOrEmpty(gender))
        {
            TempData["Gender.Error"] = "  Gender  cannot be empty";
            problemYok = false;
        }

        if (password != passwordAgain)
        {
            TempData["Stop"] = "password does not match";
            problemYok = false;
        }

        if (problemYok)
        {
            var kullanıcıvar = false;
            var result =
                appDbContext.User.ToList();

            foreach (var row in result)
            {
                if (name == row.userName)
                {
                    kullanıcıvar = true;
                    break;
                }
            }

            if (kullanıcıvar)
            {
                TempData["UserName.Error"] = $"there is already a user {name}";
            }
            else
            {
                TempData["Stop"] = "";
                var user1 = new User();

                user1.userName = name;
                user1.password = password;

                if (gender == "1")
                {
                    user1.GenderId = 1;
                }
                else
                {
                    user1.GenderId = 2;
                }

                appDbContext.User.Add(user1);
                appDbContext.SaveChanges();

                var user = appDbContext.User.SingleOrDefault(user => user.userName == name);
                var token1 = Helpers.CreateToken();
                var session = new Session();

                session.userId = user.id;
                session.token = token1;

                appDbContext.Session.Add(session);
                appDbContext.SaveChanges();
                Response.Cookies.Append("LoginProject2AppSessionToken", token1);
                return RedirectToPage("ToDo");
            }
        }


        return RedirectToPage("SıgnUp");
        }
    }