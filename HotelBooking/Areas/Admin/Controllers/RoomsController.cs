using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models.Entities;

namespace HotelBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Rooms
        public async Task<IActionResult> Index(int? hotelId)
        {
            var query = _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .AsQueryable();

            if (hotelId.HasValue)
            {
                query = query.Where(r => r.HotelId == hotelId.Value);
            }

            var rooms = await query.OrderBy(r => r.HotelId).ThenBy(r => r.PricePerNight).ToListAsync();
            ViewBag.Hotels = await _context.Hotels.ToListAsync();
            ViewBag.CurrentHotelId = hotelId;

            return View(rooms);
        }

        // GET: /Admin/Rooms/Create
        public async Task<IActionResult> Create(int? hotelId)
        {
            ViewBag.HotelId = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", hotelId);
            ViewBag.RoomTypeId = new SelectList(await _context.RoomTypes.ToListAsync(), "Id", "Name");
            return View(new Room { HotelId = hotelId ?? 0, Capacity = 2, TotalQuantity = 10, AvailableQuantity = 10, PricePerNight = 3500000 });
        }

        // POST: /Admin/Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                room.AvailableQuantity = room.TotalQuantity;
                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Thêm phòng \"{room.Name}\" thành công!";
                return RedirectToAction(nameof(Index), new { hotelId = room.HotelId });
            }

            ViewBag.HotelId = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", room.HotelId);
            ViewBag.RoomTypeId = new SelectList(await _context.RoomTypes.ToListAsync(), "Id", "Name", room.RoomTypeId);
            return View(room);
        }

        // GET: /Admin/Rooms/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            ViewBag.HotelId = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", room.HotelId);
            ViewBag.RoomTypeId = new SelectList(await _context.RoomTypes.ToListAsync(), "Id", "Name", room.RoomTypeId);
            return View(room);
        }

        // POST: /Admin/Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var room = await _context.Rooms.FindAsync(id);
                if (room == null)
                {
                    return NotFound();
                }

                room.HotelId = model.HotelId;
                room.RoomTypeId = model.RoomTypeId;
                room.RoomNumber = model.RoomNumber;
                room.Name = model.Name;
                room.Description = model.Description;
                room.PricePerNight = model.PricePerNight;
                room.Capacity = model.Capacity;
                room.BedInfo = model.BedInfo;
                room.AreaM2 = model.AreaM2;
                room.TotalQuantity = model.TotalQuantity;
                room.AvailableQuantity = model.AvailableQuantity;
                room.IsActive = model.IsActive;
                room.ImageUrl = model.ImageUrl;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật thông tin phòng \"{room.Name}\" thành công!";
                return RedirectToAction(nameof(Index), new { hotelId = room.HotelId });
            }

            ViewBag.HotelId = new SelectList(await _context.Hotels.ToListAsync(), "Id", "Name", model.HotelId);
            ViewBag.RoomTypeId = new SelectList(await _context.RoomTypes.ToListAsync(), "Id", "Name", model.RoomTypeId);
            return View(model);
        }

        // POST: /Admin/Rooms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.BookingDetails)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room != null)
            {
                if (room.BookingDetails.Any())
                {
                    room.IsActive = false;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Phòng \"{room.Name}\" đã được chuyển sang trạng thái Ngưng Hoạt Động (do đã có lịch sử đặt).";
                }
                else
                {
                    _context.Rooms.Remove(room);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã xóa phòng \"{room.Name}\" thành công!";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
