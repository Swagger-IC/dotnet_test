using Microsoft.AspNetCore.Components;

namespace Rise.Client.Layout
{
    public partial class Page
    {
        [Parameter, EditorRequired] public required string Title { get; set; }
        [Parameter] public RenderFragment? Actions { get; set; }
        [Parameter, EditorRequired] public required RenderFragment ChildContent { get; set; }
    }
}