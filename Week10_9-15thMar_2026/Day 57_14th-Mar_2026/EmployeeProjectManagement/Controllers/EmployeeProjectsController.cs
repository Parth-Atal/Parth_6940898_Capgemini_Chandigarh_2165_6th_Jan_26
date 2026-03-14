using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeProjectManagement.Data;
using EmployeeProjectManagement.Models;

namespace EmployeeProjectManagement.Controllers
{
    public class EmployeeProjectsController : Controller
    {
        private readonly EmployeeProjectManagementContext _context;

        public EmployeeProjectsController(EmployeeProjectManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employeeProjectManagementContext = _context.EmployeeProject.Include(e => e.Employee).Include(e => e.Project);
            return View(await employeeProjectManagementContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProject = await _context.EmployeeProject
                .Include(e => e.Employee)
                .Include(e => e.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeProject == null)
            {
                return NotFound();
            }

            return View(employeeProject);
        }

        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Name");
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,ProjectId,AssignedDate")] EmployeeProject employeeProject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Name", employeeProject.EmployeeId);
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "Title", employeeProject.ProjectId);
            return View(employeeProject);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProject = await _context.EmployeeProject.FindAsync(id);
            if (employeeProject == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Name", employeeProject.EmployeeId);
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "Title", employeeProject.ProjectId);
            return View(employeeProject);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,ProjectId,AssignedDate")] EmployeeProject employeeProject)
        {
            if (id != employeeProject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeProjectExists(employeeProject.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Name", employeeProject.EmployeeId);
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "Title", employeeProject.ProjectId);
            return View(employeeProject);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProject = await _context.EmployeeProject
                .Include(e => e.Employee)
                .Include(e => e.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeProject == null)
            {
                return NotFound();
            }

            return View(employeeProject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeProject = await _context.EmployeeProject.FindAsync(id);
            if (employeeProject != null)
            {
                _context.EmployeeProject.Remove(employeeProject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeProjectExists(int id)
        {
            return _context.EmployeeProject.Any(e => e.Id == id);
        }
    }
}
