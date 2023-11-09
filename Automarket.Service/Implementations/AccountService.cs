using Automarket.DataAccessLayer.Inerfaces;
using Automarket.Domain.Entity;
using Automarket.Domain.Enum;
using Automarket.Domain.Helpers;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Account;
using Automarket.Service.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Automarket.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IBaseRepository<Profile> _profileRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IBaseRepository<Profile> profileRepository, IBaseRepository<User> userRepository, ILogger<AccountService> logger)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<BaseResponse<string>> SendConfirmationCode(string email)
        {
            try
            {
                Random rand = new Random();
                string code = rand.Next(100000, 1000000).ToString();
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Automarket", "prostops4@yandex.ru"));
                message.To.Add(new MailboxAddress("yandex", email));
                message.Subject = "Registration Confirmation";
                message.Body = new TextPart("plain")
                {
                    Text = $"Your confirmation code: {code}"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.yandex.ru", 587);
                    client.Authenticate("prostops4@yandex.ru", "a2ug1jk78!");
                    client.Send(message);
                    client.Disconnect(true);
                }
                return new BaseResponse<string>()
                {
                    Data = code,
                    StatusCode = StatusCode.Ok,
                    Description = "Код отправлен"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[SendConfirmationCode]: {ex.Message}");
                return new BaseResponse<string>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
            
        }

        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model)
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == model.Name);
            try
            {
                if (user != null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь с таким логином уже есть"
                    };
                }


                if (model.SendedCode != model.VerifyCode)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный код подтверждения"
                    };
                }

                user = new User()
                {
                    Name = model.Name,
                    Role = Domain.Enum.Role.User,
                    Password = HashPasswordHelper.HashPassowrd(model.Password)
                };

                var profile = new Profile()
                {
                    UserId = user.Id,
                    User = user
                };

                await _userRepository.Create(user);
                await _profileRepository.Create(profile);
                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    Description = "Объект добавился",
                    StatusCode = Domain.Enum.StatusCode.Ok
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Register]: {ex.Message}");
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
        {
            try
            {
                var user = await _userRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Name == model.Name);
                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден"
                    };
                }

                if (user.Password != HashPasswordHelper.HashPassowrd(model.Password))
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный пароль"
                    };
                }

                var result = Authenticate(user);
                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    StatusCode = Domain.Enum.StatusCode.Ok
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Login]: {ex.Message}");
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
            
        }

        public async Task<BaseResponse<bool>> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == model.UserName);
                if (user == null)
                {
                    return new BaseResponse<bool>()
                    {
                        StatusCode = StatusCode.UserNotFound,
                        Description = "Пользователь не найден"
                    };
                }

                user.Password = HashPasswordHelper.HashPassowrd(model.NewPassword);
                await _userRepository.Update(user);

                return new BaseResponse<bool>()
                {
                    Data = true,
                    StatusCode = StatusCode.Ok,
                    Description = "Пароль обновлен"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[ChangePassword]: {ex.Message}");
                return new BaseResponse<bool>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            return new ClaimsIdentity(claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        
    }
}
