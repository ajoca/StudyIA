using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Services.quiz
{
    public class QuizBuilderService : IQuizBuilderService
    {
        public Quiz ConstruirQuiz(Guid archivoId, List<PreguntaGenerada> preguntas)
        {
            return new Quiz
            {
                ArchivoId = archivoId,
                Titulo = $"Quiz generado {DateTime.Now:yyyy-MM-dd HH:mm}",
                isQuiz = true,
                FechaGenerado = DateTime.Now,
                Preguntas = preguntas.Select(p => new Pregunta
                {
                    Enunciado = p.Enunciado,
                    Alternativas = p.Alternativas.Select(a => new Alternativa
                    {
                        Texto = a,
                        EsCorrecta = a == p.RespuestaCorrecta
                    }).ToList()
                }).ToList()
            };
        }
    }
}
