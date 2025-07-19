using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAPIBusinessLayer;
using StudentDataAccessLayer;
using System.Collections.Generic; 

namespace StudentApi.Controllers 
{
    [Authorize(Roles = "Admin")]
    [ApiController]  
    [Route("api/Students")]
    public class StudentsController : ControllerBase 
    {

        [HttpGet("All", Name ="GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous] // Allow anonymous access to this endpoint
        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents() 
        {
            
            List<StudentDTO> StudentList = StudentAPIBusinessLayer.Student.GetAllStudents();
            if(StudentList.Count == 0)
            {
                return NotFound("No Students Found!");
            }

            return Ok(StudentList); // Returns the list of students.
        }


        [HttpGet("Passed", Name = "GetPassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetPassedStudents()
        {
            List<StudentDTO> StudentList = StudentAPIBusinessLayer.Student.GetPassedStudent();
            if (StudentList.Count == 0)
            {
                return NotFound("No Students Passed");
            }

            return Ok(StudentList); 
        }


        [HttpGet("AverageGrade", Name = "GetAverageGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetAverageGrade()
        {

            // StudentDataSimulation.StudentsList.Clear();
            double averageGrade = StudentAPIBusinessLayer.Student.GetAverageGrade();
            if (averageGrade == 0)
            {
                return NotFound();
            }

            //var averageGrade = StudentDataSimulation.StudentsList.Average(student => student.Grade);
            return Ok(averageGrade);
        }


        [HttpGet("{id}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous] 
        public ActionResult<StudentDTO> GetStudentById(int id)
        {

            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            StudentAPIBusinessLayer.Student Student = StudentAPIBusinessLayer.Student.Find(id);

            if (Student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }
            StudentDTO STDO = Student.SDTO;

            return Ok(STDO);

        }

        //for add new we use Http Post

        [HttpPost(Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> AddStudent(StudentDTO newStudentDTO)
        {
            //we validate the data here
            if (newStudentDTO == null || string.IsNullOrEmpty(newStudentDTO.Name) || newStudentDTO.Age < 0 || newStudentDTO.Grade < 0)
            {
                return BadRequest("Invalid student data.");
            }
            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;
            //StudentDataSimulation.StudentsList.Add(newStudent);

            StudentAPIBusinessLayer.Student student = new StudentAPIBusinessLayer.Student(new StudentDTO(newStudentDTO.Id , newStudentDTO.Name, newStudentDTO.Age, newStudentDTO.Grade));
            student.Save();

            newStudentDTO.Id = student.Id;

            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetStudentById", new { id = newStudentDTO.Id }, newStudentDTO);

        }

        //here we use HttpDelete method
        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteStudent(int id)
        { 
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }
           

            if (Student.DeleteStudent(id))
                return Ok($"Student with ID {id} has been deleted.");
            else
                return NotFound($"Student with ID {id} not found. no rows deleted!");

        }

        //here we use http put method for update

        [HttpPut("{id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> UpdateStudent(int id, StudentDTO updatedStudent)
        {
            if (id < 1 || updatedStudent == null || string.IsNullOrEmpty(updatedStudent.Name) || updatedStudent.Age < 0 || updatedStudent.Grade < 0)
            {
                return BadRequest("Invalid student data.");
            }
            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            StudentAPIBusinessLayer.Student student = StudentAPIBusinessLayer.Student.Find(id);
            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            student.Name = updatedStudent.Name;
            student.Age = updatedStudent.Age;
            student.Grade = updatedStudent.Grade;
            if (student.Save())
                return Ok(student);
            else
                return StatusCode(500, new { message = "Error Update Student" });
        }


    }
}
