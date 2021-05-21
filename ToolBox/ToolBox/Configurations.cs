namespace ToolBox
{
  public class Configurations
  {
    public static IStartUpCnfiguration Create(params string[] args)
    {
      if (args == null || args.Length == 0)
        return new UiStartUpConfiguration();
      return new ConsoleStartUpConfiguration(args);
    }
  }
}
