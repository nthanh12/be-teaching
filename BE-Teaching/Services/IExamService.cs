using BE_Teaching.Models;

namespace BE_Teaching.Services
{
    public interface IExamService
    {
        Response<List<Exam>> GetAllExams();

        Response<string> AddExam(Exam newExam);
        Response<string> EditExam(int id, Exam editExam);
        Response<string> DeleteExam(int id);
    }
}
