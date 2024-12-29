# What's new in Version 2 | MauiReactor

MauiReactor 2 is the new version of MauiReactor targeting .NET 8. It contains some good improvements, many of which are under the hood.

If you're coming from version 1 there is good news: your code is mostly working as it is. There are just a couple of breaking changes you need to be aware of.

Source code: [https://github.com/adospace/reactorui-maui](https://github.com/adospace/reactorui-maui)
License: MIT (i.e. you can use it for your closed-source commercial projects)

Let's start with what is new.

## [Direct link to heading](\#a-new-simplified-way-to-write-components)    A new simplified way to write components

In Version 1, components are generally written with code like this:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CounterPageState
{
    public int Counter { get; set; }
}

class CounterPage : Component<CounterPageState>
{
    public override VisualNode Render()
    {
        return new ContentPage("Counter Sample")
        {
            new VStack(spacing: 10)
            {
                new Label($"Counter: {State.Counter}")
                    .VCenter()
                    .HCenter(),

                new Button("Click To Increment", () =>
                    SetState(s => s.Counter++))
            }
            .VCenter()
            .HCenter()
        };
    }
}
```

In Version 2, you _can_ use the new format that is less verbose and more easy to read

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CounterPageState
{
    public int Counter { get; set; }
}

class CounterPage : Component<CounterPageState>
{
    public override VisualNode Render()
        => ContentPage("Counter Sample",
            VStack(spacing: 10,
                Label($"Counter: {State.Counter}")
                    .HCenter(),

                Button("Click To Increment", () =>
                    SetState(s => s.Counter++))
            )
            .Center()
        );
}
```

The new code style is **optional**. You can use the other way to write your component or leave the code you may have already written as it will work in Version 2+.

## [Direct link to heading](\#prop-param-and-inject-attributes)    \[Prop\], \[Param\], and \[Inject\] attributes

Version 2 provides a few convenient source generators that should remove some boilerplate code in your components.

The PropAttribute lets specify a component prop without writing the method.

Before:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class BusyComponent : Component
{
    string _message;
    bool _isBusy;

    public BusyComponent Message(string message)
    {
        _message = message;
        return this;
    }

    public BusyComponent IsBusy(bool isBusy)
    {
        _isBusy = isBusy;
        return this;
    }
    ...
}
```

After (version 2+):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
partial class BusyComponent : Component
{
    [Prop]
    string _message;
    [Prop]
    bool _isBusy;

    ...
}
```

Please note that the component class must be partial to use the new feature.

The `ParamAttribute ` allows you to specify a parameter that is inherited from the parent component without the boilerplate code `Create/GetParameter`.

Before

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CustomParameter
{
    public int Numeric { get; set; }
}

class ParametersPage: Component
{
    IParameter<CustomParameter> _customParameter;

    public ParametersPage()
    {
        _customParameter = CreateParameter<CustomParameter>();
    }

    ...
}

class ParameterChildComponent: Component
{
    IParameter<CustomParameter> _customParameter;

    public ParameterChildComponent()
    {
        _customParameter = GetParameter<CustomParameter>();
    }

    ...
}
```

After (version 2+):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CustomParameter
{
    public int Numeric { get; set; }
}

partial class ParametersPage: Component
{
    [Param]
    IParameter<CustomParameter> _customParameter;

    ...
}

class ParameterChildComponent: Component
{
    [Param]
    IParameter<CustomParameter> _customParameter;

    ...
}
```

Finally, you may also find useful the InjectAttribute that lets you remove some repetitive code to inject services from the DI container

Before:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MyComponent : Component
{
     IMyService _service;

     public MyComponent()
     {
         _service = Services.GetRequiredService<IMyService>();
     }

}
```

After (version 2+):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
partial class MyComponent : Component
{
     [Inject]
     IMyService _service;

}
```

All the new attributes are **optional** and require the component class to be partial.

## [Direct link to heading](\#breaking-changed-simplified-creation-of-inline-components)    Breaking Changed: Simplified creation of inline components

Version 2 provides a more straightforward method for creating Inline Components that is easier to read requiring less boiler code.

Version 2+ doesn't support the old way of creating Inline Component, modifying old code to the new format should be a matter of changing a few lines of code.

Before:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
static VisualNode RenderMudEntry(string label, Action<string> textChangedAction)
    => Component.Render(context =>
    {
        MauiControls.Entry _entryRef = null;
        var state = context.UseState<(bool IsFocused, bool IsFilled)>();

        return new Grid("Auto", "*")
        {
            new Entry(entryRef => _entryRef = entryRef)
                .OnAfterTextChanged(text =>
                {
                    state.Set(s => (s.IsFocused, IsFilled: !string.IsNullOrWhiteSpace(text)));
                    textChangedAction?.Invoke(text);
                })
                .VCenter()
                .OnFocused(()=>state.Set(s => (IsFocused: true, s.IsFilled)))
                .OnUnfocused(()=>state.Set(s => (IsFocused: false, s.IsFilled))),

            new Label(label)
                .OnTapped(() =>_entryRef?.Focus())
                .Margin(5,0)
                .HStart()
                .VCenter()
                .TranslationY(state.Value.IsFocused || state.Value.IsFilled ? -20 : 0)
                .ScaleX(state.Value.IsFocused || state.Value.IsFilled ? 0.8 : 1.0)
                .AnchorX(0)
                .TextColor(!state.Value.IsFocused || !state.Value.IsFilled ? Colors.Gray : Colors.Red)
                .WithAnimation(duration: 200),
        }
        .VCenter();
    });
```

After (version 2+):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
static VisualNode RenderMudEntry(string label, Action<string> textChangedAction)
    => Render<(bool IsFocused, bool IsFilled)>(state =>
    {
        MauiControls.Entry _entryRef = null;

        return Grid("Auto", "*",
            Entry(entryRef => _entryRef = entryRef)
                .OnAfterTextChanged(text =>
                {
                    state.Set(s => (s.IsFocused, IsFilled: !string.IsNullOrWhiteSpace(text)));
                    textChangedAction?.Invoke(text);
                })
                .VCenter()
                .OnFocused(()=>state.Set(s => (IsFocused: true, s.IsFilled)))
                .OnUnfocused(()=>state.Set(s => (IsFocused: false, s.IsFilled))),

            Label(label)
                .OnTapped(() =>_entryRef?.Focus())
                .Margin(5,0)
                .HStart()
                .VCenter()
                .TranslationY(state.Value.IsFocused || state.Value.IsFilled ? -20 : 0)
                .ScaleX(state.Value.IsFocused || state.Value.IsFilled ? 0.8 : 1.0)
                .AnchorX(0)
                .TextColor(!state.Value.IsFocused || !state.Value.IsFilled ? Colors.Gray : Colors.Red)
                .WithAnimation(duration: 200),
        )
        .VCenter();
    });
```

## [Direct link to heading](\#breaking-changed-reactor.maui.scaffoldgenerator-package-dismissed)    Breaking changed: Reactor.Maui.ScaffoldGenerator package dismissed

When you authored components wrapping native controls coming from external libraries you were required to install an additional packaged Reactor.Maui.ScaffoldGenerator.

This is no longer required as the source generators library is now directly linked by the main package Reactor.Maui.

[PreviousWhat is MauiReactor?](/mauireactor) [NextWhat's new in Version 3](/mauireactor/whats-new-in-version-3)

Last updated 5 months ago

---

# State-less Components | MauiReactor

Each MauiReactor page is composed of one or more `Component` s which is described by a series of `VisualNode` and/or other `Component` s organized in a tree.

The root component is the first created by the application in the `Program.cs` file with the call to the `UseMauiReactorApp<TComponent>()`.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiReactorApp<MainPage>(app =>
            {
                app.AddResource("Resources/Styles/Colors.xaml");
                app.AddResource("Resources/Styles/Styles.xaml");

                app.SetWindowsSpecificAssectDirectory("Assets");
            })
#if DEBUG
            .EnableMauiReactorHotReload()
#endif

            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
            });

        return builder.Build();
    }
```

The following code creates a component that renders a `ContentPage` with title "Home Page":

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    public override VisualNode Render()
     => ContentPage()
            .Title("Home Page");
}
```

Line 3. Every component must override the Render method and return the visual tree of the component

Line 5. The ContentPage visual node pairs with the ContentPage native control.

Line 6. The Title property sets the title of the page and updates the Title dependency property on the native page.

You can also pass the title to the Constructor:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    public override VisualNode Render()
      => ContentPage("Home Page");
}
```

Line 5. The title of the page is set by passing it to the `ContentPage` constructor.

Running the app you should see an empty page titled "Home Page"

You can build any complex UI in the render method of the component but often it's better to compose more than one component to create a page or app.

For example, consider this component:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    public override VisualNode Render()
        => ContentPage("Login",
            VStack(
                Label("User:"),
                Entry(),
                Label("Password:"),
                Entry(),
                HStack(
                    Button("Login"),
                    Button("Register")
                )
            )
            .Center()
        );
}
```

We could create a component like this:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
partial class EntryWithLabel : Component
{
    [Prop]
    string _label;

    public override VisualNode Render()
      => VStack(
            Label(_label),
            Entry()
        );
}
```

and reuse it on the main page as shown in the following code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    public override VisualNode Render()
        => ContentPage("Login",
            VStack(
                new EntryWithLabel()
                    .Label("User:"),
                new EntryWithLabel()
                    .Label("Password:"),
                HStack(
                    Button("Login"),
                    Button("Register")
                )
            )
            .Center()
        );
}
```

Reusing components is a key feature in MauiReactor: decomposing a large page into small components that are easier to test is also beneficial to the overall performance of the application.

[PreviousGetting Started](/mauireactor/getting-started) [NextStateful Components](/mauireactor/components/stateful-components)

Last updated 11 months ago

---

# Getting Started | MauiReactor

## [Direct link to heading](\#creating-a-new-mauireactor-project-from-cli)    Creating a new MauiReactor project from CLI

MauiReactor provides a convenient dotnet project template you can install to easily create a new dotnet maui project with MauiReactor bindings:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
dotnet new install Reactor.Maui.TemplatePack
```

To create a new project just issue

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
dotnet new maui-reactor-startup -o my-new-project
```

To build the project just move inside the project directory and run the usual dotnet build command like this (in the example below we'll use the android target, the same applies to the other targets too of course: net8.0-ios\|net8.0-maccatalyst\|windows10.0.19041.0):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
cd .\my-new-project\
dotnet build -f net8.0-android
```

To run the app under the Android platform execute the following command:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
dotnet build -t:Run -f net8.0-android
```

You can run the ios app under MAC with the command:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
dotnet build -t:Run /p:_DeviceName=:v2:udid=<device_id> -f net8.0-ios
```

where the device\_id is the Guid of the device that should be targeted. To find the list of available devices with the corresponding IDs, run the command:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
xcrun simctl list
```

## [Direct link to heading](\#install-mauireactor-hot-reload-console)    Install MauiReactor hot-reload console

MauiReactor also provides a Hot-Reload console program that automatically hot reloads the project you are working on as you save changes to any file.

To install it, just type the following command in the terminal:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
dotnet tool install -g Reactor.Maui.HotReload
```

If you want to upgrade the tool to the latest version run the command:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
dotnet tool update -g Reactor.Maui.HotReload
```

## [Direct link to heading](\#mauireactor-hot-reload-console)    MauiReactor hot-reload console

MauiReactor hot reload can work in two different modes: **Simple** and **Full**

**Simple Mode (Default)**: This allows you to hot-reload the app as you save changes to the project. It's faster than Full Mode because it uses in-memory compilation but has some drawbacks:

1) Debugging is not supported: so for example, if you set a breakpoint it will not be hit after you hot-reload the app.
2) It's unable to build XAML files; so if you're planning to link XAML files for resources (like styles, brushes, etc) you may need to switch to the Full mode.

**Full Mode**: This mode uses the full-blown MS Build task to build the project after a file is changed/renamed/added. It's slower than Simple Mode but you can thoroughly debug your app even when you have hot-reloaded it. Also, it works better if you have XAML files or references to managed libraries in your project.

To start the hot-reload console in **Simple Mode**:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
dotnet-maui-reactor -f [net8.0-android|net8.0-ios|net8.0-maccatalyst|windows10.0.19041.0]
```

This is the command to start it in **Full Mode**:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
dotnet-maui-reactor -f [net8.0-android|net8.0-ios|net8.0-maccatalyst|windows10.0.19041.0] --mode Full
```

This is the typical startup messages from the hot-reload tool:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
MauiReactor Hot-Reload CLI
Version 2.0.19.0
Press Ctrl+C or Ctrl+Break to quit
Setting up build pipeline for TrackizerApp project...done.
Monitoring folder 'C:\Source\github\mauireactor-samples\TrackizerApp'...
```

MauiReactor hot-reload keeps listening to file changes in the directory but doesn't run the application: you need to open another console to run the application or run/debug it using an IDE like Visual Studio.

Under MacOS, ensure that your app is NOT executed in "sandbox mode" otherwise, hot-reload won't work.

Go to Platforms->MacCatalyst->Entitlements.plist and set/update this value:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
        <key>com.apple.security.app-sandbox</key>
        <false/>
```

Please note that before publishing to the App Store this value must be set to true again.

You can also hot-reload applications running in an iOS emulator using Windows and Visual Studio: use the `-h/--host` command line arguments to set the remote Mac host.

For example:
`dotnet-maui-reactor -f net8.0-ios -h ip-of-the-mac`

For more info on how to debug iOS applications from Visual Studio under Windows please get a look at the .NET MAUI official documentation:
[https://learn.microsoft.com/en-us/dotnet/maui/ios/remote-simulator?view=net-maui-8.0](https://learn.microsoft.com/en-us/dotnet/maui/ios/remote-simulator?view=net-maui-8.0)

## [Direct link to heading](\#net-built-in-hot-reload)    .NET built-in hot-reload

Since version 1.0.116 MauiReactor also supports .NET built-in hot-reload. This feature is enabled by default when you call the `EnableMauiReactorHotReload()` method on your application builder.

To enable the hot-reload for MAUI projects and an updated list of supported edits please look at the official documentation [here](https://learn.microsoft.com/en-us/visualstudio/debugger/hot-reload?view=vs-2022).

## [Direct link to heading](\#create-a-new-project-in-visual-studio-2022)    Create a new project in Visual Studio 2022

After you have installed the dotnet project template you should see it in the Visual Studio project creation dialog:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252Fzh664AmGOSVI2KLBmhUX%252Fimage.png%3Falt%3Dmedia%26token%3Ddb728821-219d-43cb-9fc1-e595740fcf7a&width=768&dpr=4&quality=100&sign=29777c32&sv=2)

Select the MauiReactor based app template

## [Direct link to heading](\#create-a-new-project-in-visual-studio-2022-for-mac)    Create a new project in Visual Studio 2022 for Mac

After you have installed the dotnet project template you should see it in the Visual Studio project creation dialog:

Microsoft has deprecated Visual Studio 2022 for Mac. To create a .NET MAUI application on Mac please get a look at the .NET Maui extensions for VsCode ( [https://devblogs.microsoft.com/visualstudio/announcing-the-dotnet-maui-extension-for-visual-studio-code/](https://devblogs.microsoft.com/visualstudio/announcing-the-dotnet-maui-extension-for-visual-studio-code/))

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FXiyQZSwIXXVzEetypksG%252Fimage.png%3Falt%3Dmedia%26token%3D6caa8ff3-9af3-4bc7-acdb-88bbc1563807&width=768&dpr=4&quality=100&sign=ef9aee78&sv=2)

Select Other -> Custom -> MauiReactor based app

Hot-reloading of an Android application requires the presence of the **adb** tool.

Check the adb tool is installed and working by listing the device list with the command:

`adb devices`

If the command is not recognized then you could install it with `brew`:

1. Install the [homebrew](http://brew.sh/) package manager if not installed







Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install.sh)"
```

2. Install adb







Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
    brew install android-platform-tools
```

3. Start using adb







Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
    adb devices
```


## [Direct link to heading](\#migrate-from-the-default-maui-project-template)    Migrate from the default MAUI project template

It's fine to start from a standard MAUI project template: below we'll see what is required to migrate a brand-new project to MauiReactor. This short guide also helps to make a port from an existing MVVM project to MauiReactor.

#### [Direct link to heading](\#step-1)    Step 1

Include the latest version of the MauiReactor package (select the latest version):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<PackageReference Include="Reactor.Maui" Version="2.0.*" />
```

Even if not strictly required, I suggest removing the ImplicitUsings directive in the csproj file:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<ImplicitUsings>disable</ImplicitUsings>
```

and add a GlobalUsings.cs file containing these global usings:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
global using System;

global using Microsoft.Maui;
global using Microsoft.Maui.Hosting;
global using Microsoft.Maui.Graphics;
global using MauiControls = Microsoft.Maui.Controls;
```

This will avoid problems with namespacing conflicts between MAUI and MauiReactor.

#### [Direct link to heading](\#step-2)    Step 2

Remove the file App.xaml, AppShell.xaml, and MainPage.xaml (including the code-behind files).

#### [Direct link to heading](\#step-3)    Step 3

Create a MainPage component: add a HomePage.cs file in the project root folder with this code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    public override VisualNode Render()
    => ContentPage(
           Label("Hi!")
            .Center()
        );
}
```

#### [Direct link to heading](\#step-4)    Step 4

Finally, replace the Program.cs main content:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiReactorApp<MainPage>()
#if DEBUG
        .EnableMauiReactorHotReload() //enable hot reload only in debug
        .OnMauiReactorUnhandledException(ex =>
        {
            //log any MauiReactor error to the console (replace with your favorite issue tracker)
            System.Diagnostics.Debug.WriteLine(ex.ExceptionObject);
        })
#endif
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
        });

    return builder.Build();
}
```

## [Direct link to heading](\#best-practices)    Best practices

When the app is hot-reloaded, a new assembly is compiled on the fly and injected into the running app. This means that the new component lives in a different assembly from the original one. It's recommended to follow these best practices to avoid type mismatch issues:

- Component state class should contain only public properties whose types are value-type or string.

- If you need a component state with properties other than value-type/string (i.e. classes), host them in a separate assembly (project) so that it's not hot reloaded.


[PreviousWhat's new in Version 3](/mauireactor/whats-new-in-version-3) [NextState-less Components](/mauireactor/components/state-less-components)

Last updated 5 months ago

---

# What's new in Version 3 | MauiReactor

Version 3 of MauiReactor targets MAUI .NET 9. If you're using an earlier version of MauiReactor, please note the following short-step list of changes required to move forward to version 3.

MauiReactor 3 is still in beta, even if the external API surface has not changed much many parts of the code have been modified under the hood to enhance performance. If you want to experiment with the latest version please update your packages to the latest -beta version ( [https://www.nuget.org/packages/Reactor.Maui](https://www.nuget.org/packages/Reactor.Maui))

For an up-to-date list of new features for MauiReactor 3 please head to [https://github.com/adospace/reactorui-maui/issues/263](https://github.com/adospace/reactorui-maui/issues/263)

## [Direct link to heading](\#hot-reload-changes)    Hot-reload changes

Hot-reload must be enabled using the new Feature switch available in .NET 9 ( [https://github.com/dotnet/designs/blob/main/accepted/2020/feature-switch.md](https://github.com/dotnet/designs/blob/main/accepted/2020/feature-switch.md)).

Remove the EnableMauiReactorHotReload() call in `program.cs` :

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiReactorApp<HomePage>()
#if DEBUG
    .EnableMauiReactorHotReload()
#endif
    ...;

```

In the project definition add the following lines:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<ItemGroup Condition="'$(Configuration)'=='Debug'">
  <RuntimeHostConfigurationOption Include="MauiReactor.HotReload" Value="true" Trim="false" />
</ItemGroup>
```

## [Direct link to heading](\#xaml-resources)    XAML resources

Due to an internal change of the .NET Maui 9 framework, loading the resources (i.e. styles, brushes, etc) from XAML dictionaries is slightly less straightforward.

First, create an App.xaml and App.xaml.cs (or copy them from the standard .NET MAUI template) and link your resources as merged dictionaries of the app:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<?xml version = "1.0" encoding = "UTF-8" ?>
<local:MauiReactorApplication xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:MauiReactor.TestApp"
             x:Class="MauiReactor.TestApp.App">
    <local:MauiReactorApplication.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </local:MauiReactorApplication.Resources>
</local:MauiReactorApplication>
```

Ensure that the file build action is set to MauiXaml:
![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FAfjW7qLrDcPdXHJe4U5k%252Fimage.png%3Falt%3Dmedia%26token%3D28218d67-6508-4d99-96e3-4b03576b1342&width=300&dpr=4&quality=100&sign=d93cfa1a&sv=2)

In the App.xaml.cs file, create a derived class of the standard MAUI Application like the following:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public partial class App: ReactorApplication<HomePage>
{
    public App(IServiceProvider serviceProvider)
        :base(serviceProvider)
    {
        InitializeComponent();
    }
}
```

Note that the class injects the IServiceProvider object and passes it to the base class. `HomePage` class represents the root component of the application.

Finally, use the standard UseMauiApp method in your Program.cs file load the application:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    ...

```

If you need also to customize the MauiReactor application, i.e. also adding your c# styles from the [application theming](/mauireactor/components/theming) feature, you need to create an additional class that derives from ReactorApplication and set your application theme as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public partial class App : MauiReactorApplication
{
    public App(IServiceProvider serviceProvider)
        :base(serviceProvider)
    {
        InitializeComponent();
    }
}

public abstract class MauiReactorApplication : ReactorApplication<HomePage>
{
    public MauiReactorApplication(IServiceProvider serviceProvider)
        :base(serviceProvider)
    {
        this.UseTheme<AppTheme>();
    }
}
```

## [Direct link to heading](\#aot-compliance)    AOT compliance

MauiReactor 3 is fully AOT compatible: to compile your application to a native iOS or Mac Catalyst application you have to add these lines to your project definition:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<ItemGroup Condition="'$(Configuration)'=='Release'">
    <RuntimeHostConfigurationOption Include="MauiReactor.HotReload" Value="false" Trim="true" />
</ItemGroup>
```

Please follow official guidelines to enable the AOT compilation in your .NET 9 MAUI app: [https://learn.microsoft.com/en-us/dotnet/maui/deployment/nativeaot?view=net-maui-9.0](https://learn.microsoft.com/en-us/dotnet/maui/deployment/nativeaot?view=net-maui-9.0)

[PreviousWhat's new in Version 2](/mauireactor/whats-new-in-version-2) [NextGetting Started](/mauireactor/getting-started)

Last updated 3 days ago

---

# Component life-cycle | MauiReactor

Any MauiReactor application is composed of a logical tree of components. Each component renders its content using the overload function `Render()`. Inside it, the component can create a logical tree of Maui controls and/or other components.

It's important to understand how components are Created, Removed, or "Migrated" when a new logical tree is created after the current tree is invalidated (calling a method like `SetState` or `Invalidate`).

The key events in the life of a component are:

- **Mounted** (aka Created) is raised after the component is created and added to the logical tree.
It's the ideal place to initialize the state of the component, directly setting state properties or calling an external web service that will update the state. You can think of this event more or less like a class constructor.

- **WillUnmount** (aka Removing) is raised when the component is about to be removed from the logical tree. It's the perfect place to unsubscribe from service events or deallocate unmanaged resources (for example, use this overload to unsubscribe from a web socket event in a chat app).

- **PropsChanged** (aka Migrated) is raised when the component has been migrated to the next logical tree. It's the ideal place to verify whether it is required to update the state. Often this overload contains code similar to the code used for the Mounted event.


Each of these events is called under the main UI dispatcher thread so be sure to put any expensive call (for example calls to the network or file system) in a separate task using the async/await pattern.

To better understand how these events are fired we can create a sample like the following tracing down in the console when they are called:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPageState
{
    public bool Toggle { get; set; }
    public int CurrentValue { get; set; }
}

class MainPage : Component<MainPageState>
{
    public override VisualNode Render()
    =>  ContentPage(
            VStack(spacing: 5,
                Button($"Use {(!State.Toggle ? "increment" : "decrement")} button", ()=> SetState(s => s.Toggle = !s.Toggle)),
                State.Toggle ?
                    new IncrementalCounter()
                        .CurrentValue(State.CurrentValue)
                        .ValueChanged(v => SetState(s => s.CurrentValue = v))
                    :
                    new DecrementalCounter()
                        .CurrentValue(State.CurrentValue)
                        .ValueChanged(v => SetState(s => s.CurrentValue = v))
            )
            .Center()
        );
}

partial class IncrementalCounter : Component
{
    [Prop]
    int _currentValue;
    [Prop]
    private Action<int> _valueChanged;

    protected override void OnMounted()
    {
        Debug.WriteLine("[IncrementalCounter] OnMounted()");
        base.OnMounted();
    }

    protected override void OnPropsChanged()
    {
        Debug.WriteLine($"[IncrementalCounter] OnPropsChanged(_currentValue={_currentValue})");
        base.OnPropsChanged();
    }

    protected override void OnWillUnmount()
    {
        Debug.WriteLine("[IncrementalCounter] OnWillUnmount()");
        base.OnWillUnmount();
    }

    public override VisualNode Render()
    {
        Debug.WriteLine("[IncrementalCounter] Render()");

        return Button($"Increment from {_currentValue}!")
            .OnClicked(() => _valueChanged?.Invoke(++_currentValue));
    }
}

class DecrementalCounter : Component
{
    [Prop]
    int _currentValue;
    [Prop]
    private Action<int> _valueChanged;

    protected override void OnMounted()
    {
        Debug.WriteLine("[DecrementalCounter] OnMounted()");
        base.OnMounted();
    }

    protected override void OnPropsChanged()
    {
        Debug.WriteLine($"[DecrementalCounter] OnPropsChanged(_currentValue={_currentValue})");
        base.OnPropsChanged();
    }

    protected override void OnWillUnmount()
    {
        Debug.WriteLine("[DecrementalCounter] OnWillUnmount()");
        base.OnWillUnmount();
    }

    public override VisualNode Render()
    {
        Debug.WriteLine("[DecrementalCounter] Render()");

        return new Button($"Decrement from {_currentValue}!")
            .OnClicked(() => _valueChanged?.Invoke(--_currentValue));
    }
}
```

Running this code you should see an app like this:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252F8BgOJ7v96Akxde8G816C%252Fimage.png%3Falt%3Dmedia%26token%3Dd91b02a8-ee34-43ad-99f6-fa5b9f8fc82c&width=768&dpr=4&quality=100&sign=d671f59&sv=2)

Sample app tracing component life-cycle events

Playing a bit with it, you should be able to see tracing lines like the following that could help to understand how events are sequenced:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
[0:] [DecrementalCounter] OnMounted()
[0:] [DecrementalCounter] Render()

[0:] [DecrementalCounter] OnWillUnmount()
[0:] [IncrementalCounter] OnMounted()
[0:] [IncrementalCounter] Render()

[0:] [IncrementalCounter] OnPropsChanged(_currentValue=1)
[0:] [IncrementalCounter] Render()

[0:] [IncrementalCounter] OnPropsChanged(_currentValue=2)
[0:] [IncrementalCounter] Render()

[0:] [IncrementalCounter] OnPropsChanged(_currentValue=3)
[0:] [IncrementalCounter] Render()
```

[PreviousInline Components](/mauireactor/components/stateful-components/inline-components) [NextComponent Properties](/mauireactor/components/component-properties)

Last updated 11 months ago

---

# Component with children | MauiReactor

A component class derives from `Component` and must implement the `Render()` method. Inside it, local fields, properties, and of course State properties of stateful components are directly accessible and can be used to compose the resulting view.

Components can also render their children by calling the base method `Children()`. This opens up a powerful feature that can be useful if we want to build a component that arranges its children in some way.

Say we want, for example, to create a component that arranges its children within a customizable grid, like this:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FBo1TebQIru1hEzuB6XIJ%252FReactorUI_ComponentChildrenDemo.gif%3Falt%3Dmedia%26token%3Da2a2cdf0-a34d-4594-aef8-ac9e8d180135&width=768&dpr=4&quality=100&sign=2c1ab917&sv=2)

To start, let's create a component that builds our page:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class PageComponent : Component
{
    public override VisualNode Render()
        => NavigationPage(
            ContentPage("Component With Children")
            );
}
```

This should show an empty page with just a title, then create a component for our grid (call it `WrapGrid`)

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class WrapGrid : Component
{
    public override VisualNode Render()
    {
    }
}
```

Every `Component` class can access its children using the `Children()` method (it's similar to the `{this.props.children}` property in ReactJS)

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class WrapGrid : Component
{
    public override VisualNode Render()
        => Grid(Children());
}

```

We can add a `ColumnCount` property and simple logic to arrange and wrap any children passed to the component like this:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
partial class WrapGrid : Component
{
    [Prop]
    private int _columnCount = 4;

    public override VisualNode Render()
    {
        int rowIndex = 0, colIndex = 0;

        int rowCount = Math.DivRem(Children().Count, _columnCount, out var divRes);
        if (divRes > 0)
            rowCount++;

        return new Grid(
            Enumerable.Range(1, rowCount).Select(_ => new RowDefinition() { Height = GridLength.Auto }),
            Enumerable.Range(1, _columnCount).Select(_ => new ColumnDefinition()))
        {
            Children().Select(child =>
            {
                child.GridRow(rowIndex);
                child.GridColumn(colIndex);

                colIndex++;
                if (colIndex == _columnCount)
                {
                    colIndex = 0;
                    rowIndex++;
                }

                return child;
            }).ToArray()
        };
    }
}
```

Finally, we just need to create the component from the main page and fill it with a list of child buttons:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class PageState
{
    public int ColumnCount { get; set; } = 1;

    public int ItemCount { get; set; } = 3;
}

class PageComponent : Component<PageState>
{
    public override VisualNode Render()
    {
        return new NavigationPage()
        {
            new ContentPage()
            {
                new StackLayout()
                {
                    new Label($"Columns {State.ColumnCount}")
                        .FontSize(14),
                    new Stepper()
                        .Minimum(1)
                        .Maximum(10)
                        .Increment(1)
                        .Value(State.ColumnCount)
                        .OnValueChanged(_=> SetState(s => s.ColumnCount = (int)_.NewValue)),
                    new Label($"Items {State.ItemCount}")
                        .FontSize(Xamarin.Forms.NamedSize.Large),
                    new Stepper()
                        .Minimum(1)
                        .Maximum(20)
                        .Increment(1)
                        .Value(State.ItemCount)
                        .OnValueChanged(_=> SetState(s => s.ItemCount = (int)_.NewValue)),

                    new WrapGrid()
                    {
                        Enumerable.Range(1, State.ItemCount)
                            .Select(_=> new Button($"Item {_}"))
                            .ToArray()
                    }
                    .ColumnCount(State.ColumnCount)
                }
                .Padding(10)
                .WithVerticalOrientation()
            }
            .Title("Component With Children")
        };
    }
}
```

[PreviousComponent Properties](/mauireactor/components/component-properties) [NextComponent Parameters](/mauireactor/components/component-parameters)

Last updated 11 months ago

---

# Stateful Components | MauiReactor

A stateful component is a component tied to a state class that is used to keep its "state" during its lifetime.

A state is just a C# class with an empty constructor.

When a Component is first displayed on the page, i.e. the MAUI widget is added to the page visual tree, MauiReactor calls the method `OnMounted()`.

Before the component is removed from the page visual tree MauiReactor calls the `OnWillUnmount()` method.

Every time a Component is "migrated" (i.e. it is preserved between a state change) the `OnPropsChanged()` overload is called.

`OnMounted()` is the ideal point to initialize the component, for example calling web services or querying the local database to get the required information to render it in the `Render()` method.

For example, in this code we'll show an activity indicator while the Component is loading:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class BusyPageState
{
    public bool IsBusy { get; set; }
}

public class BusyPageComponent : Component<BusyPageState>
{
    protected override void OnMounted()
    {
        //Here is not advisable to call SetState() as the component is still not rendered yet
        State.IsBusy = true;

        //just for a test run a background task
        Task.Run(async () =>
        {
            //Simulate lengthy work
            await Task.Delay(3000);

            //finally reset state IsBusy property
            SetState(_ => _.IsBusy = false);
        });

        base.OnMounted();
    }

    public override VisualNode Render()
        => ContentPage(
            ActivityIndicator()
                .Center()
                .IsRunning(State.IsBusy)
        );
}

```

and this is the resulting app:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FLJia9MY0HojW9JgRpnDw%252FReactorUI_BusyDemo.gif%3Falt%3Dmedia%26token%3Dcb4ce957-b405-45ba-9944-99af9619a88a&width=768&dpr=4&quality=100&sign=7090b8ce&sv=2)

Do not use constructors to pass parameters to the component, but public properties instead (take a look at the [Components Properties](/mauireactor/components/component-properties) documentation page).

## [Direct link to heading](\#updating-the-component-state)    Updating the component State

When you need to update the state of your component you have to call the `SetState` method as shown above.

When you call `SetState` the component is marked as _Invalid_ and MauiReactor triggers a refresh of the component. This happens following a series of steps in a fixed order

1. The component is marked as _Invalid_

2. The parent and ancestors up to the root component of the page are all marked as _Invalid_

3. MauiReactor triggers a refresh under the UI thread that creates a new Visual tree traversing the component tree

4. All the components that are _Valid_ are re-used (maintained in the VisualTree) while the components marked as _Invalid_ are discarded and a new version is created and its `Render` method called

5. The new component version creates a new tree of child nodes/components that are compared with the tree linked to the old version of the component

6. The old visual tree is compared to the new one: new nodes are created along with the native control (i.e. are mounted), removed nodes are eliminated along with the native control (i.e. are unmounted), and finally, nodes that are only changed (i.e. old and new nodes are of the same type) are migrated (i.e. native control is reused and its properties are updated according to properties of the new visual node)

7. In the end, the native controls are added, removed, or updated


For example, let's consider what happens when we tap the Increment button in the sample component below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CounterPageState
{
    public int Counter { get; set; }
}

class CounterPage : Component<CounterPageState>
{
    public override VisualNode Render()
    => ContentPage("Counter Sample",
            VStack(spacing: 10,
                Label($"Counter: {State.Counter}")
                    .Center(),

                Button("Click To Increment", () =>
                    SetState(s => s.Counter++))
            )
            .Center()
        );
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FodVu58HEAF90u4N1ixx9%252Fvisualtree.drawio.png%3Falt%3Dmedia%26token%3Daf7a874d-b96e-4be0-9443-758988fc6916&width=768&dpr=4&quality=100&sign=68040174&sv=2)

All components are migrated/updated

Let's now consider this revisited code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CounterPageState
{
    public int Counter { get; set; }
}

class CounterPage : Component<CounterPageState>
{
    public override VisualNode Render()
        => ContentPage("Counter Sample",
            VStack(spacing: 10,
                State.Counter == 0 ? new Label($"Counter: {State.Counter}")
                    .VCenter()
                    .HCenter() : null,

                Button("Click To Increment", () =>
                    SetState(s => s.Counter++))
            )
            .Center()
        );
}
```

When the button is clicked the variable `State.Counter` is updated to 1 so the component is re-rendered and the `Label` is umounted (i.e. removed from the visual tree) and the native control is removed from the parent `VStack` Control list (i.e. de-allocated):

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252Forh9gnoFmQQCROCiPFeE%252Fvisualtree.drawio%2520%281%29.png%3Falt%3Dmedia%26token%3D38886996-392c-48cd-823b-d4e2817eca93&width=768&dpr=4&quality=100&sign=96045181&sv=2)

Label is unmounted (i.e. removed from visual tree)

If we click the button again, the `Label` component is found, again, in the new version of the Tree, so it's mounted and a new instance of the `Label` component is created (along with the Native control that is created and added to the parent `VStack` control list).

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FeO37Awt0fr17S01IH7ED%252Fvisualtree.drawio%2520%282%29.png%3Falt%3Dmedia%26token%3Da81b7ac0-189f-4440-a216-92e6544f28e1&width=768&dpr=4&quality=100&sign=ed67a08&sv=2)

Label is mounted

## [Direct link to heading](\#updating-the-state-without-triggering-a-refresh)    Updating the state "without" triggering a refresh

Re-creating the visual tree can be expensive, especially if the component tree is deep or the components contain many nodes; but sometimes you can update the state "without" triggering a refresh of the tree resulting in a pretty good performance improvement.

For example, consider the counter sample but with a debug message added that helps trace when the component is rendered/created (line 10):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CounterPageState
{
    public int Counter { get; set; }
}

class CounterPage : Component<CounterPageState>
{
    public override VisualNode Render()
    {
        Debug.WriteLine("Render");
        return ContentPage("Counter Sample",
            VStack(spacing: 10,
                Label($"Counter: {State.Counter}")
                    .VCenter()
                    .HCenter(),

                Button("Click To Increment", () =>
                    SetState(s => s.Counter++))
            )
            .Center()
        );
    }
}
```

Each time you click the button you should see the "Render" string output in the console of your IDE: this means, as explained, that a new Visual Tree has been created.

Now, imagine for example that we just want to update the label text and nothing else. In this case, we can take full advantage of a MauiReactor feature that lets us just update the native control _without_ requiring a complete refresh.

Let's change the sample code to this:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CounterPageState
{
    public int Counter { get; set; }
}

class CounterPage : Component<CounterPageState>
{
    public override VisualNode Render()
    {
        Debug.WriteLine("Render");
        return ContentPage("Counter Sample",
            VStack(spacing: 10,
                Label(()=> $"Counter: {State.Counter}")
                    .Center(),

                Button("Click To Increment", () =>
                    SetState(s => s.Counter++, invalidateComponent: false))
            )
            .Center()
        );
    }
}
```

Notice the changes to lines 15 and 20:

15: we use an overload of the `Label()` the class that accepts a `Func<string>`
20: secondly we call `SetState(..., invalidateComponent: false)`

Now if you click the button, no Render message should be written to the console output: this proves that we're updating the native Label `without` recreating the component.

Of course, this is not possible every time (for example when a change in the state should result in a change of the component tree) but when it is, it should improve the responsiveness of the app.

[PreviousState-less Components](/mauireactor/components/state-less-components) [NextInline Components](/mauireactor/components/stateful-components/inline-components)

Last updated 11 months ago

---

# Inline Components | MauiReactor

MauiReactor also features a nice ability to wrap a stateful component within a function.

Inline components are somewhat inspired by React hooks so you may notice some similarities if you already have experience with them.

For example, consider that we want to create a custom Entry control that works like the Material entry widget:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FUM88cNylfuaQ5E4Fhnqn%252FTestMudEntry.gif%3Falt%3Dmedia%26token%3Df6b7785a-44fd-4c8c-9327-a4441f25d46f&width=768&dpr=4&quality=100&sign=cc8c835c&sv=2)

This is the usual way we would create it in MauiReactor:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MudEntryState
{
    public bool Focused { get; set; }

    public bool IsFilled { get; set; }
}

partial class MudEntry : Component<MudEntryState>
{
    MauiControls.Entry _entryRef;

    [Prop]
    Action<string>? _textChanged;
    [Prop]
    string _label;

    public override VisualNode Render()
     => Grid("Auto", "*",
            Entry(entryRef => _entryRef = entryRef)
                .OnAfterTextChanged(OnTextChanged)
                .VCenter()
                .OnFocused(()=>SetState(s => s.Focused = true))
                .OnUnfocused(()=>SetState(s => s.Focused = false)),

            Label(_label)
                .OnTapped(()=>_entryRef?.Focus())
                .Margin(5,0)
                .HStart()
                .VCenter()
                .TranslationY(State.Focused || State.IsFilled ? -20 : 0)
                .ScaleX(State.Focused || State.IsFilled ? 0.8 : 1.0)
                .AnchorX(0)
                .TextColor(!State.Focused || !State.IsFilled ? Colors.Gray : Colors.Red)
                .WithAnimation(duration: 200),
        )
        .VCenter();

    private void OnTextChanged(string text)
    {
        SetState(s => s.IsFilled = !string.IsNullOrWhiteSpace(text));
        _textChanged?.Invoke(text);
    }
}
```

MauiReactor allows you to collapse the component + state in the single declaration that you can return from a function:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
static VisualNode RenderMudEntry(string label, Action<string> textChangedAction)
    => Render<(bool IsFocused, bool IsFilled)>(state =>
    {
        MauiControls.Entry _entryRef = null;

        return new Grid("Auto", "*")
        {
            new Entry(entryRef => _entryRef = entryRef)
                .OnAfterTextChanged(text =>
                {
                    state.Set(s => (s.IsFocused, IsFilled: !string.IsNullOrWhiteSpace(text)));
                    textChangedAction?.Invoke(text);
                })
                .VCenter()
                .OnFocused(()=>state.Set(s => (IsFocused: true, s.IsFilled)))
                .OnUnfocused(()=>state.Set(s => (IsFocused: false, s.IsFilled))),

            new Label(label)
                .OnTapped(() =>_entryRef?.Focus())
                .Margin(5,0)
                .HStart()
                .VCenter()
                .TranslationY(state.Value.IsFocused || state.Value.IsFilled ? -20 : 0)
                .ScaleX(state.Value.IsFocused || state.Value.IsFilled ? 0.8 : 1.0)
                .AnchorX(0)
                .TextColor(!state.Value.IsFocused || !state.Value.IsFilled ? Colors.Gray : Colors.Red)
                .WithAnimation(duration: 200),
        }
        .VCenter();
    });

```

In Line 2 we use the static method Component.Render() which takes a Func<VisualNode>. It creates a hidden stateful component on the fly and uses the passed function to render its content.

In Line 5 we declare our state using the method UseState<S>() where S is the type of state we need to render the component.
You can pass any type to it but, of course, the beauty of the solution is to take everything contained in the function without external references. In the above sample, we use a c# Tuple that has 2 properties IsFocused and IsFilled.

The rest of the function is more or less the same as the content of the Render method.

Inline Components are a perfect choice to render small components that have simple states, you can for example put it in a generic static class that provides general theming functions to the app.

[PreviousStateful Components](/mauireactor/components/stateful-components) [NextComponent life-cycle](/mauireactor/components/component-life-cycle)

Last updated 11 months ago

---

# Label | MauiReactor

> The .NET Multi-platform App UI (.NET MAUI) [Label](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.label) displays single-line and multi-line text. Text displayed by a [Label](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.label) can be colored, spaced, and can have text decorations.

Creating a Label in MauiReactor is pretty easy:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
Label("My label text")
```

Formatted text can be created as well with a code like this:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
Label()
    .FormattedText(()=>
    {
        //of course FomattedText here, being static, can be created as a static variable and passed to Label().FormattedText(myStaticFormattedText)
        FormattedString formattedString = new();
        formattedString.Spans.Add(new Span { Text = "Red bold, ", TextColor = Colors.Red, FontAttributes = FontAttributes.Bold });

        Span span = new() { Text = "default, " };
        span.GestureRecognizers.Add(new MauiControls.TapGestureRecognizer { Command = new Command(async () => await ContainerPage!.DisplayAlert("Tapped", "This is a tapped Span.", "OK")) });
        formattedString.Spans.Add(span);
        formattedString.Spans.Add(new Span { Text = "italic small.", FontAttributes = FontAttributes.Italic, FontSize = 14 });

        return formattedString;
    })
```

The above code produces a formatted text like the following:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FoxhgaUdPTSkbKxpR60vs%252Fimage.png%3Falt%3Dmedia%26token%3D31dd7cf5-4b3f-4c4e-99c0-c5c7c1f1ee34&width=768&dpr=4&quality=100&sign=5a86e07f&sv=2)

If the FormattedText object is static (i.e., not changed with the state), consider creating a static variable to pass to the `Label().FormattedText(myStaticVar)` method.

[PreviousShell](/mauireactor/components/controls/shell) [NextWrap 3rd party controls](/mauireactor/components/wrap-3rd-party-controls)

Last updated 6 months ago

---

# Lottie animations | MauiReactor

This article is based on the package [SkiaSharp.Extended.UI.Maui](https://mono.github.io/SkiaSharp.Extended/api/ui-maui/index.html) which it's currently in preview.

Since Xamarin Forms, developers can integrate powerful animations inside their applications thanks to [Lottie](https://lottiefiles.com/) animation system implementation.

In .NET MAUI, you have the ability to play Lottie files using a relatively recent Skia-based implementation available inside [https://github.com/mono/SkiaSharp.Extended/](https://github.com/mono/SkiaSharp.Extended/) package.

Using Lottie controls in MauiReactor is straightforward, it just requires you to add a package reference in csproj and to create a few scaffold classes inside your project.

First of all, add the reference to [SkiaSharp.Extended.UI.Maui](https://mono.github.io/SkiaSharp.Extended/api/ui-maui/index.html) and MauiReactor Scaffold Generator packages:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<PackageReference Include="Reactor.Maui.ScaffoldGenerator" Version="1.0.74-beta" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
<PackageReference Include="SkiaSharp.Extended.UI.Maui" Version="2.0.0-preview.61" />
```

The next step is to call the method `UseSkiaSharp()` inside the MainProgram.cs file to enable SkiaSharp handlers.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiReactorApp<MainPage>()
    .UseSkiaSharp()
```

After that, you have to create a few MauiReactor wrappers for the control SKLottieView which can load and play the animation:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
[Scaffold(typeof(SkiaSharp.Extended.UI.Controls.SKSurfaceView))]
partial class SKSurfaceView { }

[Scaffold(typeof(SkiaSharp.Extended.UI.Controls.SKAnimatedSurfaceView))]
partial class SKAnimatedSurfaceView { }

[Scaffold(typeof(SkiaSharp.Extended.UI.Controls.SKLottieView))]
partial class SKLottieView { }

```

Note that you can't just wrap the `SKLottieView` but you have to generate wrappers also for the ancestors `SKAnimatedSurfaceView` and `SKSurfaceView` up to the `TemplateView` class which is already wrapped by MauiReactor itself.

Finally, you can use the control inside your component:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new SKLottieView()
    .Source(new SkiaSharp.Extended.UI.Controls.SKFileLottieImageSource()
    {
        File = "dotnetbot.json"
    })
    .IsAnimationEnabled(true)
    .RepeatCount(-1)
    .HeightRequest(200)
    .WidthRequest(200)
    .HCenter()

```

The animation file (dotnetbot.json in the above example) must be placed inside the Resources\\Raw folder.

This should be the final page with the animation running:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FvNhDJ0X7dgPqCkiI2chc%252FMauiReactorLottie.gif%3Falt%3Dmedia%26token%3D8f45462d-8ce8-43be-ab4a-fa3ea3736ad8&width=768&dpr=4&quality=100&sign=5529ede7&sv=2)

Lottie animation in MauiReactor

[PreviousWrap 3rd party controls](/mauireactor/components/wrap-3rd-party-controls) [NextProvide DataTemplate to native controls](/mauireactor/components/wrap-3rd-party-controls/provide-datatemplate-to-native-controls)

Last updated 1 year ago

---

# Window | MauiReactor

When you start an app in a desktop environment like Windows or Mac, .NET MAUI creates under the hood the main window that contains the root page.

MauiReactor has some nice features that allow you to change its properties like the title or position and interact with it for example responding to the SizeChanged event.

## [Direct link to heading](\#change-the-title-of-the-window)    Change the title of the window

Often all you need for your app is to change its Title. The easiest method to modify basic properties on the parent window is to use some helper function on the containing page.

For example, this is the code if you want to change the window title:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new ContentPage
{
...
}
.WindowTitle("My Window Title")
```

## [Direct link to heading](\#working-with-the-window)    Working with the Window

If you need more control over the parent window you can create a Window visual node and host the page inside it:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
{
    return new Window
    {
        new ContentPage
        {
            ...
        }
    }
    .Title("My Window Title")
    .OnSizeChanged(OnSizeChanged)
    ;
}

private void OnSizeChanged(object sender, EventArgs args)
{
    var window = ((MauiControls.Window)sender);
    System.Diagnostics.Debug.WriteLine($"Window size changed to {window.Width}x{window.Height}");
    System.Diagnostics.Debug.WriteLine($"Window position changed to {window.X},{window.Y}");
}

```

This way you can access all the properties of the window and receive all the callbacks you need attaching events like the SizeChanged.

## [Direct link to heading](\#application-lifetime-events)    Application lifetime events

You can handle application lifetime events attaching to the specific handler as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    public override VisualNode Render()
    {
        return new Window
        {
            new ContentPage
            {
            }
        }
        .OnCreated(() => Debug.WriteLine("Created"))
        .OnActivated(() => Debug.WriteLine("Activated"))
        .OnDeactivated(() => Debug.WriteLine("Deactivated"))
        .OnStopped(() => Debug.WriteLine("Stopped"))
        .OnResumed(() => Debug.WriteLine("Resumed"))
        .OnDestroying(() => Debug.WriteLine("Destroying"))
        ;
    }
}
```

[PreviousCanvasView control](/mauireactor/components/graphics/canvasview-control) [NextTesting](/mauireactor/components/testing)

Last updated 1 year ago

---

# How to deal with state shared across Components? | MauiReactor

Hi, given that you can choose the best approach that fits better for your project here are 3 different ways to achieve it in MauiReactor/C# that I can think of:

1. Pass global state/parameters down to your component tree. For example, say you have a class that resembles your settings pass it down to your components:


Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class Settings
{
  public int MySettingValue {get;set;}
}

class MyComponent
{
   Settings? _settings;
    public MyComponent Settings(Settings settings)
    {
       _settings = settings;
      return this;
    }

    public override VisualNode Render()
    {
      //use _settings and pass down to components used here _settings object
    }
}
```

1. Use dependency injection to "inject" your global state/parameters:


Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
interface IGlobalSettings //->implemented somewhere and registered in MauiProgram.cs
{
    int MySettingValue {get;set;}
}

class MyComponent
{
    public override VisualNode Render()
    {
          var settings = Services.GetRequiredService<IGlobalSettings>();
         //use settings, but no need to pass down to components used here (they can easily access the settings class with DI as well)
    }
}
```

1. Use the "Parameters" feature provided by MauiReactor: it allows you to create a Parameter in a parent component and access it in read-write mode from child components:


Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CustomParameter
{
    public int Numeric { get; set; }
}

class ParametersPage: Component
{
    private readonly IParameter<CustomParameter> _customParameter;

    public ParametersPage()
    {
        _customParameter = CreateParameter<CustomParameter>();
    }

    public override VisualNode Render()
    {
        return new ContentPage("Parameters Sample")
        {
            new VStack(spacing: 10)
            {
                new Button("Increment from parent", () => _customParameter.Set(_=>_.Numeric += 1   )),
                new Label(_customParameter.Value.Numeric),

                new ParameterChildComponent()
            }
            .VCenter()
            .HCenter()
        };
    }
}

class ParameterChildComponentc: Component
{
    public override VisualNode Render()
    {
        var customParameter = GetParameter<CustomParameter>();

        return new VStack(spacing: 10)
        {
            new Button("Increment from child", ()=> customParameter.Set(_=>_.Numeric++)),

            new Label(customParameter.Value.Numeric),
        };
    }
}
```

for more info on this specific feature please take a look at:

- [https://github.com/adospace/reactorui-maui/blob/main/samples/MauiReactor.TestApp/Pages/ParametersPage.cs](https://github.com/adospace/reactorui-maui/blob/main/samples/MauiReactor.TestApp/Pages/ParametersPage.cs)

- [https://github.com/adospace/kee-mind](https://github.com/adospace/kee-mind) (KeeMind app makes extensive use of this feature)

- [https://app.gitbook.com/s/h1eh1igwiwRzrw2kbxSp/~/changes/55/components/component-parameters](https://app.gitbook.com/s/h1eh1igwiwRzrw2kbxSp/~/changes/55/components/component-parameters)


[PreviousSource and Sample Applications](/mauireactor/resources/source-and-sample-applications) [NextDoes this support ObservableCollection for CollectionView?](/mauireactor/q-and-a/does-this-support-observablecollection-for-collectionview)

Last updated 1 year ago

---

# IndicatorView | MauiReactor

> The .NET Multi-platform App UI (.NET MAUI) [IndicatorView](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.indicatorview) is a control that displays indicators that represent the number of items, and current position, in a [CarouselView](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.carouselview).

Official documentation:
[https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/indicatorview](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/indicatorview)

MauiReactor sample app:
[https://github.com/adospace/mauireactor-samples/tree/main/Controls/IndicatorViewTestApp](https://github.com/adospace/mauireactor-samples/tree/main/Controls/IndicatorViewTestApp)

The sample code below shows how to implement the IndicatorView control in MauiReactor:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPageState
{
    public Animal[] Animals { get; set; }
}

class MainPage : Component<MainPageState>
{
    protected override void OnMounted()
    {
        State.Animals = new Animal[]
        {
            new Animal
            {
                Name = "American Black Bear",
                Location = "North America",
                Details = "The American black bear is a medium-sized bear native to North America. It is the continent's smallest and most widely distributed bear species. American black bears are omnivores, with their diets varying greatly depending on season and location. They typically live in largely forested areas, but do leave forests in search of food. Sometimes they become attracted to human communities because of the immediate availability of food. The American black bear is the world's most common bear species.",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/0/08/01_Schwarzbär.jpg"
            },
            new Animal
            {
                Name = "Asian Black Bear",
                Location = "Asia",
                Details = "The Asian black bear, also known as the moon bear and the white-chested bear, is a medium-sized bear species native to Asia and largely adapted to arboreal life. It lives in the Himalayas, in the northern parts of the Indian subcontinent, Korea, northeastern China, the Russian Far East, the Honshū and Shikoku islands of Japan, and Taiwan. It is classified as vulnerable by the International Union for Conservation of Nature (IUCN), mostly because of deforestation and hunting for its body parts.",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b7/Ursus_thibetanus_3_%28Wroclaw_zoo%29.JPG/180px-Ursus_thibetanus_3_%28Wroclaw_zoo%29.JPG"
            },
            new Animal
            {
                Name = "Brown Bear",
                Location = "Northern Eurasia & North America",
                Details = "The brown bear is a bear that is found across much of northern Eurasia and North America. In North America the population of brown bears are often called grizzly bears. It is one of the largest living terrestrial members of the order Carnivora, rivaled in size only by its closest relative, the polar bear, which is much less variable in size and slightly larger on average. The brown bear's principal range includes parts of Russia, Central Asia, China, Canada, the United States, Scandinavia and the Carpathian region, especially Romania, Anatolia and the Caucasus. The brown bear is recognized as a national and state animal in several European countries.",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5d/Kamchatka_Brown_Bear_near_Dvuhyurtochnoe_on_2015-07-23.jpg/320px-Kamchatka_Brown_Bear_near_Dvuhyurtochnoe_on_2015-07-23.jpg"
            },
            new Animal
            {
                Name = "Grizzly-Polar Bear Hybrid",
                Location = "Canadian Artic",
                Details = "A grizzly–polar bear hybrid is a rare ursid hybrid that has occurred both in captivity and in the wild. In 2006, the occurrence of this hybrid in nature was confirmed by testing the DNA of a unique-looking bear that had been shot near Sachs Harbour, Northwest Territories on Banks Island in the Canadian Arctic. The number of confirmed hybrids has since risen to eight, all of them descending from the same female polar bear.",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7e/Grolar.JPG/276px-Grolar.JPG"
            },
        };

        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new IndicatorView()
                .ItemsSource(State.Animals)
                .IndicatorColor(Colors.Red)
                .IndicatorSize(14)
                .IndicatorsShape(MauiControls.IndicatorShape.Circle)
                .VCenter()
                .HCenter()
        };
    }
}

```

The IndicatorView control is almost always used in conjunction with the CarouselView to indicate its current view

## [Direct link to heading](\#custom-indicatorview)    Custom IndicatorView

If you need more control over the indicator items, consider an alternative approach as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class TestModel
{
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string Description { get; set; }
}

class MainPageCustomState
{
    public List<TestModel> Models { get; set; } = new();

    public TestModel SelectedModel { get; set; }

}

class MainPageCustom : Component<MainPageCustomState>
{
    private MauiControls.CarouselView _carouselView;

    protected override void OnMounted()
    {
        State.Models = new List<TestModel>()
        {
            new TestModel
            {
                Title = "Lorem Ipsum Dolor Sit Amet",
                SubTitle = "Consectetur Adipiscing Elit",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed in libero non quam accumsan sodales vel vel nunc."
            },
            new TestModel
            {
                Title = "Praesent Euismod Tristique",
                SubTitle = "Vestibulum Fringilla Egestas",
                Description = "Praesent euismod tristique vestibulum. Vivamus vestibulum justo in massa aliquam, id facilisis odio feugiat. Duis euismod massa id elit imperdiet."
            },
            new TestModel
            {
                Title = "Aliquam Erat Volutpat",
                SubTitle = "Aenean Feugiat In Mollis",
                Description = "Aliquam erat volutpat. Aenean feugiat in mollis ac. Nullam eget justo ut orci dictum auctor."
            },
            new TestModel
            {
                Title = "Suspendisse Tincidunt",
                SubTitle = "Faucibus Ligula Quis",
                Description = "Suspendisse tincidunt, arcu eget auctor efficitur, nulla justo tristique neque, et fermentum orci ante eget nunc."
            },
        };

        State.SelectedModel = State.Models.First();

        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new VStack
            {
                new Grid("* 40", "*")
                {
                    new CarouselView(r => _carouselView = r)
                        .HeightRequest(450)
                        .HorizontalScrollBarVisibility(ScrollBarVisibility.Never)
                        .ItemsSource(State.Models, _=> new Grid("*", "*")
                        {
                            new VStack
                            {
                                  new Label(_.Title)
                                    .Margin(0,5),
                                  new Label(_.SubTitle)
                                    .Margin(0,5),
                                 new Label(_.Description)
                                    .Margin(0,8)
                            }
                        })
                        .CurrentItem(() => State.SelectedModel!)
                        .OnCurrentItemChanged((s, args) => SetState(s => s.SelectedModel = (TestModel)args.CurrentItem))
                        ,

                    new HStack(spacing: 5)
                    {
                        State.Models.Select(item =>
                            new Image(State.SelectedModel == item ? "sun.png" : "circle.png")
                                .WidthRequest(20)
                                .HeightRequest(20)
                                .OnTapped(()=>SetState(s=>s.SelectedModel = item, false))
                            )
                    }
                    .HCenter()
                    .VCenter()
                    .GridRow(1)
                }
            }
            .Padding(8,0)
            .VCenter()
        };
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252Fhhh6w9BrIdhiVgSGKhfc%252Fimage.png%3Falt%3Dmedia%26token%3D50ed345c-48c5-4267-8966-a929d20ba2ef&width=768&dpr=4&quality=100&sign=ca992a79&sv=2)

Custom indicator view sample

[PreviousGrouping](/mauireactor/components/controls/collectionview/grouping) [NextPicker](/mauireactor/components/controls/picker)

Last updated 1 year ago

---

# Behaviors | MauiReactor

> .NET Multi-platform App UI (.NET MAUI) behaviors let you add functionality to user interface controls without having to subclass them. Instead, the functionality is implemented in a behavior class and attached to the control as if it was part of the control itself.

Behaviors in MauiReactor are much less useful than in a classic XAML-MVVM project but sometimes you may need to integrate custom functionalities that are provided as behaviors.

Consider carefully whether or not you really need to use Behaviors in MauiReactor. Historically they were created to handle specific functionalities that MVVM approach would have required otherwise custom code-behind.

Behaviors like EventToCommandBehavior or EmailValidationBehavior are useless in MauiReactor while IconTintColorBehavior provides useful functionalities.

Generally, platform behaviors ( [https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/behaviors#platform-behaviors](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/behaviors#platform-behaviors)) provide unique functionalities very welcome even in a MauiReactor project.

For example, let's consider the `IconTintColorBehavior` (from MAUI CommunityToolkit): it allows to quickly set/change the color used to render a SVG image.

In MauiReactor, you need to scaffold the behavior and put it inside the `View` you want to attach as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
[Scaffold(typeof(CommunityToolkit.Maui.Behaviors.IconTintColorBehavior))]
partial class IconTintColorBehavior { }

class BehaviorTestPageState
{
    public Color Color { get; set; } = Colors.Red;
}

class BehaviorTestPage : Component<BehaviorTestPageState>
{
    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new VStack(spacing: 10)
            {
                new Image("shield.png")
                {
                    new IconTintColorBehavior()
                        .TintColor(State.Color)
                },

                new HStack(spacing: 5)
                {
                    new Button(nameof(Colors.Red), () => SetState(s => s.Color = Colors.Red)),
                    new Button(nameof(Colors.Green), () => SetState(s => s.Color = Colors.Green)),
                    new Button(nameof(Colors.Black), () => SetState(s => s.Color = Colors.Black)),
                }
                .HCenter()
            }
            .Center()
        };
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FkS5U8VvCLuirI7h5dXKG%252FBehavior.gif%3Falt%3Dmedia%26token%3D1330c5f3-833d-4399-a545-4df5d6379e09&width=768&dpr=4&quality=100&sign=d74b5698&sv=2)

Behaviors in MauiReactor

[PreviousUsing XAML Resources](/mauireactor/deep-dives/using-xaml-resources) [NextSource and Sample Applications](/mauireactor/resources/source-and-sample-applications)

Last updated 1 year ago

---

# Dependency injection | MauiReactor

In MauiReactor, it's recommended to put service implementations and models in a project (assembly) different from the one containing the app.
Hot-reloading a project that contains dependency-injected services requires them to be hosted in a different assembly/project.

MAUI.NET is deeply integrated with the Microsoft Dependency injection extensions library and provides a structured way to inject services and consume them inside ViewModel classes.

MauiReactor works mainly the same except you can access services through the `Services` property inside your components.

For example, let's see how to consume a simple Calc service like this (created in a class library referenced by the main MAUI project):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class CalcService
{
    public double Add(double x, double y) => x + y;
}
```

We, first, have to register the services during the startup of the app:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiReactorApp<MainPage>()
#if DEBUG
        .EnableMauiReactorHotReload()
#endif
        );

    builder.Services.AddSingleton<CalcService>()

    return builder.Build();
}
```

Then we can access it inside our components:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class MainPageState
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Result { get; set; }
}

public class MainPage : Component<MainPageState>
{
    [Inject]
    CalcService _calcService;

    public override VisualNode Render()
        => NavigationPage(
             ContentPage("DI Container Sample",
                HStack(spacing: 10,
                    NumericEntry(State.X, v => SetState(s => s.X = v)),
                    Label(" + ")
                        .VCenter(),
                    NumericEntry(State.Y, v => SetState(s => s.Y = v)),
                    Button(" + ")
                        .OnClick(OnAdd),
                    Label(State.Result)
                        .VCenter()
                )
                .Center()
            )
        );

    private void OnAdd()
    {
        SetState(s => s.Result = _calcService.Add(State.X, State.Y));
    }

    private Entry NumericEntry(double value, Action<double> onSet)
        => Entry(value.ToString())
            .OnAfterTextChanged(s => onSet(double.Parse(s)))
            .Keyboard(Keyboard.Numeric)
            .VCenter();
}
```

At line 38 we get the reference to the singleton service and call its Add method().

[PreviousNative tree and Visual tree](/mauireactor/deep-dives/native-tree-and-visual-tree) [NextWorking with the GraphicsView](/mauireactor/deep-dives/working-with-the-graphicsview)

Last updated 11 months ago

---

# Shell | MauiReactor

> .NET Multi-platform App UI (.NET MAUI) Shell reduces the complexity of app development by providing the fundamental features that most apps require, including:
>
> - A single place to describe the visual hierarchy of an app.
>
> - A common navigation user experience.
>
> - A URI-based navigation scheme that permits navigation to any page in the app.
>
> - An integrated search handler.

Official documentation:
[https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/)

MauiReactor sample app:
[https://github.com/adospace/mauireactor-samples/tree/main/Controls/ShellTestPage](https://github.com/adospace/mauireactor-samples/tree/main/Controls/ShellTestPage) [https://github.com/adospace/mauireactor-samples/tree/main/Controls/ShellNavTestPage](https://github.com/adospace/mauireactor-samples/tree/main/Controls/ShellNavTestPage)

## [Direct link to heading](\#shell-with-shellcontents)    Shell with ShellContents

The sample code below shows how to create a Shell with some pages using the ShellContent class:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPageState
{
    public int Counter { get; set; }
}

class MainPage : Component<MainPageState>
{
    public override VisualNode Render()
        => new Shell
        {
            new ShellContent("Home")
                .Icon("home.png")
                .RenderContent(()=> new HomePage()),

            new ShellContent("Comments")
                .Icon("comments.png")
                .RenderContent(()=> new CommentsPage()),
        };
}

class HomePage : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Home")
        {
            new Label("Home")
                .VCenter()
                .HCenter()
        };
    }
}

class CommentsPage : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Comments")
        {
            new Label("Comments")
                .VCenter()
                .HCenter()
        };
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252F67E29Q8HjHs8HwyOdmlQ%252FShell.gif%3Falt%3Dmedia%26token%3D6cc624a5-ed36-42f0-8cf4-849728049ea2&width=768&dpr=4&quality=100&sign=a3c99f1c&sv=2)

Shell in action in Android

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FdntawsoOqYrhBQUISx9Y%252FShell2.gif%3Falt%3Dmedia%26token%3D4c1d93a8-1c8f-414f-8def-24959657688b&width=768&dpr=4&quality=100&sign=2f09b9a5&sv=2)

Shell in action under Windows

## [Direct link to heading](\#shell-with-tab-and-flyoutitem-asmultipleitems)    Shell with Tab and FlyoutItem (AsMultipleItems)

Following it's another sample of Shell with more items arranged inside a FlyoutItem and Tab:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
    public override VisualNode Render()
        => new Shell
        {
            new FlyoutItem
            {
                new Tab
                {
                    new ShellContent("Home")
                        .Icon("home.png")
                        .RenderContent(()=> new HomePage()),

                    new ShellContent("Comments")
                        .Icon("comments.png")
                        .RenderContent(()=> new CommentsPage()),
                }
                .Title("Notifications")
                .Icon("bell.png"),

                new ShellContent("Home")
                    .Icon("database.png")
                    .RenderContent(()=> new DatabasePage()),

                new ShellContent("Comments")
                    .Icon("bell.png")
                    .RenderContent(()=> new NotificationsPage()),
            }
            .FlyoutDisplayOptions(MauiControls.FlyoutDisplayOptions.AsMultipleItems)
        };

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FJvg8LtmYkQkKkEfzUvI9%252FShell3.gif%3Falt%3Dmedia%26token%3Db1c4c2fd-071e-4f4c-bf55-9cc04a0bc941&width=768&dpr=4&quality=100&sign=9fd31f41&sv=2)

Shell under Windows

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FGOZB3D0aJ1yTznwO3SU6%252FShell4.gif%3Falt%3Dmedia%26token%3D27fcc031-7229-48f8-b32f-77ace3bbf651&width=768&dpr=4&quality=100&sign=429ecad9&sv=2)

Shell under Android

## [Direct link to heading](\#custom-flyoutitem-appearance)    Custom FlyoutItem appearance

In the following code, FlyoutItems appearance is customized:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
    => new Shell
    {
        new ShellContent("Home")
            .Icon("home.png")
            .RenderContent(()=> new HomePage()),

        new ShellContent("Comments")
            .Icon("comments.png")
            .RenderContent(()=> new CommentsPage()),
    }
    .ItemTemplate(RenderItemTemplate);

static VisualNode RenderItemTemplate(MauiControls.BaseShellItem item)
     => new Grid("68", "Auto, *")
     {
         new Image()
            .Source(item.FlyoutIcon)
            .Margin(4),

         new Label(item.Title)
            .GridColumn(1)
            .VCenter()
            .TextDecorations(TextDecorations.Underline)
            .FontAttributes(MauiControls.FontAttributes.Bold)
            .Margin(10,0)
     };
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FSeltrMEcfoELcNueXMIl%252Fimage.png%3Falt%3Dmedia%26token%3Daf9de848-84f8-43ff-b54a-bece3b8e0f35&width=768&dpr=4&quality=100&sign=9c3e1f33&sv=2)

Customized FlyoutItems

## [Direct link to heading](\#custom-flyoutcontent)    Custom FlyoutContent

You can also provide custom content for the Flyout as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
    => new Shell
    {
        new ShellContent("Home")
            .Route(nameof(HomePage))
            .Icon("home.png")
            .RenderContent(()=> new HomePage()),

        new ShellContent("Comments")
            .Route(nameof(CommentsPage))
            .Icon("comments.png")
            .RenderContent(()=> new CommentsPage()),
    }
    .FlyoutContent(RenderFlyoutContent());

VisualNode RenderFlyoutContent()
{
    return new ScrollView
    {
        new VStack(spacing: 5)
        {
            new Button("Home")
                .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync($"//{nameof(HomePage)}")),

            new Button("Comments")
                .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync($"//{nameof(CommentsPage)}")),
        }
    };
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FCz9C8IxSjDx1F7oYhGqm%252Fimage.png%3Falt%3Dmedia%26token%3D43d48451-9951-4ff0-818b-03ae9553bff9&width=768&dpr=4&quality=100&sign=f293960a&sv=2)

Custom Shell FlyoutContent

## [Direct link to heading](\#shell-menu-items)    Shell menu items

You can also create a simple menu item inside the shell with a custom command:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
    => new Shell
    {
        new ShellContent("Home")
            .Route(nameof(HomePage))
            .Icon("home.png")
            .RenderContent(()=> new HomePage()),

        new ShellContent("Comments")
            .Route(nameof(CommentsPage))
            .Icon("comments.png")
            .RenderContent(()=> new CommentsPage()),

        new MenuItem("Click me!")
            .OnClicked(async ()=> await ContainerPage.DisplayAlert("MauiReactor", "Clicked!", "OK"))
    };
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FV4CaX49mm2AWKMpx0kWA%252Fimage.png%3Falt%3Dmedia%26token%3D08272ec9-03fa-4c43-b7f1-e9942fc49f22&width=768&dpr=4&quality=100&sign=868e4a83&sv=2)

In the following sample code, we're going to customize the MenuItems:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
    => new Shell
    {
        new MenuItem("Click me!")
            .IconImageSource("gear.png")
            .OnClicked(async ()=> await ContainerPage.DisplayAlert("MauiReactor", "Clicked!", "OK"))
    }
    .MenuItemTemplate(menuItem =>
        new Grid("65", "Auto, *")
        {
            new Image()
                .Source(menuItem.IconImageSource)
                .VCenter(),

            new Label(menuItem.Text)
                .TextColor(Colors.Red)
                .VCenter()
                .Margin(10,0)
                .FontAttributes(MauiControls.FontAttributes.Bold)
                .GridColumn(1)
        }
        .Padding(10,0)
        );
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252F41qnEYTbw5LCoEx1q7XG%252Fimage.png%3Falt%3Dmedia%26token%3D2d0ac7c9-e356-4d44-bc05-f148bc4a8411&width=768&dpr=4&quality=100&sign=39104eb0&sv=2)

Custom MenuItem

## [Direct link to heading](\#flyout-header-and-footer)    Flyout Header and Footer

You can customize the flyout header and footer as well:

The following code uses the LinearGradient class provided by the MauiReactor framework ideal for describing a linear gradient brush in a single line

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
    => new Shell
    {
        new ShellContent("Home")
            .Route(nameof(HomePage))
            .Icon("home.png")
            .RenderContent(()=> new HomePage()),

        new ShellContent("Comments")
            .Route(nameof(CommentsPage))
            .Icon("comments.png")
            .RenderContent(()=> new CommentsPage()),
    }
    .FlyoutBackground(new LinearGradient(45.0, new Color(255, 175, 189), new Color(100, 216, 243)))
    .FlyoutHeader(RenderHeader())
    .FlyoutFooter(RenderFooter())
    ;

private VisualNode RenderHeader()
{
    return new VStack(spacing: 5)
    {
        new Label("MauiReactor")
            .TextColor(Colors.White)
            .FontSize(24)
            .HorizontalTextAlignment(TextAlignment.Center)
            .FontAttributes(MauiControls.FontAttributes.Bold)
    };
}

private VisualNode RenderFooter()
{
    return new Image("dotnet_bot.png");
}

```

This is the resulting effect:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FNy24vmsuDwCdtC14vvN1%252FShell5.gif%3Falt%3Dmedia%26token%3Dd22d0530-5a8b-44c6-9db7-0fdc9c875cd6&width=768&dpr=4&quality=100&sign=d6e19acf&sv=2)

Custom Flyout Header and Footer + custom background

## [Direct link to heading](\#tabs)    Tabs

You can create Tabs on top and bottom; just nest shell contents within Tab and TabBar objects as shown in the below example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
    => new Shell
    {
        new TabBar
        {
            new ShellContent("Home")
                .Icon("home.png")
                .RenderContent(()=> new HomePage()),

            new Tab("Engage")
            {
                new ShellContent("Notifications")
                    .RenderContent(()=> new NotificationsPage()),

                new ShellContent("Comments")
                    .RenderContent(()=> new CommentsPage()),
            }
            .Icon("comments.png")
        }
    };
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FKWrYwkpvvDeq3CEU8Nqy%252FShell6.gif%3Falt%3Dmedia%26token%3D01c2b8a8-4715-4110-b8d6-3804ac8ac889&width=768&dpr=4&quality=100&sign=1110c4ab&sv=2)

Shell top and bottom tab bar

You can also change tab bar properties like the background color or select a specific tab. The following code shows how to:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
private MauiControls.ShellContent _notificationsPage;

public override VisualNode Render()
    => new Shell
    {
        new TabBar
        {
            new ShellContent("Home")
                .Icon("home.png")
                .RenderContent(()=> new HomePage()),

            new Tab("Engage")
            {
                new ShellContent(pageRef => _notificationsPage = pageRef)
                    .Title("Notifications")
                    .RenderContent(()=> new NotificationsPage()),

                new ShellContent("Comments")
                {
                    new ContentPage
                    {
                        new Button("Go to notifications")
                            .VCenter()
                            .HCenter()
                            .OnClicked(()=> MauiControls.Shell.Current.CurrentItem = _notificationsPage)
                    }
                }
            }
            .Icon("comments.png")
        }
        .Set(MauiControls.Shell.TabBarBackgroundColorProperty, Colors.Aquamarine)
    };
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FZ6ptWZ2On3H6rFdBMYb9%252FShell7.gif%3Falt%3Dmedia%26token%3Dab203792-6427-479b-91c9-aa6451eb9e11&width=768&dpr=4&quality=100&sign=14186e0b&sv=2)

Custom tab bar color and selection of tab

To set an attached dependency property for a control in MauiReactor you have to use the Set() method.

For example, to set the `TabBarIsVisible` for a `ShellContent` use a code like this:

`Set(MauiControls.Shell.TabBarIsVisibleProperty, true)`

## [Direct link to heading](\#navigation)    Navigation

> .NET Multi-platform App UI (.NET MAUI) Shell includes a URI-based navigation experience that uses routes to navigate to any page in the app, without having to follow a set navigation hierarchy. In addition, it also provides the ability to navigate backwards without having to visit all of the pages on the navigation stack.

MauiReactor allows the registration of components with routes just like you do with Page in normal Maui applications.
To register a route you have to use the `Routing.RegisterRoute<Component>("page name")` method.

The following example shows how to register a few routes and how to navigate to them:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    protected override void OnMounted()
    {
        Routing.RegisterRoute<Page2>(nameof(Page2));
        Routing.RegisterRoute<Page3>(nameof(Page3));

        base.OnMounted();
    }

    public override VisualNode Render()
        => new Shell
        {
            new Page1()
        };
}

class Page1 : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Page1")
        {
            new VStack
            {
                new Button("Goto Page2")
                     .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync(nameof(Page2)))
            }
            .HCenter()
            .VCenter()
        };
    }
}

class Page2 : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Page2")
        {
            new VStack
            {
                new Button("Goto Page3")
                    .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync(nameof(Page3)))
            }
            .HCenter()
            .VCenter()
        };
    }
}

class Page3 : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Page3")
        {
            new VStack
            {
                new Button("Open ModalPage")
                    .OnClicked(async () => await Navigation.PushModalAsync<ModalPage>())
            }
            .HCenter()
            .VCenter()
        };
    }
}

class ModalPage : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Modal Page")
        {
            new VStack
            {
                new Button("Back")
                    .OnClicked(async () => await Navigation.PopModalAsync())
            }
            .HCenter()
            .VCenter()
        };
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FEjKGSxtH58Zhj1w76WWt%252FShell8.gif%3Falt%3Dmedia%26token%3D903164c3-e35a-4d43-b03a-92b2b1350db6&width=768&dpr=4&quality=100&sign=9915b6da&sv=2)

Shell navigation in action

Passing arguments to pages (components) would mean creating a Props class for the target page and using an overload of the GotoToAsync as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class Page2 : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Page2")
        {
            new VStack
            {
                new Button("Goto Page3")
                    .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync<PageWithArgumentsProps>(nameof(PageWithArguments), props => props.ParameterPassed = "Hello from Page2!"))
            }
            .HCenter()
            .VCenter()
        };
    }
}

class PageWithArgumentsState
{ }

class PageWithArgumentsProps
{
    public string ParameterPassed { get; set; }
}

class PageWithArguments : Component<PageWithArgumentsState, PageWithArgumentsProps>
{
    public override VisualNode Render()
    {
        return new ContentPage("PageWithArguments")
        {
            new VStack(spacing: 10)
            {
                new Label($"Parameter: {Props.ParameterPassed}")
                    .HCenter(),

                new Button("Open ModalPage")
                    .OnClicked(async () => await Navigation.PushModalAsync<ModalPage>())
            }
            //.HCenter()
            .VCenter()
        };
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FVzIhLaIuF3bG6pc2PVeU%252FShell9.gif%3Falt%3Dmedia%26token%3Df69cf5b4-6d0c-4a94-8aa7-39bac5151938&width=768&dpr=4&quality=100&sign=2f6c67ed&sv=2)

Passing arguments to pages

If you want to pass arguments to a modal page use the overload of the `Navigation.PushModalAsync<Page, Props>()` method.

[PreviousPicker](/mauireactor/components/controls/picker) [NextLabel](/mauireactor/components/controls/label)

Last updated 6 months ago

---

# Button | MauiReactor

> The .NET Multi-platform App UI (.NET MAUI) [Button](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.button) displays text and responds to a tap or click that directs the app to carry out a task.

You can create a Button in MauiReactor just like any other widget:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
Button("Button")
    .OnClicked(...)
```

## [Direct link to heading](\#visual-states)    Visual states

To modify the look of the button when it assumes a specific state, append one or more calls to VisualState() method specifying which state to link and which property and value to set.

For example, the following code changes the background color of a button when it's pressed:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
Button("Button")
    .VisualState(nameof(CommonStates), CommonStates.Normal)
    .VisualState(nameof(CommonStates), "Pressed", MauiControls.VisualElement.BackgroundColorProperty, Colors.Aqua)

```

Use [theming](/mauireactor/components/theming) to configure the visual states of all the buttons of your application, as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
ButtonStyles.Default = _ => _
    .VisualState(nameof(CommonStates), CommonStates.Normal)
    .VisualState(nameof(CommonStates), "Pressed", MauiControls.VisualElement.BackgroundColorProperty, Colors.Aqua);
```

[PreviousControls](/mauireactor/components/controls) [NextRadioButton](/mauireactor/components/controls/radiobutton)

Last updated 8 months ago

---

# Graphics | MauiReactor

Sometimes you want to directly just draw lines, text, or shapes in general inside a blank canvas. Drawing directly has some advantages:

- You can follow UI designs at the pixel level, for example, you can perfectly round corners at the required value or use the same shadow color as specified in a Figma project

- You need to boost your application performance, especially when dealing with a lot of views present at the same time on a page

- You require that a specific control or group of controls have exactly the same appearance among all platforms


Some disadvantages:

- Often dealing with low-level commands like DrawLine or DrawString results in a more complex code to write a text

- Is sometimes difficult to handle correctly all the user interactions as the native control does: take for example the simple Button view that appears and behaves differently under Android and iOS


MauiReactor features a complete set of tools that allows you to write graphics objects and different levels of abstraction.

From the higher level to the lower:

- Using the MauiReactor CanvasView allows declaring graphics objects and interacting with them like any MauiReactor visual node

- Using GraphicsView standard MAUI control as described [here](/mauireactor/deep-dives/working-with-the-graphicsview)

- Using SkiaSharp package to directly issue commands to a Skia canvas


[PreviousAnimation with the AnimationController](/mauireactor/components/animation/animation-with-the-animationcontroller) [NextCanvasView control](/mauireactor/components/graphics/canvasview-control)

Last updated 2 years ago

---

# CanvasView control | MauiReactor

MAUI developers can draw lines, shapes, or text (generally speaking called drawing objects) directly on a canvas through the `GraphicsView` control or the `SKCanvas` class provided by the [SkiaSharp library](https://github.com/mono/SkiaSharp) in an _imperative_ approach.

MauiRector introduces another way to draw graphics and it's entirely _declarative._ In short, this method consists in describing the tree of the objects you want to show inside a canvas as child nodes of the CanvasView control.

For example, consider this code that declares a `CanvasView` control inside a `ContentPage`:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CanvasPage : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Canvas Test Page")
        {
            new CanvasView
            {

            }
        };
    }
}
```

Consider we want to draw a red rounded rectangle, then we just need to declare it as:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new CanvasView
{
    new Box()
        .Margin(10)
        .BackgroundColor(Colors.Red)
        .CornerRadius(10)
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FgwQC3vdRI6OU1GUOSxR6%252Fimage.png%3Falt%3Dmedia%26token%3Daaf752d9-4337-4291-bee4-24863d0317fa&width=768&dpr=4&quality=100&sign=26a39334&sv=2)

Just a box inside a CanvasView

As you can notice, we have also added a margin to distantiate it from the container (the ContentPage).

Of course, there is much more than this. As a starter, you are allowed to place more widgets inside a CanvasView and moreover arrange them in a Stack, Row, or Column layout similar to what you can do using normal MAUI controls and usual layout systems.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new CanvasView
{
    new Column
    {
        new Box()
            .Margin(10)
            .BackgroundColor(Colors.Green)
            .CornerRadius(10),
        new Box()
            .Margin(10)
            .BackgroundColor(Colors.Red)
            .CornerRadius(10)
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FLoNjwNOKxmRpmhBmRTOb%252Fimage.png%3Falt%3Dmedia%26token%3D026b42f2-21f0-4f4a-ab75-20245bef690e&width=768&dpr=4&quality=100&sign=54d94355&sv=2)

A Column layout with equally spaced children

Column and Row layouts are the most common way to arrange CanvasView elements, and render children in a way much similar to MAUI Grid layout.

For example, you can set a fixed size for one or more elements, giving proportional space for the other:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new CanvasView
{
    new Column("100,*")
    {
        new Box()
            .Margin(10)
            .BackgroundColor(Colors.Green)
            .CornerRadius(10),
        new Box()
            .Margin(10)
            .BackgroundColor(Colors.Red)
            .CornerRadius(10)
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FzRBWJf0FyaywhbOL5ilW%252Fimage.png%3Falt%3Dmedia%26token%3D4e4a284b-96e8-42f2-8f59-1ca250d91c69&width=768&dpr=4&quality=100&sign=317c628d&sv=2)

Row and Column layouts do not support currently Auto sizing for children (as happens instead for the standard Grid control) and it's an intentional design decision: CanvasView layout system is built without the render pass in order to be as fast as possible in rendering graphics.

Many controls embeddable in MauiReactor CanvasView can contain a child. For example, the Box element can contain a Text, Image, or Row/Column controls like it's shown in the following code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new CanvasView
{
    new Column("100,*")
    {
        new Box()
        {
            new Text("This a Text element!")
                .HorizontalAlignment(HorizontalAlignment.Center)
                .VerticalAlignment(VerticalAlignment.Center)
                .FontColor(Colors.White)
                .FontSize(24)
        }
        .Margin(10)
        .BackgroundColor(Colors.Green)
        .CornerRadius(10)
        ,
        new Box()
        {
            new Column("*, 50")
            {
                new Picture("MauiReactor.TestApp.Resources.Images.Embedded.norway_1.jpeg"),
                new Text("Awesome Norway!")
                    .HorizontalAlignment(HorizontalAlignment.Center)
                    .VerticalAlignment(VerticalAlignment.Center)
                    .FontColor(Colors.White)
                    .FontSize(24)
            },
        }
        .Margin(10)
        .BackgroundColor(Colors.Red)
        .CornerRadius(10)
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FpxWRIJPIcaMegexH8XqX%252Fimage.png%3Falt%3Dmedia%26token%3Da77687d9-84e4-4349-a361-5ad5983c8349&width=768&dpr=4&quality=100&sign=279bb3ad&sv=2)

A bit more complex CanvasView element tree

CanvasView uses a GraphicsView control behind the scenes, this means that to load an image you have to embed it in your assembly: add the image under the Resources folder and set the build action to "Embedded resource"

Following is the list of elements you can embed inside a CanvasView currently:

- Box: Simple and efficient container that renders a rectangle that you can customize with properties like Background, Borders, and Padding

- Row/Column: Arrange children in a Row or Column using a layout system similar to the Grid with one Row or one Column.

- Group: container of elements that stacks children one on top of the other (sometimes called Z-Stack in other frameworks)

- Ellipse: draw an ellipse/circle that also contains a child

- Text: draw a string, customizable with properties like FontSize, FontColor, or Font.

- Align: it's one of the most useful elements because it aligns its child inside the rendered rectangle, for example to the borders of it.

- PointInteractionHandler: This is the element that triggers an event in the form of a callback function when the user taps inside its render area. Also supports events like mouse over and double click.

- Picture: draw an image loading it from an embedded resource


[PreviousGraphics](/mauireactor/components/graphics) [NextWindow](/mauireactor/components/window)

Last updated 2 years ago

---

# Layout | MauiReactor

CollectionView control can arrange items in 4 different layouts. For more info about the properties of each layout please take a look at the official documentation [https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview/layout](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview/layout).

The sample code below shows how to set a specific layout in MauiReactor:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
enum LayoutType
{
    LinearVertical,

    LinearHorizontal,

    VerticalGrid,

    HorizontalGrid
}

class MainPageLayoutState
{
    public Person[] Persons { get; set; }

    public LayoutType CurrentLayout { get; set; }
}

class MainPageLayout : Component<MainPageLayoutState>
{
    protected override void OnMounted()
    {
        var person1 = new Person("John", "Doe", new DateTime(1980, 5, 10));
        var person2 = new Person("Jane", "Smith", new DateTime(1990, 6, 20));
        var person3 = new Person("Alice", "Johnson", new DateTime(1985, 7, 30));
        var person4 = new Person("Bob", "Williams", new DateTime(2000, 8, 15));
        var person5 = new Person("Charlie", "Brown", new DateTime(1995, 9, 25));
        var person6 = new Person("Daniel", "Robinson", new DateTime(1982, 10, 2));
        var person7 = new Person("Ella", "Martin", new DateTime(1992, 11, 13));
        var person8 = new Person("Frank", "Garcia", new DateTime(1987, 3, 19));
        var person9 = new Person("Grace", "Rodriguez", new DateTime(1979, 4, 23));
        var person10 = new Person("Harry", "White", new DateTime(1999, 2, 28));

        State.Persons = new [] { person1, person2, person3, person4, person5, person6, person7, person8, person9, person10 };
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new Grid("Auto, *", "*")
            {
                new Picker()
                    .ItemsSource(Enum.GetValues<LayoutType>().Select(_=>_.ToString()).ToList())
                    .SelectedIndex((int)State.CurrentLayout)
                    .OnSelectedIndexChanged(index => SetState(s => s.CurrentLayout = (LayoutType)index)),

                new CollectionView()
                    .ItemsSource(State.Persons, RenderPerson)
                    .ItemsLayout(GetCollectionViewLayout())
                    .GridRow(1)
            }
        };
    }

    private IItemsLayout GetCollectionViewLayout()
    {
        return State.CurrentLayout switch
        {
            LayoutType.LinearVertical => new VerticalLinearItemsLayout(),
            LayoutType.LinearHorizontal => new HorizontalLinearItemsLayout(),
            LayoutType.VerticalGrid => new VerticalGridItemsLayout(span: 3),
            LayoutType.HorizontalGrid => new HorizontalGridItemsLayout(span: 2),
            _ => throw new NotImplementedException(),
        };
    }

    private VisualNode RenderPerson(Person person)
    {
        return new VStack(spacing: 5)
        {
            new Label($"{person.FirstName} {person.LastName}"),
            new Label(person.DateOfBirth.ToShortDateString())
                .FontSize(12)
                .TextColor(Colors.Gray)
        }
        .Margin(5,10);
    }
}

```

[PreviousInteractions](/mauireactor/components/controls/collectionview/interactions) [NextSelection](/mauireactor/components/controls/collectionview/selection)

Last updated 1 year ago

---

# FlyoutPage | MauiReactor

> The .NET Multi-platform App UI (.NET MAUI) [FlyoutPage](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.flyoutpage) is a page that manages two related pages of information – a flyout page that presents items, and a detail page that presents details about items on the flyout page.

Official documentation:
[https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/flyoutpage](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/flyoutpage)

MauiReactor sample app:
[https://github.com/adospace/mauireactor-samples/tree/main/Controls/RadioButtonTestApp](https://github.com/adospace/mauireactor-samples/tree/main/Controls/RadioButtonTestApp)

The following code shows how you can create a FlyoutPage in MauiReactor.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
enum PageType
{
    Contacts,

    TodoList,

    Reminders
}

class MainPageState
{
    public PageType CurrentPageType { get; set; }
}

class MainPage : Component<MainPageState>
{
    public override VisualNode Render()
    {
        return new FlyoutPage
        {
            DetailPage()
        }
        .Flyout(new FlyoutMenuPage()
            .OnPageSelected(pageType => SetState(s => s.CurrentPageType = pageType))
        );
    }

    VisualNode DetailPage()
    {
        return State.CurrentPageType switch
        {
            PageType.Contacts => new ContactsPage(),
            PageType.TodoList => new TodoListPage(),
            PageType.Reminders => new RemindersPage(),
            _ => throw new InvalidOperationException(),
        };
    }

}

class FlyoutMenuPage : Component
{
    private Action<PageType> _selectAction;

    public FlyoutMenuPage OnPageSelected(Action<PageType> action)
    {
        _selectAction = action;
        return this;
    }

    public override VisualNode Render()
    {
        return new ContentPage("Personal Organiser")
        {
            new CollectionView()
                .ItemsSource(Enum.GetValues<PageType>(), pageType =>
                    new Label(pageType.ToString())
                        .Margin(10,5)
                        .VCenter())
                .SelectionMode(Microsoft.Maui.Controls.SelectionMode.Single)
                .OnSelected<CollectionView, PageType>(pageType => _selectAction?.Invoke(pageType))
        };
    }
}

class ContactsPage : Component
{
    public override VisualNode Render()
        => new NavigationPage
        {
            new ContentPage("Contacts")
            {
                new Label("Contacts")
                    .VCenter()
                    .HCenter()
            }
        };
}

class TodoListPage : Component
{
    public override VisualNode Render()
        => new NavigationPage
        {
            new ContentPage("TodoList")
            {
                new Label("TodoList")
                    .VCenter()
                    .HCenter()
            }
        };
}

class RemindersPage : Component
{
    public override VisualNode Render() =>
        new NavigationPage
        {
            new ContentPage("Reminders")
            {
                new Label("Reminders")
                    .VCenter()
                    .HCenter()
            }
        };
}

```

Use `FlyoutLayoutBehavior(...)` to select the required flyout behavior (only valid for desktop/tablet layouts)

[PreviousRadioButton](/mauireactor/components/controls/radiobutton) [NextCollectionView](/mauireactor/components/controls/collectionview)

Last updated 8 months ago

---

# Theming | MauiReactor

In .NET MAUI application, one, usually, creates control styles in a bunch of XAML files in the Resources folder.

For example, the following XAML styles the Label with a custom text color and font family.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<Style TargetType="Label">
    <Setter Property="TextColor" Value="{DynamicResource Primary}" />
    <Setter Property="FontFamily" Value="OpenSansRegular" />
</Style>
```

MauiReactor supports XAML styling as well, you just need to reference your XAML files when setting up the MauiReactor application as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
builder
    .UseMauiReactorApp<HomePage>(app =>
    {
        app.AddResource("Resources/Styles/DefaultTheme.xaml");
    })
```

Starting from version 2.0.34, MauiReactor also allows control styling using C#.

To create your theme, you have to define a class deriving from the MauiReactor Theme class and override the OnApply method.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class AppTheme : Theme
{
   protected override void OnApply()
   {
   ...
   }
}
```

You have to register the theme in the app definition in your program.cs file as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiReactorApp<MainPage>(app =>
    {
        app.UseTheme<AppTheme>();
    })
```

For example, the following theme class defines a few default styles for the Label and Button controls.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class AppTheme : Theme
{
    public static Color Primary = Color.FromArgb("#512BD4");
    public static Color PrimaryDark = Color.FromArgb("#ac99ea");
    public static Color PrimaryDarkText = Color.FromArgb("#242424");
    public static Color Secondary = Color.FromArgb("#DFD8F7");
    public static Color SecondaryDarkText = Color.FromArgb("#9880e5");
    public static Color Tertiary = Color.FromArgb("#2B0B98");
    public static Color White = Color.FromArgb("White");
    public static Color Black = Color.FromArgb("Black");
    public static Color Magenta = Color.FromArgb("#D600AA");
    public static Color MidnightBlue = Color.FromArgb("#190649");
    public static Color OffBlack = Color.FromArgb("#1f1f1f");
    public static Color Gray100 = Color.FromArgb("#E1E1E1");
    public static Color Gray200 = Color.FromArgb("#C8C8C8");
    public static Color Gray300 = Color.FromArgb("#ACACAC");
    public static Color Gray400 = Color.FromArgb("#919191");
    public static Color Gray500 = Color.FromArgb("#6E6E6E");
    public static Color Gray600 = Color.FromArgb("#404040");
    public static Color Gray900 = Color.FromArgb("#212121");
    public static Color Gray950 = Color.FromArgb("#141414");

    public static SolidColorBrush PrimaryBrush = new(Primary);
    public static SolidColorBrush SecondaryBrush = new(Secondary);
    public static SolidColorBrush TertiaryBrush = new(Tertiary);
    public static SolidColorBrush WhiteBrush = new(White);
    public static SolidColorBrush BlackBrush = new(Black);
    public static SolidColorBrush Gray100Brush = new(Gray100);
    public static SolidColorBrush Gray200Brush = new(Gray200);
    public static SolidColorBrush Gray300Brush = new(Gray300);
    public static SolidColorBrush Gray400Brush = new(Gray400);
    public static SolidColorBrush Gray500Brush = new(Gray500);
    public static SolidColorBrush Gray600Brush = new(Gray600);
    public static SolidColorBrush Gray900Brush = new(Gray900);
    public static SolidColorBrush Gray950Brush = new(Gray950);

    private static bool LightTheme => Application.Current?.UserAppTheme == Microsoft.Maui.ApplicationModel.AppTheme.Light;

    protected override void OnApply()
    {
        ButtonStyles.Default = _ => _
            .TextColor(LightTheme ? White : PrimaryDarkText)
            .FontFamily("OpenSansRegular")
            .BackgroundColor(LightTheme ? Primary : PrimaryDark)
            .FontSize(14)
            .BorderWidth(0)
            .CornerRadius(8)
            .Padding(14,10)
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44)
            .VisualState("CommonStates", "Disabled", MauiControls.Button.TextColorProperty, LightTheme ? Gray950 : Gray200)
            .VisualState("CommonStates", "Disabled", MauiControls.Button.BackgroundColorProperty, LightTheme ? Gray200 : Gray600)
            ;

        LabelStyles.Default = _ => _
            .TextColor(LightTheme ? Black : White)
            .FontFamily("OpenSansRegular")
            .FontSize(14)
            .VisualState("CommonStates", "Disabled", MauiControls.Label.TextColorProperty, LightTheme ? Gray300 : Gray600)
            ;

    }
}
```

All the MauiReactor controls can be styled using the class named after the control name (i.e. LabelStyles, ButtonStyles, ViewStyles, etc).

Theming fully supports hot reload: style changes are reflected in the running application.

You can also use "selectors" (like in CSS) to define additional styles for each control. A selector is a unique string attached to the style.

For example, I can define a different style for the label as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
LabelStyles.Themes["Title"] = _ => _
    .FontSize(20);
```

You can select the style using the `ThemeKey ` property:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
Label()
   .ThemeKey("Title");
```

Given that selectors should be unique, a common approach is to create a const string property to use in the style definition and with the ThemeKey property.

For example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public const string Title = nameof(Title);

LabelStyles.Themes[Title] = _ => _
        .FontSize(20);

Label()
   .ThemeKey(AppTheme.Title);

```

Theming also works with custom third-party controls that are scaffolded as described [here](/mauireactor/components/wrap-3rd-party-controls).

## [Direct link to heading](\#dark-theme-support)    Dark theme support

The theming feature allows you to define different styles for the Dark and Light theme.

For example, consider the following app theme definition:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class AppTheme : Theme
{
    public static void ToggleCurrentAppTheme()
    {
        if (MauiControls.Application.Current != null)
        {
            MauiControls.Application.Current.UserAppTheme = IsDarkTheme ? Microsoft.Maui.ApplicationModel.AppTheme.Light : Microsoft.Maui.ApplicationModel.AppTheme.Dark;
        }
    }

    public static Color DarkBackground { get; } = Color.FromArgb("#FF17171C");
    public static Color DarkText { get; } = Color.FromArgb("#FFFFFFFF");
    public static Color LightBackground { get; } = Color.FromArgb("#FFF1F2F3");
    public static Color LightText { get; } = Color.FromArgb("#FF000000");

    protected override void OnApply()
    {
        ContentPageStyles.Default = _ => _
            .BackgroundColor(IsDarkTheme ? DarkBackground : LightBackground);

        LabelStyles.Default = _ => _
            .TextColor(IsDarkTheme ? DarkText : LightText);
    }
}
```

MauiReactor automatically responds to user or system theme change requests and accordingly calls the OnApply overload to allow you to change styles for the Dark and Light theme.

For example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
 => ContentPage(
        VStack(
            Label($"Current Theme: {Theme.CurrentAppTheme} "),

            Button("Toggle", ()=>AppTheme.ToggleCurrentAppTheme())
            )
        .Spacing(10)
        .Center()
    );
```

The above code builds this app page:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FTRv5FdNlpOYQc7KUlbtI%252FDarkThemeSupport.gif%3Falt%3Dmedia%26token%3Daf089fae-e10b-432c-8d63-41f61e394c99&width=768&dpr=4&quality=100&sign=64bbe190&sv=2)

Dark theme sample app

[PreviousComponent Parameters](/mauireactor/components/component-parameters) [NextNavigation](/mauireactor/components/navigation)

Last updated 6 months ago

---

# Does this support ObservableCollection for CollectionView? | MauiReactor

MauiReactor wraps standard .NET MAUI controls, so everything works exactly in the same way it behaves in the .NET MAUI framework.

The CollectionView control doesn't make exceptions. The only difference is that you should not create a Binding between the CollectionView ItemsSource property and the underlying collection of models.

## [Direct link to heading](\#read-only-collectionview)    Read-Only CollectionView

When you're dealing with read-only collections of items to render or you are going to just replace the entire collection during the lifetime of the application you can just connect the CollectionView ItemsSource property providing a Render function used by MauiReactor to actually render each item.

For example, in this code the CollectionView renders its items as labels:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CollectionViewPage: Component
{
    static CollectionViewPage()
    {
        ItemsSource = Enumerable.Range(1, 200)
            .Select(_ => new Tuple<string, string>($"Item{_}", $"Details{_}"))
            .ToArray();
    }

    static Tuple<string, string>[] ItemsSource { get; }

    public override VisualNode Render()
    {
        return new ContentPage("CollectionView")
        {
            new CollectionView()
                .ItemsSource(ItemsSource, RenderItem)
        };
    }

    private VisualNode RenderItem(Tuple<string, string> item)
        => new VStack(spacing: 5)
        {
            new Label(item.Item1),
            new Label(item.Item2)
        };
}
```

More often, the list of items is stored inside the State of the component and read/updated by a call to an external service (like a rest API).

The Render method passed to the ItemsSource property (row 17 above) is used by MauiReactor to create a DataTemplate used by the native CollectionView to render its items. The DataTemplate is cached and reused by MauiReactor every time is requested by the native control.

## [Direct link to heading](\#read-write-collectionview)    Read-Write CollectionView

In case you are going to modify the collection of items without just resetting it, for example, adding/removing single or multiple items at runtime, you can opt for the ObservableCollection type.

For example, this code lets the user add/remove items from the underlying collection and the CollectionView control updates accordnly:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class EditableCollectionViewState
{
    public ObservableCollection<string> Items { get; set; } = new();
}

class EditableCollectionView: Component<EditableCollectionViewState>
{
    static readonly Random _random = new();
    public override VisualNode Render()
    {
        return new ContentPage("CollectionView")
        {
            new Grid("48, 44, *", "*, *")
            {
                new Button("Add Item")
                    .OnClicked(()=>SetState(s => s.Items.Insert(_random.Next(0, s.Items.Count), $"Item {DateTime.Now}"), invalidateComponent: false)),

                new Button("Remove Item")
                    .OnClicked(()=>SetState(s => s.Items.RemoveAt(_random.Next(0, s.Items.Count - 1)), invalidateComponent: false))
                    .GridColumn(1)
                    .IsEnabled(()=>State.Items.Count > 0),

                new Label()
                    .Text(()=> $"{State.Items.Count} Items")
                    .GridRow(1)
                    .GridColumnSpan(2)
                    .HCenter()
                    .VCenter(),

                new CollectionView()
                    .ItemsSource(State.Items, _ => new Label(_))
                    .GridRow(2)
            }
        };
    }
}
```

It's not required (actually it's discouraged) to get a reference to the native CollectionView and forcibly set its ItemsSource property.

[PreviousHow to deal with state shared across Components?](/mauireactor/q-and-a/how-to-deal-with-state-shared-across-components) [NextDo we need to add states to create simple animations such as ScaleTo, FadeTo, etc on tap?](/mauireactor/q-and-a/do-we-need-to-add-states-to-create-simple-animations-such-as-scaleto-fadeto-etc-on-tap)

Last updated 1 year ago

---

# NavigationPage | MauiReactor

You can navigate between pages (i.e. between root components) using the following extensions API:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
INavigation.PushAsync<T>()
```

where T is the type of component to create as the root of the new page.

For example, this is a component that moves to a second page hosting the ChildPageComponent when the user click a button:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class MainPageComponent : Component
{
    public override VisualNode Render()
    {
        return new NavigationPage()
        {
            new ContentPage()
            {
                new Button("Move To Page")
                    .VCenter()
                    .HCenter()
                    .OnClicked(OpenChildPage)
            }
            .Title("Main Page")
        };
    }

    private async void OpenChildPage()
    {
        await Navigation.PushAsync<ChildPageComponent>();
    }
}
```

and, this is the code implementing the child page component with a button to go back

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class ChildPageComponent : Component
{
    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new Button("Back")
                .VCenter()
                .HCenter()
                .OnClicked(GoBack)
        }
        .Title("Child Page");
    }

    private async void GoBack()
    {
        await Navigation.PopAsync();
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FoWL37cUIAS4G38Ieal4s%252FReactorUI_NavigationDemo.gif%3Falt%3Dmedia%26token%3Dace24981-effa-4e02-b9be-a9cbd6aad675&width=768&dpr=4&quality=100&sign=5dc8be72&sv=2)

## [Direct link to heading](\#passing-data-between-pages)    Passing data between pages

We can pass parameters to other pages through component _Props_. Modify the main page component from the above sample to hold a state:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class MainPageComponentState
{
    public int Value { get; set; }
}

public class MainPageComponent : Component<MainPageComponentState>
{
    public override VisualNode Render()
    {
        return new NavigationPage()
        {
            new ContentPage()
            {
                new StackLayout()
                {
                    new Label($"Value: {State.Value}")
                        .FontSize(NamedSize.Large),
                    new Button("Move To Page")
                        .OnClicked(OpenChildPage)
                }
                .VCenter()
                .HCenter()
            }
            .Title("Main Page")
        };
    }

    private async void OpenChildPage()
    {

    }
}
```

In the `OpenChildPage` callback we need to open the child page passing the current value: we want to edit the value in entry control.
Let's change the child page by adding a state and a props class as shown in the following code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class ChildPageComponentState
{
    public int Value { get; set; }
}

public class ChildPageComponentProps
{
    public int InitialValue { get; set; }

    public Action<int> OnValueSet { get; set; }
}

public class ChildPageComponent : Component<ChildPageComponentState, ChildPageComponentProps>
{
    protected override void OnMounted()
    {
        State.Value = Props.InitialValue;

        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new StackLayout()
            {
                new Entry(State.Value.ToString())
                    .OnAfterTextChanged(_=>SetState(s => s.Value = int.Parse(_)))
                    .Keyboard(Keyboard.Numeric),
                new Button("Back")
                    .OnClicked(GoBack)
            }
            .VCenter()
            .HCenter()
        }
        .Title("Child Page");
    }

    private async void GoBack()
    {
        Props.OnValueSet(State.Value);

        await Navigation.PopAsync();
    }
}

```

Now the ChildPageComponent has a _state_ and a _props_: the latter will hold the initial value and a callback action to call when the user click the back button.

Finally, call the child page setting its _props_ properties:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
private async void OpenChildPage()
{
    await Navigation.PushAsync<ChildPageComponent, ChildPageComponentProps>(_=>
    {
        _.InitialValue = State.Value;
        _.OnValueSet = this.OnValueSetFromChilPage;
    });
}

private void OnValueSetFromChilPage(int newValue)
{
    SetState(s => s.Value = newValue);
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FIjh39g5DWeCC5W7LwXwT%252FReactorUI_NavicationParamsDemo.gif%3Falt%3Dmedia%26token%3D6cd3b9cc-7d8c-4010-985c-d75105abe335&width=768&dpr=4&quality=100&sign=7b59bb6&sv=2)

Resulting app behavior

## [Direct link to heading](\#handle-notifications-from-pages-when-appearing-or-disappearing)    Handle notifications from pages when appearing or disappearing

In MauiReactor components are deactivated when are no more referenced by any other parent components: when this happens MauiReactor calls the `OnWillUnmount` method to let the developer do extra finalization stuff.
Makes exceptions the case when the component is the root of the page. For performance reasons when the page disappears, MauiReactor doesn't traverse for the last time the visual tree so it doesn't call the `OnWillUnmount` on the root component.

Maybe you need to do some work when the page is disappearing; the following code shows how you handle the disappearing event from both the calling and the called page.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class FirstPage : Component
{
    protected override void OnWillUnmount()
    {
        //this won't be called because is the root component of the page
        Debug.WriteLine("OnWillUnmount FirstPage");
    }

    protected override void OnMounted()
    {
        Debug.WriteLine("OnMounted FirstPage");
    }

    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new VStack()
            {
                new Label("First!"),
                new Button("Next")
                    .OnClicked(async () =>
                    {
                        var secondPage = await Navigation?.PushAsync<SecondPage>();
                        secondPage.Disappearing+=(s,e)=> Debug.WriteLine("Disappearing SecondPage");
                    })
            }
            .Spacing(20)
            .HCenter()
            .VCenter()
        }.Title("First");
    }
}

public class SecondPage : Component
{
    protected override void OnWillUnmount()
    {
        //this won't be called because is the root component of the page
        Debug.WriteLine("OnWillUnmount SecondPage");
    }

    protected override void OnMounted()
    {
        Debug.WriteLine("OnMounted SecondPage");
    }

    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new VStack()
            {
                new Label("Second!"),
                new Button("Back")
                    .OnClicked(() => Navigation?.PopAsync())
            }
            .Spacing(20)
            .HCenter()
            .VCenter()
        }
        .Title("Second")
        .OnDisappearing(()=> Debug.WriteLine("Disappearing SecondPage from SecondPage"));
    }
}

public class App : Component
{
    public override VisualNode Render()
    {
        return new NavigationPage()
        {
            new FirstPage()
        }.Set(MauiControls.NavigationPage.BarTextColorProperty, Colors.White);
    }
}c
```

[PreviousNavigation](/mauireactor/components/navigation) [NextShell](/mauireactor/components/navigation/shell)

Last updated 11 months ago

---

# Property-Base animation | MauiReactor

This kind of animation is the first introduced in ReactorUI for Xamarin Forms library and essentially means _animating properties between component states_.

.NET MAUI already contains a set of useful functions that let you move properties of controls over time according to a tween function. You can still use standard animation functions in MauiReactor, just get a reference to the control as explained [here](/mauireactor/components/accessing-native-controls).

In the most simple scenarios, enabling animations in MauiReactor is as simple as calling the function `WithAnimation()` over the component you want to animate.

Let's create an example to illustrate the overall process. Consider a page that contains a frame and an image inside it that is initially hidden. When the user taps the image we want to gradually show it and scale it to full screen.

This is the initial code of the sample:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class MainPageState
{
    public bool Toggle { get; set; }
}

public class MainPage : RxComponent<MainPageState>
{
    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new Frame()
            {
                new Image()
                    .HCenter()
                    .VCenter()
                    .Source("city.jpg")
            }
            .OnTap(()=>SetState(s => s.Toggle = !s.Toggle))
            .HasShadow(true)
            .Scale(State.Toggle ? 1.0 : 0.5)
            .Opacity(State.Toggle ? 1.0 : 0.0)
            .Margin(10)
        };
    }
}
```

Running the application you should see something like this:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FgejrqoLJ5cKdFarFwvoi%252FReactorUI_Animation2.gif%3Falt%3Dmedia%26token%3D8dadd5e6-3542-45a7-bf23-79fd6e221bec&width=768&dpr=4&quality=100&sign=e3db0801&sv=2)

Animating Scale and Opacity properties

Now let's add `WithAnimation()` to image component:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
return new NavigationPage()
{
    new ContentPage("Animation Demo")
    {
        new Image()
            .HCenter()
            .VCenter()
            .Source("city.jpg")
            .OnTap(()=>SetState(s => s.Toggle = !s.Toggle))
            .Opacity(State.Toggle ? 1.0 : 0.0)
            .Scale(State.Toggle ? 1.0 : 0.5)
            .Margin(10)
            .WithAnimation()
    }
};
```

By default `WithAnimation()` enables any pending animation applied to the component before it: in the above sample `Opacity()` and `Scale()` by default add a tween animation each for the respective properties.

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FHUxd8SZAm7LUa7UoZIth%252FReactorUI_Animation3.gif%3Falt%3Dmedia%26token%3D0b59089a-2809-4ab7-9476-e03fef9b3053&width=768&dpr=4&quality=100&sign=2895af07&sv=2)

Sample app with animation (video ported from ReactorUI for Xamarin Forms)

Try experimenting with modifying the position of the call to WithAnimation() or passing a custom duration (600ms is the default value) or easing function (Linear is the default).

Many other properties are _animatable_ by default but you can create custom animation with more complex algorithms for other properties or combinations of properties.

This kind of animation in MauiReactor is inspired by the SwiftUI animation system, so you can even take a look at its documentation to discover other powerful ways of animating between states.

[PreviousAnimation](/mauireactor/components/animation) [NextAnimation with the AnimationController](/mauireactor/components/animation/animation-with-the-animationcontroller)

Last updated 1 year ago

---

# XAML Integration | MauiReactor

Sometimes you may want to adopt the MVU approach only for a portion of the application, for example, when you have already developed a XAML-based app and are not ready to completely re-write it using a different framework like MauiReactor.

XAML integration in MauiReactor allows us to run a MauiReactor component inside a standard MAUI page or view. You can even use MauiReactor component for an entire page while the rest of the app uses a classic MVVM approach.

Using MauiReactor (i.e. MVU) for the entire application remains the recommended way.

Consider this feature if you are facing the following scenarios:

1) You have completed the development of a standard MAUI application or are in an advanced stage of the process where moving to MauiReactor is not feasible but you're interested in adopting MauiReactor in the future

2) You may want to adopt/experiment with the MVU approach for some specific portion or page of the UI

Do not mix MVU and MVVM code/approaches but use the dependency inject to share services.

Let's start by adding MauiReactor Nuget package to the project:

`dotnet add package Reactor.Maui`

To host a MauiReactor component on your page please use the `ComponenHost` class like it's shown in the following snippet code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:MauiReactor="clr-namespace:MauiReactor.Integration;assembly=MauiReactor"
             xmlns:components="clr-namespace:IntegrationTest.Components"
             x:Class="IntegrationTest.MainPage">

    <ScrollView>
        <VerticalStackLayout
            Spacing="25"
            Padding="30,0"
            VerticalOptions="Center">

            <Image
                Source="dotnet_bot.png"
                SemanticProperties.Description="Cute dot net bot waving hi to you!"
                HeightRequest="200"
                HorizontalOptions="Center" />

            <Label
                Text="Hello, World!"
                SemanticProperties.HeadingLevel="Level1"
                FontSize="32"
                HorizontalOptions="Center" />

            <Label
                Text="Welcome to .NET Multi-platform App UI"
                SemanticProperties.HeadingLevel="Level2"
                SemanticProperties.Description="Welcome to dot net Multi platform App U I"
                FontSize="18"
                HorizontalOptions="Center" />

            <MauiReactor:ComponentHost
                Component="{x:Type components:Counter}"/>

            <Button
                Text="Click Me!"
                Clicked="Button_Clicked"
                />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>

```

In line 4, you have to specify the MauiReactor namespace and assembly containing the `ComponentHost` class.

In lines 33 and 34 we're going to create an instance of the control `ComponentHost` passing the `Counter` component type as a parameter: the `ComponentHost` class will instantiate and run the component.

Of course the same can be done in code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
var componentHost = new MauiReactor.ComponentHost
{
    Component = typeof(Counter)
}
```

Another way to integrate a MauiReactor component is to navigate to another page passing a component as its root using the overload of the `Navigation` class `Navigation.PushAsync<Component-Type>()`

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
await Navigation.PushAsync<ChildPage>();
```

Where ChildPage is defined as:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class ChildPage : Component
{
    public override VisualNode Render()
    {
        return new MauiReactor.ContentPage()
        {
            new MauiReactor.Label("Hello from MauiReactor!")
        };
    }
}
```

The same works with the Shell as well. You need to register the route using the static method `Routing.RegisterRoute` and navigate to it with the usual `Shell.Current.GoToAsync` call:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
Routing.RegisterRoute<Page2>("page-2");
...
await MauiControls.Shell.Current.GoToAsync("page-2");
```

[PreviousTesting](/mauireactor/components/testing) [NextNative tree and Visual tree](/mauireactor/deep-dives/native-tree-and-visual-tree)

Last updated 1 year ago

---

# Scrolling | MauiReactor

> The .NET Multi-platform App UI (.NET MAUI) [CollectionView](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.collectionview) defines two `ScrollTo` methods, that scroll items into view. One of the overloads scrolls the item at the specified index into view, while the other scrolls the specified item into view. Both overloads have additional arguments that can be specified to indicate the group the item belongs to, the exact position of the item after the scroll has completed, and whether to animate the scroll.

In MauiReactor, you need a reference to the native `CollectionView` and call the `ScrollTo` method on it.

The following sample code shows how to scroll to a specific position index:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public record IndexedPersonWithAddress(int Index, string FirstName, string LastName, int Age, string Address);

class MainPageScrollingState
{
    public List<IndexedPersonWithAddress> Persons { get; set; }

    public MauiControls.ItemsViewScrolledEventArgs LatestEventArgs { get; set; }

    public int ScrollToIndex {  get; set; }

    public MauiControls.ScrollToPosition ScrollToPosition { get; set; }

    public bool IsAnimationDisabled { get; set; }
}

class MainPageScrolling : Component<MainPageScrollingState>
{
    private MauiControls.CollectionView _collectionViewRef;

    private static List<IndexedPersonWithAddress> GenerateSamplePersons(int count)
    {
        var random = new Random();
        var firstNames = new[] { "John", "Jim", "Joe", "Jack", "Jane", "Jill", "Jerry", "Jude", "Julia", "Jenny" };
        var lastNames = new[] { "Brown", "Green", "Black", "White", "Blue", "Red", "Gray", "Smith", "Doe", "Jones" };
        var cities = new[] { "New York", "London", "Sidney", "Tokyo", "Paris", "Berlin", "Mumbai", "Beijing", "Cairo", "Rio" };

        var persons = new List<IndexedPersonWithAddress>();

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[random.Next(0, firstNames.Length)];
            var lastName = lastNames[random.Next(0, lastNames.Length)];
            var age = random.Next(20, 60);
            var city = cities[random.Next(0, cities.Length)];
            var address = $"{city} No. {random.Next(1, 11)} Lake Park";

            persons.Add(new IndexedPersonWithAddress(i, firstName, lastName, age, address));
        }

        return persons;
    }

    protected override void OnMounted()
    {
        State.Persons = GenerateSamplePersons(100);
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new Grid("Auto, Auto,*", "*")
            {
                new VStack(spacing: 5)
                {
                    new Label($"CenterItemIndex: {State.LatestEventArgs?.CenterItemIndex}"),
                    new Label($"LastVisibleItemIndex: {State.LatestEventArgs?.LastVisibleItemIndex}"),
                    new Label($"FirstVisibleItemIndex: {State.LatestEventArgs?.FirstVisibleItemIndex}"),
                    new Label($"VerticalDelta: {State.LatestEventArgs?.VerticalDelta}"),
                    new Label($"VerticalOffset: {State.LatestEventArgs?.VerticalOffset}"),
                },

                new VStack(spacing: 5)
                {
                    new Entry()
                        .Keyboard(Keyboard.Numeric)
                        .OnTextChanged(_ =>
                        {
                            if (int.TryParse(_, out var scrollToIndex) &&
                                scrollToIndex >= 0 &&
                                scrollToIndex < State.Persons.Count)
                            {
                                SetState(s => s.ScrollToIndex = scrollToIndex);
                            }
                        }),

                    new Picker()
                        .ItemsSource(Enum.GetValues<MauiControls.ScrollToPosition>().Select(_=>_.ToString()).ToArray())
                        .OnSelectedIndexChanged(index => SetState(s => s.ScrollToPosition = (MauiControls.ScrollToPosition)index)),

                    new HStack(spacing: 5)
                    {
                        new CheckBox()
                            .OnCheckedChanged((sender, args) => SetState(s => s.IsAnimationDisabled = args.Value)),

                        new Label("Disable animation")
                            .HFill()
                            .VCenter(),
                    },

                    new Button("Scroll To")
                        .OnClicked(()=> _collectionViewRef?.ScrollTo(State.ScrollToIndex, position: State.ScrollToPosition, animate: !State.IsAnimationDisabled))
                }
                .GridRow(1),

                new CollectionView(collectionViewRef => _collectionViewRef = collectionViewRef)
                    .ItemsSource(State.Persons, RenderPerson)
                    .GridRow(2)
                    .OnScrolled(OnScrolled)
            }
        };
    }

    private void OnScrolled(object sender, MauiControls.ItemsViewScrolledEventArgs args)
    {
        SetState(s => s.LatestEventArgs = args);
    }

    private VisualNode RenderPerson(IndexedPersonWithAddress person)
    {
        return new VStack(spacing: 5)
        {
            new Label($"{person.Index}. {person.FirstName} {person.LastName} ({person.Age})"),
            new Label(person.Address)
                .FontSize(12)
        }
        .HeightRequest(56)
        .VCenter();
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FDczu5vmBi7dhwarWoC6g%252Fimage.png%3Falt%3Dmedia%26token%3D931cc0c4-eb04-492b-be30-fc5ef6bad7ba&width=768&dpr=4&quality=100&sign=f597e5f2&sv=2)

Scrolling collection view to a specific position index

The above sample could be easily modified to scroll to a specific item as well.

### [Direct link to heading](\#control-scroll-position-when-new-items-are-added)    Control scroll position when new items are added

You can control the behavior of the scrolling when new items are added to the CollectionView using the property ItemsUpdatingScrollMode.
For example, setting it to `MauiControls.ItemsUpdatingScrollMode.KeepLastItemInView`, the CollectionView will scroll to the latest item added.

This is an example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPageScrollingModeState
{
    public ObservableCollection<IndexedPersonWithAddress> Persons { get; set; }

    public MauiControls.ItemsUpdatingScrollMode CurrentScrollMode { get; set; }
}

class MainPageScrollingMode : Component<MainPageScrollingModeState>
{
    private MauiControls.CollectionView _collectionViewRef;

    private static List<IndexedPersonWithAddress> GenerateSamplePersons(int count)
    {
        var random = new Random();
        var firstNames = new[] { "John", "Jim", "Joe", "Jack", "Jane", "Jill", "Jerry", "Jude", "Julia", "Jenny" };
        var lastNames = new[] { "Brown", "Green", "Black", "White", "Blue", "Red", "Gray", "Smith", "Doe", "Jones" };
        var cities = new[] { "New York", "London", "Sidney", "Tokyo", "Paris", "Berlin", "Mumbai", "Beijing", "Cairo", "Rio" };

        var persons = new List<IndexedPersonWithAddress>();

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[random.Next(0, firstNames.Length)];
            var lastName = lastNames[random.Next(0, lastNames.Length)];
            var age = random.Next(20, 60);
            var city = cities[random.Next(0, cities.Length)];
            var address = $"{city} No. {random.Next(1, 11)} Lake Park";

            persons.Add(new IndexedPersonWithAddress(i, firstName, lastName, age, address));
        }

        return persons;
    }

    protected override void OnMounted()
    {
        State.Persons = new ObservableCollection<IndexedPersonWithAddress>(GenerateSamplePersons(100));
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new Grid("Auto, Auto, *", "*")
            {
                new Picker()
                    .ItemsSource(Enum.GetValues<MauiControls.ItemsUpdatingScrollMode>().Select(_=>_.ToString()).ToArray())
                    .OnSelectedIndexChanged(index => SetState(s => s.CurrentScrollMode = (MauiControls.ItemsUpdatingScrollMode)index)),

                new Button("Add item", ()=> State.Persons.Add(GenerateSamplePersons(1).First() with { Index = State.Persons.Count }))
                    .GridRow(1),

                new CollectionView()
                    .ItemsSource(State.Persons, RenderPerson)
                    .ItemsUpdatingScrollMode(State.CurrentScrollMode)
                    .GridRow(2)
            }
        };
    }

    private VisualNode RenderPerson(IndexedPersonWithAddress person)
    {
        return new VStack(spacing: 5)
        {
            new Label($"{person.Index}. {person.FirstName} {person.LastName} ({person.Age})"),
            new Label(person.Address)
                .FontSize(12)
        }
        .HeightRequest(56)
        .VCenter();
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FfB7pRV8KEgTfKg5moehE%252FCollectionView.gif%3Falt%3Dmedia%26token%3Dfc096d66-98bf-438e-b7e3-8f964a55a43e&width=768&dpr=4&quality=100&sign=ae2ceced&sv=2)

Sample app showing the ItemsUpdatingScrollMode behavior

[PreviousEmpty view](/mauireactor/components/controls/collectionview/empty-view) [NextGrouping](/mauireactor/components/controls/collectionview/grouping)

Last updated 1 year ago

---

# Animation with the AnimationController | MauiReactor

MauiReactor features a second powerful way to animate views inside a component through the `AnimationController` class and `Animation`-derived types.

`AnimationController` is a standard `VisualNode`-derived class that you can render inside any component tree. Even if you can have more than one AnimationController inside a single component often just one is flexible enough to accomplish most of the animations.

Each `AnimationController` has internally a timer that you can control by playing it or putting it in Pause/Stop.

The `AnimationController` class itself host a list of Animation objects.

These are the types of `Animation` available in MauiReactor so far:

- `ParallelAnimation`: executes child animations in parallel

- `SequenceAnimation`: runs child animations in sequence (i.e. one after another)

- `DoubleAnimation`: Is a tween animation that fires an event containing a value between 2 doubles (From/To). You can customize how this value is generated using an Easing function.

- `CubicBezierPathAnimation`: is a tween animation that generates values as Points between StartPoint and EndPoint using a bezier function in which you can control setting ControlPoint1 and ControlPoint2

- `QuadraticBezierPathAnimation`: is a tween animation similar to the Bezier animation that generates a point between StartPoint and EndPoint using a quadratic bezier function which you can control by setting a ControlPoint


Each `Animation` has a duration and you can compose them as you like in a tree structure.

This is an example of an animation tree extracted from the MauiReactor sample app:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new AnimationController
{
    new SequenceAnimation
    {
        new DoubleAnimation()
            .StartValue(0)
            .TargetValue(300)
            .Duration(TimeSpan.FromSeconds(2.5))
            .Easing(Easing.CubicOut)
            .OnTick(v => ....),

        new DoubleAnimation()
            .StartValue(0)
            .TargetValue(300)
            .Duration(TimeSpan.FromSeconds(1.5))
            .OnTick(v => ....)
    }

    new SequenceAnimation
    {
        new DoubleAnimation()
            .StartValue(0)
            .TargetValue(300)
            .Duration(TimeSpan.FromSeconds(2))
            .OnTick(v => ....),

        new CubicBezierPathAnimation()
            .StartPoint(new Point(0,100))
            .EndPoint(new Point(300,100))
            .ControlPoint1(new Point(0,0))
            .ControlPoint2(new Point(300,200))
            .OnTick(v => ....),

        new QuadraticBezierPathAnimation()
            .StartPoint(new Point(300,100))
            .EndPoint(new Point(0,100))
            .ControlPoint(new Point(150,200))
            .OnTick(v => ....)
    }
}
```

`SequenceAnimation` and `ParallelAnimation` are `Animation` containers that do not fire events (i.e. do not have `OnTick()` property) because their purpose is only to control child animations.

TweenAnimation types like `DoubleAnimation`, `CubicBezierPathAnimation`, and `QuadraticBezierPathAnimation` fire events that you can register with the `OnTick` callback where you can easily set component State properties.

Moving objects is then as easy as connecting animated property values inside the component render to State properties.

Even is technically doable, doesn't make much sense to use an `AnimationController` inside a State-less component

The ` AnimationController` object can be paused ( `IsPaused` = true/false): when an animation is paused it keeps internal values and restarts from the same point.

An `AnimationController` can also be stopped ( `IsEnabled` = true/false) and in this case, the animations of all child objects are restored to the initial state.

Each `Animation` type has specific properties (for example `Animation` containers like SequenceAnimation and ParallelAnimation have InitalDelay or Loop properties) that help you describe exactly the generated values at the correct time.

`AnimationController` is correctly hot-reloaded and keeps its internal state (as any other MauiReactor object) between iterations.

MauiReactor GitHub repository contains several samples that show how to use `AnimationController` in different scenarios.

[PreviousProperty-Base animation](/mauireactor/components/animation/property-base-animation) [NextGraphics](/mauireactor/components/graphics)

Last updated 1 year ago

---

# Empty view | MauiReactor

The following code shows how you can set a string or a full-blow view to show when the CollectionView is empty:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPageEmptyViewState
{
    public ObservableCollection<Person> Persons { get; set; }
}

class MainPageEmptyView : Component<MainPageEmptyViewState>
{
    protected override void OnMounted()
    {
        var person1 = new Person("John", "Doe", new DateTime(1980, 5, 10));
        var person2 = new Person("Jane", "Smith", new DateTime(1990, 6, 20));
        var person3 = new Person("Alice", "Johnson", new DateTime(1985, 7, 30));
        var person4 = new Person("Bob", "Williams", new DateTime(2000, 8, 15));
        var person5 = new Person("Charlie", "Brown", new DateTime(1995, 9, 25));

        State.Persons = new ObservableCollection<Person>(new[] { person1, person2, person3, person4, person5 });
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new Grid("Auto * *", "*")
            {
                new Button("Remove")
                    //here we use SetState even for the Persons ObservableCollection
                    //because we need to update the IsEnabled property of the button
                    .OnClicked(()=>SetState(s => s.Persons.RemoveAt(0)))
                    .IsEnabled(State.Persons.Count > 0),

                new CollectionView()
                    .ItemsSource(State.Persons, RenderPerson)
                    //1 use just a string
                    .EmptyView("No more persons!")
                    .GridRow(1),

                new CollectionView()
                    .ItemsSource(State.Persons, RenderPerson)
                    //pass in a full view
                    .EmptyView(new VStack(spacing: 5)
                    {
                        new Label("No more persons!"),

                        new Image("error.png")
                    }
                    .VCenter()
                    .HCenter())
                    .GridRow(2)
            }
        };
    }

    private VisualNode RenderPerson(Person person)
    {
        return new VStack(spacing: 5)
        {
            new Label($"{person.FirstName} {person.LastName}"),
            new Label(person.DateOfBirth.ToShortDateString())
                .FontSize(12)
                .TextColor(Colors.Gray)
        }
        .Margin(5,10);
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FagcqyuUiQqLS2PJxuaYD%252Fimage.png%3Falt%3Dmedia%26token%3D487cd634-17f2-4769-96f5-26fa962bc702&width=768&dpr=4&quality=100&sign=330d17ad&sv=2)

Shows 2 diffrent empty views for the CollectionView

[PreviousSelection](/mauireactor/components/controls/collectionview/selection) [NextScrolling](/mauireactor/components/controls/collectionview/scrolling)

Last updated 1 year ago

---

# Accessing native controls | MauiReactor

Sometimes you need a reference to the native control (i.e. MAUI control); for example, say you need to move focus to Entry control when the component starts up.

In MauiReactor, you can get a reference to the underlying widget passing an Action argument when constructing the visual object accepting the reference to the native control: you can easily save it in a private field and reuse it wherever.

For example, in this code when a button is clicked, the textbox (i.e. Entry) is focused:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class MainPage : Component
{
    private MauiControls.Entry _entryRef;

    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new VStack(spacing: 10)
            {
                new Entry(entryRef => _entryRef = entryRef),

                new Button("Click here!")
                    .OnClick(OnButtonClicked)
                    .VCenter()
                    .HCenter()
            }
        };
    }

    private void OnButtonClicked()
        => _entryRef?.Focus();
}
```

The page containing the current component is also accessible with the convenient `ContainerPage` property

Sometimes you need to set a specific dependency property of the native control:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new Button()
    .Set(BindableProperty property, object? value);
```

If you want to set an attached dependency property:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new Button()
    .SetAttachedProperty(BindableProperty property, object? value);
```

[PreviousProvide DataTemplate to native controls](/mauireactor/components/wrap-3rd-party-controls/provide-datatemplate-to-native-controls) [NextAnimation](/mauireactor/components/animation)

Last updated 1 year ago

---

# Using XAML Resources | MauiReactor

MauiReactor supports the XAML styling system. You can specify your resources in the startup stage with code as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiReactorApp<ShellPage>(app =>
    {
        app.AddResource("Resources/Styles/DefaultTheme.xaml");
    })
```

Hot-reloading an application that links XAML files may require you to set the Full mode option: `dotnet-maui-reactor -f net6.0-android --mode Full`

Every resource found in the specified files is directly accessible inside components just by accessing the Application.Resources dictionary:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new Button()
.Set(VisualElement.StyleProperty, Application.Current.Resources["MyStyle"])
```

Even if technically doable it's not possible to store or retrieve resources from the `VisualElement.Resources` dictionary. Resource lookup (from the current VisualElement up to the Application) is actually discouraged in MauiReactor while it's perfectly fine to have all the resources in files that are linked at startup as shown above.

It's even possible (but not recommended) to create a DynamicResource link between a DependencyProperty and a Resource using code like shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new Button(buttonRef => buttonRef?.SetDynamicResource(VisualElement.StyleProperty, "MyStyle"))
```

DynamicResource in .NET MAUI allows you to "link" a dependency property to a Resource (like a Style for example) so that when you change the resource the dependency property is updated accordingly. MauiReactor doesn't play well with it because it directly refreshes dependency properties after component invalidation (for more info get a look at the [Native vs Visual Tree page](/mauireactor/deep-dives/native-tree-and-visual-tree).

MauiReactor repo on GitHub contains the WeatherTwentyOne sample project that is a direct porting from a classic .NET MAUI project that makes heavy usage of XAML resource files (this is the [link](https://github.com/adospace/reactorui-maui/tree/main/samples/MauiReactor.WeatherTwentyOne)).

[PreviousMigrating from MVVM Model](/mauireactor/deep-dives/migrating-from-mvvm-model) [NextBehaviors](/mauireactor/deep-dives/behaviors)

Last updated 2 years ago

---

# Grouping | MauiReactor

> Large data sets can often become unwieldy when presented in a continually scrolling list. In this scenario, organizing the data into groups can improve the user experience by making it easier to navigate the data.

In MauiReactor you can create a grouped view of your collection using an overload of the ItemsSource prop method as shown in the example below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class Animal
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Details { get; set; }
    public string ImageUrl { get; set; }

    public override string ToString()
    {
        return Name;
    }
}

public class AnimalGroup : List<Animal>
{
    public string Name { get; private set; }

    public AnimalGroup(string name, List<Animal> animals) : base(animals)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}

class MainPageGroupingState
{
    public IEnumerable<AnimalGroup> Animals { get; set; }
}

class MainPageGrouping : Component<MainPageGroupingState>
{
    protected override void OnMounted()
    {
        State.Animals = CreateAnimalsCollection(false);

        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new CollectionView()
                .IsGrouped(true)
                .ItemsSource<CollectionView, AnimalGroup, Animal>(State.Animals, RenderPerson, RenderHeader, RenderFooter)
        };
    }
    private VisualNode RenderPerson(Animal animal)
    {
        return new Grid("*,*", "64, *")
        {
            new Image(animal.ImageUrl)
                .GridRowSpan(2),

            new Label(animal.Name)
                .GridColumn(1),

            new Label(animal.Location)
                .GridRow(1)
                .GridColumn(1)
                .FontSize(12)
                .TextColor(Colors.Gray)
        }
        .HeightRequest(68)
        .Padding(4);
    }

    private VisualNode RenderHeader(AnimalGroup group)
    {
        return new Label(group.Name)
            .Padding(5)
            .FontSize(40)
            .BackgroundColor(Colors.Gray)
            .TextColor(Colors.White);
    }

    private VisualNode RenderFooter(AnimalGroup group)
    {
        return new Label($"Animals in the group: {group.Count}")
            .Padding(5);
    }

    private static IEnumerable<AnimalGroup> CreateAnimalsCollection(bool includeEmptyGroups)
    {
        ...

        return animals;
    }

}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FIcdIP7nBwRkj0mGmQfos%252Fimage.png%3Falt%3Dmedia%26token%3Dd119b486-28b1-4548-86ab-1964b0277885&width=768&dpr=4&quality=100&sign=93dd902b&sv=2)

A CollectionView with group headers and footers

[PreviousScrolling](/mauireactor/components/controls/collectionview/scrolling) [NextIndicatorView](/mauireactor/components/controls/indicatorview)

Last updated 1 year ago

---

# Provide DataTemplate to native controls | MauiReactor

When integrating external libraries in MauiReactor, sometimes you need to provide a DataTemplate to the native control in order to render its content.

For example, consider the SfPopup control from Syncfusion vendor: if you want to customize its content you need to set a DataTemplate to the ContentTemplate property ( [https://help.syncfusion.com/maui/popup/layout-customizations](https://help.syncfusion.com/maui/popup/layout-customizations)).

In MauiReactor you can use a code like this:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
[Scaffold(typeof(Syncfusion.Maui.Popup.SfPopup))]
public partial class SfPopup
{
    public SfPopup Content(Func<VisualNode> render)
    {
        this.Set(Syncfusion.Maui.Popup.SfPopup.ContentTemplateProperty,
            new MauiControls.DataTemplate(() => TemplateHost.Create(render()).NativeElement));

        return this;
    }
}

[Scaffold(typeof(Syncfusion.Maui.Core.SfView))]
public abstract class SfView { }

class MainPageState
{
    public bool IsPopupOpen { get; set; }
}

class MainPage : Component<MainPageState>
{
    public override VisualNode Render()
    {
        return new ContentPage("Pagina uno")
        {
            new VStack() {
                new Label("Main page"),
                new Button().Text("Apri popup").OnClicked(ApriPopup),

                new SfPopup()
                    .Content(()=>
                        new VStack()
                        {
                            new Label("my label")
                        })
                    .IsOpen(State.IsPopupOpen)
                    .OnClosed(()=>SetState(s => s.IsPopupOpen = false, false))

            }.VCenter().HCenter()
        };
    }

    private void ApriPopup()
    {
        SetState(s => s.IsPopupOpen = true);
    }
}
```

Integrating native controls that require DataTemplate to render their content (for example dealing with CollectionView-like controls) could be a complex task.

Often the best approach is following what is already realized in MauiReactor itself, for example looking at CollectionView or ListView implementations.

If you encounter a problem, please look for similar issues or open a new one in MauiReactor GitHub repository.

[PreviousLottie animations](/mauireactor/components/wrap-3rd-party-controls/lottie-animations) [NextAccessing native controls](/mauireactor/components/accessing-native-controls)

Last updated 1 year ago

---

# How to create a Menu/ContextMenu? | MauiReactor

.NET Maui allows developers to create Menus and ContextMenu for desktop apps.

In MauiReactor you can easily add a Menu bar to a desktop application by adding the `MenuBarItem` control under a page like it's shown in the following example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new ContentPage
{
    new MenuBarItem("File")
    {
        new MenuFlyoutSubItem("Project")
        {
            new MenuFlyoutItem("Open...")
                .OnClicked(OnOpenProject),
        },
        new MenuFlyoutSeparator(),
        new MenuFlyoutItem("Exit")
            .OnClicked(OnExitApplication),
    },

    RenderBody()
}
```

For the context menu, you have to add the MenuFlyout under the control that should show the menu like it's shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new Label(person.Initial)
{
    new MenuFlyout
    {
        new MenuFlyoutItem("MenuItem1")
            .OnClicked(()=>OnClickMenuItem("MenuItem1")),
        new MenuFlyoutItem("MenuItem2")
            .OnClicked(()=>OnClickMenuItem("MenuItem2")),
        new MenuFlyoutItem("MenuItem3")
            .OnClicked(()=>OnClickMenuItem("MenuItem3")),
    }
}
```

Please note that not all controls support the context menu (like the ViewCell): for a list of unsupported controls please refer to official .NET MAUI documentation.

[PreviousHow to deal with custom dialogs/popups?](/mauireactor/q-and-a/how-to-deal-with-custom-dialogs-popups)

Last updated 1 year ago

---

# Picker | MauiReactor

> The .NET Multi-platform App UI (.NET MAUI) [Picker](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.picker) displays a short list of items, from which the user can select an item.

Official documentation:
[https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/picker](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/picker)

MauiReactor sample app:
[https://github.com/adospace/mauireactor-samples/tree/main/Controls/PickerTestApp](https://github.com/adospace/mauireactor-samples/tree/main/Controls/PickerTestApp)

The sample code below shows how to implement the Picker control in MauiReactor:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]

public record Person(string FirstName, string LastName, DateTime DateOfBirth)
{
    public override string ToString()
        => $"{FirstName} {LastName} ({(DateTime.Today.Year - DateOfBirth.Year)})";
};

class MainPageState
{
    public Person[] Persons { get; set; }

    public int SelectedPersonIndex { get; set; } = -1;

    public Person SelectedPerson
        => SelectedPersonIndex == -1 ? null : Persons[SelectedPersonIndex];
}

class MainPage : Component<MainPageState>
{
    protected override void OnMounted()
    {
        var person1 = new Person("John", "Doe", new DateTime(1980, 5, 10));
        var person2 = new Person("Jane", "Smith", new DateTime(1990, 6, 20));
        var person3 = new Person("Alice", "Johnson", new DateTime(1985, 7, 30));
        var person4 = new Person("Bob", "Williams", new DateTime(2000, 8, 15));
        var person5 = new Person("Charlie", "Brown", new DateTime(1995, 9, 25));

        State.Persons = new[] { person1, person2, person3, person4, person5 };
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new VStack(spacing: 25)
            {
                new Picker()
                    .Title("Select a person")
                    .SelectedIndex(State.SelectedPersonIndex)
                    .ItemsSource(State.Persons.Select(_=> $"{_.FirstName} {_.LastName}").ToList())
                    .OnSelectedIndexChanged(index => SetState(s => s.SelectedPersonIndex = index)),

                new Label($"Selected: {State.SelectedPerson}"),

                new Button("Reset")
                    .OnClicked(()=> SetState(s => s.SelectedPersonIndex = -1))
            }
            .VCenter()
            .Padding(10,0)
        };
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FMrPW5W697U29F3jdxjES%252Fimage.png%3Falt%3Dmedia%26token%3Dd151b2c9-2be6-421e-aed2-ab471d326cfc&width=768&dpr=4&quality=100&sign=54828957&sv=2)

MauiReactor Picker in action

[PreviousIndicatorView](/mauireactor/components/controls/indicatorview) [NextShell](/mauireactor/components/controls/shell)

Last updated 11 months ago

---

# Migrating from MVVM Model | MauiReactor

#### [Direct link to heading](\#component-based-ui-vs-view-viewmodel)    Component-Based UI vs View-ViewModel

.NET MAUI promotes the separation between the View (usually written in XAML) and the Model (usually written in C#). This pattern is called View-ViewModel and has been historically adopted by a lot of UI frameworks like WPF/SL, Angular, etc.

Recently Component based pattern has gained much popularity thanks to ReactJS and Flutter.

#### [Direct link to heading](\#net-maui-view-viewmodel)    .NET MAUI View-ViewModel

Let's take for example a login page written in .NET MAUI composed of a MainPage (XAML) and a ViewModel MainPageViewModel (c#):

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:d="http://xamarin.com/schemas/2014/forms/design"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d"
             x:Class="XamarinApp1.MainPage">
    <StackLayout
        VerticalOptions="Center"
        HorizontalOptions="Center">
        <Entry Placeholder="Username" Text="{Binding Username}" />
        <Entry Placeholder="Password" Text="{Binding Password}" />
        <Button Text="Login" Command="{Binding LoginCommand}" />
    </StackLayout>
</ContentPage>
```

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class MainPageViewModel: BindableObject
{
    private string _username;

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged();
                LoginCommand.ChangeCanExecute();
            }
        }
    }

    private string _password;

    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged();
                LoginCommand.ChangeCanExecute();
            }
        }
    }

    private Command _loginCommand;

    public Command LoginCommand
    {
        get
        {
            _loginCommand = _loginCommand ?? new Command(OnLogin, () => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));
            return _loginCommand;
        }
    }

    private void OnLogin()
    {
        //Username contains username and Password contains password
        //make login..
    }
}

```

Nowadays, developers can take advantage of the latest [MVVM toolkit](https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction) package that reduces much of the verbosity in writing ViewModels class.

#### [Direct link to heading](\#reactorui-component-based)    ReactorUI Component-Based

Following is the same login page but written using MauiReactor:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class MainPageState
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class MainPage : Component<MainPageState>
{
    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new StackLayout()
            {
                new Entry()
                    .Placeholder("Username")
                    .OnTextChanged((s,e)=> SetState(_ => _.Username = e.NewTextValue)),
                new Entry()
                    .Placeholder("Password")
                    .OnTextChanged((s,e)=> SetState(_ => _.Password = e.NewTextValue)),
                new Button("Login")
                    .IsEnabled(!string.IsNullOrWhiteSpace(State.Username) && !string.IsNullOrWhiteSpace(State.Password))
                    .OnClick(OnLogin)
            }
            .VCenter()
            .HCenter()
        };
    }

    private void OnLogin()
    {
        //use State.Username and State.Password to login...
    }
}
```

[PreviousWorking with the GraphicsView](/mauireactor/deep-dives/working-with-the-graphicsview) [NextUsing XAML Resources](/mauireactor/deep-dives/using-xaml-resources)

Last updated 2 years ago

---

# Selection | MauiReactor

MauiReactor provides 2 ways to be notified when the user selects an item:

1. OnSelected<CollectionView, T> is called when a new item is selected, T is the type of the model bound to the ItemsSource property of the CollectionView

2. OnSelectionChanged() reports old and new selections, particularly useful when you have SelectionMode set to Multiple


By default selection is disabled, set SelectionMode to either Single or Multiple to enable it

In the following sample code we see how selection mode affects the selection of the items of a CollectionView:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPageSelectionState
{
    public Person[] Persons { get; set; }

    public ObservableCollection<string> EventMessages = new();

    public MauiControls.SelectionMode SelectionMode { get; set; }
}

class MainPageSelection : Component<MainPageSelectionState>
{
    protected override void OnMounted()
    {
        var person1 = new Person("John", "Doe", new DateTime(1980, 5, 10));
        var person2 = new Person("Jane", "Smith", new DateTime(1990, 6, 20));
        var person3 = new Person("Alice", "Johnson", new DateTime(1985, 7, 30));
        var person4 = new Person("Bob", "Williams", new DateTime(2000, 8, 15));
        var person5 = new Person("Charlie", "Brown", new DateTime(1995, 9, 25));

        State.Persons = new [] { person1, person2, person3, person4, person5 };
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new Grid("Auto,*,*", "*")
            {
                new Picker()
                    .ItemsSource(Enum.GetValues<MauiControls.SelectionMode>().Select(_=>_.ToString()).ToList())
                    .SelectedIndex((int)State.SelectionMode)
                    .OnSelectedIndexChanged(index => SetState(s => s.SelectionMode = (MauiControls.SelectionMode)index)),

                new CollectionView()
                    .ItemsSource(State.Persons, RenderPerson)
                    .SelectionMode(State.SelectionMode)
                    //use either the OnSelectionChanged callback or OnSelected<CollectionView, Person>
                    .OnSelectionChanged(OnSelectionChanged)
                    //.OnSelected<CollectionView, Person>(OnSelectedSinglePerson)
                    .GridRow(1),

                new CollectionView()
                    .ItemsSource(State.EventMessages, message => new Label(message).Margin(4,8))
                    .GridRow(2),
            }
        };
    }

    private void OnSelectedSinglePerson(Person person)
    {
        State.EventMessages.Add($"Selected: {person}");
    }

    private void OnSelectionChanged(object sender, MauiControls.SelectionChangedEventArgs args)
    {
        State.EventMessages.Add($"Previous selection: {string.Join(",", args.PreviousSelection)}");
        State.EventMessages.Add($"Current selection: {string.Join(",", args.CurrentSelection)}");
    }

    private VisualNode RenderPerson(Person person)
    {
        return new VStack(spacing: 5)
        {
            new Label($"{person.FirstName} {person.LastName}"),
            new Label(person.DateOfBirth.ToShortDateString())
                .FontSize(12)
                .TextColor(Colors.Gray)
        }
        .Margin(5,10);
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252F93u1L9cYV0IgFOmXOIrH%252Fimage.png%3Falt%3Dmedia%26token%3Dbcf5f749-1390-4d3e-b6bd-efa79d124796&width=768&dpr=4&quality=100&sign=ad675ae3&sv=2)

The CollectionView control with 2 items selected

You can change the selected item using the method SelectedItem() shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPageSelection2State
{
    public List<Person> Persons { get; set; }

    public Person SelectedPerson { get; set; }
}

class MainPageSelection2 : Component<MainPageSelection2State>
{
    protected override void OnMounted()
    {
        var person1 = new Person("John", "Doe", new DateTime(1980, 5, 10));
        var person2 = new Person("Jane", "Smith", new DateTime(1990, 6, 20));
        var person3 = new Person("Alice", "Johnson", new DateTime(1985, 7, 30));
        var person4 = new Person("Bob", "Williams", new DateTime(2000, 8, 15));
        var person5 = new Person("Charlie", "Brown", new DateTime(1995, 9, 25));

        State.Persons = new List<Person>(new[] { person1, person2, person3, person4, person5 });
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new Grid("Auto,*", "*")
            {
                new Grid("*", "*,*,*")
                {
                    new Button("Up")
                        .OnClicked(MoveUp),
                    new Button("Down")
                        .GridColumn(1)
                        .OnClicked(MoveDown),
                    new Button("Reset")
                        .GridColumn (2)
                        .OnClicked(()=>SetState(s => s.SelectedPerson = null)),
                },

                new CollectionView()
                    .ItemsSource(State.Persons, RenderPerson)
                    .SelectedItem(State.SelectedPerson)
                    .SelectionMode(MauiControls.SelectionMode.Single)
                    .OnSelected<CollectionView, Person>(OnSelectedSinglePerson)
                    .GridRow(1),
            }
        };
    }

    private void MoveUp()
    {
        if (State.SelectedPerson == null)
        {
            return;
        }

        var indexOfPerson = State.Persons.IndexOf(State.SelectedPerson);
        if (indexOfPerson > 0)
        {
            indexOfPerson--;
            SetState(s => s.SelectedPerson = State.Persons[indexOfPerson]);
        }
    }

    private void MoveDown()
    {
        if (State.SelectedPerson == null)
        {
            return;
        }

        var indexOfPerson = State.Persons.IndexOf(State.SelectedPerson);
        if (indexOfPerson < State.Persons.Count - 1)
        {
            indexOfPerson++;
            SetState(s => s.SelectedPerson = State.Persons[indexOfPerson]);
        }
    }

    private void OnSelectedSinglePerson(Person person)
    {
        SetState(s => s.SelectedPerson = person);
    }

    private VisualNode RenderPerson(Person person)
    {
        return new VStack(spacing: 5)
        {
            new Label($"{person.FirstName} {person.LastName}"),
            new Label(person.DateOfBirth.ToShortDateString())
                .FontSize(12)
                .TextColor(Colors.Gray)
        }
        .Margin(5,10);
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FRftFpQeR4mCfpMQUnC76%252Fimage.png%3Falt%3Dmedia%26token%3Dea52ed07-0cbc-49f6-9742-1a3aff599817&width=768&dpr=4&quality=100&sign=48e3c7da&sv=2)

Move the selected item up and down

Selection background colors can be modified using an XAML resource as shown [here](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview/selection#change-selected-item-color). In MauiReactor, you can also modify the render of each item accordnly to the state of the component.

For example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
private VisualNode RenderPerson(Person person)
{
    return new VStack(spacing: 5)
    {
        new Label($"{person.FirstName} {person.LastName}"),
        new Label(person.DateOfBirth.ToShortDateString())
            .FontSize(12)
            .TextColor(Colors.Gray)
    }
    .BackgroundColor(State.SelectedPerson == person ? Colors.Green : Colors.White)
    .Padding(5,10);
}
```

## [Direct link to heading](\#visual-states)    Visual States

Visual states allow you to modify some properties of the item containers like the background color when items assume specific states.

For example, you can link the background of the items to their selected state:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new CollectionView()
    .SelectionMode(MauiControls.SelectionMode.Single)
    .ItemsSource(ItemsSource, RenderItem)
    .ItemVisualState(nameof(CommonStates), CommonStates.Normal, MauiControls.VisualElement.BackgroundColorProperty, Colors.Transparent)
    .ItemVisualState(nameof(CommonStates), CommonStates.Selected, MauiControls.VisualElement.BackgroundColorProperty, Colors.LightCoral)

```

[PreviousLayout](/mauireactor/components/controls/collectionview/layout) [NextEmpty view](/mauireactor/components/controls/collectionview/empty-view)

Last updated 8 months ago

---

# Working with the GraphicsView | MauiReactor

[GraphicsView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/graphicsview) is a powerful control in .NET MAUI package that is capable of hiding all the complexity of using direct drawing commands among all the supported platforms: in other words, you can just place the control and issue drawing commands (like DrawReactangle, or DrawString).

Drawing graphics objects like Text, Rectangles, or Shapes in GraphicsView is generally more performant than creating native controls like Labels. In the brief article will see how to instantiate and use a GraphicsView in MauiReactor.

MauiReactor supports other means of drawing directly on a canvas. Please refer to this page to learn which are the other ways.

GraphicsView usage is pretty simple: you have just to create it and provide a function for its OnDraw callback where you can issue commands on the Canvas object in order to draw lines, text, shapes, and so on.

Let's start creating a page with a GraphicsView control inside a page and handle the OnDraw callback:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class GraphicsViewPage : Component
{
    public override VisualNode Render()
    {
        return new ContentPage("GraphicsView Sample")
        {
            new GraphicsView()
                .OnDraw(OnDraw)
        };
    }

    private void OnDraw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FontColor = Colors.Red;
        canvas.FontSize = 24;
        canvas.DrawString("GraphicsView", dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
```

This should result in something similar to the below screen:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FBhojQruR83KvSNIIjG7F%252Fimage.png%3Falt%3Dmedia%26token%3D4ca8fa16-33d0-4e4e-8d73-af30204cc0ad&width=768&dpr=4&quality=100&sign=b8fc273&sv=2)

Just a GraphicsView with a text centered

Please note that under iOS and macOS the GraphicsView has a default black background while under Android has transparent default. You should consider setting the background of the canvas to a specific value.

To make the test a bit more interesting let's convert the component into a stateful one and rotate the text with a slider as shown in the following code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class GraphicsViewState
{
    public double Rotation { get; set; }
}

class GraphicsViewPage : Component<GraphicsViewState>
{
    public override VisualNode Render()
    {
        return new ContentPage("GraphicsView Sample")
        {
            new Grid("Auto, *", "*")
            {
                new Slider()
                    .Minimum(0)
                    .Maximum(360)
                    .Value(State.Rotation)
                    .OnValueChanged((s,e)=>SetState(s => s.Rotation = e.NewValue))
                    ,
                new GraphicsView()
                    .GridRow(1)
                    .OnDraw(OnDraw)
            }
        };
    }

    private void OnDraw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Rotate((float)State.Rotation, dirtyRect.Center.X, dirtyRect.Center.Y);
        canvas.FontColor = Colors.Red;
        canvas.FontSize = 24;
        canvas.DrawString("GraphicsView", dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
```

As you can see we can access State properties inside the `OnDraw` method as well.

Hot-reloading can handle any modifications to the GraphicsView control including the OnDraw callback.

This should be the resulting behavior:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252F6wbVDx9IEde30SOHZIxA%252FMauiReactor_GraphicsView.gif%3Falt%3Dmedia%26token%3Dcca90ae8-c41d-4d30-a7fc-9995d6a12b31&width=768&dpr=4&quality=100&sign=b5b8d519&sv=2)

[PreviousDependency injection](/mauireactor/deep-dives/dependency-injection) [NextMigrating from MVVM Model](/mauireactor/deep-dives/migrating-from-mvvm-model)

Last updated 2 years ago

---

# Testing | MauiReactor

Testing an application usually involves 3 different kinds of tests.

1. **Unit Tests**: Tests of single functions or classes aiming to prove the correctness of a single behavior or feature or absence of a specific issue. You can create unit tests for your .NET MAUI app as you would for any other c# program.

2. **Component or Widget Tests**: These kinds of tests are specific to UI apps that use an MVU framework and serve to prove the quality of single components.
MauiReactor provides some neat classes and functions that let you test MauiReactor components: this article describes how to use them.

3. **Integration Tests**: This kind of test, generally, involves the loading of external tools that simulate the real environment, the execution of the app with different settings, and the simulation of a user interacting with the app.
You could create integration tests for MauiReactor using tools like [https://appium.io/](https://appium.io/)


## [Direct link to heading](\#preliminary-steps)    Preliminary steps

First, you need to modify your .NET MAUI application project to be linked to a test project.

Open your project definition; it should be something like the following:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-maccatalyst</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net7.0-windows10.0.19041.0</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>KeeMind</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <Nullable>enable</Nullable>
    ....
  </PropertyGroup>
</Project>

```

Add the target framework `net8.0` (or the current one the app is targeting) and put a condition on the `OutputType` header so that the MSBuild task doesn't produce an exe under the plain `net8.0` framework.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-maccatalyst;net8.0</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net7.0-windows10.0.19041.0</TargetFrameworks>
    <OutputType Condition="'$(TargetFramework)' != 'net8.0'">Exe</OutputType>
    <RootNamespace>KeeMind</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <Nullable>enable</Nullable>
    ....
  </PropertyGroup>
</Project>
```

This way the app project can be referenced in the test project.

Now let's create a test project, and choose the framework you like most (MSTest, xUnit, nUnit, etc).

As a final step, you have to reference the MAUI controls library in the test project adding the `<UseMaui>` header in the project definition:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UseMaui>true</UseMaui>

    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Maui.Controls" Version="8.0.*" />
    <PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="8.0.*" />
  </ItemGroup>

```

## [Direct link to heading](\#test-components)    Test components

The `TemplateHost ` is the key class to use when you want to test a component: it creates a virtual tree of nodes starting from the component you pass to it as the constructor parameter.

To create a `TemplateHost ` just use a code like this:
`TemplateHost.Create(new MyComponent());`

As you have the template host you can use some helper methods on it to traverse the tree to access native controls, check properties, and raise events.

For example, say we want to test this component:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new ContentPage
{
    new VStack
    {
        new Label($"Counter: {State.Counter}"),

        new Button("Click To Increment", () =>
            SetState(s => s.Counter++))
    }
}
```

First, we need to add a way for each control to be identified, one easy way is to augment the code with the AutomationId() method like it's shown in the following snippet:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new ContentPage
{
    new VStack
    {
        new Label($"Counter: {State.Counter}")
            .AutomationId("Counter_Label"),

        new Button("Click To Increment", () =>
            SetState(s => s.Counter++))
            .AutomationId("Counter_Button")
    }
}
```

We can finally create a test to verify that the counter is working:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
var mainPageNode = TemplateHost.Create(new CounterWithServicePage());

// Check that the counter is 0
mainPageNode.Find<MauiControls.Label>("Counter_Label")
    .Text
    .ShouldBe($"Counter: 0");

// Click on the button
mainPageNode.Find<MauiControls.Button>("Counter_Button")
    .SendClicked();

// Check that the counter is 1
mainPageNode.Find<MauiControls.Label>("Counter_Label")
    .Text
    .ShouldBe($"Counter: 1");

```

TemplateHost has many find overloads that let you traverse the tree of controls to find the one you need to test.

## [Direct link to heading](\#components-that-inject-services)    Components that inject services

In case your components need services from DI you have to inject them before creating the TemplateHost through the ServiceContext as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
using var serviceContext = new ServiceContext(services => services.AddSingleton<IncrementService>());
var mainPageNode = TemplateHost.Create(new CounterWithServicePage());
...
```

You have to dispose of the `ServiceContext` object at the end of the test, even better, wrap it with the `using` clause

## [Direct link to heading](\#attach-components-created-with-a-page-or-modal)    Attach components created with a page or modal

Often your components are hosted on a page that is created at runtime for example when the user pushes a button.
In this case, you need to "attach" the new component using the NavigationContainer class as shown in the following sample code:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
using var navigationContainer = new NavigationContainer();

var mainPageNode = TemplateHost.Create(new NavigationMainPage());

// Verify that initially the value is 0
mainPageNode.Find<MauiControls.Label>("MainPage_Label")
    .Text
    .ShouldBe("Value: 0");

// Click the button to open the second page
mainPageNode.Find<MauiControls.Button>("MoveToChildPage_Button")
    .SendClicked();

// here we attach the new component created after the button is clicked
var childPageNode = navigationContainer.AttachHost();

// se entry text to 12
childPageNode.Find<MauiControls.Entry>("ChildPage_Entry")
    .Text = "12";

// click the button to go back to main page
childPageNode.Find<MauiControls.Button>("MoveToMainPage_Button")
    .SendClicked();

// Verify that now the label reports the updated text
mainPageNode.Find<MauiControls.Label>("MainPage_Label")
    .Text
    .ShouldBe("Value: 12");

```

As for the `ServiceConteiner` also the `NavigationContainer` the object must be disposed of when the test ends; again it's a good idea to wrap it with the `using ` keyword

[PreviousWindow](/mauireactor/components/window) [NextXAML Integration](/mauireactor/components/xaml-integration)

Last updated 5 months ago

---

# Back button | MauiReactor

The back button in MAUI has different behavior and settings based on whether you're hosting the child page under a NavigationPage or Shell.

## [Direct link to heading](\#shell)    Shell

Under the Shell, you can use the BackButtonBehavior class to customize the BackButton.

To handle back button behavior you have to provide a call-back action in the `OnBackButtonPressed`

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new ContentPage()
    .OnBackButtonPressed(()=>...);
```

You can also set visibility, icon source etc directly to the Page, like this:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new ContentPage()
    .BackButtonIsVisible(true/false)
    .BackButtonIsEnabled(true/false)
    .BackButtonText(...)
    .BackButtonIcon(...)
```

## [Direct link to heading](\#navigationpage)    NavigationPage

If the child page is hosted under a NavigationPage the BackButton can be disabled using the AttachedProperty HasBackButtonDisable dependency property.
In MauiReactor use this code:

For the BackButton pressed event you have instead to hack it a bit as there isn't a BackButtonPressed event out of the box and the OnBackButtonPressed can't work.

To handle such an event we have to create a Page-derived custom class and override the OnBackButtonPressed method:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public class CustomContentPage : MauiControls.ContentPage
{
    public event EventHandler BackButtonPressed;

    protected override bool OnBackButtonPressed()
    {
        BackButtonPressed?.Invoke(this, EventArgs.Empty);
        return base.OnBackButtonPressed();
    }
}
```

Put the above code in a separate file/namespace different from the namespace containing the component that will scaffold it.

Finally, scaffold and use it in your component:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
[Scaffold(typeof(Project24.Controls.CustomContentPage))]
public partial class CustomContentPage
{
}

public class MainPage : Component
{
    public override VisualNode Render()
    {
        return new NavigationPage()
        {
            new ContentPage()
            {
                new Button("Move To Page")
                    .VCenter()
                    .HCenter()
                    .OnClicked(OpenChildPage)
            }
            .Title("Main Page")
        }
            ;
    }

    private async void OpenChildPage()
    {
        await Navigation.PushAsync<ChildPage>();
    }
}

public class ChildPage : Component
{
    public override VisualNode Render()
    {
        return new CustomContentPage()
        {
            new Button("Back")
                .VCenter()
                .HCenter()
                .OnClicked(GoBack)
        }
        .Set(MauiControls.NavigationPage.HasBackButtonProperty, true)
        .OnBackButtonPressed(OnBack)
        .Title("Child Page");
    }

    async void OnBack()
    {
        await ContainerPage.DisplayAlert("Hey", "hey", "cancel");
    }

    private async void GoBack()
    {
        await Navigation.PopAsync();
    }
}
```

[PreviousShell](/mauireactor/components/navigation/shell) [NextControls](/mauireactor/components/controls)

Last updated 1 year ago

---

# Wrap 3rd party controls | MauiReactor

MauiReactor lets you build MAUI applications from components in an MVU environment. Often you need to integrate external controls that are provided by Microsoft itself, by community OSS projects, or by commercial vendors.

To use these controls in MauiReactor you have to create wrappers that can handle the initializations (i.e. Mount), update (Render), and finalization (Unmount) of the native widget.

For this, MauiReactor provides a source generator that is triggered by the attribute `ScaffoldAttribute`

_Source generators_ are a powerful way to augment user source code. ScaffoldGenerator library creates a MauiReactor wrapper for a native control. You are not strictly required to know how _source generators_ work but if interested take a look [https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)

You just need to create a partial class with the name you like and apply the attribute `Scaffold` with the type of native control that you'd like to embed.

For example, the following code creates a `VisualNode` for the LiveChartsCore.SkiaSharpView.Maui.CartesianChart control from the great charting library :

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
[Scaffold(typeof(LiveChartsCore.SkiaSharpView.Maui.CartesianChart))]
partial class CartesianChart { }
```

A VisualNode is a node in the Visual Tree that MauiReactor creates for each page. It's a lightweight class that handles the initialization, update, and release of a native control in the MVU framework. For more info please take a look at [Native tree and Visual tree](/mauireactor/deep-dives/native-tree-and-visual-tree)

MauiReactor can handle any kind of MAUI controls, but for some, the bare-bone wrapper is created automatically by the `ScaffoldGenerator ` is not enough to handle every aspect of the native widget (for example when a `DataTemplate ` is used to render child items). For those that are required to handle such behavior please take a look at the MauiReactor code (especially the implementation of wrappers for ItemsView and Shell) or open an issue [here](https://github.com/adospace/reactorui-maui/issues)

As you have the wrapper you can use it in a component as any other control:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]

[Scaffold(typeof(LiveChartsCore.SkiaSharpView.Maui.PieChart))]
partial class PieChart { }

[Scaffold(typeof(LiveChartsCore.SkiaSharpView.Maui.CartesianChart))]
partial class CartesianChart { }

[Scaffold(typeof(LiveChartsCore.SkiaSharpView.Maui.PolarChart))]
partial class PolarChart { }

class ChartPageState
{
    public double[] Values { get; set; } = new double[] { 2, 1, 2, 3, 2, 3, 3 };
}

class ChartPage : Component<ChartPageState>
{
    static readonly Random _rnd = new();

    public override VisualNode Render()
        => ContentPage("Chart Sample",
            Grid("*, *, Auto", "*",
               new PolarChart()
                    .Series(() => new ISeries[]
                    {
                        new PolarLineSeries<double>
                        {
                            Values = State.Values,
                            Fill = null,
                            IsClosed = false,
                            Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 5 },
                        }
                    }),

                new CartesianChart()
                    .Series(() => new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Values = State.Values,
                            Fill = null,
                            Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 },

                        }
                    })
                    .GridRow(1),

                Slider()
                    .GridRow(2)
                    .Minimum(2)
                    .Maximum(25)
                    .Margin(5)
                    .Value(()=>State.Values.Length)
                    .OnValueChanged((s, args)=>
                    {
                        SetState(s => s.Values =
                            Enumerable.Range(1, (int)args.NewValue)
                            .Select(_=>_rnd.NextDouble()*20.0)
                            .ToArray(), false);
                    })
            )
        )
        .BackgroundColor(Colors.Black);
}
```

[PreviousLabel](/mauireactor/components/controls/label) [NextLottie animations](/mauireactor/components/wrap-3rd-party-controls/lottie-animations)

Last updated 11 months ago

---

# Animation | MauiReactor

In MauiReactor you have 3 different approaches to animation:

- Property-based animation (RxAnimation)

- AnimationController animation

- Standard MAUI.NET animation (getting a reference to the native control like described [here](/mauireactor/components/accessing-native-controls)) more info at official [.NET Maui documentation](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/animation/basic)


There isn't a preferred approach to animation in MauiReactor: you can even use all of them in a single component.

[PreviousAccessing native controls](/mauireactor/components/accessing-native-controls) [NextProperty-Base animation](/mauireactor/components/animation/property-base-animation)

Last updated 2 years ago

---

# How to deal with custom dialogs/popups? | MauiReactor

This solution is for the CommunityToolkit.Maui Popup:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
[Scaffold(typeof(CommunityToolkit.Maui.Views.Popup))]
partial class Popup
{
    protected override void OnAddChild(VisualNode widget, MauiControls.BindableObject childNativeControl)
    {
        if (childNativeControl is MauiControls.View content)
        {
            Validate.EnsureNotNull(NativeControl);
            NativeControl.Content = content;
        }

        base.OnAddChild(widget, childNativeControl);
    }

    protected override void OnRemoveChild(VisualNode widget, MauiControls.BindableObject childNativeControl)
    {
        Validate.EnsureNotNull(NativeControl);

        if (childNativeControl is MauiControls.View content &&
            NativeControl.Content == content)
        {
            NativeControl.Content = null;
        }
        base.OnRemoveChild(widget, childNativeControl);
    }
}

class PopupHost : Component
{
    private CommunityToolkit.Maui.Views.Popup? _popup;
    private bool _isShown;
    private Action<object?>? _onCloseAction;
    private readonly Action<CommunityToolkit.Maui.Views.Popup?>? _nativePopupCreateAction;

    public PopupHost(Action<CommunityToolkit.Maui.Views.Popup?>? nativePopupCreateAction = null)
    {
        _nativePopupCreateAction = nativePopupCreateAction;
    }

    public PopupHost IsShown(bool isShown)
    {
        _isShown = isShown;
        return this;
    }

    public PopupHost OnClosed(Action<object?> action)
    {
        _onCloseAction = action;
        return this;
    }

    protected override void OnMounted()
    {
        InitializePopup();
        base.OnMounted();
    }

    protected override void OnPropsChanged()
    {
        InitializePopup();
        base.OnPropsChanged();
    }

    void InitializePopup()
    {
        if (_isShown && MauiControls.Application.Current != null)
        {
            MauiControls.Application.Current?.Dispatcher.Dispatch(() =>
            {
                if (ContainerPage == null ||
                    _popup == null)
                {
                    return;
                }

                ContainerPage.ShowPopup(_popup);
            });
        }
    }

    public override VisualNode Render()
    {
        var children = Children();
        return _isShown ?
            new Popup(r =>
            {
                _popup = r;
                _nativePopupCreateAction?.Invoke(r);
            })
            {
                children[0]
            }
            .OnClosed(OnClosed)
            : null!;
    }

    void OnClosed(object? sender, PopupClosedEventArgs args)
    {
        _onCloseAction?.Invoke(args.Result);
    }
}
```

and this is how you can use it in your components:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class ShowPopupTestPage : Component<ShowPopupTestPageState>
{
    private CommunityToolkit.Maui.Views.Popup? _popup;

    public override VisualNode Render()
    {
        return new ContentPage()
        {
            new Grid
            {
                new Button(State.Result == null ? "Show popup" : $"Result: {State.Result.GetValueOrDefault()}")
                    .HCenter()
                    .VCenter()
                    .OnClicked(ShowPopup),

                new PopupHost(r => _popup = r)
                {
                    new VStack(spacing: 10)
                    {
                        new Label("Hi!"),

                        new HStack(spacing: 10)
                        {
                            new Button("OK", ()=> _popup?.Close(true)),

                            new Button("Cancel", ()=> _popup?.Close(false)),
                        }
                    }
                }
                .IsShown(State.IsShown)
                .OnClosed(result => SetState(s =>
                {
                    s.IsShown = false;
                    s.Result = (bool?)result;
                }))
            }
        };
    }

    private void ShowPopup()
    {
        SetState(s => s.IsShown = true);
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2Fuser-images.githubusercontent.com%2F10573253%2F223133271-30c626ed-1fae-4e44-80f3-b351e0618ffb.gif&width=768&dpr=4&quality=100&sign=6082df3&sv=2)

CommunityToolkit MAUI Popup

[PreviousDo we need to add states to create simple animations such as ScaleTo, FadeTo, etc on tap?](/mauireactor/q-and-a/do-we-need-to-add-states-to-create-simple-animations-such-as-scaleto-fadeto-etc-on-tap) [NextHow to create a Menu/ContextMenu?](/mauireactor/how-to-create-a-menu-contextmenu)

Last updated 1 year ago

---

# Shell | MauiReactor

Hosting pages inside a NavigationPage is a basic and flexible way to allow your users to navigate between pages of your application. For more info on how you can use it in MauiReactor please get a look at [this](/mauireactor/components/navigation/navigation).

.NET MAUI provides also a more powerful way to navigate through the pages of your application better fitted for complex applications that have many pages with a well-structured hierarchy.

The Shell is a standard .NET MAUI control that provides many features including a routing-based navigation system. Often, developers set the Shell control as the root of their application.

[Official .NET MAUI documentation](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/?view=net-maui-7.0) provides a good starting point to understand how to use the Shell control and it's mostly applicable to MauiReactor too.

In the following code we'll create a Shell with 2 FlyoutItem:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    public override VisualNode Render()
        => Shell(
            FlyoutItem("Page1",
                ContentPage("Page1")
            ),
            FlyoutItem("Page2",
                ContentPage("Page2")
            )
        )
        .ItemTemplate(RenderItemTemplate);

    static VisualNode RenderItemTemplate(MauiControls.BaseShellItem item)
        => Grid("68", "*",
            Label(item.Title)
                .VCenter()
                .Margin(10,0)
        );
}
```

This is the resulting behavior:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252Fc1MXKRhC5yfKNT082UzC%252FMauiReactor_Shell1.gif%3Falt%3Dmedia%26token%3D2e1ecad9-d29c-4ed7-a77f-6c689ff2385d&width=768&dpr=4&quality=100&sign=f7282383&sv=2)

Shell with FlyouItems

The Shell can have many different kinds of children like FlyoutItem, ShellContent, Tab, ManuItem, etc. (for a full list of combinations please take a look at the official documentation).

## [Direct link to heading](\#loading-pages-on-demand)    Loading pages on demand

In the above example, both pages are loaded at Shell startup. Often you want to load the pages on demand when the user interacts with the app, for example, by clicking on the menu item.

To load pages on demand you have to provide a callback function to the `ShellContent.RenderContent()` method.

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
    => Shell(
        FlyoutItem("Page1",
            ShellContent()
                .RenderContent(() => new ContentPage("Page1"))
        ),
        FlyoutItem("Page2",
            ShellContent()
                .RenderContent(() => new ContentPage("Page2"))
        )
    )
    .ItemTemplate(RenderItemTemplate);
```

## [Direct link to heading](\#navigation-and-routing)    Navigation and Routing

You can attach a route to a Shell item and navigate between pages calling the static method Shell.Current.GoToAsync() as shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public override VisualNode Render()
    => Shell(
        FlyoutItem("Page1",
            ShellContent()
                .RenderContent(() => new ContentPage("Page1",
                    Button("Goto to Page2")
                        .HCenter()
                        .VCenter()
                    .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync("//page-2"))
                ))
        )
        .Route("page-1"),

        FlyoutItem("Page2",
            ShellContent()
                .RenderContent(() => ContentPage("Page2",
                    Button("Goto to Page1")
                        .HCenter()
                        .VCenter()
                    .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync("//page-1"))
                ))
        )
        .Route("page-2")
    )
    .ItemTemplate(RenderItemTemplate);

```

and this is the resulting effect:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FzbPmbF7cLfhrY2oxCdmy%252FMauiReactor_Shell2.gif%3Falt%3Dmedia%26token%3D444b67b9-8eb0-4ce4-a401-961c7d59b881&width=768&dpr=4&quality=100&sign=c2e160d5&sv=2)

Navigating between pages using routes

It's not required that you declare routes for all of your pages using items of the shell. You can register a route also using the Routing.RegisterRoute<Component>() method as shown in the below example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    protected override void OnMounted()
    {
        Routing.RegisterRoute<Page2>("page-2");
        base.OnMounted();
    }

    public override VisualNode Render()
        => Shell(
            FlyoutItem("Page1",
                ShellContent()
                    .RenderContent(() => ContentPage("Page1",
                        Button("Goto to Page2")
                            .HCenter()
                            .VCenter()
                        .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync("page-2"))
                    ))
            )
        )
        .ItemTemplate(RenderItemTemplate);

    static VisualNode RenderItemTemplate(MauiControls.BaseShellItem item)
        => Grid("68", "*",
            Label(item.Title)
                .VCenter()
                .Margin(10,0)
        );
}

class Page2 : Component
{
    public override VisualNode Render()
    {
        return ContentPage("Page2",
            Button("Goto back")
                .HCenter()
                .VCenter()
            .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync(".."))
        );
    }
}
```

In MauiReactor you have to register a route for a Component that renders to a Page as shown in line 5 above.

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FCtni7Rviy6W4cvOQTTLk%252FMauiReactor_Shell3.gif%3Falt%3Dmedia%26token%3D6da8037b-51c5-4e9d-bdb8-a379bb63fe87&width=768&dpr=4&quality=100&sign=1d1d22de&sv=2)

Shell custom routes

## [Direct link to heading](\#passing-parameters-to-pages)    Passing parameters to pages

Often you have to pass arguments to the page when navigating to it (for example consider the case of a _ProductDetails_ page that requires the If of the product).

To pass parameters to a page you have to declare a component that accepts props like those shown below:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    protected override void OnMounted()
    {
        Routing.RegisterRoute<Page2>("page-2");
        base.OnMounted();
    }

    public override VisualNode Render()
        => Shell(
            FlyoutItem("Page1",
                ShellContent()
                    .RenderContent(() => ContentPage("Page1",
                        Button("Goto to Page2")
                            .HCenter()
                            .VCenter()
                        .OnClicked(async ()=>
                        await MauiControls.Shell.Current.GoToAsync<Page2Props>
                            ("page-2", props => props.Id = 23))
                    ))
            )
        )
        .ItemTemplate(RenderItemTemplate);

    static VisualNode RenderItemTemplate(MauiControls.BaseShellItem item)
        => Grid("68", "*",
            Label(item.Title)
                .VCenter()
                .Margin(10,0)
        );
}

class Page2State
{ }

class Page2Props
{
    public int Id { get; set; }
}
class Page2 : Component<Page2State, Page2Props>
{
    public override VisualNode Render()
    {
        return ContentPage("Page2",
             Button($"Received Id: {Props.Id}")
                .HCenter()
                .VCenter()
            .OnClicked(async ()=> await MauiControls.Shell.Current.GoToAsync(".."))
        );
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FrQX6lEhWLH6yaWy5w4mA%252FMauiReactor_Shell4.gif%3Falt%3Dmedia%26token%3Db9b0eb5b-397d-44ab-954e-1828b40665f1&width=768&dpr=4&quality=100&sign=1c64bf33&sv=2)

[PreviousNavigationPage](/mauireactor/components/navigation/navigation) [NextBack button](/mauireactor/components/navigation/back-button)

Last updated 4 months ago

---

# Native tree and Visual tree | MauiReactor

This article describes some internals of the MauiReactor library; even if it's not strictly required for the developer to know it, it's still useful to have a good understanding of how MauiReactor handles Native and Visual trees and their differences.

.NET MAUI app is composed of a tree of visual elements (classes deriving from `Element`). More precisely any page is the root of a controls tree that describes its UI.

For example, you could have a `ContentPage` that contains a `ScrollViewer` that in turn contains a `StackPanel` with children and so on.
This kind of visual tree structure is pretty common to many other frameworks or technologies (think of HTML DOM or WPF visual tree just to name a few).
It's often called the _native_ visual tree because it represents the lowest layer of abstraction in UI just above pixels on the screen.

Frameworks like MauiReactor (which is freely inspired to React/Flutter libraries) create a second higher visual tree (sometimes called _ghost_ tree).

When you create a page with children in MauiReactor, you're actually creating a visual tree of objects deriving from `VisualNode`. Often a visual node in ReactorUI pairs with a corresponding native control usually with the same name.

A MauiReactor component is an object that describes its UI in terms of visual nodes. A component is also a VisualNode so you can include components in a Visual tree.

A few things happen when a component calls its `Invalidate` method ( `SetState` in components with State)
1) MauiReactor calls its `Render` method and created a second version of the component _ghost_ tree
2) The previous tree is compared to the new one: new nodes are "mounted" and old ones are "unmounted"
3) Any mounted visual node usually creates its paired native control and adds it to the native tree in the correct position; any unmounted node is removed from the native tree
4) Any new or preserved node _usually_ updates the corresponding properties of the native counterpart

This section of articles or tutorials will essentially show you how to create ghost visual nodes for any control you want to use in MauiReactor. This could appear complex at first, but you'll see that most of the time is mostly a repetitive process that consists in creating a few small classes that for more clarity are usually contained in a single c# file.

[PreviousXAML Integration](/mauireactor/components/xaml-integration) [NextDependency injection](/mauireactor/deep-dives/dependency-injection)

Last updated 1 year ago

---

# CollectionView | MauiReactor

> The .NET Multi-platform App UI (.NET MAUI) [CollectionView](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.collectionview) is a view for presenting lists of data using different layout specifications. It aims to provide a more flexible, and performant alternative to [ListView](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.listview).

Official documentation:
[https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview/](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview/)

MauiReactor sample app:
[https://github.com/adospace/mauireactor-samples/tree/main/Controls/CollectionViewTestApp](https://github.com/adospace/mauireactor-samples/tree/main/Controls/CollectionViewTestApp)

## [Direct link to heading](\#display-data)    Display data

Use the method `ItemsSource()` to link a collection of items and to define how each item is displayed.

Bind the CollectionView to an IEnumerable (i.e. List or Array) you do not need to modify the collection (read-only collections).
If you want to update the list, adding or removing items, use an ObservableCollection instead.

For example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
protected override void OnMounted()
{
    var person1 = new Person("John", "Doe", new DateTime(1980, 5, 10));
    var person2 = new Person("Jane", "Smith", new DateTime(1990, 6, 20));
    var person3 = new Person("Alice", "Johnson", new DateTime(1985, 7, 30));
    var person4 = new Person("Bob", "Williams", new DateTime(2000, 8, 15));
    var person5 = new Person("Charlie", "Brown", new DateTime(1995, 9, 25));

    State.Persons = new [] { person1, person2, person3, person4, person5 };
    base.OnMounted();
}

public override VisualNode Render()
{
    return ContentPage(
        CollectionView()
            .ItemsSource(State.Persons, RenderPerson)
    );
}

private VisualNode RenderPerson(Person person)
{
    return VStack(spacing: 5,
        Label($"{person.FirstName} {person.LastName}"),
        Label(person.DateOfBirth.ToShortDateString())
            .FontSize(12)
            .TextColor(Colors.Gray)
    )
    .Margin(5,10);
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252F2TA95miTwRnXFQof7PtG%252Fimage.png%3Falt%3Dmedia%26token%3Dc265ba82-6dad-41e4-a995-f3fd1185e279&width=768&dpr=4&quality=100&sign=5f9592a3&sv=2)

CollectionView showing some data

[PreviousFlyoutPage](/mauireactor/components/controls/flyoutpage) [NextInteractions](/mauireactor/components/controls/collectionview/interactions)

Last updated 5 months ago

---

# What is MauiReactor? | MauiReactor

MauiReactor is a .NET library that implements a **component-based UI framework** on top of the .NET MAUI.

MauiReactor version 2 features a new way to write components. Some code on this documentation site uses the classic format.
The writing component using the old way is perfectly valid and working, and will be maintained/supported also in the coming versions.
Choosing between the two formats is up to you and your preferences.

If you already have some experience in React Native, Flutter, or Swift you may find some similarities. MauiReactor borrows some implementation designs from them while maintaining the normal MAUI application development you're used to.

NO XAML needed, just using C# you can write fluent declarative UI with MauiReactor components:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPage : Component
{
    public override VisualNode Render()
        => ContentPage("Login",
            VStack( // Vertical Stack
                Label("User:"),
                Entry(),
                Label("Password:"),
                Entry(),
                HStack( // Horizontal Stack
                    Button("Login"),
                    Button("Register")
                )
            )
            .Center()
        );
}
```

ReactorMaui ports .NET MAUI controls to C#:

1. Dependency properties that deal with simple types (i.e. excluding templates or views) are translated to Prop fluent methods:







Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
Button()
        .Text("Click!")
```

2. Events are translated to methods that accept a callback:







Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
    Button()
         .OnClicked(()=> ...)
```


You can read for examples and more complex controls at [Controls](https://adospace.gitbook.io/mauireactor/components/controls)

Also, you can opt to use ReactorMaui C# components within XAML files as described at [XAML Intregration](https://adospace.gitbook.io/mauireactor/components/xaml-integration)

**Good to know:** There are several design patterns you can choose when you build a UI. One of the most adopted is the MVVM (Model-View-View-Model) approach, used for years to build desktop and mobile applications. React made famous the MVU (Model-View-Update) pattern that aims to solve some issues of the first one. MAUI app defaults to MVVM while MauiReactor lets you build a MAUI app using the MVU pattern.

GitHub repo: [https://github.com/adospace/reactorui-maui](https://github.com/adospace/reactorui-maui)

[NextWhat's new in Version 2](/mauireactor/whats-new-in-version-2)

Last updated 11 months ago

---

# RadioButton | MauiReactor

> The .NET Multi-platform App UI (.NET MAUI) [RadioButton](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.radiobutton) is a type of button that allows users to select one option from a set. Each option is represented by one radio button, and you can only select one radio button in a group.

Official documentation:
[https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/radiobutton](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/radiobutton)

MauiReactor sample app:
[https://github.com/adospace/mauireactor-samples/tree/main/Controls/RadioButtonTestApp](https://github.com/adospace/mauireactor-samples/tree/main/Controls/RadioButtonTestApp)

## [Direct link to heading](\#group-of-radio-buttons-with-string-content)    Group of radio buttons with string content

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new VStack(spacing: 5)
{
    new RadioButton("Radio 1"),
    new RadioButton("Radio 2"),
    new RadioButton("Radio 3"),
    new RadioButton("Radio 4")
        .IsChecked(true)
},
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FUa3b4ehDdSvWHEFUqu0D%252Fimage.png%3Falt%3Dmedia%26token%3D403912d0-f5e4-4069-9aaf-515f2fb57859&width=768&dpr=4&quality=100&sign=786ebf4a&sv=2)

Simple radio button group

## [Direct link to heading](\#group-of-radio-buttons-with-custom-content)    Group of radio buttons with custom content

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
new VStack
{
    new RadioButton
    {
        new Image("icon_email.png")
    },
    new RadioButton
    {
        new Image("icon_lock.png")
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FUiRbocrTij8BW3tCHahY%252Fimage.png%3Falt%3Dmedia%26token%3D94367bf2-68e2-42ee-92f3-8aeac1b80ae1&width=768&dpr=4&quality=100&sign=2c8eaa1c&sv=2)

Radio buttons with custom content (iOS)

On Android, you must define a ControlTemplate to show custom content for the radio button.

[https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/radiobutton?view=net-maui-7.0#redefine-radiobutton-appearance](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/radiobutton?view=net-maui-7.0#redefine-radiobutton-appearance)

In MauiReactor you can place the control template in a XAML resource file

This is a sample control template for Android:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
<Style TargetType="RadioButton">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}" />
    <Setter Property="FontFamily" Value="OpenSansRegular"/>
    <Setter Property="FontSize" Value="14"/>
    <Setter Property="VisualStateManager.VisualStateGroups">
        <VisualStateGroupList>
            <VisualStateGroup x:Name="CommonStates">
                <VisualState x:Name="Normal" />
                <VisualState x:Name="Disabled">
                    <VisualState.Setters>
                        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateGroupList>
    </Setter>
    <Setter Property="ControlTemplate">
        <ControlTemplate>
            <Grid Margin="4">
                <VisualStateManager.VisualStateGroups>
                    <VisualStateGroupList>
                        <VisualStateGroup x:Name="CheckedStates">
                            <VisualState x:Name="Checked">
                                <VisualState.Setters>
                                    <Setter TargetName="check"
                                            Property="Opacity"
                                            Value="1" />
                                </VisualState.Setters>
                            </VisualState>
                            <VisualState x:Name="Unchecked">
                                <VisualState.Setters>
                                    <Setter TargetName="check"
                                            Property="Opacity"
                                            Value="0" />
                                </VisualState.Setters>
                            </VisualState>
                        </VisualStateGroup>
                    </VisualStateGroupList>
                </VisualStateManager.VisualStateGroups>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition/>
                </Grid.ColumnDefinitions>
                <Grid Margin="4"
                      WidthRequest="18"
                      HeightRequest="18"
                      HorizontalOptions="End"
                      VerticalOptions="Start">
                    <Ellipse Stroke="Blue"
                             Fill="White"
                             WidthRequest="16"
                             HeightRequest="16"
                             HorizontalOptions="Center"
                             VerticalOptions="Center" />
                    <Ellipse x:Name="check"
                             Fill="Blue"
                             WidthRequest="8"
                             HeightRequest="8"
                             HorizontalOptions="Center"
                             VerticalOptions="Center" />
                </Grid>
                <ContentPresenter
                        Grid.Column="1"
                        VerticalOptions="Center"/>
            </Grid>
        </ControlTemplate>
    </Setter>
</Style>
```

Using the above style:

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FP8RzAT59wag1eh0B1isA%252Fimage.png%3Falt%3Dmedia%26token%3D4859dbb9-fc82-4610-aac0-3b060dc8c9e4&width=768&dpr=4&quality=100&sign=41159123&sv=2)

Radio buttons with custom style

## [Direct link to heading](\#bonus-tip-radio-buttons-for-an-enum)    Bonus tip: Radio buttons for an Enum

Say you have an Enum like this:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
enum VehicleType
{
    Car,
    Coach,
    Motorcycle
}
```

This code enumerates its values and displays them in a button group:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public VisualNode RadioButtonsEnum<T>() where T : struct, System.Enum
{
    return new VStack
    {
        Enum.GetValues<T>()
            .Select(_=> new RadioButton(_.ToString()))
    };
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FIlM5WFPoy1ygKrccvbW4%252Fimage.png%3Falt%3Dmedia%26token%3D16a435e2-bf6b-412f-ac38-a4be7d8a30a1&width=768&dpr=4&quality=100&sign=f483d2bb&sv=2)

Radio buttons for an enum type

[PreviousButton](/mauireactor/components/controls/button) [NextFlyoutPage](/mauireactor/components/controls/flyoutpage)

Last updated 8 months ago

---

# Controls | MauiReactor

This section is under construction: I'll update it in the coming weeks including all the controls from the official .NET Maui documentation

This section of articles will pair one-to-one with .NET official MAUI documentation articles in order to describe how each standard .NET MAUI control can be created in MauiReactor.

This is the official list of controls:
[https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/)

## [Direct link to heading](\#navigation)    Navigation

[Shell](/mauireactor/components/controls/shell)

## [Direct link to heading](\#layouts)    Layouts

## [Direct link to heading](\#pages)    Pages

### [Direct link to heading](\#flyoutpage)    FlyoutPage

[FlyoutPage](/mauireactor/components/controls/flyoutpage)

## [Direct link to heading](\#views)    Views

### [Direct link to heading](\#initiate-commands)    Initiate commands

[RadioButton](/mauireactor/components/controls/radiobutton)

### [Direct link to heading](\#display-collections)    Display collections

[CollectionView](/mauireactor/components/controls/collectionview) [IndicatorView](/mauireactor/components/controls/indicatorview) [Picker](/mauireactor/components/controls/picker)

[PreviousBack button](/mauireactor/components/navigation/back-button) [NextButton](/mauireactor/components/controls/button)

Last updated 8 months ago

---

# Navigation | MauiReactor

.NET MAUI provides two different way to navigate between pages:

1. [NavigationPage](/mauireactor/components/navigation/navigation)

2. [Shell](/mauireactor/components/navigation/shell)


[PreviousTheming](/mauireactor/components/theming) [NextNavigationPage](/mauireactor/components/navigation/navigation)

Last updated 11 months ago

---

# Interactions | MauiReactor

## [Direct link to heading](\#context-menus)    Context menus

Use the SwipeView to define custom commands for each item. Below it's shown an example:

In the following example, the `CollectionView` is bound to an `ObservableCollection` so that when an item is removed from the list the control automatically refreshes without reloading the entire list.
Using a simple `List` or `Array` it would be necessary to update the entire list using a call to `SetState()`

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPageSwipeState
{
    public ObservableCollection<Person> Persons { get; set; }
}

class MainPageSwipe : Component<MainPageSwipeState>
{
    protected override void OnMounted()
    {
        var person1 = new Person("John", "Doe", new DateTime(1980, 5, 10));
        var person2 = new Person("Jane", "Smith", new DateTime(1990, 6, 20));
        var person3 = new Person("Alice", "Johnson", new DateTime(1985, 7, 30));
        var person4 = new Person("Bob", "Williams", new DateTime(2000, 8, 15));
        var person5 = new Person("Charlie", "Brown", new DateTime(1995, 9, 25));

        State.Persons = new ObservableCollection<Person>(new[] { person1, person2, person3, person4, person5 });
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return ContentPage(
            CollectionView()
                .ItemsSource(State.Persons, RenderPerson)
        );
    }

    private VisualNode RenderPerson(Person person)
    {
        return SwipeView(
            VStack(spacing: 5,
                Label($"{person.FirstName} {person.LastName}"),
                Label(person.DateOfBirth.ToShortDateString())
                    .FontSize(12)
            )
            .VCenter()
        )
        .LeftItems(new SwipeItems
        {
            new SwipeItem()
                .IconImageSource("archive.png")
                .Text("Archive")
                .BackgroundColor(Colors.Green),
            new SwipeItem()
                .IconImageSource("delete.png")
                .Text("Delete")
                .BackgroundColor(Colors.Red)
                .OnInvoked(()=>State.Persons.Remove(person))
        })
        .HeightRequest(60);
    }
}
```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FX2hl1DFZGs8ZgpboAAwG%252Fimage.png%3Falt%3Dmedia%26token%3Dd524875e-aed8-4506-ab77-fbc7fd72e302&width=768&dpr=4&quality=100&sign=25914b80&sv=2)

CollectionView with context commands

## [Direct link to heading](\#pull-to-refresh)    Pull to refresh

The "pull to refresh" feature allows you to execute a command when the user pulls down the collection view control to refresh the list.

Please note again that we're using an ObservableCollection instead of a List or Array so that control can subscribe to collection change events using the System.Collections.Specialized.INotifyCollectionChanged interface and update itself accordingly and efficiently

For example:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class MainPagePullToRefreshState
{
    public ObservableCollection<Person> Persons { get; set; }

    public bool IsRefreshing {  get; set; }
}

class MainPagePullToRefresh : Component<MainPagePullToRefreshState>
{
    protected override void OnMounted()
    {
        var person1 = new Person("John", "Doe", new DateTime(1980, 5, 10));
        var person2 = new Person("Jane", "Smith", new DateTime(1990, 6, 20));
        var person3 = new Person("Alice", "Johnson", new DateTime(1985, 7, 30));
        var person4 = new Person("Bob", "Williams", new DateTime(2000, 8, 15));
        var person5 = new Person("Charlie", "Brown", new DateTime(1995, 9, 25));

        State.Persons = new ObservableCollection<Person>(new[] { person1, person2, person3, person4, person5 });
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage(
            RefreshView(
                CollectionView()
                    .ItemsSource(State.Persons, RenderPerson)
            )
            .IsRefreshing(State.IsRefreshing)
            .OnRefreshing(OnRefresh)
        );
    }

    private void OnRefresh()
    {
        SetState(s => s.IsRefreshing = true);

        Task.Run(async () =>
        {
            await Task.Delay(2000);

            var person6 = new Person("Daniel", "Robinson", new DateTime(1982, 10, 2));
            var person7 = new Person("Ella", "Martin", new DateTime(1992, 11, 13));
            var person8 = new Person("Frank", "Garcia", new DateTime(1987, 3, 19));

            State.Persons.Insert(0, person6);
            State.Persons.Insert(0, person7);
            State.Persons.Insert(0, person8);

            SetState(s => s.IsRefreshing = false);
        });
    }

    private VisualNode RenderPerson(Person person)
    {
        return new VStack(spacing: 5)
        {
            new Label($"{person.FirstName} {person.LastName}"),
            new Label(person.DateOfBirth.ToShortDateString())
                .FontSize(12)
        }
        .VCenter();
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FYb9Lw8DAoT5AIpXEMDxa%252Fimage.png%3Falt%3Dmedia%26token%3D94458208-3c72-42c3-aabe-06f4b4b4bc15&width=768&dpr=4&quality=100&sign=1e0008e8&sv=2)

Pull to refresh in action

## [Direct link to heading](\#load-data-incrementally)    Load data incrementally

The below example shows how you can load data incrementally as the user scrolls down the list:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
public record PersonWithAddress(string FirstName, string LastName, int Age, string Address);

class MainPageLoadDataIncrementallyState
{
    public ObservableCollection<PersonWithAddress> Persons { get; set; }

    public bool IsLoading {  get; set; }
}

class MainPageLoadDataIncrementally : Component<MainPageLoadDataIncrementallyState>
{
    private static IEnumerable<PersonWithAddress> GenerateSamplePersons(int count)
    {
        var random = new Random();
        var firstNames = new[] { "John", "Jim", "Joe", "Jack", "Jane", "Jill", "Jerry", "Jude", "Julia", "Jenny" };
        var lastNames = new[] { "Brown", "Green", "Black", "White", "Blue", "Red", "Gray", "Smith", "Doe", "Jones" };
        var cities = new[] { "New York", "London", "Sidney", "Tokyo", "Paris", "Berlin", "Mumbai", "Beijing", "Cairo", "Rio" };

        var persons = new List<PersonWithAddress>();

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[random.Next(0, firstNames.Length)];
            var lastName = lastNames[random.Next(0, lastNames.Length)];
            var age = random.Next(20, 60);
            var city = cities[random.Next(0, cities.Length)];
            var address = $"{city} No. {random.Next(1, 11)} Lake Park";

            persons.Add(new PersonWithAddress(firstName, lastName, age, address));
        }

        return persons;
    }

    protected override void OnMounted()
    {
        State.Persons = new ObservableCollection<PersonWithAddress>(GenerateSamplePersons(100));
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage
        {
            new Grid("*", "*")
            {
                new CollectionView()
                    .ItemsSource(State.Persons, RenderPerson)
                    .RemainingItemsThreshold(50)
                    .OnRemainingItemsThresholdReached(LoadMorePersons),

                new ActivityIndicator()
                    .IsRunning(State.IsLoading)
                    .VCenter()
                    .HCenter()
            }
        };
    }

    private void LoadMorePersons()
    {
        if (State.IsLoading == true)
        {
            return;
        }

        SetState(s => s.IsLoading = true);

        Task.Run(async () =>
        {
            await Task.Delay(3000);

            foreach (var newPerson in GenerateSamplePersons(50))
            {
                //is not required here to call set state because we don't want to refresh the entire component
                //the collection view is bound to State.Persons
                State.Persons.Add(newPerson);
            }

            SetState(s => s.IsLoading = false);
        });
    }

    private VisualNode RenderPerson(PersonWithAddress person)
    {
        return new VStack(spacing: 5)
        {
            new Label($"{person.FirstName} {person.LastName} ({person.Age})"),
            new Label(person.Address)
                .FontSize(12)
        }
        .HeightRequest(56)
        .VCenter();
    }
}
```

You may be tempted to use code like SetState(s => s.Persons.Add(person)) but, in this specific case, it's not required: the CollectionView is bound to the Persons ObservableCollection and it's automatically refreshed at every change of the collection without requiring the component refresh.

Please note that, in all other cases (i.e. excluding ObservableCollection properties) you must update the state using the SetState() method when you need to update the view.

[PreviousCollectionView](/mauireactor/components/controls/collectionview) [NextLayout](/mauireactor/components/controls/collectionview/layout)

Last updated 5 months ago

---

# Do we need to add states to create simple animations such as ScaleTo, FadeTo, etc on tap? | MauiReactor

Even if RxAnimations or AnimationController class are a primary way to create animations in MauiReactor, it's not, of course, strictly required.

Take for example how we can create an animated button: we want the user to see a scale animation when clicking over a widget (Image, Button, etc).

Apart from RxAnimation, we can easily just call the ScaleTo view extension method over the native control to achieve the same effect.

Following, we'll see possible implementations:

## [Direct link to heading](\#use-a-component-with-a-layout-control-containing-the-children-element-and-animate-it-when-tapped)    Use a component, with a layout control containing the Children element and animate it when tapped:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class AnimatedComponent: Component
{
    private Func<Task>? _action;
    private MauiControls.Grid? _containerGrid;

    public AnimatedComponent OnTapped(Func<Task> action)
    {
        _action = action;
        return this;
    }

    public override VisualNode Render()
    {
        return new Grid(grid => _containerGrid = grid)
        {
            Children()
        }
        .OnTapped(async () =>
        {
            if (_containerGrid != null && _action != null)
            {
                await MauiControls.ViewExtensions.ScaleTo(_containerGrid, 0.7);
                await _action.Invoke();
                await MauiControls.ViewExtensions.ScaleTo(_containerGrid, 1.0);
            }
        });
    }
}
```

## [Direct link to heading](\#create-an-extension-function-for-the-mauireactor-widget-that-realizes-the-animation)    Create an extension function for the MauiReactor widget that realizes the animation:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
static class AnimatedButtonExtensions
{
    public static T OnTappedWithAnimation<T>(this T view, Func<Task> action) where T : IView
    {
        view.OnTapped(async (sender, args) =>
        {
            var visualElement = (MauiControls.VisualElement?)sender;
            if (visualElement == null)
            {
                return;
            }
            await MauiControls.ViewExtensions.ScaleTo(visualElement, 0.7);
            await action.Invoke();
            await MauiControls.ViewExtensions.ScaleTo(visualElement, 1.0);
        });
        return view;
    }
}
```

## [Direct link to heading](\#create-a-component-that-renders-just-one-child-animated-when-tapped)    Create a component that renders just one child animated when tapped:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class AnimatedComponent2: Component
{
    private Func<Task>? _action;

    public AnimatedComponent2 OnTapped(Func<Task> action)
    {
        _action = action;
        return this;
    }

    public override VisualNode Render()
    {
        var child = Children().FirstOrDefault();
        if (child == null)
        {
            return null!;
        }

        if (child is IView viewChild)
        {
            viewChild.OnTapped(async (sender, args) =>
            {
                var visualElement = (MauiControls.VisualElement?)sender;
                if (visualElement == null)
                {
                    return;
                }

                if (_action != null)
                {
                    await MauiControls.ViewExtensions.ScaleTo(visualElement, 0.7);
                    await _action.Invoke();
                    await MauiControls.ViewExtensions.ScaleTo(visualElement, 1.0);
                }
            });
        }

        return child;
    }
}
```

Let's put everything on a page to show the same resulting effect of each variation:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class AnimatedButtonPage: Component
{
    public override VisualNode Render()
    {
        return new ContentPage("Animated image sample")
        {
            new VStack(spacing: 20)
            {
                new AnimatedComponent
                {
                    new Image("tab_home.png"),
                }
                .OnTapped(OnTapped),

                new Image("tab_home.png")
                    .OnTappedWithAnimation(OnTapped),

                new AnimatedComponent2
                {
                    new Image("tab_home.png"),
                }
                .OnTapped(OnTapped)
            }
            .VCenter()
        };
    }

    async Task OnTapped()
    {
        if (ContainerPage != null)
        {
            await ContainerPage.DisplayAlert("MauiReactor", "Tapped!", "OK");
        }
    }
}

```

![](https://adospace.gitbook.io/~gitbook/image?url=https%3A%2F%2F877538538-files.gitbook.io%2F%7E%2Ffiles%2Fv0%2Fb%2Fgitbook-x-prod.appspot.com%2Fo%2Fspaces%252Fh1eh1igwiwRzrw2kbxSp%252Fuploads%252FKHYc3OtPnqxM8FRDgIMn%252FAnimatedButtonSample.gif%3Falt%3Dmedia%26token%3Da55bb280-1257-4615-9383-16db94d27ed8&width=768&dpr=4&quality=100&sign=fa73428c&sv=2)

[PreviousDoes this support ObservableCollection for CollectionView?](/mauireactor/q-and-a/does-this-support-observablecollection-for-collectionview) [NextHow to deal with custom dialogs/popups?](/mauireactor/q-and-a/how-to-deal-with-custom-dialogs-popups)

Last updated 1 year ago

---

# Component Parameters | MauiReactor

MauiReactor has a nice feature that developers can integrate into their app to share data between a component and its tree of children.

A parameter is a way to automatically transfer an object from the parent component to its children down the hierarchy up to the lower ones.

Each component accessing a parameter can read and write its value freely.

When a parameter is modified all the components referencing it are automatically invalidated so that they can re-render according to the new value.

You can access a parameter created from any ancestor, not just the direct parent component

For example, in the following code, we're going to define a parameter in a component:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class CustomParameter
{
    public int Numeric { get; set; }
}

partial class ParametersPage: Component
{
    [Param]
    IParameter<CustomParameter> _customParameter;

    public override VisualNode Render()
     => ContentPage("Parameters Sample",
        => VStack(spacing: 10,
                Button("Increment from parent", () => _customParameter.Set(_=>_.Numeric += 1   )),
                Label(_customParameter.Value.Numeric),

                new ParameterChildComponent()
            )
            .Center()
        );
}
```

To access the component from a child, just reference it:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
partial class ParameterChildComponent: Component
{
    [Param]
    IParameter<CustomParameter> _customParameter;

    public override VisualNode Render()
      => VStack(spacing: 10,
            Button("Increment from child", ()=> _customParameter.Set(_=>_.Numeric++)),

            Label(customParameter.Value.Numeric)
        );
}
```

When you modify a parameter value, MauiReactor updates any component starting from the parent one that has defined it down to its children.
You can control this behavior using the overload `void Set(Action setAction, bool invalidateComponent = true)` of the `IParameter<T>` interface
Passing false to the `invalidateComponent` MauiReactor doesn't invalidate the components referencing the `Parameter` but it just updates the properties that are referencing it inside the callback ()=>...

[PreviousComponent with children](/mauireactor/components/component-with-children) [NextTheming](/mauireactor/components/theming)

Last updated 11 months ago

---

# Source and Sample Applications | MauiReactor

[MauiReactor GitHub Repository](https://github.com/adospace/reactorui-maui)

[Twitter profile](https://twitter.com/adolfomarinucci)

[Test Application](https://github.com/adospace/reactorui-maui/tree/main/samples/MauiReactor.TestApp)

[WeatherTwentyOne App](https://github.com/adospace/reactorui-maui/tree/main/samples/MauiReactor.WeatherTwentyOne)

[Chart App](https://github.com/adospace/reactorui-maui/tree/main/samples/ChartApp)

[Contentics App](https://github.com/adospace/reactorui-maui/tree/main/samples/Contentics)

[Sample Calendar Widget](https://github.com/adospace/reactorui-maui/tree/main/samples/MauiReactor.Calendar)

[SlidingPuzzle Game App](https://github.com/adospace/reactorui-maui/tree/main/samples/SlidingPuzzle)

[KeeMind](https://github.com/adospace/kee-mind)

[RiveApp](https://github.com/adospace/rive-app)

[Ordering App](https://github.com/adospace/mauireactor-samples/tree/main/OrderingApp)

[PreviousBehaviors](/mauireactor/deep-dives/behaviors) [NextHow to deal with state shared across Components?](/mauireactor/q-and-a/how-to-deal-with-state-shared-across-components)

Last updated 1 year ago

---

# Component Properties | MauiReactor

When creating a component you almost always need to pass props (or parameters/property values) to customize its appearance or behavior. In MauiReactor you can use plain properties.

Take for example this component that implements an activity indicator with a label:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
partial class BusyComponent : Component
{
    [Prop]
    string _message;
    [Prop]
    bool _isBusy;

    public override VisualNode Render()
     => StackLayout(
            ActivityIndicator()
                .IsRunning(_isBusy),
            Label()
                .Text(_message)
        );
}
```

and this is how we can use it on a page:

Copy

```min-w-full inline-grid [grid-template-columns:auto_1fr] py-2 px-2 [counter-reset:line]
class BusyPageState : IState
{
    public bool IsBusy { get; set; }
}

class BusyPageComponent : Component<BusyPageState>
{
    protected override void OnMounted()
    {
        SetState(_ => _.IsBusy = true);

        //OnMounted is called on UI Thread, move the slow code to a background thread
        Task.Run(async () =>
        {
            //Simulate lenghty work
            await Task.Delay(3000);

            SetState(_ => _.IsBusy = false);
        });

        base.OnMounted();
    }

    public override VisualNode Render()
      => ContentPage(
            State.IsBusy ?
            new BusyComponent()
                .Message("Loading")
                .IsBusy(true)
            :
            RenderPage()
        );

    private VisualNode RenderPage()
        => Label("Done!")
            .Center();
}
```

If you need to set properties of components hosted on a different page you should use a Props object (see [Navigation](/mauireactor/components/navigation/navigation))

[PreviousComponent life-cycle](/mauireactor/components/component-life-cycle) [NextComponent with children](/mauireactor/components/component-with-children)

Last updated 11 months ago

---

