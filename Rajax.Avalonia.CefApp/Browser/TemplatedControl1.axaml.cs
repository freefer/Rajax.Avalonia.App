using Avalonia.Controls.Primitives;

namespace Rajax.Avalonia.CefGlueApp.Browser;

public class TemplatedControl1 : TemplatedControl
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var a=e.NameScope.Find("aa");
    }
}