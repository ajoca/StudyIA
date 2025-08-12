using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;
using study_ia_web_app.Repositories.archivo;
using study_ia_web_app.Repositories.quiz;

namespace study_ia_web_app.Services.quiz
{
    public class QuizService : IQuizService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IAiQuizGenerator _generator;
        private readonly IQuizBuilderService _quizBuilderService;
        private readonly IQuizRepository _quizRepository;

        public QuizService(IFileRepository fileRepository, 
            IAiQuizGenerator aiQuizGenerator,
            IQuizBuilderService quizBuilderService,
            IQuizRepository quizRepository)
        {
            _fileRepository = fileRepository;
            _generator = aiQuizGenerator;
            _quizBuilderService = quizBuilderService;
            _quizRepository = quizRepository;
        }

        public async Task<Quiz> GenerarQuizAsync(Guid archivoId, int cantidadPreguntas)
        {
            var archivo = await _fileRepository.GetFileByIdAsync(archivoId);

            var texto = archivo?.TextoExtraido;

            if (texto == null)
            {
                throw new ArgumentNullException(nameof(texto));
            }

            var preguntas = await _generator.GenerarPreguntasAsync(texto, cantidadPreguntas);

            var quiz = _quizBuilderService.ConstruirQuiz(archivoId, preguntas);

            await _quizRepository.AddAsync(quiz);

            return quiz;
            
        }

        public async Task<QuizDetalleDTO?> ObtenerQuizPorArchivoIdAsync(Guid archivoId)
        {
            var quiz = await _quizRepository.ObtenerQuizPorArchivoIdAsync(archivoId);

            if (quiz == null) return null;

            return new QuizDetalleDTO
            {
                QuizId = quiz.QuizId,
                ArchivoId = quiz.ArchivoId,
                NombreArchivo = quiz.Archivo?.NombreArchivo ?? "",
                Titulo = quiz.Titulo,
                FechaGenerado = quiz.FechaGenerado,
                Preguntas = quiz.Preguntas.Select(p => new PreguntaDto
                {
                    PreguntaId = p.PreguntaId,
                    Enunciado = p.Enunciado,
                    Alternativas = p.Alternativas.Select(a => new AlternativaDto
                    {
                        AlternativaId = a.AlternativaId,
                        Texto = a.Texto,
                        EsCorrecta = a.EsCorrecta,
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<List<QuizListDto>> ObtenerTodosQuizzesAsync()
        {
            var quizzes = await _quizRepository.GetAllWithArchivoAsync();

            return quizzes.Select(q => new QuizListDto
            {
                QuizId = q.QuizId,
                ArchivoId = q.ArchivoId,
                NombreArchivo = q.Archivo.NombreArchivo ?? "Desconocido",
                Titulo = q.Titulo,
                FechaGenerado = q.FechaGenerado,
                CantidadPreguntas = q.Preguntas?.Count ?? 0
            }).ToList();
        }
    }
}
