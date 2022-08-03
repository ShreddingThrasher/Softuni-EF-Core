using System;
using System.Collections.Generic;
using System.Text;

namespace TeisterMask.Common
{
    public static class GlobalConstants
    {
        //Employee constants
        public const int EmployeeUsernameMaxLength = 40;
        public const int EmployeePhoneMaxLength = 12;
        public const string EmployeeUsernameRegex = @"^[A-Za-z0-9]+$";
        public const string EmployeePhoneRegex = @"^\d{3}-\d{3}-\d{4}$";

        //Project constants
        public const int ProjectNameMaxLength = 40;

        //Task constants
        public const int TaskNameMaxLength = 40;
    }
}
