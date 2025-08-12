using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;
using study_ia_web_app.Repositories.archivo;
using study_ia_web_app.Repositories.resumen;
using study_ia_web_app.Services;
using study_ia_web_app.Services.quiz;
using study_ia_web_app.Services.studio;

namespace study_ia_web_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivoController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IFileRepository _fileRepository;
        private readonly IAiService _aiService;
        private readonly IResumenRepository _resumenRepository;
        private readonly IQuizService _quizService;
        private readonly IPlanEstudioService _planEstudioService;

        private readonly GoogleDriveService _googleDriveService;

        public ArchivoController(IFileService fileService, 
            GoogleDriveService googleDriveService, 
            IFileRepository fileRepository, 
            IAiService aiService, IResumenRepository resumenRepository,
            IQuizService quizService,
            IPlanEstudioService planEstudioService)
        {
            _fileService = fileService;
            _googleDriveService = googleDriveService;
            _fileRepository = fileRepository;
            _aiService = aiService;
            _resumenRepository = resumenRepository;
            _quizService = quizService;
            _planEstudioService = planEstudioService;
        }

        [HttpPost("generate-plan-studio-ia")]
        public async Task<IActionResult> GenerarPlan([FromBody] GenerarPlanIARequest request) 
        {
            try
            {
                var plan = await _planEstudioService.GenerarPlanIaAsync(
                request.ArchivoId, request.FechaInicio, request.FechaFin, 
                request.HorasPorDia, request.Nivel, request.TiposDeTarea);

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("list-quizzes")]
        public async Task<IActionResult> ListarQuizzes()
        {
            var quizzes = await _quizService.ObtenerTodosQuizzesAsync();
            return Ok(quizzes);
        }

        [HttpGet("get-quiz-by-file/{fileId}")]
        public async Task<IActionResult> GetQuizByFile(Guid fileId)
        {
            var quiz = await _quizService.ObtenerQuizPorArchivoIdAsync(fileId);

            if (quiz == null)
            {
                return NotFound(new { message = $"Not found quiz by fileId: {fileId} " });
            }

            return Ok(quiz);
        }

        [HttpPost("generate-quiz/{fileId}")]
        public async Task<IActionResult> GenerateQuiz(Guid fileId, [FromQuery] int cantidad = 5)
        {
            try
            {
                var quiz = await _quizService.GenerarQuizAsync(fileId, cantidad);

                return Ok(quiz.ArchivoId);

            } catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("archivo-upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {

                if (file == null || file.ContentType != "application/pdf")
                    return BadRequest("Archivo inválido. Solo se aceptan PDFs.");


                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var fileId = await _googleDriveService.UploadPdfSync(memoryStream, file.FileName);

                var texto = await _fileService.ExtraerTextoAsync(file);

                var descripcion = await _aiService.ArchivoGetDescription(texto);

                var fileEntity = new Archivo
                {
                    ArchivoId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    NombreArchivo = file.FileName,
                    RutaArchivo = fileId,
                    TextoExtraido = texto,
                    Descripcion = descripcion.Descripcion,
                    FechaSubida = DateTime.UtcNow,
                    Resumenes = new List<Resumen>()
                };

                await _fileRepository.GuardarAsync(fileEntity);

                return Ok(new
                {
                    fileId = fileEntity.ArchivoId,
                    fileDrive  = fileId,
                    filetexto = texto
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en el controlador: " + ex.Message);
                return StatusCode(500, new
                {
                    mensaje = "Error al subir el archivo.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListFiles()
        {
            var files = await _fileService.getAllFilesAsync();
            return Ok(files);
        }

        [HttpGet("get/{fileId}")]
        public async Task<IActionResult> getFileById(Guid fileId)
        {
            var file = await _fileService.getFileById(fileId);

            if (file == null)
            {
                return NotFound(new { message = $"File not found by ID: {fileId} "});
            }

            return Ok(file);
        }

        [HttpGet("resumen-list/{fileId}")]
        public async Task<IActionResult> ResumenFile(Guid fileId)
        {
            var file = await _fileService.getResumenById(fileId);

            if (file == null)
            {
                return NotFound(new { message = $"Resumen not found by ID: {fileId} " });
            }

            return Ok(file);
        }

        [HttpPost("generar-resumen/{fileId}")]
        public async Task<IActionResult> GenerarResumen(Guid fileId)
        {
            try
            {
                var file = await _fileRepository.GetFileByIdAsync(fileId);

                if (file == null)
                {
                    return NotFound(new { message = $"File not found by ID: {fileId} " });
                }

                var summaryFile = await _aiService.GenerarResumenConGeminiAsync(file.TextoExtraido);


                var newResumen = new Resumen
                {
                    ResumenId = Guid.NewGuid(),
                    ArchivoId = fileId,
                    Introduccion = summaryFile.Introduccion,
                    Conclusion = summaryFile.Conclusion,
                    isResumen = true,
                    FechaGenerado = DateTime.UtcNow,
                    PuntosClave = summaryFile.PuntosClave.Select(p => new PuntoClave
                    {
                        PuntoClaveId = Guid.NewGuid(),
                        Texto = p
                    }).ToList(),
                };

                await _resumenRepository.AgregarResumenAsync(newResumen);

                return Ok(new { newResumen.ArchivoId });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        mensaje = "Error al generar resumen",
                        error = ex.Message
                    });
            }
        }

        [HttpPost("descripcion/{fileId}")]
        public async Task<IActionResult> DescripcionTexto(Guid fileId)
        {
            try
            {
                var file = await _fileRepository.GetFileByIdAsync(fileId);

                if (file == null)
                {
                    return NotFound(new { message = $"File not found by ID: {fileId} " });
                }

                var descriocion = await _aiService.FileGetDescription(file.TextoExtraido);

                return Ok(new { descriocion });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        mensaje = "Error al obtener descripcion texto",
                        error = ex.Message
                    });
            }
        }

        [HttpPost("generar-pregunta")]
        public async Task<IActionResult> GenerarPregunta([FromBody] GenerarPreguntaRequest request)
        {
            try
            {
                var pregunta = await _aiService.GenerarPreguntaAsync(
                    request.TipoEntrevista, request.Carrera);

                return Ok(new { pregunta });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        mensaje = "Error al generar resumen",
                        error = ex.Message
                    });
            }
        }

        // GENERAR VOZ 
        [HttpPost("pregunta-voz")]
        public async Task<IActionResult> PreguntaVoz([FromBody] GenerarPreguntaRequest request)
        {
            /*
             * {
                  "tipoEntrevista": "Tecnica",
                  "carrera": "Ingeniería de Sistemas"
                }
             */
            try
            {
                var pregunta = await _aiService.GenerarPreguntaAsync(
                    request.TipoEntrevista, request.Carrera);
                var audio = await _aiService.ConvertirTextoAVozAsync(pregunta.Pregunta);

                return File(audio, "audio/mpeg", "pregunta.mp3");
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        mensaje = "Error al generar el texto.",
                        error = ex.Message
                    });
            }
        }

        // preguntar
        [HttpPost("evaluar-feedback")]
        public async Task<IActionResult> EvaluarFeedback([FromBody] EvaluarRespuestaRequest request)
        {
            try
            {
                var feedback = await _aiService.EvaluarRespuestaAsync(
                    request.PreguntaOriginal,
                    request.RespuestaTranscrita
                );

                return Ok(new { feedback });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al evaluar la respuesta",
                    error = ex.Message
                });
            }
        }
    }
}
