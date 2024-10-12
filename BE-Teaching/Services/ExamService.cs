using BE_Teaching.Models;
using System.Data.SqlClient;
using System.Data;

namespace BE_Teaching.Services
{
    public class ExamService : IExamService
    {
        private readonly string _connectionString;

        public ExamService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Teaching");
        }

        public Response<List<Exam>> GetAllExams()
        {
            var exams = new List<Exam>();
            Response<List<Exam>> response = new Response<List<Exam>>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ShowExam", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exams.Add(new Exam
                            {
                                ExamID = (int)reader["ExamID"],
                                ExamName = reader["ExamName"].ToString()
                            });
                        }
                    }

                    if (exams.Count > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Exams retrieved successfully!";
                        response.Data = exams;
                    }
                    else
                    {
                        response.StatusCode = 204;
                        response.StatusMessage = "No exams found!";
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
        public Response<string> AddExam(Exam newExam)
        {
            Response<string> response = new Response<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_AddExam", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@ExamName", newExam.ExamName);

                    conn.Open();

                    int rs = cmd.ExecuteNonQuery();

                    if (rs > 0)
                    {
                        response.StatusCode = 201;
                        response.StatusMessage = "Exam added successfully!";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Failed to add exam!";
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

        public Response<string> EditExam(int id, Exam editExam)
        {
            Response<string> response = new Response<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open(); 
               
                    SqlCommand checkCmd = new SqlCommand("sp_CheckExam", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    checkCmd.Parameters.AddWithValue("@ExamID", id);

                    // Không cần kiểm tra null, trực tiếp ép kiểu
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        response.StatusCode = 404;
                        response.StatusMessage = "Exam not found!";
                        return response;
                    }

                    // Chỉnh sửa Exam
                    SqlCommand cmd = new SqlCommand("sp_EditExam", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@ExamID", id);
                    cmd.Parameters.AddWithValue("@ExamName", editExam.ExamName);

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Exam edited successfully!";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Failed to edit exam!";
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


        public Response<string> DeleteExam(int id)
        {
            Response<string> response = new Response<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand checkCmd = new SqlCommand("sp_CheckExam", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    checkCmd.Parameters.AddWithValue("@ExamID", id);

                    // Không cần kiểm tra null, trực tiếp ép kiểu
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        response.StatusCode = 404;
                        response.StatusMessage = "Exam not found!";
                        return response;
                    }

                    // Xoa Exam
                    SqlCommand cmd = new SqlCommand("sp_DeleteExam", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@ExamID", id);

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Exam deleted successfully!";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Failed to delete exam!";
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
