using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiksComService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<Role> roleManager,
    ILogger<AccountController> logger)
    : ControllerBase
    {
        [HttpPost("/api/Account/SignUp")]
        public async Task<IActionResult> SignUp(SignUpRequest signUpRequest)
        {
            try
            {
                User foundUserByName = await userManager.FindByNameAsync(signUpRequest.UserName);
                User foundUserByEmail = await userManager.FindByEmailAsync(signUpRequest.Email);

                if (foundUserByName?.UserName?.ToUpper() == signUpRequest.UserName.ToUpper())
                {
                    var sameLogin = "Ten login zosta³ ju¿ wybrany. Wybierz inny od " + signUpRequest.UserName + ".";
                    return BadRequest(sameLogin);
                }
                else if (foundUserByEmail?.Email == signUpRequest.Email)
                {
                    var sameEmail = "Ten email zosta³ ju¿ wybrany. Wybierz inny od " + signUpRequest.Email + ".";
                    return BadRequest(sameEmail);
                }
                else if (foundUserByEmail?.Email == signUpRequest.Email && foundUserByName?.UserName == signUpRequest.UserName)
                {
                    var sameEmailAndLogin = "Ten email oraz login zosta³ ju¿ wybrany. Spróbuj ponownie.";
                    return BadRequest(sameEmailAndLogin);
                }
                else
                {
                    User user = new()
                    {
                        UserName = signUpRequest.UserName,
                        Email = signUpRequest.Email,
                        LockoutEnabled = false,
                        PhoneNumber = signUpRequest.PhoneNumber,
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        NormalizedEmail = signUpRequest.Email.ToUpper(),
                        NormalizedUserName = signUpRequest.UserName.ToUpper(),
                        EmailConfirmed = true
                    };

                    IdentityResult userManagerResult = await userManager.CreateAsync(user, signUpRequest.Password);

                    if (userManagerResult.Succeeded)
                    {
                        var addedUser = await userManager.FindByNameAsync(user.UserName);
                        var role = await roleManager.FindByNameAsync("Client");
                        IdentityResult roleResult = await userManager.AddToRoleAsync(addedUser, role.Name);

                        await userManager.UpdateAsync(addedUser);
                        await roleManager.UpdateAsync(role);

                        logger.LogInformation("New user: {}.", addedUser.UserName);
                        return Ok("Zarejestrowano siê pomyœlnie");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while adding new user.");
            }

            return BadRequest("Coœ posz³o nie tak... Spróbuj ponownie póŸniej.");
        }

        [HttpPost("/api/Account/SignIn")]
        public async Task<IActionResult> SignIn(SignInRequest signInRequest)
        {
            var result = await signInManager.PasswordSignInAsync(signInRequest.UserName, signInRequest.Password, false, false);

            if (result.Succeeded)
            {
                User appUser = await userManager.FindByNameAsync(signInRequest.UserName);

                if (appUser != null)
                {
                    if (await userManager.IsInRoleAsync(appUser, "Client"))
                    {
                        return Ok(new 
                        { 
                            message = "Zalogowano siê jako klient.",
                            userName = appUser.UserName, 
                            cookie = HttpContext.Response.Headers.SetCookie 
                        });
                    }
                    else if (await userManager.IsInRoleAsync(appUser, "Administrator"))
                    {
                        return Ok(new 
                        { 
                            message = "Zalogowano siê jako administrator.",
                            userName = appUser.UserName,
                            cookie = HttpContext.Response.Headers.SetCookie 
                        });
                    }
                }
                else
                {
                    return BadRequest("Nie mo¿na odnaleŸæ u¿ytkownika.");
                }
            }

            return BadRequest("B³êdne dane logowania. Spróbuj ponownie.");
        }

        [HttpPost("/api/Account/SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return Ok("Pomyœlne wylogowanie.");
        }

        //http://localhost:5046/api/account/EditUser
        [Authorize(Roles = "Administrator, Client")]
        [HttpPost("/api/Account/EditUser")]
        public async Task<IActionResult> EditUser(User user)
        {
            if (user.Id < 1)
                return BadRequest("Niepoprawany identyfikator u¿ytkownika.");

            User userFound = await userManager.FindByIdAsync(user.Id.ToString());

            if (userFound != null)
            {
                userFound.PhoneNumber = user.PhoneNumber ?? userFound.PhoneNumber;
                userFound.UserName = user.UserName ?? userFound.UserName;
                userFound.Email = user.Email ?? userFound.Email;
                userFound.NormalizedEmail = user.Email?.ToUpper() ?? userFound.NormalizedEmail;
                userFound.NormalizedUserName = user.UserName?.ToUpper() ?? userFound.NormalizedUserName;

                var result = await userManager.UpdateAsync(userFound);

                if (result.Succeeded)
                {
                    return Ok("Poprawnie edytowano konto.");
                }

                return BadRequest("Coœ posz³o nie tak. Spróbuj ponownie póŸniej.");
            }

            return BadRequest("Nie mo¿na odnaleŸæ u¿ytkownika.");
        }

        //http://localhost:5046/api/account/ChangePassword
        [Authorize(Roles = "Administrator, Client")]
        [HttpPost("/api/Account/ChangePassword")]
        public async Task<IActionResult> ChangePassword(EditPasswordRequest editPasswordRequest)
        {
            User userFound = await userManager.FindByIdAsync(editPasswordRequest.UserId.ToString());

            if (userFound != null)
            {
                var result = await userManager.ChangePasswordAsync(userFound, editPasswordRequest.CurrentPassword, editPasswordRequest.NewPassword);
                
                if (result.Succeeded)
                {
                    return Ok("Poprawnie zmieniono has³o.");
                }

                return BadRequest("Coœ posz³o nie tak. Spróbuj ponownie póŸniej.");
            }

            return BadRequest("Nie mo¿na odnaleŸæ u¿ytkownika.");
        }

        //http://localhost:5046/api/account/GetUsers
        [Authorize(Roles = "Administrator")]
        [HttpGet("/api/Account/GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var listOfUsers = await userManager
                .Users
                .Select(x => new { x.UserName, x.Email, x.PhoneNumber, x.Id})
                .ToListAsync();

            return Ok(listOfUsers);
        }
    }
}
