using Application.Dtos.Request;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Implementation
{
    public class AdminService
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task BlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден.");

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue; // Блокировка навсегда

            await _userManager.UpdateAsync(user);
        }

        public async Task UnblockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден.");

            user.LockoutEnd = null; // Снимаем блокировку

            await _userManager.UpdateAsync(user);
        }

        public async Task<Operator> CreateOperatorAsync(OperatorDto newOperator, string password)
        {
            try
            {
                var operatorEntity = new Operator
                {
                    FullName = newOperator.FullName, // Добавлен маппинг FullName
                    Email = newOperator.Email,
                    UserName = newOperator.Username,
                    AssignedDepartment = newOperator.DepartmentOperator,
                    LockoutEnabled = true
                };
                var result = await _userManager.CreateAsync(operatorEntity, password);
                if (!result.Succeeded)
                    throw new InvalidOperationException("Ошибка при создании оператора.");

                await _userManager.AddToRoleAsync(operatorEntity, "Operator");
                return operatorEntity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Ошибка сервера,разработчик безрукий,извините");
            }
        }

    }
}