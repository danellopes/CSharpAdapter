using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Autofac;
using Autofac.Features.Metadata;

public interface ICommand
{
    void Execute();
}

class SaveCommand : ICommand
{
    public void Execute()
    {
        System.Console.WriteLine("Saving a file...");
    }
}

class OpenCommand : ICommand
{
    public void Execute()
    {
        System.Console.WriteLine("Opening a file...");
    }
}

public class Button
{
    private ICommand command;
    private string name;

    public Button(ICommand command, string name)
    {
        if (command == null)
        {
            throw new ArgumentNullException("command");
        }

        this.command = command;
        this.name = name;
    }

    public void Click()
    {
        command.Execute();
    }

    public void PrintMe()
    {
        System.Console.WriteLine($"I am a button called {name}");
    }
}

public class Editor
{
    private IEnumerable<Button> buttons;

    public Editor(IEnumerable<Button> buttons)
    {
        if (buttons == null)
        {
            throw new ArgumentNullException("buttons");
        }

        this.Buttons = buttons;
    }

    public IEnumerable<Button> Buttons { get => buttons; set => buttons = value; }

    public void ClickAll()
    {
        foreach (var btn in Buttons)
        {
            btn.Click();
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var b = new ContainerBuilder();
        b.RegisterType<SaveCommand>().As<ICommand>().WithMetadata("Name", "Save");
        b.RegisterType<OpenCommand>().As<ICommand>().WithMetadata("Name", "Open");
        b.RegisterAdapter<Meta<ICommand>, Button>(cmd => new Button(cmd.Value, (string)cmd.Metadata["Name"]));
        b.RegisterType<Editor>();

        using (var c = b.Build())
        {
            var editor = c.Resolve<Editor>();

            foreach (var btn in editor.Buttons)
            {
                btn.PrintMe();
            }
        }
    }
}