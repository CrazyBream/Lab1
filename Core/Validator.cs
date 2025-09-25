using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core
{
    public static class Validator
    {
        public static bool ValidateName(string name) => Regex.IsMatch(name, @"^[A-Za-zА-Яа-яЁёІіЇї Ґґ'-]+$");

        public static bool ValidateCountry(string country) => Regex.IsMatch(country, @"^[A-Za-zА-Яа-яЁёІі Її Ґґ ]+$");

        public static bool ValidateStudentId(string id) => Regex.IsMatch(id, @"^[A-Z]{2}\d{6}$");

        public static bool ValidateCourse(int course) => course >= 1 && course <= 5;

        public static bool ValidateRecordBookNumber(string num) => Regex.IsMatch(num, @"^\d{5}$");

        public static bool ValidateAverageGrade(double grade) => grade >= 0 && grade <= 5;
    }
}
