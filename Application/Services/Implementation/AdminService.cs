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

        public async Task<Operator> CreateOperatorAsync(Operator newOperator, string password)
        {
            newOperator.LockoutEnabled = true; // Разрешаем блокировку
            var result = await _userManager.CreateAsync(newOperator, password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Ошибка при создании оператора.");

            await _userManager.AddToRoleAsync(newOperator, "Operator");
            return newOperator;
        }
    }
}