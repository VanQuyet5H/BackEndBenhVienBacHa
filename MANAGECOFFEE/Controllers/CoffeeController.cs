using MANAGECOFFEE.Data;
using ManagementCoffee.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MANAGECOFFEE.Controllers
{
    public class CoffeeController : Controller
    {
        private readonly CoffeeShopContext _context;

        public CoffeeController(CoffeeShopContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            var coffees = _context.Coffee.ToList();

            // Return a view with the coffees
            return View(coffees);
        }
        [HttpGet]
        //[Authorize(Roles = "admin")]
        public IActionResult DanhSachCoffee()
        {
            // Get the list of coffee from the database
            var coffees = _context.Coffee.ToList();

            // Return the list of coffee to the view
            return View(coffees);
        }

        // Show details of a specific coffee
        public IActionResult Details(int id)
        {
            // Get the coffee from the database
            var coffee = _context.Coffee.Find(id);

            // Return a view with the coffee
            return View(coffee);
        }

        // Add new coffee
        public IActionResult Create()
        {
            // Create a new coffee object
            var coffee = new Coffee();

            // Return a view with the coffee
            return View(coffee);
        }

        // Save new coffee
        public IActionResult Post([FromBody] Coffee coffee)
        {
            // Save the coffee to the database
            _context.Coffee.Add(coffee);
            _context.SaveChanges();

            // Redirect to the coffees index
            return RedirectToAction("Index");
        }

        // Update existing coffee
        public IActionResult Edit(int id)
        {
            // Get the coffee from the database
            var coffee = _context.Coffee.Find(id);

            // Return a view with the coffee
            return View(coffee);
        }

        // Save updated coffee
        public IActionResult Put([FromBody] Coffee coffee)
        {
            // Update the coffee in the database
            _context.Update(coffee);
            _context.SaveChanges();

            // Redirect to the coffees index
            return RedirectToAction("Index");
        }

        //Delete coffee
        public IActionResult Delete(Coffee product)
        {
			// Get the coffee from the database
			_context.Coffee.Remove(product);
			_context.SaveChanges();
			// Redirect to the product list page
			return RedirectToAction("Index");
		}
    }
}
