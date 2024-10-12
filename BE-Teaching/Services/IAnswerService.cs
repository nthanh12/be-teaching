using BE_Teaching.Models;

namespace BE_Teaching.Services
{
    public interface IAnswerService
    {
        Response<List<Answer>> GetAllAnswers();

        Response<string> AddAnswer(Answer newAnswer);
        Response<string> EditAnswer(int id, Answer editAnswer);
        Response<string> DeleteAnswer(int id);
    }
}
