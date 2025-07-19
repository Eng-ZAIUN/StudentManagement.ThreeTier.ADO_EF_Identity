using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace StudentDataAccessLayer
{
    public class StudentDTO
    {
        public StudentDTO(int id, string name, int age, int grade)
        {
            this.Id = id;
            this.Name = name;
            this.Age = age;
            this.Grade = grade;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }
    }

   public class StudentData
    {
        static string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=StudentsDB;Integrated Security=True;";

        public static List<StudentDTO> GetAllStudent()
       {
            var StudentList = new List<StudentDTO>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllStudents", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        
                        
                        while (reader.Read())
                        {
                            StudentList.Add(new StudentDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("Id")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.GetInt32(reader.GetOrdinal("Age")),
                                    reader.GetInt32(reader.GetOrdinal("Grade"))
                                ));
                       
                        }

                    }

                }

              return StudentList;

            }
       }

        public static List<StudentDTO> GetPassedStudents()
        {
            var StudentList = new List<StudentDTO>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetPassedStudents", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    conn.Open();

                    using (SqlDataReader reader =cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            StudentList.Add(new StudentDTO
                            (
                                   reader.GetInt32(reader.GetOrdinal("Id")),
                                   reader.GetString(reader.GetOrdinal("Name")),
                                   reader.GetInt32(reader.GetOrdinal("Age")),
                                   reader.GetInt32(reader.GetOrdinal("Grade"))
                            ));

                        }

                    }

                }

                return StudentList;

            }
        }

        public static double GetAverageGrade()
        {
            double averageGrade = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAverageGrade", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        averageGrade = Convert.ToDouble(result);
                    }
                    else
                        averageGrade = 0;
                }
            }

            return averageGrade;
        }

        public static StudentDTO GetStudentById(int StudnetId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetStudentById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentId", StudnetId);
                   
                    connection.Open();

                    using( var reader =  command.ExecuteReader())
                    {
                       
                        if(reader.Read())
                        {
                            return new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))

                            );
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
            }
        }

        public static int AddNewStudent(StudentDTO studentDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_AddStudent", connection))
                {
                    command.CommandType =CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Name", studentDTO.Name);
                    command.Parameters.AddWithValue("Age", studentDTO.Age);
                    command.Parameters.AddWithValue("Grade", studentDTO.Grade);

                    var outputIdParam = new SqlParameter("@NewStudentId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputIdParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputIdParam.Value;


                }
            }
        }

        public static bool UpdateStudnet(StudentDTO studentDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_UpdateStudent", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@StudentId", studentDTO.Id);
                command.Parameters.AddWithValue("@Name", studentDTO.Name);
                command.Parameters.AddWithValue("@Age", studentDTO.Age);
                command.Parameters.AddWithValue("@Grade", studentDTO.Grade);

                try
                {
                     connection.Open();
                     command.ExecuteNonQuery();
                        return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
              

            }

        }

        public static bool DeleteStudent(int StudebtID)
        {
            using(var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_DeleteStudent", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StudentId", StudebtID);

                try
                {
                    connection.Open();

                    int rowsAffected = (int)command.ExecuteScalar();
                    return (rowsAffected == 1);

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

   }

}
