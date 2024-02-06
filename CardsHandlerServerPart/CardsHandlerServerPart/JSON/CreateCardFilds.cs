using System;

namespace CardsHandlerServerPart.JSON
{
    internal class CreateCardFilds
    {
        public int Number { get; set; }

        public DateTime ExpDate { get; set; }

        public int Balance { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

    }
}
