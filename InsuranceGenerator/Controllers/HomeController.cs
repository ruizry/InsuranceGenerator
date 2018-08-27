using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InsuranceGenerator.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CalcRate(string firstName, string lastName, string email, string dob, string make1, string model1, string carYear1, string coverage1, string make2, string model2, string carYear2, string coverage2, string make3, string model3, string carYear3, string coverage3, int ticketToggle, string ticket1, string ticket2, string ticket3, string ticket4, string ticket5, int duiToggle, string dui)
        {
            //convert all year inputs to ints
            bool Year1Check, Year2Check, Year3Check;
            Year1Check = int.TryParse(carYear1, out int Year1Value);
            Year2Check = int.TryParse(carYear2, out int Year2Value);
            Year3Check = int.TryParse(carYear3, out int Year3Value);

            Year1Value = (Year1Check) ? Year1Value : DateTime.Now.Year;
            Year2Value = (Year2Check) ? Year2Value : DateTime.Now.Year;
            Year3Value = (Year3Check) ? Year3Value : DateTime.Now.Year;

            //convert date of birth data
            bool DobCheck;
            DobCheck = DateTime.TryParse(dob, out DateTime DateOfBirth);
            //var age = DateTime.Now.Year - DateOfBirth.Year;
            var daysalive = DateTime.Now - DateOfBirth;
            int age = daysalive.Days / 365;

            //convert ticket input
            bool[] TicketCheck = new bool[5];
            DateTime[] Ticket = new DateTime[5];
            TicketCheck[0] = DateTime.TryParse(ticket1, out Ticket[0]);
            TicketCheck[1] = DateTime.TryParse(ticket2, out Ticket[1]);
            TicketCheck[2] = DateTime.TryParse(ticket3, out Ticket[2]);
            TicketCheck[3] = DateTime.TryParse(ticket4, out Ticket[3]);
            TicketCheck[4] = DateTime.TryParse(ticket5, out Ticket[4]);

            //convert dui input
            bool DuiCheck;
            DuiCheck = DateTime.TryParse(dui, out DateTime Dui);

            int DuiActive = 0;
            if (duiToggle == 1)
            {
                Dui = (DuiCheck) ? Dui : DateTime.Now;
                DuiActive = (DateTime.Now - Dui).Days < 3650 ? 1 : 0;
            }

            using (InsuranceEntities db = new InsuranceEntities())
            {
                var Users = db.Users;
                var FoundUsers = Users.Where(x => x.Email == email).ToList().Count;

                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(dob) || string.IsNullOrEmpty(make1))
                {
                    return View("FormError");
                }
                // checks if email/account is already in system
                else if (FoundUsers > 0)
                {
                    return View("AccountError");
                }
                // check for valid car year inputs
                else if ((Year1Check == true && (Year1Value < 1900 || Year1Value > DateTime.Now.Year + 1)) || (Year2Check == true && (Year2Value < 1900 || Year2Value > DateTime.Now.Year + 1)) || (Year3Check == true && (Year3Value < 1900 || Year3Value > DateTime.Now.Year + 1)))
                {
                    return View("YearError");
                }
                // checks if user entered valid age
                else if (age > 130 || age < 14)
                {
                    return View("AgeError");
                }
                else
                {
                    var tempUser = new User();
                    tempUser.FirstName = firstName;
                    tempUser.LastName = lastName;
                    tempUser.Email = email;
                    tempUser.DateOfBirth = DateOfBirth;
                    tempUser.DuiActive = DuiActive;
                    if (duiToggle == 1)
                    {
                        tempUser.DuiIssued = (DuiCheck) ? Dui : DateTime.Now;
                    }            
                    else tempUser.DuiIssued = null;


                    db.Users.Add(tempUser);
                    db.SaveChanges();

                    var currentuser = Users.Where(x => x.Email == email).First();

                    for (int i = 0; i < ticketToggle; i++)
                    {
                        var tempTicket = new Ticket();
                        tempTicket.IssueDate = (TicketCheck[i]) ? Ticket[i] : DateTime.Now;
                        tempTicket.UserId = currentuser.Id;
                        db.Tickets.Add(tempTicket);
                    }
                    db.SaveChanges();

                    //use the next 3 lines in calquote and take out here
                    var Tickets = db.Tickets;  // remove this
                    var userTickets = db.Tickets.Where(x => x.UserId == currentuser.Id).ToList();
                    int Points = userTickets.Count(x => (DateTime.Now - x.IssueDate).Days <= 1095);

                    //create temp car object here
                    var tempCar = new Car();
                    tempCar.Make = make1;
                    tempCar.Model = model1;
                    tempCar.Year = Year1Value;


                    //pass in user and car
                    decimal rate1 = CalcCarQuote(currentuser, tempCar, coverage1);

                    //make quote object here using rate1, add quote to db and save
                    
                    //finish building tempCar with references, add car to db and save

                    //repeat the following 3 steps for car 2 if carToggle > 1 AND make2 has a value
                    //repeat the 3 steps for car 3 if carToggle > 2 AND make3 has a value

                    //calc total quote by passing email to totalquote method, this will return deciaml, it will retrieve all cars referenced
                    //set TotalRate in user to this rate, edit entry in table and save db


                    return Content("Test " + age + " " + Points + " " + coverage1);
                    //return View("Success");
                }
            }
        }

        [HttpGet]
        public ActionResult LookupRate(string lookupEmail)
        {
            if (string.IsNullOrEmpty(lookupEmail))
            {
                return View("~/Views/Shared/Error.cshtml");
            }
            else
            {
                return View("Success");
            }
        }

        private decimal CalcCarQuote(User user, Car car, string coverage)
        {
            double rate = 50.00;
            //calc age again
            //put calc points lines here
            //convert coverage
            
            return Convert.ToDecimal(rate);
        }

    }
}