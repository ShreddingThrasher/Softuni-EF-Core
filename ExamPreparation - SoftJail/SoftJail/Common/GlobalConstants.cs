using System;
using System.Collections.Generic;
using System.Text;

namespace SoftJail.Common
{
    public static class GlobalConstants
    {
        //Prisoner constants
        public const int PrisonerFullNameMinLength = 3;
        public const int PrisonerFullNameMaxLength = 20;
        public const string PrisonerNicknameRegex = @"^The [A-Z]{1}[a-z]+";
        public const int PrisonerAgeMinValue = 18;
        public const int PrisonerAgeMaxValue = 65;
        public const double PrisonerBailMinValue = 0;
        public const double PrisonerBailMaxValue = double.MaxValue;


        //Cell constants
        public const int CellNumberMinValue = 1;
        public const int CellNumberMaxValue = 1000;


        //Officer constants
        public const int OfficerFullNameMinLength = 3;
        public const int OfficerFullNameMaxLength = 30;
        public const double OfficerSalaryMinValue = 0;
        public const double OfficerSalaryMaxValue = double.MaxValue;


        //Department constants
        public const int DepartmentNameMinLength = 3;
        public const int DepartmentNameMaxLength = 25;


        //Mail constants
        public const string MailAddressRegex = @"[A-Za-z0-9 ]+(str\.)$";
    }
}
