using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Models.Entities;

namespace HotelBooking.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            // Ensure database is created and migrations applied
            await context.Database.MigrateAsync();

            // 1. Seed Roles
            string[] roleNames = { "Admin", "Staff", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole(roleName) { Description = $"Role {roleName}" });
                }
            }

            // 2. Seed Users
            // Admin User
            var adminEmail = "admin@hotelbooking.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Hệ Thống Quản Trị (Admin)",
                    PhoneNumber = "0901234567",
                    Address = "Hà Nội, Việt Nam",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Staff User
            var staffEmail = "staff@hotelbooking.com";
            var staffUser = await userManager.FindByEmailAsync(staffEmail);
            if (staffUser == null)
            {
                staffUser = new AppUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    FullName = "Nguyễn Văn Lễ Tân",
                    PhoneNumber = "0908888999",
                    Address = "Đà Nẵng, Việt Nam",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(staffUser, "Staff@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staffUser, "Staff");
                }
            }

            // Customer User
            var customerEmail = "customer@hotelbooking.com";
            var customerUser = await userManager.FindByEmailAsync(customerEmail);
            if (customerUser == null)
            {
                customerUser = new AppUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    FullName = "Nguyễn Văn An",
                    PhoneNumber = "0912345678",
                    Address = "Hà Nội, Việt Nam",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(customerUser, "Customer@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, "Customer");
                }
            }

            // Additional Demo Customer
            var customer2Email = "demo.user@hotelbooking.com";
            var customer2User = await userManager.FindByEmailAsync(customer2Email);
            if (customer2User == null)
            {
                customer2User = new AppUser
                {
                    UserName = customer2Email,
                    Email = customer2Email,
                    FullName = "Trần Thị Mai Hương",
                    PhoneNumber = "0987654321",
                    Address = "TP. Hồ Chí Minh, Việt Nam",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(customer2User, "Customer@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer2User, "Customer");
                }
            }

            // 3. Seed Amenities
            if (!context.Amenities.Any())
            {
                var amenities = new List<Amenity>
                {
                    new() { Name = "Wifi tốc độ cao miễn phí", Icon = "bi-wifi", Category = "General" },
                    new() { Name = "Hồ bơi vô cực", Icon = "bi-water", Category = "Wellness" },
                    new() { Name = "Buffet sáng sang trọng", Icon = "bi-cup-hot", Category = "Service" },
                    new() { Name = "Phòng Gym & Yoga hiện đại", Icon = "bi-activity", Category = "Wellness" },
                    new() { Name = "Spa & Massage thư giãn", Icon = "bi-heart-pulse", Category = "Wellness" },
                    new() { Name = "Bãi đỗ xe an toàn miễn phí", Icon = "bi-p-circle", Category = "General" },
                    new() { Name = "Xe đưa đón sân bay", Icon = "bi-car-front", Category = "Service" },
                    new() { Name = "Quầy Bar / Sky Lounge", Icon = "bi-cup-straw", Category = "Service" },
                    new() { Name = "Bãi biển riêng", Icon = "bi-umbrella", Category = "General" },
                    new() { Name = "Lễ tân phục vụ 24/7", Icon = "bi-clock-history", Category = "General" },
                    new() { Name = "Điều hòa 2 chiều", Icon = "bi-snow", Category = "Room" },
                    new() { Name = "Smart TV 4K Netflix", Icon = "bi-tv", Category = "Room" },
                    new() { Name = "Bồn tắm nằm Jacuzzi", Icon = "bi-droplet", Category = "Room" },
                    new() { Name = "Ban công view toàn cảnh", Icon = "bi-eye", Category = "Room" },
                    new() { Name = "Két sắt an toàn điện tử", Icon = "bi-shield-check", Category = "Room" },
                    new() { Name = "Máy pha cafe Espresso cao cấp", Icon = "bi-cup", Category = "Room" },
                    new() { Name = "Mini Bar & Tủ lạnh nhỏ", Icon = "bi-archive", Category = "Room" }
                };

                await context.Amenities.AddRangeAsync(amenities);
                await context.SaveChangesAsync();
            }

            // 4. Seed RoomTypes
            if (!context.RoomTypes.Any())
            {
                var roomTypes = new List<RoomType>
                {
                    new() { Name = "Standard Room", Description = "Phòng tiêu chuẩn ấm cúng, đầy đủ tiện nghi thiết yếu." },
                    new() { Name = "Superior City View", Description = "Phòng cao cấp ngắm toàn cảnh thành phố rực rỡ về đêm." },
                    new() { Name = "Deluxe Ocean View", Description = "Phòng hạng sang có ban công ngắm nhìn đại dương tuyệt đẹp." },
                    new() { Name = "Executive Suite", Description = "Căn hộ suite sang trọng với phòng khách riêng và bồn tắm Jacuzzi." },
                    new() { Name = "Family Luxury Suite", Description = "Không gian rộng rãi, thiết kế lý tưởng cho kỳ nghỉ gia đình 4-6 người." },
                    new() { Name = "Presidential Suite", Description = "Đẳng cấp nguyên thủ quốc gia với nội thất hoàng gia đỉnh cao." }
                };

                await context.RoomTypes.AddRangeAsync(roomTypes);
                await context.SaveChangesAsync();
            }

            // 5. Seed Hotels & Rooms
            if (!context.Hotels.Any())
            {
                var allAmenities = await context.Amenities.ToListAsync();
                var rTypes = await context.RoomTypes.ToListAsync();
                var stdType = rTypes.First(r => r.Name.Contains("Standard"));
                var supType = rTypes.First(r => r.Name.Contains("Superior"));
                var dlxType = rTypes.First(r => r.Name.Contains("Deluxe"));
                var exeType = rTypes.First(r => r.Name.Contains("Executive"));
                var famType = rTypes.First(r => r.Name.Contains("Family"));
                var presType = rTypes.First(r => r.Name.Contains("Presidential"));

                var hotels = new List<Hotel>
                {
                    // 1. Da Nang
                    new Hotel
                    {
                        Name = "InterContinental Danang Sun Peninsula Resort",
                        Slug = "intercontinental-danang-sun-peninsula-resort",
                        Description = "Khu nghỉ dưỡng sang trọng bậc nhất thế giới nằm ẩn mình bên bán đảo Sơn Trà hoang sơ, với thiết kế đỉnh cao của kiến trúc sư lừng danh Bill Bensley hòa quyện cùng thiên nhiên và di sản văn hóa Việt Nam.",
                        Address = "Bán đảo Sơn Trà, Thọ Quang",
                        City = "Đà Nẵng",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0236 393 8888",
                        Email = "reservations.icdanang@ihg.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-60),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Resort Classic Ocean View",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "IC-101",
                                Description = "Phòng rộng 70m2 với ban công ngoài trời hướng biển Đông ngút ngàn, bồn tắm đá cẩm thạch nguyên khối.",
                                PricePerNight = 8500000,
                                Capacity = 2,
                                BedInfo = "1 Giường King hoặc 2 Giường đơn",
                                AreaM2 = 70.0,
                                TotalQuantity = 12,
                                AvailableQuantity = 10,
                                ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Terrace Suite Ocean View",
                                RoomTypeId = exeType.Id,
                                RoomNumber = "IC-201",
                                Description = "Suite cao cấp với hiên tắm nắng riêng ngắm hoàng hôn, phòng khách riêng biệt và đồ uống mini bar thượng hạng.",
                                PricePerNight = 14200000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Lớn",
                                AreaM2 = 95.0,
                                TotalQuantity = 6,
                                AvailableQuantity = 5,
                                ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Sun Peninsula Royal Villa",
                                RoomTypeId = presType.Id,
                                RoomNumber = "IC-VIP1",
                                Description = "Biệt thự Hoàng Gia 2 tầng view trực diện vịnh biển, hồ bơi vô cực riêng và quản gia cá nhân phục vụ 24/7.",
                                PricePerNight = 35000000,
                                Capacity = 4,
                                BedInfo = "2 Giường King Size Thượng Hạng",
                                AreaM2 = 300.0,
                                TotalQuantity = 2,
                                AvailableQuantity = 2,
                                ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 2. Phu Quoc
                    new Hotel
                    {
                        Name = "JW Marriott Phu Quoc Emerald Bay Resort & Spa",
                        Slug = "jw-marriott-phu-quoc-emerald-bay",
                        Description = "Kiệt tác nghỉ dưỡng bên bãi Khem cát trắng mịn như kem, thiết kế theo trường đại học Lamarck huyền thoại với các phân khoa độc đáo và hồ bơi vỏ sò mang tính biểu tượng.",
                        Address = "Bãi Khem, An Thới",
                        City = "Phú Quốc",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0297 377 9999",
                        Email = "mhrs.pqcjw.reservations@marriott.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-50),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Emerald Bay View Deluxe",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "JW-102",
                                Description = "Phòng Deluxe sang trọng nhìn ra vịnh ngọc lam biếc, trần nhà cao thoáng đãng cùng nội thất phong cách cổ điển thanh lịch.",
                                PricePerNight = 6800000,
                                Capacity = 2,
                                BedInfo = "1 Giường Đôi Cực Lớn",
                                AreaM2 = 54.0,
                                TotalQuantity = 15,
                                AvailableQuantity = 12,
                                ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Turquoise Suite",
                                RoomTypeId = exeType.Id,
                                RoomNumber = "JW-205",
                                Description = "Căn suite màu ngọc bích lãng mạn có ban công ngắm bình minh trên biển, bồn tắm đứng độc lập và trà chiều miễn phí.",
                                PricePerNight = 12500000,
                                Capacity = 2,
                                BedInfo = "1 Giường King",
                                AreaM2 = 90.0,
                                TotalQuantity = 8,
                                AvailableQuantity = 6,
                                ImageUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Lamarck Family Villa",
                                RoomTypeId = famType.Id,
                                RoomNumber = "JW-FAM1",
                                Description = "Biệt thự gia đình 2 phòng ngủ có sân vườn riêng và hồ bơi mini, lý tưởng cho chuyến nghỉ dưỡng sum vầy.",
                                PricePerNight = 21000000,
                                Capacity = 5,
                                BedInfo = "2 Giường King + 1 Giường Đơn",
                                AreaM2 = 180.0,
                                TotalQuantity = 4,
                                AvailableQuantity = 3,
                                ImageUrl = "https://images.unsplash.com/photo-1591088398332-8a7791972843?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 3. Ho Chi Minh City
                    new Hotel
                    {
                        Name = "Vinpearl Luxury Landmark 81",
                        Slug = "vinpearl-luxury-landmark-81",
                        Description = "Khách sạn trên cao nhất Đông Nam Á, tọa lạc từ tầng 47 đến tầng 77 của tòa tháp Landmark 81, đem đến trải nghiệm nghỉ dưỡng chạm mây giữa trung tâm Sài Gòn phồn hoa.",
                        Address = "720A Điện Biên Phủ, Phường 22, Quận Bình Thạnh",
                        City = "TP. Hồ Chí Minh",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0283 910 1280",
                        Email = "res.vplm81@vinpearl.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-45),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Premier Panoramic City View",
                                RoomTypeId = supType.Id,
                                RoomNumber = "LM-501",
                                Description = "Phòng nghỉ tầng 52 với cửa kính tràn bờ từ sàn đến trần chiêm ngưỡng toàn cảnh dòng sông Sài Gòn thơ mộng.",
                                PricePerNight = 5200000,
                                Capacity = 2,
                                BedInfo = "1 Giường King hoặc 2 Giường Đơn",
                                AreaM2 = 48.0,
                                TotalQuantity = 20,
                                AvailableQuantity = 18,
                                ImageUrl = "https://images.unsplash.com/photo-1595526114035-0d45ed16cfbf?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Executive Skyline Suite",
                                RoomTypeId = exeType.Id,
                                RoomNumber = "LM-650",
                                Description = "Suite góc tầng 65 nhìn ngắm 360 độ toàn cảnh thành phố Hồ Chí Minh, quyền lợi Club Lounge thượng hạng.",
                                PricePerNight = 9800000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Hoàng Gia",
                                AreaM2 = 82.0,
                                TotalQuantity = 10,
                                AvailableQuantity = 8,
                                ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 4. Ha Noi
                    new Hotel
                    {
                        Name = "Sofitel Legend Metropole Hanoi",
                        Slug = "sofitel-legend-metropole-hanoi",
                        Description = "Khách sạn cổ kính mang tính biểu tượng lịch sử phong cách Pháp thuộc từ năm 1901 ngay trung tâm thủ đô Hà Nội, chỉ vài bước chân tới Hồ Gươm và Nhà Hát Lớn.",
                        Address = "15 Phố Ngô Quyền, Phường Tràng Tiền, Quận Hoàn Kiếm",
                        City = "Hà Nội",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0243 826 6919",
                        Email = "h1555@sofitel.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-40),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Opera Wing Premium Room",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "MET-105",
                                Description = "Nội thất gỗ phong cách tân cổ điển Pháp, sàn gỗ lim ấm áp, bồn tắm vintage và dịch vụ mở giường đặc trưng.",
                                PricePerNight = 7200000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Metropole MyBed",
                                AreaM2 = 48.0,
                                TotalQuantity = 15,
                                AvailableQuantity = 12,
                                ImageUrl = "https://images.unsplash.com/photo-1598928506311-c55ded91a20c?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Grand Prestige Heritage Suite",
                                RoomTypeId = exeType.Id,
                                RoomNumber = "MET-302",
                                Description = "Nơi từng đón tiếp các nguyên thủ quốc gia và nghệ sĩ nổi tiếng, có thư phòng cổ điển, phòng khách tráng lệ.",
                                PricePerNight = 18500000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Sang Trọng",
                                AreaM2 = 120.0,
                                TotalQuantity = 3,
                                AvailableQuantity = 3,
                                ImageUrl = "https://images.unsplash.com/photo-1613490493576-7fde63acd811?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 5. Nha Trang
                    new Hotel
                    {
                        Name = "Six Senses Ninh Van Bay",
                        Slug = "six-senses-ninh-van-bay",
                        Description = "Khu nghỉ dưỡng 5 sao biệt lập ẩn mình giữa vịnh biển hoang sơ, bao bọc bởi những vách đá ấn tượng và rừng nguyên sinh nhiệt đới tươi tốt.",
                        Address = "Vịnh Ninh Vân, Thị xã Ninh Hòa",
                        City = "Nha Trang",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0258 352 4777",
                        Email = "reservations-ninhvan@sixsenses.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-35),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Hill Top Pool Villa",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "SS-08",
                                Description = "Biệt thự trên đồi với hồ bơi vô cực riêng ngắm toàn cảnh vịnh biển Ninh Vân trong vắt như ngọc bích.",
                                PricePerNight = 16500000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Lớn",
                                AreaM2 = 158.0,
                                TotalQuantity = 8,
                                AvailableQuantity = 7,
                                ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Beachfront Pool Villa",
                                RoomTypeId = exeType.Id,
                                RoomNumber = "SS-15",
                                Description = "Biệt thự bước thẳng ra bãi cát trắng mịn, có khu vườn riêng, hầm rượu vang nhỏ và bồn tắm gỗ thủ công ngoài trời.",
                                PricePerNight = 22000000,
                                Capacity = 3,
                                BedInfo = "1 Giường King + 1 Sofa Bed",
                                AreaM2 = 176.0,
                                TotalQuantity = 6,
                                AvailableQuantity = 4,
                                ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 6. Sa Pa
                    new Hotel
                    {
                        Name = "Hotel de la Coupole - MGallery Sapa",
                        Slug = "hotel-de-la-coupole-mgallery-sapa",
                        Description = "Bản giao hưởng kỳ diệu giữa thời trang cao cấp Haute Couture nước Pháp thập niên 1920-1930 và sắc màu thổ cẩm vùng cao Tây Bắc mộng mơ.",
                        Address = "1 Phố Hoàng Liên, Thị xã Sa Pa",
                        City = "Sapa",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0214 362 9999",
                        Email = "HA5V2-RE@accor.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-30),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Classic Mountain View Room",
                                RoomTypeId = supType.Id,
                                RoomNumber = "MG-401",
                                Description = "Phòng mang phong cách Pháp với màu vàng mù tạt ấm áp, ban công view thung lũng Mường Hoa bồng bềnh biển mây.",
                                PricePerNight = 3800000,
                                Capacity = 2,
                                BedInfo = "1 Giường King hoặc 2 Giường Đơn",
                                AreaM2 = 33.0,
                                TotalQuantity = 20,
                                AvailableQuantity = 16,
                                ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Executive Suite Sapa Clouds",
                                RoomTypeId = exeType.Id,
                                RoomNumber = "MG-702",
                                Description = "Suite sang trọng tầng cao nhất với lò sưởi ấm áp, nội thất xa hoa và view dãy núi Fansipan hùng vĩ.",
                                PricePerNight = 7600000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Lớn",
                                AreaM2 = 75.0,
                                TotalQuantity = 5,
                                AvailableQuantity = 4,
                                ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 7. Hoi An
                    new Hotel
                    {
                        Name = "Four Seasons Resort The Nam Hai",
                        Slug = "four-seasons-resort-the-nam-hai",
                        Description = "Ốc đảo thanh bình bên bờ biển Hà My thơ mộng, nơi nghệ thuật phong thủy truyền thống hòa cùng phong cách sống đỉnh cao của tập đoàn Four Seasons.",
                        Address = "Khối Hà My Đông B, Điện Dương, Điện Bàn",
                        City = "Hội An",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0235 394 0000",
                        Email = "reservations.hoian@fourseasons.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-25),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "One-Bedroom Oceanfront Villa",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "FS-12",
                                Description = "Biệt thự giường tầng phong cách cung đình, vòi tắm hoa sen ngoài trời giữa khu vườn nhiệt đới thơm ngát.",
                                PricePerNight = 15500000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Size",
                                AreaM2 = 80.0,
                                TotalQuantity = 10,
                                AvailableQuantity = 8,
                                ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 8. Da Nang - Pullman
                    new Hotel
                    {
                        Name = "Pullman Danang Beach Resort",
                        Slug = "pullman-danang-beach-resort",
                        Description = "Khu nghỉ dưỡng biển quốc tế 5 sao nằm ngay trên bờ biển Bắc Mỹ An tuyệt đẹp, chỉ 10 phút di chuyển từ sân bay quốc tế Đà Nẵng.",
                        Address = "101 Đường Võ Nguyên Giáp, Phường Khuê Mỹ, Quận Ngũ Hành Sơn",
                        City = "Đà Nẵng",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0236 395 8888",
                        Email = "h8838@accor.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1540541338287-41700207dee6?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-20),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Superior Garden View",
                                RoomTypeId = supType.Id,
                                RoomNumber = "PL-204",
                                Description = "Phòng nghỉ tiện nghi nhìn ra khu vườn nhiệt đới xanh mát và hồ bơi uốn lượn.",
                                PricePerNight = 3200000,
                                Capacity = 2,
                                BedInfo = "1 Giường King hoặc 2 Giường Đơn",
                                AreaM2 = 42.0,
                                TotalQuantity = 18,
                                AvailableQuantity = 15,
                                ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=800&q=80"
                            },
                            new Room
                            {
                                Name = "Deluxe Ocean Cliff Room",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "PL-405",
                                Description = "Phòng ban công riêng ngắm sóng biển dạt dào, bồn tắm sâu và minibar phục vụ đồ uống tươi mát.",
                                PricePerNight = 4800000,
                                Capacity = 2,
                                BedInfo = "1 Giường King",
                                AreaM2 = 50.0,
                                TotalQuantity = 12,
                                AvailableQuantity = 10,
                                ImageUrl = "https://images.unsplash.com/photo-1595526114035-0d45ed16cfbf?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 9. TP. Ho Chi Minh - Sheraton
                    new Hotel
                    {
                        Name = "Sheraton Saigon Grand Opera Hotel",
                        Slug = "sheraton-saigon-grand-opera-hotel",
                        Description = "Khách sạn 5 sao cao cấp tọa lạc tại vị trí vàng trên đường Đồng Khởi sầm uất, liền kề Nhà Hát Thành Phố và phố đi bộ Nguyễn Huệ.",
                        Address = "88 Đường Đồng Khởi, Phường Bến Nghé, Quận 1",
                        City = "TP. Hồ Chí Minh",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0283 827 2828",
                        Email = "sheratonsaigon@sheraton.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-18),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Premier Deluxe City View",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "SH-1402",
                                Description = "Phòng nghỉ sang trọng nhìn ra đường Đồng Khởi rực rỡ ánh đèn hoa lệ, nệm Sheraton Signature Sleep Experience.",
                                PricePerNight = 4500000,
                                Capacity = 2,
                                BedInfo = "1 Giường King hoặc 2 Giường Đơn",
                                AreaM2 = 38.0,
                                TotalQuantity = 25,
                                AvailableQuantity = 22,
                                ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 10. Ninh Thuan - Amanoi
                    new Hotel
                    {
                        Name = "Amanoi Resort Vinh Hy",
                        Slug = "amanoi-resort-vinh-hy",
                        Description = "Khu nghỉ dưỡng 6 sao xa hoa bậc nhất Việt Nam thuộc tập đoàn Aman, nép mình giữa Vườn Quốc Gia Núi Chúa nhìn xuống Vịnh Vĩnh Hy tuyệt sắc.",
                        Address = "Thôn Vĩnh Hy, Xã Vĩnh Hải, Huyện Ninh Hải",
                        City = "Ninh Thuận",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0259 377 0777",
                        Email = "amanoi.res@aman.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-15),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Lake Pavilion Zen Villa",
                                RoomTypeId = exeType.Id,
                                RoomNumber = "AM-01",
                                Description = "Biệt thự thiền định nổi trên mặt hồ sen tĩnh lặng, phòng xông hơi riêng và không gian yoga biệt lập.",
                                PricePerNight = 28000000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Cực Lớn",
                                AreaM2 = 125.0,
                                TotalQuantity = 4,
                                AvailableQuantity = 4,
                                ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 11. Nha Trang - Melia Vinpearl
                    new Hotel
                    {
                        Name = "Melia Vinpearl Nha Trang Empire",
                        Slug = "melia-vinpearl-nha-trang-empire",
                        Description = "Tòa nhà khách sạn 41 tầng hiện đại ngay trung tâm phố biển Nha Trang, hồ bơi vô cực tầng 6 ngắm trọn vịnh Nha Trang xanh ngát.",
                        Address = "44-46 Đường Lê Thánh Tôn, Lộc Thọ",
                        City = "Nha Trang",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0258 359 9999",
                        Email = "melia.empire@vinpearl.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-12),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Deluxe Bay View Suite",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "ML-1808",
                                Description = "Căn hộ khách sạn đầy đủ khu bếp mini, phòng khách thoáng đãng và ban công ngắm biển.",
                                PricePerNight = 2600000,
                                Capacity = 2,
                                BedInfo = "1 Giường King",
                                AreaM2 = 45.0,
                                TotalQuantity = 25,
                                AvailableQuantity = 20,
                                ImageUrl = "https://images.unsplash.com/photo-1598928506311-c55ded91a20c?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    },

                    // 12. Hue - Banyan Tree Lang Co
                    new Hotel
                    {
                        Name = "Banyan Tree Lang Co Resort",
                        Slug = "banyan-tree-lang-co-resort",
                        Description = "Khu nghỉ dưỡng biệt thự biệt lập với hồ bơi riêng, lấy cảm hứng từ nghệ thuật chạm khắc cung đình Huế bên vịnh Lăng Cô thơ mộng.",
                        Address = "Xã Lộc Vĩnh, Huyện Phú Lộc",
                        City = "Huế",
                        Country = "Việt Nam",
                        StarRating = 5,
                        Phone = "0234 369 5888",
                        Email = "langco@banyantree.com",
                        MainImageUrl = "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?auto=format&fit=crop&w=1200&q=80",
                        IsFeatured = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        Rooms = new List<Room>
                        {
                            new Room
                            {
                                Name = "Lagoon Pool Villa",
                                RoomTypeId = dlxType.Id,
                                RoomNumber = "BT-06",
                                Description = "Biệt thự thanh bình bên đầm phá tự nhiên, có hồ bơi riêng, sân tắm nắng và bồn tắm phong cách Á Đông.",
                                PricePerNight = 9200000,
                                Capacity = 2,
                                BedInfo = "1 Giường King Cỡ Lớn",
                                AreaM2 = 131.0,
                                TotalQuantity = 10,
                                AvailableQuantity = 8,
                                ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=800&q=80"
                            }
                        }
                    }
                };

                await context.Hotels.AddRangeAsync(hotels);
                await context.SaveChangesAsync();

                // Gán Amenities cho từng khách sạn
                foreach (var hotel in hotels)
                {
                    // Lấy ngẫu nhiên hoặc gán 6-8 tiện ích hàng đầu
                    var hotelAmenities = allAmenities.Take(8).Select(a => new HotelAmenity
                    {
                        HotelId = hotel.Id,
                        AmenityId = a.Id
                    }).ToList();

                    await context.HotelAmenities.AddRangeAsync(hotelAmenities);

                    // Thêm ảnh bổ sung cho khách sạn
                    var extraImages = new List<HotelImage>
                    {
                        new() { HotelId = hotel.Id, ImageUrl = hotel.MainImageUrl, Caption = "Mặt tiền khách sạn", IsPrimary = true },
                        new() { HotelId = hotel.Id, ImageUrl = "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?auto=format&fit=crop&w=800&q=80", Caption = "Khuôn viên & Hồ bơi", IsPrimary = false },
                        new() { HotelId = hotel.Id, ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80", Caption = "Nhà hàng sang trọng", IsPrimary = false }
                    };
                    await context.HotelImages.AddRangeAsync(extraImages);
                }
                await context.SaveChangesAsync();
            }

            // 6. Seed Sample Reviews
            if (!context.Reviews.Any())
            {
                var hotels = await context.Hotels.Take(4).ToListAsync();
                var reviews = new List<Review>();

                if (hotels.Count > 0 && customerUser != null)
                {
                    reviews.Add(new Review
                    {
                        HotelId = hotels[0].Id,
                        UserId = customerUser.Id,
                        CustomerName = customerUser.FullName,
                        Rating = 5,
                        Title = "Kỳ nghỉ tuyệt vời không thể nào quên!",
                        Comment = "Dịch vụ 5 sao đích thực, nhân viên phục vụ tận tình chu đáo. Phòng ốc sạch sẽ, view biển đẹp ngỡ ngàng. Chắc chắn sẽ quay lại!",
                        Status = "Approved",
                        CreatedAt = DateTime.UtcNow.AddDays(-14)
                    });
                }

                if (hotels.Count > 1 && customer2User != null)
                {
                    reviews.Add(new Review
                    {
                        HotelId = hotels[1].Id,
                        UserId = customer2User.Id,
                        CustomerName = customer2User.FullName,
                        Rating = 5,
                        Title = "Trải nghiệm đẳng cấp, đồ ăn ngon tuyệt",
                        Comment = "Buffet sáng rất đa dạng món Á - Âu, hồ bơi tuyệt đẹp, không gian yên bình cho gia đình nghỉ dưỡng cuối tuần.",
                        Status = "Approved",
                        CreatedAt = DateTime.UtcNow.AddDays(-7)
                    });
                }

                if (hotels.Count > 2)
                {
                    reviews.Add(new Review
                    {
                        HotelId = hotels[2].Id,
                        CustomerName = "Lê Hoàng Nam",
                        Rating = 5,
                        Title = "View từ tầng cao ngắm Sài Gòn đỉnh chóp",
                        Comment = "Khách sạn sạch sẽ, sang trọng, vị trí trung tâm di chuyển rất tiện. Nhân viên thân thiện và chu đáo.",
                        Status = "Approved",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    });
                }

                if (reviews.Any())
                {
                    await context.Reviews.AddRangeAsync(reviews);
                    await context.SaveChangesAsync();
                }
            }

            // 7. Seed Sample Bookings & Payments
            if (!context.Bookings.Any())
            {
                var firstHotel = await context.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync();
                if (firstHotel != null && firstHotel.Rooms.Any() && customerUser != null)
                {
                    var room = firstHotel.Rooms.First();
                    var checkIn = DateTime.Today.AddDays(3);
                    var checkOut = DateTime.Today.AddDays(5);
                    int nights = 2;
                    decimal subTotal = room.PricePerNight * nights * 1;
                    decimal tax = subTotal * 0.08m;
                    decimal fee = subTotal * 0.05m;
                    decimal total = subTotal + tax + fee;

                    var booking1 = new Booking
                    {
                        BookingCode = "HB-20260819-A8F2",
                        UserId = customerUser.Id,
                        CustomerName = customerUser.FullName,
                        CustomerEmail = customerUser.Email!,
                        CustomerPhone = customerUser.PhoneNumber ?? "0912345678",
                        CustomerAddress = customerUser.Address,
                        HotelId = firstHotel.Id,
                        CheckInDate = checkIn,
                        CheckOutDate = checkOut,
                        TotalGuests = 2,
                        TotalRooms = 1,
                        SubTotal = subTotal,
                        TaxAmount = tax,
                        ServiceFee = fee,
                        TotalAmount = total,
                        Status = "Confirmed",
                        Notes = "Yêu cầu phòng tầng cao, giường đôi sạch sẽ.",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        BookingDetails = new List<BookingDetail>
                        {
                            new BookingDetail
                            {
                                RoomId = room.Id,
                                RoomName = room.Name,
                                PricePerNight = room.PricePerNight,
                                Quantity = 1,
                                Nights = nights,
                                SubTotal = subTotal
                            }
                        },
                        Payment = new Payment
                        {
                            PaymentCode = "PAY-VNPay-998822",
                            Amount = total,
                            PaymentMethod = "VNPay",
                            Status = "Completed",
                            TransactionRef = "VNP14892301",
                            PaidAt = DateTime.UtcNow.AddDays(-2),
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        }
                    };

                    // Sample Booking 2 (Pending)
                    var booking2 = new Booking
                    {
                        BookingCode = "HB-20260819-B1C4",
                        UserId = customer2User?.Id,
                        CustomerName = customer2User?.FullName ?? "Trần Thị Mai Hương",
                        CustomerEmail = customer2User?.Email ?? "demo.user@hotelbooking.com",
                        CustomerPhone = customer2User?.PhoneNumber ?? "0987654321",
                        CustomerAddress = "TP. Hồ Chí Minh",
                        HotelId = firstHotel.Id,
                        CheckInDate = DateTime.Today.AddDays(7),
                        CheckOutDate = DateTime.Today.AddDays(10),
                        TotalGuests = 2,
                        TotalRooms = 1,
                        SubTotal = room.PricePerNight * 3,
                        TaxAmount = (room.PricePerNight * 3) * 0.08m,
                        ServiceFee = (room.PricePerNight * 3) * 0.05m,
                        TotalAmount = (room.PricePerNight * 3) * 1.13m,
                        Status = "Pending",
                        Notes = "Đến nhận phòng sau 14h.",
                        CreatedAt = DateTime.UtcNow.AddHours(-5),
                        BookingDetails = new List<BookingDetail>
                        {
                            new BookingDetail
                            {
                                RoomId = room.Id,
                                RoomName = room.Name,
                                PricePerNight = room.PricePerNight,
                                Quantity = 1,
                                Nights = 3,
                                SubTotal = room.PricePerNight * 3
                            }
                        },
                        Payment = new Payment
                        {
                            PaymentCode = "PAY-MoMo-447711",
                            Amount = (room.PricePerNight * 3) * 1.13m,
                            PaymentMethod = "MoMo",
                            Status = "Pending",
                            TransactionRef = "MOMO778899",
                            CreatedAt = DateTime.UtcNow.AddHours(-5)
                        }
                    };

                    await context.Bookings.AddRangeAsync(booking1, booking2);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
