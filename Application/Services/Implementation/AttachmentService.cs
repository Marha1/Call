using Application.Services.Interfaces;
using Domain.Models;
using Infrastructure.Repository.Implementation;
using System.IO;

namespace Application.Services.Implementation
{
    public class AttachmentService : IAttachmentService
    {
        private readonly AttachmentRepository _attachmentRepository;

        public AttachmentService(AttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        public async Task<AttachmentUser> UploadAttachmentAsync(IFormFile file, Guid requestId, CancellationToken cancellationToken = default)
        {
            if (file is null || file.Length == 0) throw new ArgumentException("File is null or empty.");

            // Корневая папка проекта
            var rootPath = Directory.GetCurrentDirectory();

            // Путь для сохранения файлов
            var uploadFolder = Path.Combine(rootPath, "wwwroot", "uploads");
            Directory.CreateDirectory(uploadFolder); // Создаем директорию, если ее нет

            // Уникальное имя файла
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            // Полный путь к файлу
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            // Относительный путь для хранения в базе данных
            var relativeFilePath = Path.Combine("uploads", uniqueFileName);

            // Создаем запись о вложении
            var attachment = new AttachmentUser
            {
                FileName = file.FileName,
                FilePath = relativeFilePath,
                RequestId = requestId
            };

            await _attachmentRepository.AddAsync(attachment, cancellationToken);
            await _attachmentRepository.SaveChangesAsync(cancellationToken);

            return attachment;
        }
    }
}