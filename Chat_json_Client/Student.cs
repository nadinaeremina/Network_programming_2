using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_json_Client
{
    public class Student
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Group { get; set; }
        public int[] Grades { get; set; }
        public override string ToString()
        {
            return $"LastName: {Lastname}\nAverageScore: {Grades.Average()}";
        }
    }
}
