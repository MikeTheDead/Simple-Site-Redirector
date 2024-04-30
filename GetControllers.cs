using altsite.Controllers;

namespace altsite;

public class GetControllers
{
    public static HomeController home;
    public static MainController site;
    public GetControllers(HomeController _home, MainController _site)
    {
        home = _home;
        site = _site;
    }
}