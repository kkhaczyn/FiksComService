using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost("/Account/SignUp")]
        public async Task<IActionResult> SignUp(SignUpRequest signUpRequest)
        {
            try
            {
                User foundUserByName = await userManager.FindByNameAsync(signUpRequest.UserName);
                User foundUserByEmail = await userManager.FindByEmailAsync(signUpRequest.Email);

                if (foundUserByName?.UserName?.ToUpper() == signUpRequest.UserName.ToUpper())
                {
                    var sameLogin = "Ten login zosta� ju� wybrany. Wybierz inny od " + signUpRequest.UserName + ".";
                    return BadRequest(sameLogin);
                }
                else if (foundUserByEmail?.Email == signUpRequest.Email)
                {
                    var sameEmail = "Ten email zosta� ju� wybrany. Wybierz inny od " + signUpRequest.Email + ".";
                    return BadRequest(sameEmail);
                }
                else if (foundUserByEmail?.Email == signUpRequest.Email && foundUserByName?.UserName == signUpRequest.UserName)
                {
                    var sameEmailAndLogin = "Ten email oraz login zosta� ju� wybrany. Spr�buj ponownie.";
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
                        return Ok("Zarejestrowano si� pomy�lnie");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while adding new user.");
            }

            return BadRequest("Co� posz�o nie tak... Spr�buj ponownie p�niej.");
        }

        [HttpPost("/Account/SignIn")]
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
                        return Ok("Zalogowano si� jako klient");
                    }
                    else if (await userManager.IsInRoleAsync(appUser, "Administrator"))
                    {
                        return Ok("Zalogowano si� jako administrator.");
                    }
                }
                else
                {
                    return BadRequest("Nie mo�na odnale�� u�ytkownika.");
                }
            }

            return BadRequest("B��dne dane logowania. Spr�buj ponownie.");
        }

        [HttpPost("/Account/SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return Ok("Pomy�lne wylogowanie.");
        }
    }
}
