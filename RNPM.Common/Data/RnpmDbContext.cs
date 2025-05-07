using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RNPM.Common.Interfaces;
using RNPM.Common.Models;
using Serilog;

namespace RNPM.Common.Data;

public class RnpmDbContext: IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        string,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken>,
        IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILoggerFactory _loggerFactory;
    
    public RnpmDbContext(DbContextOptions<RnpmDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService,
        ILoggerFactory loggerFactory)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
        _loggerFactory = loggerFactory;
    }
    
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<IdentityClaim> Claims { get; set; }
    public DbSet<ApplicationUserRole> UserRoles { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<NetworkRequest> NetworkRequests { get; set; }
    public DbSet<HttpRequestInstance> HttpRequestInstances { get; set; }
    public DbSet<Navigation> Navigations { get; set; }
    public DbSet<NavigationInstance> NavigationInstances { get; set; }
    public DbSet<ScreenComponent> ScreenComponents { get; set; }
    public DbSet<ScreenComponentRender> ScreenComponentRenders { get; set; }
    public DbSet<OptimizationSuggestion> OptimizationSuggestions { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case 
                        EntityState.Added:
                        if (!string.IsNullOrEmpty(_currentUserService.UserId)) entry.Entity.CreatorId = _currentUserService.UserId;
                        entry.Entity.CreatedDate = _dateTimeService.Now;
                        break;
                    case EntityState.Modified:
                        if (!string.IsNullOrEmpty(_currentUserService.UserId)) entry.Entity.ModifiedBy = _currentUserService.UserId;
                        entry.Entity.ModifiedDate = _dateTimeService.Now;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            throw;
        }
    }

    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //SeedRoles(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            // Each User can have many UserClaims
            b.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            b.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            b.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            
            b.HasMany(e => e.Applications)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        builder.Entity<ApplicationRole>(b =>
        {
            // Each Role can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Each Role can have many associated RoleClaims
            b.HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        });
        
        builder.Entity<ApplicationUserLogin>(b =>
        {
            b.HasKey(l => l.UserId); // Assuming 'UserId' is the primary key property

            b.Property(l => l.UserId)
                .ValueGeneratedOnAdd(); // This configures 'UserId' to be auto-generated
        });
        
        builder.Entity<ApplicationUserRole>(b =>
        {
            b.HasKey(ur => new { ur.UserId, ur.RoleId });
        });
        
        builder.Entity<ApplicationUserToken>(b =>
        {
            b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
        });

        builder.Entity<IdentityClaim>(b =>
        {
            b.HasKey(c => c.Id);
        });
        
        builder.Entity<Application>(b =>
        {
            b.Property(a => a.Id)
                .ValueGeneratedOnAdd();
            b.HasMany(a => a.Navigations)
                .WithOne(n => n.Application)
                .HasForeignKey(n => n.ApplicationId);
            
            b.HasMany(a => a.NetworkRequests)
                .WithOne(n => n.Application)
                .HasForeignKey(n => n.ApplicationId);
            
            b.HasMany(a => a.ScreenComponents)
                .WithOne(n => n.Application)
                .HasForeignKey(n => n.ApplicationId);
        });
        
        builder.Entity<Navigation>(b =>
        {
            b.Property(n => n.Id)
                .ValueGeneratedOnAdd();
            b.HasMany(a => a.NavigationInstances)
                .WithOne(n => n.Navigation)
                .HasForeignKey(n => n.NavigationId);
        });
        
        builder.Entity<NetworkRequest>(b =>
        {
            b.Property(n => n.Id)
                .ValueGeneratedOnAdd();
            b.HasMany(a => a.HttpRequestInstances)
                .WithOne(n => n.NetworkRequest)
                .HasForeignKey(n => n.RequestId);
        });
        
        builder.Entity<ScreenComponent>(b =>
        {
            b.Property(n => n.Id)
                .ValueGeneratedOnAdd();
            b.HasMany(a => a.ScreenComponentRenders)
                .WithOne(n => n.ScreenComponent)
                .HasForeignKey(n => n.ComponentId);
            
        });

        builder.Entity<ScreenComponentRender>(b =>
        {
            b.Property(n => n.Id)
                .ValueGeneratedOnAdd();
            b.Property(p => p.RenderTime)
                .HasColumnType("decimal(18,6)");
        });
        
        builder.Entity<HttpRequestInstance>(b =>
        {
            b.Property(n => n.Id)
                .ValueGeneratedOnAdd();
            b.Property(p => p.RequestCompletionTime)
                .HasColumnType("decimal(18,3)");
        });
        builder.Entity<NavigationInstance>(b =>
        {
            b.Property(n => n.Id)
                .ValueGeneratedOnAdd();
            b.Property(p => p.NavigationCompletionTime)
                .HasColumnType("decimal(18,3)");
        });


    }
}