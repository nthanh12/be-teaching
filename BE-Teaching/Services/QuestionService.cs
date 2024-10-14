using BE_Teaching.Models;
using System.Data.SqlClient;
using System.Data;

namespace BE_Teaching.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly string _connectionString;

        public QuestionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Teaching");
        }

        public Response<string> AddQuestion(Question newQuestion)
        {
            Response<string> response = new Response<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_AddQuestion", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@QuestionText", newQuestion.QuestionText);

                    conn.Open();

                    int rs = cmd.ExecuteNonQuery();

                    if (rs > 0)
                    {
                        response.StatusCode = 201;
                        response.StatusMessage = "Question added successfully!";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Failed to add question!";
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        public Response<string> DeleteQuestion(int id)
        {
            Response<string> response = new Response<string>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Bắt đầu transaction
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        SqlCommand checkCmd = new SqlCommand("sp_CheckQuestion", conn, transaction)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        checkCmd.Parameters.AddWithValue("@QuestionID", id);

                        // Không cần kiểm tra null, trực tiếp ép kiểu
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 0)
                        {
                            response.StatusCode = 404;
                            response.StatusMessage = "Question not found!";
                            return response;
                        }

                        // Xóa các bản ghi trong bảng Answer trước
                        SqlCommand cmdA = new SqlCommand("sp_DeleteAnswerFromQuestion", conn, transaction)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmdA.Parameters.AddWithValue("@QuestionID", id);

                        int rowQ = cmdA.ExecuteNonQuery();

                        if (rowQ < 0)
                        {
                            response.StatusCode = 400;
                            response.StatusMessage = "Failed to delete answers associated with the question!";
                            return response; // Kết thúc sớm nếu không xóa được answers
                        }

                        // Sau đó xóa bản ghi trong bảng Question
                        SqlCommand cmd = new SqlCommand("sp_DeleteQuestion", conn, transaction)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Parameters.AddWithValue("@QuestionID", id);

                        int row = cmd.ExecuteNonQuery();

                        if (row > 0)
                        {
                            // Commit transaction nếu mọi thứ thành công
                            transaction.Commit();
                            response.StatusCode = 200;
                            response.StatusMessage = "Question and associated answers deleted successfully!";
                        }
                        else
                        {
                            response.StatusCode = 400;
                            response.StatusMessage = "Failed to delete question!";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Nếu có lỗi, rollback transaction
                        transaction.Rollback();
                        response.StatusCode = 500;
                        response.StatusMessage = $"An error occurred: {ex.Message}";
                    }
                }
            }

            return response;
        }

        public Response<string> EditQuestion(int id, Question editQuestion)
        {
            Response<string> response = new Response<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand checkCmd = new SqlCommand("sp_CheckQuestion", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    checkCmd.Parameters.AddWithValue("@QuestionID", id);

                    // Không cần kiểm tra null, trực tiếp ép kiểu
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        response.StatusCode = 404;
                        response.StatusMessage = "Question not found!";
                        return response;
                    }

                    // Chỉnh sửa Exam
                    SqlCommand cmd = new SqlCommand("sp_EditQuestion", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@QuestionID", id);
                    cmd.Parameters.AddWithValue("@QuestionText", editQuestion.QuestionText);

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Question edited successfully!";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Failed to edit question!";
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        public Response<List<Question>> GetAllQuestions()
        {
            var questions = new List<Question>();
            Response<List<Question>> response = new Response<List<Question>>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ShowQuestion", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new Question
                            {
                                QuestionID = (int)reader["QuestionID"],
                                QuestionText = reader["QuestionText"].ToString()
                            });
                        }
                    }

                    if (questions.Count > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Questions retrieved successfully!";
                        response.Data = questions;
                    }
                    else
                    {
                        response.StatusCode = 204;
                        response.StatusMessage = "No question found!";
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"An error occurred: {ex.Message}";
            }

            return response;
        }
    }
}
