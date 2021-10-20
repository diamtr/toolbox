using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace ToolBox.Desktop.Base
{
  public class SettingsContext : DbContext
  {
    public DbSet<Setting> Settings { get; set; }

    public string DbPath { get; private set; }

    public SettingsContext()
    {
      this.DbPath = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}settings.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      options.UseSqlite($"DataSource={this.DbPath}");
    }
  }
}
