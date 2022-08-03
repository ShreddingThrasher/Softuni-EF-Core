using System;
using System.Collections.Generic;
using System.Text;

namespace Artillery.Common
{
    public static class GlobalConstants
    {
        //Country constants
        public const int CountryNameMinLength = 4;
        public const int CountryNameMaxLength = 60;
        public const int CountryArmySizeMinValue = 50000;
        public const int CountryArmySizeMaxValue = 10000000;


        //Manufacturer constants
        public const int ManufacturerNameMinLength = 4;
        public const int ManufacturerNameMaxLength = 40;
        public const int ManufacturerFoundedMinLength = 10;
        public const int ManufacturerFoundedMaxLength = 100;


        //Shell constants
        public const int ShellCaliberMinLength = 4;
        public const int ShellCaliberMaxLength = 30;
        public const int ShellWeightMinValue = 2;
        public const int ShellWeightMaxValue = 1680;


        //Gun constants
        public const int GunWeightMinValue = 100;
        public const int GunWeightMaxValue = 1350000;
        public const double GunBarrelLengthMinValue = 2.00;
        public const double GunBarrelLengthMaxValue = 35.00;
        public const int GunRangeMinValue = 1;
        public const int GunRangeMaxValue = 100000;
    }
}
