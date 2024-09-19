using Wpf.Ui;

// Summary
// This class creates a public INavigationService for use when navigating between pages via code.
// This does not impact navigation via the left navbar, but is necessary for navigating back and forth between pages (i.e. when a scan completes and it sends you back to the scan selection screen.

namespace SimpleAntivirus.GUI.Services
{
    public static class NavigationServiceIntermediary
    {
        public static INavigationService NavigationService { get; set; }
    }
}
