using MauiReactor;
using KSpeackEnglish.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Media;

namespace KSpeackEnglish
{
  public static class MauiProgram
  {
    public static MauiApp CreateMauiApp()
    {
      var builder = MauiApp.CreateBuilder();
      builder
          .UseMauiReactorApp<HomePage>()
          .UseMauiCommunityToolkit()
          .ConfigureFonts(fonts =>
          {
              fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          });

      // #if ANDROID || IOS
      //   builder.Services.AddTransient<IAudioPlayerService, AudioPlayerService>();
      //   builder.Services.AddTransient<IRecordAudioService, RecordAudioService>();
      // #endif
      
      builder.Services.AddSingleton<ISpeechToText>(SpeechToText.Default);

      return builder.Build();
    }
  }

    internal interface IRecordAudioService
    {
    }

    internal interface IAudioPlayerService
    {
    }
}
