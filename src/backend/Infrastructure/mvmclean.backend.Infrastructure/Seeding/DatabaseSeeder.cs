using MediatR;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Contractor.Commands;
using Microsoft.Extensions.Logging;

namespace mvmclean.backend.Infrastructure.Seeding;

public class DatabaseSeeder
{
    private readonly IMediator _mediator;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly string _seederFilePath;

    public DatabaseSeeder(IMediator mediator, ILogger<DatabaseSeeder> logger)
    {
        _mediator = mediator;
        _logger = logger;
        
    }

    public async Task SeedAsync()
    {
        if (false)
        {
            _logger.LogInformation("Database has already been seeded. Skipping seeding.");
            return;
        }

        try
        {
            _logger.LogInformation("Starting database seeding...");

            // Seed services and get their IDs
            var serviceIds = await SeedServicesAsync();

            // Seed contractors and get their IDs
            var contractorIds = await SeedContractorsAsync();

            // Seed contractor coverage areas
            await SeedCoverageAreasAsync(contractorIds);

            // Seed working hours
            await SeedWorkingHoursAsync(contractorIds);

            // Seed unavailability slots
            await SeedUnavailabilitySlotsAsync(contractorIds);

            // Mark as seeded
            File.WriteAllText(_seederFilePath, DateTime.UtcNow.ToString("o"));

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding the database.");
            throw;
        }
    }

    private async Task<List<Guid>> SeedServicesAsync()
    {
        try
        {
            _logger.LogInformation("Seeding services...");

            var serviceIds = new List<Guid>();

            // Carpet Cleaning Services
            var carpetCleaningServices = new List<CreateServiceRequest>
            {
                new CreateServiceRequest
                {
                    Name = "1 Bed Carpet Cleaning",
                    Description = "Professional deep cleaning for 1 bedroom carpets including living room and hallway",
                    Shortcut = "1BED_CARPET",
                    BasePrice = 79.99m,
                    EstimatedDurationMinutes = 60,
                    Category = "Carpet Cleaning"
                },
                new CreateServiceRequest
                {
                    Name = "2 Bed Carpet Cleaning",
                    Description = "Professional deep cleaning for 2 bedroom carpets including living room, hallway and stairs",
                    Shortcut = "2BED_CARPET",
                    BasePrice = 119.99m,
                    EstimatedDurationMinutes = 90,
                    Category = "Carpet Cleaning"
                },
                new CreateServiceRequest
                {
                    Name = "3 Bed Carpet Cleaning",
                    Description = "Professional deep cleaning for 3 bedroom carpets including all living areas and stairs",
                    Shortcut = "3BED_CARPET",
                    BasePrice = 159.99m,
                    EstimatedDurationMinutes = 120,
                    Category = "Carpet Cleaning"
                }
            };

            // Sofa Cleaning Services
            var sofaCleaningServices = new List<CreateServiceRequest>
            {
                new CreateServiceRequest
                {
                    Name = "1 Seater Sofa Cleaning",
                    Description = "Deep clean and sanitize for 1 seater sofas including fabric protection",
                    Shortcut = "1SEAT_SOFA",
                    BasePrice = 49.99m,
                    EstimatedDurationMinutes = 45,
                    Category = "Sofa Cleaning"
                },
                new CreateServiceRequest
                {
                    Name = "2 Seater Sofa Cleaning",
                    Description = "Deep clean and sanitize for 2 seater sofas including fabric protection",
                    Shortcut = "2SEAT_SOFA",
                    BasePrice = 69.99m,
                    EstimatedDurationMinutes = 60,
                    Category = "Sofa Cleaning"
                },
                new CreateServiceRequest
                {
                    Name = "3 Seater Sofa Cleaning",
                    Description = "Deep clean and sanitize for 3 seater sofas including fabric protection",
                    Shortcut = "3SEAT_SOFA",
                    BasePrice = 89.99m,
                    EstimatedDurationMinutes = 75,
                    Category = "Sofa Cleaning"
                }
            };

            // Combine all services
            var allServices = carpetCleaningServices.Concat(sofaCleaningServices).ToList();

            foreach (var service in allServices)
            {
                try
                {
                    var response = await _mediator.Send(service);
                    serviceIds.Add(Guid.Parse(response.ServiceId));
                    _logger.LogInformation($"Service created: {service.Name} (ID: {response.ServiceId})");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Service '{service.Name}' may already exist or error occurred: {ex.Message}");
                }
            }

            _logger.LogInformation("Services seeding completed.");
            return serviceIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding services.");
            throw;
        }
    }

    private async Task<List<string>> SeedContractorsAsync()
    {
        try
        {
            _logger.LogInformation("Seeding contractors...");

            var contractorIds = new List<string>();

            var contractors = new List<CreateContractorRequest>
            {
                new CreateContractorRequest
                {
                    FirstName = "Sait",
                    LastName = "Han Uzun",
                    PhoneNumber = "07700123456",
                    Email = "sait.hanuzun@example.com",
                    Username = "saithanuzun",
                    Password = "SecurePassword123!",
                    ImageUrl = null
                },
                new CreateContractorRequest
                {
                    FirstName = "Ahmed",
                    LastName = "Hassan",
                    PhoneNumber = "07701234567",
                    Email = "ahmed.hassan@example.com",
                    Username = "ahmedhassan",
                    Password = "SecurePassword123!",
                    ImageUrl = null
                },
                new CreateContractorRequest
                {
                    FirstName = "Emily",
                    LastName = "Thompson",
                    PhoneNumber = "07702345678",
                    Email = "emily.thompson@example.com",
                    Username = "emilythompson",
                    Password = "SecurePassword123!",
                    ImageUrl = null
                }
            };

            foreach (var contractor in contractors)
            {
                try
                {
                    var response = await _mediator.Send(contractor);
                    contractorIds.Add(response.ContractorId.ToString());
                    _logger.LogInformation($"Contractor created: {contractor.FirstName} {contractor.LastName} (ID: {response.ContractorId})");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Contractor '{contractor.FirstName} {contractor.LastName}' may already exist or error occurred: {ex.Message}");
                }
            }

            _logger.LogInformation("Contractors seeding completed.");
            return contractorIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding contractors.");
            throw;
        }
    }

    private async Task SeedCoverageAreasAsync(List<string> contractorIds)
    {
        try
        {
            _logger.LogInformation("Seeding coverage areas...");

            // Coverage areas for each contractor - system automatically matches by first 2 characters
            var coverageAreaMap = new Dictionary<int, List<string>>
            {
                // Sait Han Uzun covers London postcodes
                { 0, new List<string> 
                { 
                    "SW1A 0AA", "SW1A 1AA", "SW1A 2AA",
                    "N1 1AA", "N1 2AA", "N1 3AA",
                    "E1 6AN", "E1 7AA", "E1 8AA",
                    "W1A 1AA", "W1A 2AA", "W1B 1AA"
                } },
                // Ahmed Hassan covers Leicester postcodes
                { 1, new List<string> 
                { 
                    "LE1 3RA", "LE1 4TA", "LE1 5AA",
                    "LE2 0AA", "LE2 1AA", "LE2 2AA",
                    "LE3 1AA", "LE3 2AA",
                    "LE4 4AA", "LE4 5AA",
                    "LE5 0AA", "LE5 1AA"
                } },
                // Emily Thompson covers both London and Leicester
                { 2, new List<string> 
                { 
                    "SW2 1AA", "SW2 2AA", "SW2 3AA",
                    "SW3 1AA", "SW3 2AA", "SW3 3AA",
                    "LE1 6AA", "LE1 7AA",
                    "LE2 3AA", "LE2 4AA"
                } }
            };

            for (int i = 0; i < contractorIds.Count && i < coverageAreaMap.Count; i++)
            {
                foreach (var postcode in coverageAreaMap[i])
                {
                    try
                    {
                        var request = new CreateContractorCoverageRequest
                        {
                            ContractorId = contractorIds[i],
                            Postcode = postcode
                        };

                        await _mediator.Send(request);
                        _logger.LogInformation($"Coverage area added: Contractor {contractorIds[i]} - Postcode {postcode}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Coverage area for postcode '{postcode}' may already exist: {ex.Message}");
                    }
                }
            }

            _logger.LogInformation("Coverage areas seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding coverage areas.");
            throw;
        }
    }

    private async Task SeedWorkingHoursAsync(List<string> contractorIds)
    {
        try
        {
            _logger.LogInformation("Seeding working hours...");

            // Working hours for all contractors: Monday to Friday, 8:30 AM to 6:30 PM
            var workingDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            var startTime = new TimeOnly(8, 30);
            var endTime = new TimeOnly(18, 30);

            foreach (var contractorId in contractorIds)
            {
                foreach (var day in workingDays)
                {
                    try
                    {
                        var request = new SetWorkingDaysRequest
                        {
                            ContractorId = contractorId,
                            DayOfWeek = day,
                            StartTime = startTime,
                            EndTime = endTime,
                            IsWorkingDay = true
                        };

                        await _mediator.Send(request);
                        _logger.LogInformation($"Working day set: Contractor {contractorId} - {day} ({startTime:HH:mm} - {endTime:HH:mm})");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Working day for {day} may already exist: {ex.Message}");
                    }
                }
            }

            _logger.LogInformation("Working hours seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding working hours.");
            throw;
        }
    }

    private async Task SeedUnavailabilitySlotsAsync(List<string> contractorIds)
    {
        try
        {
            _logger.LogInformation("Seeding unavailability slots...");

            // Sample unavailability slots for contractors
            foreach (var contractorId in contractorIds)
            {
                // Add some unavailability for next week
                var today = DateTime.UtcNow.Date;
                var unavailableStart = today.AddDays(3); // 3 days from now
                var unavailableEnd = unavailableStart.AddHours(4);

                try
                {
                    var request = new CreateUnavailabilityRequest
                    {
                        ContractorId = contractorId,
                        StartTime = unavailableStart,
                        EndTime = unavailableEnd
                    };

                    await _mediator.Send(request);
                    _logger.LogInformation($"Unavailability slot created: Contractor {contractorId} - {unavailableStart:yyyy-MM-dd HH:mm} to {unavailableEnd:yyyy-MM-dd HH:mm}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Unavailability slot may already exist: {ex.Message}");
                }
            }

            _logger.LogInformation("Unavailability slots seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding unavailability slots.");
            throw;
        }
    }
}
