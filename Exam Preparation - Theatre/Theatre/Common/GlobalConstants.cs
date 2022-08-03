using System;
using System.Collections.Generic;
using System.Text;

namespace Theatre.Common
{
    public static class GlobalConstants
    {
        //Theatre constants
        public const int TheatreNameMinLength = 4;
        public const int TheatreNameMaxLength = 30;
        public const sbyte TheatreHallsMinValue = 1;
        public const sbyte TheatreHallsMaxValue = 10;
        public const int TheatreDirectorMinLength = 4;
        public const int TheatreDirectorMaxLength = 30;


        //Play constants
        public const int PlayTitleMinLength = 4;
        public const int PlayTitleMaxLength = 50;
        public const int PlayDescriptionMaxLength = 700;
        public const int PlayScreenwriterMinLength = 4;
        public const int PlayScreenwriterMaxLength = 30;
        public const float PlayRatingMinValue = 0.00f;
        public const float PlayRatingMaxValue = 10.00f;


        //Cast constants
        public const int CastFullNameMinLength = 4;
        public const int CastFullNameMaxLength = 30;
        public const int CastPhoneNumberMaxLength = 15;
        public const string CastPhoneNumberRegex = @"\+44-\d{2}-\d{3}-\d{4}";


        //Ticket constants
        public const double TicketPriceMinValue = 1.00;
        public const double TicketPriceMaxValue = 100.00;
        public const sbyte TicketRowNumberMinValue = 1;
        public const sbyte TicketRowNumberMaxValue = 10;
    }
}
