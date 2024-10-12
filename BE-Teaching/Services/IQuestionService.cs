using BE_Teaching.Models;

namespace BE_Teaching.Services
{
    public interface IQuestionService
    {
        Response<List<Question>> GetAllQuestions();

        Response<string> AddQuestion(Question newQuestion);

        Response<string> EditQuestion(int id, Question editQuestion)
            ;
        Response<string> DeleteQuestion(int id);
    }
}
