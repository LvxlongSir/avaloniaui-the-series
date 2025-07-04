using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaMiaDev.ViewModels;
using AvaloniaMiaDev.ViewModels.SplitViewPane;
using AvaloniaMiaDev.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace AvaloniaMiaDev;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "No VM provided" };
        }

        var name = Assembly.GetExecutingAssembly().GetName().Name+".Views."+data.GetType().Name!
            .Replace("ViewModel", "View", StringComparison.Ordinal);

        var type = Type.GetType(name);

        if (type != null)
        {
            var obj  = Design.IsDesignMode
                ? Activator.CreateInstance(type)
                : Ioc.Default.GetService(type);
            return obj as Control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
