
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.Extensions.Options;
using study_ia_web_app.Utils;
using System.Text;

namespace study_ia_web_app.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly string _folderId;

        public GoogleDriveService(IOptions<GoogleDriveSettings> options)
        {
            var settings = options.Value;

            _folderId = settings.FolderId;

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(settings.CredentialsBase64));

            var credential = GoogleCredential
                .FromStream(new MemoryStream(Encoding.UTF8.GetBytes(json)))
                .CreateScoped(DriveService.Scope.DriveFile);

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "IA PDF APP"
            });
        }

        public async Task<string> UploadPdfSync(Stream archivoStream, string nombreArchivo)
        {
            var fileMetada = new Google.Apis.Drive.v3.Data.File
            {
                Name = nombreArchivo,
                MimeType = "application/pdf",
                Parents = string.IsNullOrEmpty(_folderId) ? null : new List<string> { _folderId }
            };

            var request = _driveService.Files.Create(fileMetada, archivoStream, "application/pdf");
            request.Fields = "id";

            var result = await request.UploadAsync();

            if (result.Status == UploadStatus.Completed)
                return request.ResponseBody.Id;

            throw new Exception($"Error al subir a Drive: {result.Exception?.Message}");
        }
    }
}
