using BE_Teaching.Models;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace BE_Teaching.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly string _connectionString;

        public AnswerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Teaching");
        }
        public Response<string> AddAnswer(Answer newAnswer)
        {
            Response<string> response = new Response<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_AddAnswer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@QuestionID", newAnswer.QuestionID);
                    cmd.Parameters.AddWithValue("@AnswerText", newAnswer.AnswerText);
                    cmd.Parameters.AddWithValue("@IsCorrect", newAnswer.IsCorrect);

                    conn.Open();

                    int rs = cmd.ExecuteNonQuery();

                    if (rs > 0)
                    {
                        response.StatusCode = 201;
                        response.StatusMessage = "Answer added successfully!";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Failed to answer question!";
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

        public Response<Dictionary<int, bool>> CheckMultipleAnswers(Dictionary<int, int> questionAnswers)
        {
            Response<Dictionary<int, bool>> response = new Response<Dictionary<int, bool>>();
            response.Data = new Dictionary<int, bool>();  // Lưu kết quả từng câu hỏi

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Chuyển danh sách câu hỏi và đáp án thành JSON
                    var json = JsonConvert.SerializeObject(
                        questionAnswers.Select(qa => new { QuestionID = qa.Key, AnswerID = qa.Value })
                    );

                    SqlCommand cmd = new SqlCommand("sp_Check", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@QuestionAnswerJson", json);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int questionID = reader.GetInt32(0); 
                            bool isCorrect = reader.GetInt32(1) == 1; 

                            // Thêm kết quả vào dictionary
                            response.Data.Add(questionID, isCorrect);
                        }
                    }

                    response.StatusCode = 200;
                    response.StatusMessage = "Check completed.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        public Response<string> DeleteAnswer(int id)
        {
            Response<string> response = new Response<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand checkCmd = new SqlCommand("sp_CheckAnswer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    checkCmd.Parameters.AddWithValue("@AnswerID", id);

                    // Không cần kiểm tra null, trực tiếp ép kiểu
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        response.StatusCode = 404;
                        response.StatusMessage = "Answer not found!";
                        return response;
                    }

                    // Xoa Exam
                    SqlCommand cmd = new SqlCommand("sp_DeleteAnswer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@AnswerID", id);

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Answer deleted successfully!";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Failed to delete answer!";
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

        public Response<string> EditAnswer(int id, Answer editAnswer)
        {
            Response<string> response = new Response<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand checkCmd = new SqlCommand("sp_CheckAnswer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    checkCmd.Parameters.AddWithValue("@AnswerID", id);

                    // Không cần kiểm tra null, trực tiếp ép kiểu
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        response.StatusCode = 404;
                        response.StatusMessage = "Answer not found!";
                        return response;
                    }

                    // Chỉnh sửa Exam
                    SqlCommand cmd = new SqlCommand("sp_EditAnswer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@AnswerID", id);
                    cmd.Parameters.AddWithValue("@QuestionID", editAnswer.QuestionID);
                    cmd.Parameters.AddWithValue("@AnswerText", editAnswer.AnswerText);
                    cmd.Parameters.AddWithValue("@IsCorrect", editAnswer.IsCorrect);

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Answer edited successfully!";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Failed to edit answer!";
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

        public Response<List<Answer>> GetAllAnswers()
        {
            var questions = new List<Answer>();
            Response<List<Answer>> response = new Response<List<Answer>>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ShowAnswer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new Answer
                            {
                                AnswerID = (int)reader["AnswerID"],
                                QuestionID = (int)reader["QuestionID"],
                                AnswerText = reader["AnswerText"].ToString(),
                                IsCorrect = (bool)reader["IsCorrect"]
                            });
                        }
                    }

                    if (questions.Count > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Answer retrieved successfully!";
                        response.Data = questions;
                    }
                    else
                    {
                        response.StatusCode = 204;
                        response.StatusMessage = "No answer found!";
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
