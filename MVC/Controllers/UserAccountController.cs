using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Helpers;
using MVC.Models;
using MVC.Models.View_Model;

namespace MVC.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly UserAccountDbContext _context;

        public UserAccountController(UserAccountDbContext context)
        {
            _context = context;
        }

        // GET: UserAccount
        public async Task<IActionResult> Index()
        {
            return View(await _context.Account.Where(x => x.IsActive == true).ToListAsync());
        }

        // GET: UserAccount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var decipher = new Decipher();

            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.Account
                .FirstOrDefaultAsync(m => m.Id == id);

            if (userAccount == null)
            {
                return NotFound();
            }

            var userDetails = new UserDetails();
            userDetails.Id = userAccount.Id;
            userDetails.FullName = userAccount.FullName;
            userDetails.EmailAddress = userAccount.EmailAddress;
            userDetails.EncryptedPassword = userAccount.Password;
            userDetails.DecryptedPassword = decipher.Decrypt(userAccount.Password);
            userDetails.DateCreated = userAccount.DateCreated;
            userDetails.IsActive = userAccount.IsActive;

            return View(userDetails);
        }

        // GET: UserAccount/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserAccount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,EmailAddress,Password")] UserAccount userAccount)
        {
            var decipher = new Decipher();
            if (ModelState.IsValid)
            {
                var account = _context.Account.Where(x => x.EmailAddress == userAccount.EmailAddress && x.IsActive == true).FirstOrDefault();
                if (account != null)
                    return Content("Email Address is already registered!");

                userAccount.Password = decipher.Encrypt(userAccount.Password);
                _context.Add(userAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userAccount);
        }

        // GET: UserAccount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.Account.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }
            return View(userAccount);
        }

        // POST: UserAccount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,EmailAddress,Password")] UserAccount userAccount)
        {
            if (id != userAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAccountExists(userAccount.Id))
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
            return View(userAccount);
        }

        // GET: UserAccount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.Account
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // POST: UserAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAccount = await _context.Account.FindAsync(id);
            userAccount.IsActive = false;
            _context.Update(userAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(int id)
        {
            return _context.Account.Any(e => e.Id == id);
        }
    }
}
