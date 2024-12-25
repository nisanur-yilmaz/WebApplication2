using System.Data;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Services;

namespace WebApplication2.Pages;

public class Welcome : PageModel
{
    public string? Password => (string?)TempData[nameof(Password)];
    public string? PasswordAgain => (string?)TempData[nameof(PasswordAgain)];
    private readonly AppDbContext appDbContext;

    public Welcome()
    {
        appDbContext = new AppDbContext();
    }

    public string Name { get; set; }
    public string Gender { get; set; }

    public IActionResult OnGet()
    {
        TempData.Remove("Password.Error");
        TempData.Remove("PasswordAgain.Error");
        TempData.Remove("Stop");
        TempData.Remove("PasswordTrue");
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

        return null;
    }
    
    public IActionResult OnPost([FromForm] string password, [FromForm] string PasswordAgain)
    {
        var token = Request.Cookies["LoginProject2AppSessionToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("Index");
        }

        var sessionList = appDbContext.Session.SingleOrDefault(session => session.token == token);
        if (sessionList == null)
        {
            return RedirectToPage("Index");
        }
       

        TempData["Password"] = password;
        TempData["PasswordAgain"] = PasswordAgain;
        var problemYok = true;
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


        if (string.IsNullOrEmpty(PasswordAgain))
        {
            TempData["PasswordAgain.Error"] = "Password Again cannot be empty";
            problemYok = false;
        }

        if (password != PasswordAgain)
        {
            TempData["Stop"] = "password does not match";
            problemYok = false;
        }

        if (problemYok)
        {
            var kullanıcıYok = true;
            var Id = sessionList.userId;
            
           var update = appDbContext.User.Find(Id);
           if (update != null)
           {
               update.password = password;
               appDbContext.SaveChanges();

           }
           
            TempData["PasswordTrue"] = "your password has been changed!";
            kullanıcıYok = false;


            if (kullanıcıYok)
            {
                return RedirectToPage("Index");
            }
        }


        return RedirectToPage("Welcome");
    }

  
}