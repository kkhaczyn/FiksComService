using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Data;

namespace FiksComService.Controllers
{
    [EnableCors("default")]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<Role> roleManager,
    ILogger<AccountController> logger,
    IOrderDetailRepository orderDetailRepository)
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

        [HttpPost("/api/Account/SignIn")]
        public async Task<IActionResult> SignIn(SignInRequest signInRequest)
        {
            var result = await signInManager.PasswordSignInAsync(signInRequest.UserName, signInRequest.Password, false, false);

            if (result.Succeeded)
            {
                User appUser = await userManager.FindByNameAsync(signInRequest.UserName);

                if (appUser != null)
                {
                    if (appUser.LockoutEnabled)
                    {
                        await signInManager.SignOutAsync();
                        return StatusCode(StatusCodes.Status403Forbidden);
                    }

                    if (await userManager.IsInRoleAsync(appUser, "Client"))
                    {
                        return Ok(new 
                        { 
                            message = "Zalogowano si� jako klient.",
                            userName = appUser.UserName, 
                            cookie = HttpContext.Response.Headers.SetCookie 
                        });
                    }
                    else if (await userManager.IsInRoleAsync(appUser, "Administrator"))
                    {
                        return Ok(new 
                        { 
                            message = "Zalogowano si� jako administrator.",
                            userName = appUser.UserName,
                            cookie = HttpContext.Response.Headers.SetCookie 
                        });
                    }
                }
                else
                {
                    return BadRequest("Nie mo�na odnale�� u�ytkownika.");
                }
            }

            return BadRequest("B��dne dane logowania. Spr�buj ponownie.");
        }

        [HttpPost("/api/Account/SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return Ok("Pomy�lne wylogowanie.");
        }

        //http://localhost:5046/api/account/EditUser
        [Authorize(Roles = "Administrator, Client")]
        [HttpPost("/api/Account/EditUser")]
        public async Task<IActionResult> EditUser(User user)
        {
            if (user.Id < 1)
                return BadRequest("Niepoprawany identyfikator u�ytkownika.");

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

                return BadRequest("Co� posz�o nie tak. Spr�buj ponownie p�niej.");
            }

            return BadRequest("Nie mo�na odnale�� u�ytkownika.");
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
                    return Ok("Poprawnie zmieniono has�o.");
                }

                return BadRequest("Co� posz�o nie tak. Spr�buj ponownie p�niej.");
            }

            return BadRequest("Nie mo�na odnale�� u�ytkownika.");
        }

        //http://localhost:5046/api/account/GetUsers
        [Authorize(Roles = "Administrator")]
        [HttpGet("/api/Account/GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var listOfUsers = await userManager.Users
                .Include(x => x.Orders)
                .ToListAsync();

            foreach (var user in listOfUsers)
            {
                foreach (var order in user.Orders)
                {
                    order.OrderDetails = orderDetailRepository.GetOrderDetailsByOrderId(order.OrderId);
                }
            }

            var usersWithOrdersAndDetails = listOfUsers.Select(user => new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Id,
                user.LockoutEnabled,
                Orders = user.Orders.Select(order => new
                { 
                    order.OrderId,
                    order.OrderDate,
                    order.Status,
                    order.TotalPrice,
                    OrdersDetails = order.OrderDetails.Select(orderDetail => new 
                    {
                        orderDetail.OrderDetailId,
                        orderDetail.Component,
                        orderDetail.Quantity,
                        orderDetail.PricePerUnit
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(usersWithOrdersAndDetails);
        }

        //http://localhost:5046/api/account/GetLoggedUser
        [Authorize(Roles = "Administrator, Client")]
        [HttpGet("/api/Account/GetLoggedUser")]
        public async Task<IActionResult> GetLoggedUser()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { user?.UserName, user?.Email, user?.PhoneNumber, user?.Id, role });
        }

        //http://localhost:5000/api/account/lockoutuser?id=2
        [Authorize(Roles = "Administrator")]
        [HttpPost("/api/Account/LockoutUser")]
        public async Task<IActionResult> LockoutUser(int id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                if (user.LockoutEnabled)
                {
                    await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddMinutes(-1));
                    await userManager.SetLockoutEnabledAsync(user, false);

                }
                else
                {
                    await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(10));
                    await userManager.SetLockoutEnabledAsync(user, true);
                }
                //await userManager.SetLockoutEnabledAsync(user, !user.LockoutEnabled);
                return Ok("Zastosowano zmiany.");
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
