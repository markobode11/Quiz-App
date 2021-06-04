using System.Linq;
using Domain.App;
using Domain.App.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.EF
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        //mvc controllers
        //dotnet aspnet-codegenerator controller -name SimplesController        -actions -m  Simple        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
        //dotnet aspnet-codegenerator controller -name ThingyController        -actions -m  UserThingy        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
        /*
        dotnet aspnet-codegenerator controller -name AnswerController        -actions -m  Answer        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
        dotnet aspnet-codegenerator controller -name AnswerableController        -actions -m  Answerable        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
        dotnet aspnet-codegenerator controller -name AppUserAnswerableController        -actions -m  AppUserAnswerable        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
        dotnet aspnet-codegenerator controller -name QuestionController        -actions -m  Question        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
        dotnet aspnet-codegenerator controller -name QuestionInAnswerableController        -actions -m  QuestionInAnswerable        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
        */

        //api controllers created
        // dotnet aspnet-codegenerator controller -name ThingyController     -m Domain.App.UserThingy     -actions -dc AppDbContext -outDir ApiControllers -api --useAsyncActions  -f

        /*
        dotnet aspnet-codegenerator controller -name AnswerController     -m Domain.App.Answer     -actions -dc AppDbContext -outDir ApiControllers -api --useAsyncActions  -f
        dotnet aspnet-codegenerator controller -name AnswerableController     -m Domain.App.Answerable     -actions -dc AppDbContext -outDir ApiControllers -api --useAsyncActions  -f
        dotnet aspnet-codegenerator controller -name AppUserAnswerableController     -m Domain.App.AppUserAnswerable     -actions -dc AppDbContext -outDir ApiControllers -api --useAsyncActions  -f
        dotnet aspnet-codegenerator controller -name QuestionController     -m Domain.App.Question     -actions -dc AppDbContext -outDir ApiControllers -api --useAsyncActions  -f
        dotnet aspnet-codegenerator controller -name QuestionInAnswerableController     -m Domain.App.QuestionInAnswerable     -actions -dc AppDbContext -outDir ApiControllers -api --useAsyncActions  -f
         */

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; } = default!;
        public DbSet<Answerable> Answerables { get; set; } = default!;
        public DbSet<AppUserAnswerable> AppUserAnswerables { get; set; } = default!;
        public DbSet<Question> Questions { get; set; } = default!;
        public DbSet<QuestionInAnswerable> QuestionInAnswerables { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //removing the cascade delete, can add back later if needed
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            
            modelBuilder.Entity<Question>()
                .HasMany(x => x.Answers)
                .WithOne(x => x.Question!)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Question>()
                .HasMany(x => x.QuestionInAnswerables)
                .WithOne(x => x.Question!)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Answerable>()
                .HasMany(x => x.Questions)
                .WithOne(x => x.Answerable!)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Answerable>()
                .HasMany(x => x.AppUserAnswerables)
                .WithOne(x => x.Answerable!)
                .OnDelete(DeleteBehavior.Cascade);
            
            
        }
    }
}