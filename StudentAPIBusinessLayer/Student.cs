using StudentDataAccessLayer;
using System.Net.Cache;
namespace StudentAPIBusinessLayer
{
    public class Student
    {
        public enum enMode { AddNew = 0, Update = 1};
        public enMode Mode = enMode.AddNew;

        public StudentDTO SDTO {
            get { return (new StudentDTO(this.Id, this.Name, this.Age, this.Grade)); }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }

        public Student(StudentDTO SDTO , enMode cMode = enMode.AddNew)
        {
            this.Id = SDTO.Id;
            this.Name = SDTO.Name;
            this.Age = SDTO.Age;
            this.Grade = SDTO.Grade;

            Mode = cMode;

        }

        public static List<StudentDTO> GetAllStudents()
        {
            return StudentData.GetAllStudent();
        }

        public static List<StudentDTO> GetPassedStudent()
        {
            return StudentData.GetPassedStudents();
        }

        public static double GetAverageGrade()
        {
            return StudentData.GetAverageGrade();
        }

        public static Student Find(int ID)
        {
            StudentDTO STOD = StudentData.GetStudentById(ID);

            if(STOD != null)
            {
                return new Student(STOD, enMode.Update);
            }
            else
            {
                return null;
            }
        }

        private bool _AddNewStudent()
        {
            this.Id = StudentData.AddNewStudent(SDTO);

            return (this.Id != -1);
        }

        private bool _UpdateStudent()
        {
            return StudentData.UpdateStudnet(SDTO);
                
        }

        public  bool Save()
        {
            switch(Mode)
            {
                case enMode.AddNew:
                    if(_AddNewStudent())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateStudent();
            }

            return false;
        }

        public static bool DeleteStudent(int ID)
        {
            return StudentData.DeleteStudent(ID);
        }
    }

}
