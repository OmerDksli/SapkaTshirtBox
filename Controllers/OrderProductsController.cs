﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using OzSapkaTShirt.Data;
using OzSapkaTShirt.Models;

namespace OzSapkaTShirt.Controllers
{
    [Authorize]
    public class OrderProductsController : Controller
    {
        private readonly ApplicationContext _context;

        public OrderProductsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: OrderProducts
        public async Task<IActionResult> Index()
        {
            
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            


            var applicationContext = _context.OrderProducts.Where(o => o.Order.UserId == userId&&o.Order.Status==0).Include(o => o.Order).Include(o => o.Product);
            return View(await applicationContext.ToListAsync());
        }
        public IActionResult MyCart(long id)
        {
           




            var applicationContext = _context.OrderProducts.Where(o => o.Order.Id == id && o.Order.Status == 1).Include(o => o.Order).Include(o => o.Order.User).Include(o => o.Product);
            return View(applicationContext.ToList());

            
        }
        // GET: OrderProducts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.OrderProducts == null)
            {
                return NotFound();
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderProduct = await _context.OrderProducts
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return View(orderProduct);
        }

        // GET: OrderProducts/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Color");
            return View();
        }

        // POST: OrderProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,Quantity,Price,Total")] OrderProduct orderProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderProduct.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Color", orderProduct.ProductId);
            return View(orderProduct);
        }

        // GET: OrderProducts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.OrderProducts == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderProduct.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Color", orderProduct.ProductId);
            return View(orderProduct);
        }

        // POST: OrderProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("OrderId,ProductId,Quantity,Price,Total")] OrderProduct orderProduct)
        {
            if (id != orderProduct.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderProductExists(orderProduct.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderProduct.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Color", orderProduct.ProductId);
            return View(orderProduct);
        }

        // GET: OrderProducts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.OrderProducts == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return View(orderProduct);
        }

        // POST: OrderProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.OrderProducts == null)
            {
                return Problem("Entity set 'ApplicationContext.OrderProducts'  is null.");
            }
            var orderProduct = await _context.OrderProducts.FindAsync(id);
            if (orderProduct != null)
            {
                _context.OrderProducts.Remove(orderProduct);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderProductExists(long id)
        {
          return (_context.OrderProducts?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
        public Order UpDateBasket(long id, short quantity)
        {
            Order? order;
            OrderProduct orderProduct = new OrderProduct();
            Product? product = _context.Products.Find(id);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            order = _context.Orders.Where(o => o.UserId == userId && o.Status == 0)
                .Include(o=>o.OrderProducts).FirstOrDefault();

            if (order == null)
            {
                
                order = new Order();
                order.OrderDate = DateTime.Today;
                order.Status = 0;
                order.TotalPrice = 0;
                order.UserId = userId;
                order.OrderProducts = new List<OrderProduct>();
                _context.Add(order);
                _context.SaveChanges();

            }
            orderProduct = order.OrderProducts.Find(z => z.ProductId == id);
            if(orderProduct==null)
            {
                orderProduct = new OrderProduct();
                orderProduct.OrderId = order.Id; orderProduct.ProductId=id;
                orderProduct.Quantity = 1;
                orderProduct.Price = product.Price;
                orderProduct.Total = orderProduct.Price;
                order.OrderProducts.Add(orderProduct);
                _context.Add(orderProduct);
            }          
            else
            {
                orderProduct.Quantity += quantity;
               
                
                if (orderProduct.Quantity == 0)
                {
                    

                    order.OrderProducts.Remove(orderProduct);
                    if (order.OrderProducts.Count == 0)
                    {
                        _context.Remove(order);
                        _context.SaveChanges();
                        return null;
                    }
                }
                else
                {
                    orderProduct.Total += product.Price * quantity;
                }
                //orderProduct.Quantity--;
                //orderProduct.Total = orderProduct.Price * orderProduct.Quantity;
                //_context.Update(orderProduct);

            }
            order.TotalPrice = order.OrderProducts.Sum(o=>o.Total);
            _context.Update(order);
            
            _context.SaveChanges();
            //HttpContext.Session.SetInt32("BasketCount",order.OrderProducts.Sum(op=>op.Quantity));

            
            return order;
        }
    }
}
