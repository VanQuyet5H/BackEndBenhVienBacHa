using MANAGECOFFEE.Data;
using ManagementCoffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MANAGECOFFEE.Controllers
{
	public class OrderController : Controller
	{
		private readonly CoffeeShopContext _context;

		public OrderController(CoffeeShopContext context)
		{
			_context = context;

		}
		public ActionResult Index()
		{
			// Get the list of orders from the database
			var orders = _context.Order.ToList();

			return View(orders);
		}

		// GET: Order/Create
		public ActionResult Create()
		{
			// Create a new order object
			Order order = new Order();

			return View(order);
		}

		// POST: Order/Store
		public ActionResult Store(Order order)
		{
			// Save the order to the database
			_context.Order.Add(order);
			_context.SaveChanges();

			// Redirect to the order list page
			return RedirectToAction("Index");
		}
	}
}
