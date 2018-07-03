using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    public class People
    {
        private Student _student;
        public People(string id)
        {
            _student = new Student();
            _id = id;
        }

        public void Say()
        {
            _student.ToString();
        }
        private string _id;
    }


    public class Student
    {
        public Student()
        {
            name = "tim";
        }
        public string name;
    }
}
